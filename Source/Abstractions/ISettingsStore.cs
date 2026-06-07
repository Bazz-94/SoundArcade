namespace SoundArcade.Abstractions
{
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
  public sealed record AppSettings
  {
    /// <summary>
    /// Gets or sets the master volume in the range 0 to 1.
    /// </summary>
    public float MasterVolume { get; set; } = 1.0f;
  }
}