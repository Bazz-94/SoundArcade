namespace SoundArcade.Infrastructure.Windows
{
  using System;
  using System.IO;
  using System.Text.Json;
  using SoundArcade.Abstractions;

  /// <summary>
  /// File-based settings persistence for desktop host settings.
  /// </summary>
  public sealed class FileSettingsStore : ISettingsStore
  {
    private const string SettingsDirectoryName = "SoundArcade";
    private const string SettingsFileName = "settings.json";

    private readonly string settingsPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSettingsStore"/> class.
    /// </summary>
    public FileSettingsStore()
    {
      string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      string settingsDirectoryPath = Path.Combine(appDataPath, SettingsDirectoryName);
      settingsPath = Path.Combine(settingsDirectoryPath, SettingsFileName);
    }

    /// <inheritdoc />
    public AppSettings Load()
    {
      if (!File.Exists(settingsPath))
      {
        return new AppSettings();
      }

      string json = File.ReadAllText(settingsPath);
      AppSettings? settings = JsonSerializer.Deserialize<AppSettings>(json);

      if (settings is null)
      {
        return new AppSettings();
      }

      return settings;
    }

    /// <inheritdoc />
    public void Save(AppSettings settings)
    {
      string? settingsDirectoryPath = Path.GetDirectoryName(settingsPath);

      if (settingsDirectoryPath is null)
      {
        return;
      }

      Directory.CreateDirectory(settingsDirectoryPath);
      string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
      File.WriteAllText(settingsPath, json);
    }
  }
}