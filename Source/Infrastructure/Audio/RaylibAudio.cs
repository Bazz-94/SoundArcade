using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Raylib_cs;
using SoundArcade.Abstractions;

namespace SoundArcade.Infrastructure.Audio;

/// <summary>
/// Raylib-backed audio manager for sound effects, music, and TTS ducking.
/// </summary>
public sealed class RaylibAudio : IAudio, IDisposable
{
  private const float DefaultMasterVolume = 1.0f;
  private const float DefaultMusicVolume = 1.0f;
  private const float DuckingMusicVolume = 0.35f;
  private const float MinimumAudibleVolume = 0.2f;
  private const float MaximumHearDistance = 20.0f;

  private readonly Dictionary<string, Sound> sounds = new(StringComparer.Ordinal);
  private readonly Dictionary<string, Music> musicTracks = new(StringComparer.Ordinal);
  private readonly object sync = new();
  private readonly ITts? tts;

  private Vector3 listenerPosition = Vector3.Zero;
  private string? activeMusicId;
  private Music activeMusic;
  private bool disposed;
  private float musicVolume = DefaultMusicVolume;
  private int ttsSpeechDepth;
  private bool musicVolumeRefreshPending;

  /// <summary>
  /// Initializes a new instance of the <see cref="RaylibAudio"/> class.
  /// </summary>
  /// <param name="tts">Optional text-to-speech source used to drive ducking.</param>
  public RaylibAudio(ITts? tts = null)
  {
    this.tts = tts;
    this.EnsureAudioDevice();

    if (this.tts is not null)
    {
      this.tts.SpeakStarted += this.OnTtsSpeakStarted;
      this.tts.SpeakCompleted += this.OnTtsSpeakCompleted;
    }
  }

  /// <summary>
  /// Releases loaded audio assets and closes the Raylib audio device.
  /// </summary>
  public void Dispose()
  {
    if (this.disposed)
    {
      return;
    }

    if (this.tts is not null)
    {
      this.tts.SpeakStarted -= this.OnTtsSpeakStarted;
      this.tts.SpeakCompleted -= this.OnTtsSpeakCompleted;
    }

    this.StopMusic();

    foreach (KeyValuePair<string, Sound> entry in this.sounds)
    {
      Raylib.UnloadSound(entry.Value);
    }

    foreach (KeyValuePair<string, Music> entry in this.musicTracks)
    {
      Raylib.UnloadMusicStream(entry.Value);
    }

    this.sounds.Clear();
    this.musicTracks.Clear();

    if (Raylib.IsAudioDeviceReady())
    {
      Raylib.CloseAudioDevice();
    }

    this.disposed = true;
  }

  /// <summary>
  /// Registers a sound effect asset under a stable identifier.
  /// </summary>
  /// <param name="soundId">Audio asset identifier.</param>
  /// <param name="assetPath">Absolute or relative file path to the asset.</param>
  public void RegisterSound(string soundId, string assetPath)
  {
    this.EnsureNotDisposed();
    ValidateRegistration(soundId, assetPath);

    Sound sound = Raylib.LoadSound(assetPath);

    lock (this.sync)
    {
      if (this.sounds.TryGetValue(soundId, out Sound existingSound))
      {
        Raylib.UnloadSound(existingSound);
      }

      this.sounds[soundId] = sound;
    }
  }

  /// <summary>
  /// Registers a music asset under a stable identifier.
  /// </summary>
  /// <param name="musicId">Music asset identifier.</param>
  /// <param name="assetPath">Absolute or relative file path to the asset.</param>
  public void RegisterMusic(string musicId, string assetPath)
  {
    this.EnsureNotDisposed();
    ValidateRegistration(musicId, assetPath);

    Music music = Raylib.LoadMusicStream(assetPath);

    lock (this.sync)
    {
      if (this.musicTracks.TryGetValue(musicId, out Music existingMusic))
      {
        Raylib.UnloadMusicStream(existingMusic);
      }

      this.musicTracks[musicId] = music;
    }
  }

  /// <inheritdoc />
  public void Update()
  {
    this.EnsureNotDisposed();

    Music music;
    bool hasMusic;
    bool refreshMusicVolume;

    lock (this.sync)
    {
      hasMusic = this.activeMusicId is not null;
      music = this.activeMusic;
      refreshMusicVolume = this.musicVolumeRefreshPending;
      this.musicVolumeRefreshPending = false;
    }

    if (hasMusic)
    {
      Raylib.UpdateMusicStream(music);

      if (refreshMusicVolume)
      {
        this.ApplyCurrentMusicVolume();
      }
    }
  }

  /// <inheritdoc />
  public void PlaySound(string soundId, float volume)
  {
    this.EnsureNotDisposed();

    Sound sound = this.GetSound(soundId);
    float playbackVolume = Math.Clamp(volume, 0.0f, 1.0f);

    Raylib.SetSoundVolume(sound, playbackVolume);
    Raylib.PlaySound(sound);
  }

  /// <inheritdoc />
  public void PlaySoundAt(string soundId, Vector3 position, float volume)
  {
    this.EnsureNotDisposed();

    Sound sound = this.GetSound(soundId);
    Vector3 listenerPosition;

    lock (this.sync)
    {
      listenerPosition = this.listenerPosition;
    }

    float playbackVolume = Math.Clamp(volume * this.ComputeDistanceAttenuation(listenerPosition, position), MinimumAudibleVolume, 1.0f);
    float pan = Math.Clamp(position.X - listenerPosition.X, -1.0f, 1.0f);

    Raylib.SetSoundPan(sound, pan);
    Raylib.SetSoundVolume(sound, playbackVolume);
    Raylib.PlaySound(sound);
  }

  /// <inheritdoc />
  public void StopSound(string soundId)
  {
    this.EnsureNotDisposed();

    if (!this.TryGetSound(soundId, out Sound sound))
    {
      return;
    }

    Raylib.StopSound(sound);
  }

  /// <inheritdoc />
  public void SetListenerPosition(Vector3 position)
  {
    this.EnsureNotDisposed();

    lock (this.sync)
    {
      this.listenerPosition = position;
    }
  }

  /// <inheritdoc />
  public void SetMasterVolume(float volume)
  {
    this.EnsureNotDisposed();
    Raylib.SetMasterVolume(Math.Clamp(volume, 0.0f, 1.0f));
  }

  /// <inheritdoc />
  public void PlayMusic(string musicId, bool loop)
  {
    this.EnsureNotDisposed();

    Music music = this.GetMusic(musicId);

    lock (this.sync)
    {
      if (this.activeMusicId is not null)
      {
        Raylib.StopMusicStream(this.activeMusic);
      }

      music.Looping = loop;
      this.activeMusic = music;
      this.activeMusicId = musicId;
    }

    Raylib.PlayMusicStream(music);
    this.ApplyCurrentMusicVolume();
  }

  /// <inheritdoc />
  public void PauseMusic()
  {
    this.EnsureNotDisposed();

    Music music;
    bool hasMusic;

    lock (this.sync)
    {
      hasMusic = this.activeMusicId is not null;
      music = this.activeMusic;
    }

    if (hasMusic)
    {
      Raylib.PauseMusicStream(music);
    }
  }

  /// <inheritdoc />
  public void ResumeMusic()
  {
    this.EnsureNotDisposed();

    Music music;
    bool hasMusic;

    lock (this.sync)
    {
      hasMusic = this.activeMusicId is not null;
      music = this.activeMusic;
    }

    if (hasMusic)
    {
      Raylib.ResumeMusicStream(music);
    }
  }

  /// <inheritdoc />
  public void StopMusic()
  {
    this.EnsureNotDisposed();

    Music music;
    bool hasMusic;

    lock (this.sync)
    {
      hasMusic = this.activeMusicId is not null;
      music = this.activeMusic;
      this.activeMusicId = null;
    }

    if (hasMusic)
    {
      Raylib.StopMusicStream(music);
    }
  }

  /// <inheritdoc />
  public void SetMusicVolume(float volume)
  {
    this.EnsureNotDisposed();

    lock (this.sync)
    {
      this.musicVolume = Math.Clamp(volume, 0.0f, 1.0f);
    }

    this.ApplyCurrentMusicVolume();
  }

  /// <summary>
  /// Ensures the Raylib audio device is ready.
  /// </summary>
  private void EnsureAudioDevice()
  {
    if (!Raylib.IsAudioDeviceReady())
    {
      Raylib.InitAudioDevice();
    }
  }

  /// <summary>
  /// Gets a registered sound effect by identifier.
  /// </summary>
  /// <param name="soundId">Audio asset identifier.</param>
  /// <returns>The loaded sound effect.</returns>
  private Sound GetSound(string soundId)
  {
    if (this.TryGetSound(soundId, out Sound sound))
    {
      return sound;
    }

    throw new KeyNotFoundException($"Sound asset '{soundId}' is not registered.");
  }

  /// <summary>
  /// Attempts to get a registered sound effect by identifier.
  /// </summary>
  /// <param name="soundId">Audio asset identifier.</param>
  /// <param name="sound">Loaded sound effect when found.</param>
  /// <returns>True when the sound exists.</returns>
  private bool TryGetSound(string soundId, out Sound sound)
  {
    lock (this.sync)
    {
      return this.sounds.TryGetValue(soundId, out sound);
    }
  }

  /// <summary>
  /// Gets a registered music track by identifier.
  /// </summary>
  /// <param name="musicId">Music asset identifier.</param>
  /// <returns>The loaded music stream.</returns>
  private Music GetMusic(string musicId)
  {
    lock (this.sync)
    {
      if (this.musicTracks.TryGetValue(musicId, out Music music))
      {
        return music;
      }
    }

    throw new KeyNotFoundException($"Music asset '{musicId}' is not registered.");
  }

  /// <summary>
  /// Applies the current music volume after ducking is considered.
  /// </summary>
  private void ApplyCurrentMusicVolume()
  {
    Music music;
    float volume;
    bool hasMusic;

    lock (this.sync)
    {
      hasMusic = this.activeMusicId is not null;
      music = this.activeMusic;
      volume = this.musicVolume;

      if (this.ttsSpeechDepth > 0)
      {
        volume *= DuckingMusicVolume;
      }
    }

    if (hasMusic)
    {
      Raylib.SetMusicVolume(music, volume);
    }
  }

  /// <summary>
  /// Calculates volume attenuation for positional playback.
  /// </summary>
  /// <param name="listener">Listener position.</param>
  /// <param name="position">Sound source position.</param>
  /// <returns>A clamped attenuation factor.</returns>
  private float ComputeDistanceAttenuation(Vector3 listener, Vector3 position)
  {
    float distance = Vector3.Distance(listener, position);
    float attenuation = 1.0f - (distance / MaximumHearDistance);
    return Math.Clamp(attenuation, MinimumAudibleVolume, 1.0f);
  }

  /// <summary>
  /// Validates asset registration arguments.
  /// </summary>
  /// <param name="assetId">Stable asset identifier.</param>
  /// <param name="assetPath">Asset file path.</param>
  private static void ValidateRegistration(string assetId, string assetPath)
  {
    if (string.IsNullOrWhiteSpace(assetId))
    {
      throw new ArgumentException("Asset identifier cannot be empty.", nameof(assetId));
    }

    if (string.IsNullOrWhiteSpace(assetPath))
    {
      throw new ArgumentException("Asset path cannot be empty.", nameof(assetPath));
    }

    if (!File.Exists(assetPath))
    {
      throw new FileNotFoundException($"Audio asset '{assetId}' was not found.", assetPath);
    }
  }

  /// <summary>
  /// Handles TTS start notifications by enabling ducking.
  /// </summary>
  /// <param name="sender">Event source.</param>
  /// <param name="e">Event data.</param>
  private void OnTtsSpeakStarted(object? sender, EventArgs e)
  {
    lock (this.sync)
    {
      this.ttsSpeechDepth++;
      this.musicVolumeRefreshPending = true;
    }
  }

  /// <summary>
  /// Handles TTS completion notifications by restoring the music mix.
  /// </summary>
  /// <param name="sender">Event source.</param>
  /// <param name="e">Event data.</param>
  private void OnTtsSpeakCompleted(object? sender, EventArgs e)
  {
    lock (this.sync)
    {
      if (this.ttsSpeechDepth > 0)
      {
        this.ttsSpeechDepth--;
      }

      this.musicVolumeRefreshPending = true;
    }
  }

  /// <summary>
  /// Ensures the instance has not been disposed.
  /// </summary>
  private void EnsureNotDisposed()
  {
    if (this.disposed)
    {
      throw new ObjectDisposedException(nameof(RaylibAudio));
    }
  }
}
