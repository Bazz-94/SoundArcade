using System.Numerics;

namespace SoundArcade.Abstractions;

/// <summary>
/// Provides platform-agnostic audio playback, including positional sound.
/// </summary>
public interface IAudio
{
  /// <summary>
  /// Advances streaming audio state.
  /// </summary>
  void Update();

  /// <summary>
  /// Plays a non-positional sound effect.
  /// </summary>
  /// <param name="soundId">Audio asset identifier.</param>
  /// <param name="volume">Playback volume.</param>
  void PlaySound(string soundId, float volume);

  /// <summary>
  /// Plays a positional sound effect in world space.
  /// </summary>
  /// <param name="soundId">Audio asset identifier.</param>
  /// <param name="position">World position of the sound source.</param>
  /// <param name="volume">Playback volume.</param>
  void PlaySoundAt(string soundId, Vector3 position, float volume);

  /// <summary>
  /// Stops a currently playing sound.
  /// </summary>
  /// <param name="soundId">Audio asset identifier.</param>
  void StopSound(string soundId);

  /// <summary>
  /// Sets the listener world position for positional audio calculations.
  /// </summary>
  /// <param name="position">Listener world position.</param>
  void SetListenerPosition(Vector3 position);

  /// <summary>
  /// Sets the global audio volume.
  /// </summary>
  /// <param name="volume">Master volume in the range 0 to 1.</param>
  void SetMasterVolume(float volume);

  /// <summary>
  /// Starts music playback for a registered asset.
  /// </summary>
  /// <param name="musicId">Music asset identifier.</param>
  /// <param name="loop">Whether the track should loop.</param>
  void PlayMusic(string musicId, bool loop);

  /// <summary>
  /// Pauses the active music stream.
  /// </summary>
  void PauseMusic();

  /// <summary>
  /// Resumes the active music stream.
  /// </summary>
  void ResumeMusic();

  /// <summary>
  /// Stops the active music stream.
  /// </summary>
  void StopMusic();

  /// <summary>
  /// Sets the music volume before ducking is applied.
  /// </summary>
  /// <param name="volume">Music volume in the range 0 to 1.</param>
  void SetMusicVolume(float volume);
}
