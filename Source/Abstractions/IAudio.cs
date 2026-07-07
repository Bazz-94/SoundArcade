namespace SoundArcade.Abstractions
{
  using System.Numerics;

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
    /// <param name="pitch">Playback pitch multiplier, where 1.0 is the asset's base pitch.</param>
    void PlaySound(string soundId, float volume, float pitch = 1.0f);

    /// <summary>
    /// Plays a positional sound effect in world space.
    /// </summary>
    /// <param name="soundId">Audio asset identifier.</param>
    /// <param name="position">World position of the sound source.</param>
    /// <param name="volume">Playback volume.</param>
    /// <param name="pitch">Playback pitch multiplier, where 1.0 is the asset's base pitch.</param>
    void PlaySoundAt(string soundId, Vector3 position, float volume, float pitch = 1.0f);

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

    /// <summary>
    /// Registers a sound asset with the audio system for later playback.
    /// </summary>
    /// <param name="soundId">Audio asset identifier.</param>
    /// <param name="assetPath">Path to the audio file.</param>
    /// <param name="gain">
    /// Per-asset volume multiplier applied to every playback of this sound, used to normalize
    /// files that were recorded louder or quieter than the others. Defaults to 1.0 (no change).
    /// </param>
    void RegisterSound(string soundId, string assetPath, float gain = 1.0f);
  }
}
