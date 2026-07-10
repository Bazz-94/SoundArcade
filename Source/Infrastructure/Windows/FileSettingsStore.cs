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
      AppSettings settings = new AppSettings();

      if (!File.Exists(settingsPath))
      {
        return settings;
      }

      string json = File.ReadAllText(settingsPath);
      SettingsDocument? document = JsonSerializer.Deserialize<SettingsDocument>(json);

      if (document is not null)
      {
        settings.SetMasterVolume(document.MasterVolume);
        settings.SetTtsVolume(document.TtsVolume);
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
      SettingsDocument document = new SettingsDocument
      {
        MasterVolume = settings.MasterVolume,
        TtsVolume = settings.TtsVolume
      };
      string json = JsonSerializer.Serialize(document, new JsonSerializerOptions { WriteIndented = true });
      File.WriteAllText(settingsPath, json);
    }

    /// <summary>
    /// Serialization shape for the settings file.
    /// </summary>
    private sealed record SettingsDocument
    {
      /// <summary>
      /// Gets or sets the master volume.
      /// </summary>
      public float MasterVolume { get; set; } = 1.0f;

      /// <summary>
      /// Gets or sets the text-to-speech volume.
      /// </summary>
      public float TtsVolume { get; set; } = 1.0f;
    }
  }
}