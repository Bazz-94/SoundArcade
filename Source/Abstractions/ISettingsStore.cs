namespace SoundArcade.Abstractions
{
  using System;

  /// <summary>
  /// Provides persistence for application settings.
  /// </summary>
  public interface ISettingsStore
  {
    /// <summary>
    /// Loads persisted settings.
    /// </summary>
    /// <returns>Loaded settings or defaults when no file exists.</returns>
    AppSettings Load();

    /// <summary>
    /// Saves settings to persistent storage.
    /// </summary>
    /// <param name="settings">Settings to persist.</param>
    void Save(AppSettings settings);
  }

  /// <summary>
  /// Settings persisted for the desktop host.
  /// </summary>
  public sealed class AppSettings
  {
    private const float MinimumVolume = 0.0f;
    private const float MaximumVolume = 1.0f;

    /// <summary>
    /// Gets the master volume in the range 0 to 1.
    /// </summary>
    public float MasterVolume { get; private set; } = MaximumVolume;

    /// <summary>
    /// Gets the text-to-speech volume in the range 0 to 1.
    /// </summary>
    public float TtsVolume { get; private set; } = MaximumVolume;

    /// <summary>
    /// Sets the master volume, clamped to the range 0 to 1.
    /// </summary>
    /// <param name="volume">Master volume.</param>
    public void SetMasterVolume(float volume)
    {
      this.MasterVolume = Math.Clamp(volume, MinimumVolume, MaximumVolume);
    }

    /// <summary>
    /// Sets the text-to-speech volume, clamped to the range 0 to 1.
    /// </summary>
    /// <param name="volume">Text-to-speech volume.</param>
    public void SetTtsVolume(float volume)
    {
      this.TtsVolume = Math.Clamp(volume, MinimumVolume, MaximumVolume);
    }

    /// <summary>
    /// Copies all values from another settings instance.
    /// </summary>
    /// <param name="other">Settings to copy from.</param>
    public void CopyFrom(AppSettings other)
    {
      this.SetMasterVolume(other.MasterVolume);
      this.SetTtsVolume(other.TtsVolume);
    }
  }
}