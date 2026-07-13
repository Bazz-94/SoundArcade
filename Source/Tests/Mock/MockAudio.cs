namespace SoundArcade.Tests.Mock
{
  using System;
  using System.Collections.Generic;
  using System.Numerics;
  using SoundArcade.Abstractions;
  using SoundArcade.Infrastructure.Audio;

  /// <summary>
  /// Recording <see cref="IAudio"/> test double. Playback calls are never sent to a device;
  /// instead every call is captured, and positional playbacks compute the same effective
  /// volume, pan and per-ear levels the real backend derives via <see cref="SpatialAudioMath"/>,
  /// so tests can assert exactly what a player would hear for a given listener and source position.
  /// </summary>
  public sealed class MockAudio : IAudio
  {
    private readonly Dictionary<string, float> soundGains = new Dictionary<string, float>(StringComparer.Ordinal);

    /// <summary>
    /// Gets the most recently set listener position.
    /// </summary>
    public Vector3 ListenerPosition { get; private set; } = Vector3.Zero;

    /// <summary>
    /// Gets the most recently set master volume.
    /// </summary>
    public float MasterVolume { get; private set; } = 1.0f;

    /// <summary>
    /// Gets every playback captured by <see cref="PlaySound"/> and <see cref="PlaySoundAt"/>, in call order.
    /// </summary>
    public List<SoundPlayback> Playbacks { get; } = new List<SoundPlayback>();

    /// <summary>
    /// Gets the generated sound profiles registered per sound identifier.
    /// </summary>
    public Dictionary<string, SoundProfile> RegisteredProfiles { get; } = new Dictionary<string, SoundProfile>(StringComparer.Ordinal);

    /// <summary>
    /// Gets the sound identifiers passed to <see cref="StopSound"/>, in call order.
    /// </summary>
    public List<string> StoppedSoundIds { get; } = new List<string>();

    /// <inheritdoc />
    public void Update()
    {
    }

    /// <inheritdoc />
    public void RegisterSound(string soundId, string assetPath, float gain = 1.0f)
    {
      this.soundGains[soundId] = gain;
    }

    /// <inheritdoc />
    public void RegisterGeneratedSound(string soundId, SoundProfile profile)
    {
      this.RegisteredProfiles[soundId] = profile;
      this.soundGains[soundId] = profile.Gain;
    }

    /// <inheritdoc />
    public void PlaySound(string soundId, float volume, float pitch = 1.0f)
    {
      float effectiveVolume = Math.Clamp(volume * this.GetGain(soundId), 0.0f, 1.0f);
      this.Playbacks.Add(new SoundPlayback(
        SoundId: soundId,
        Position: null,
        RequestedVolume: volume,
        Pitch: pitch,
        EffectiveVolume: effectiveVolume,
        Pan: 0.0f,
        LeftVolume: effectiveVolume,
        RightVolume: effectiveVolume));
    }

    /// <inheritdoc />
    public void PlaySoundAt(string soundId, Vector3 position, float volume, float pitch = 1.0f)
    {
      float effectiveVolume = SpatialAudioMath.ComputePlaybackVolume(volume, this.GetGain(soundId), this.ListenerPosition, position);
      float pan = SpatialAudioMath.ComputePan(this.ListenerPosition, position);
      (float left, float right) = SpatialAudioMath.ComputeChannelVolumes(effectiveVolume, pan);

      this.Playbacks.Add(new SoundPlayback(
        SoundId: soundId,
        Position: position,
        RequestedVolume: volume,
        Pitch: pitch,
        EffectiveVolume: effectiveVolume,
        Pan: pan,
        LeftVolume: left,
        RightVolume: right));
    }

    /// <inheritdoc />
    public void StopSound(string soundId)
    {
      this.StoppedSoundIds.Add(soundId);
    }

    /// <inheritdoc />
    public void SetListenerPosition(Vector3 position)
    {
      this.ListenerPosition = position;
    }

    /// <inheritdoc />
    public void SetMasterVolume(float volume)
    {
      this.MasterVolume = volume;
    }

    /// <inheritdoc />
    public void PlayMusic(string musicId, bool loop)
    {
    }

    /// <inheritdoc />
    public void PauseMusic()
    {
    }

    /// <inheritdoc />
    public void ResumeMusic()
    {
    }

    /// <inheritdoc />
    public void StopMusic()
    {
    }

    /// <inheritdoc />
    public void SetMusicVolume(float volume)
    {
    }

    /// <summary>
    /// Gets the registered gain for a sound, defaulting to 1.0 when none was registered.
    /// </summary>
    /// <param name="soundId">Audio asset identifier.</param>
    /// <returns>The registered gain multiplier.</returns>
    private float GetGain(string soundId)
    {
      return this.soundGains.TryGetValue(soundId, out float gain) ? gain : 1.0f;
    }
  }
}
