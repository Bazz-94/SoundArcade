using System.Numerics;

namespace SoundArcade.Abstractions;

/// <summary>
/// Provides platform-agnostic audio playback, including positional sound.
/// </summary>
public interface IAudio
{
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
}
