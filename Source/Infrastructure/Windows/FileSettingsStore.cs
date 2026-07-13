namespace SoundArcade.Infrastructure.Windows
{
  using System.IO;
  using System.Text.Json;
  using SoundArcade.Abstractions;

  /// <summary>
  /// File-based settings persistence for desktop host settings.
  /// </summary>
  public sealed class FileSettingsStore : ISettingsStore
  {
    private const string SettingsFileName = "settings.json";

    private readonly string settingsPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSettingsStore"/> class.
    /// </summary>
    public FileSettingsStore()
    {
      this.settingsPath = SettingsPaths.GetPath(SettingsFileName);
    }

    /// <inheritdoc />
    public AppSettings Load()
    {
      AppSettings settings = new AppSettings();

      if (!File.Exists(this.settingsPath))
      {
        return settings;
      }

      SettingsDocument? document;
      try
      {
        document = JsonSerializer.Deserialize<SettingsDocument>(File.ReadAllText(this.settingsPath));
      }
      catch (JsonException)
      {
        // A corrupt or hand-edited settings file must not prevent startup; defaults stay active.
        return settings;
      }

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
      string? settingsDirectoryPath = Path.GetDirectoryName(this.settingsPath);

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
      File.WriteAllText(this.settingsPath, JsonSerializer.Serialize(document, new JsonSerializerOptions { WriteIndented = true }));
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
