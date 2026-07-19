namespace SoundArcade.Infrastructure.Windows
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Text.Json;
  using SoundArcade.Abstractions;

  /// <summary>
  /// File-backed scoreboard persistence: one JSON file per game id in the settings directory.
  /// </summary>
  public sealed class FileScoreboardStore : IScoreboardStore
  {
    private const string FileNameFormat = "scoreboard-{0}.json";

    private readonly string? directoryOverride;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileScoreboardStore"/> class
    /// storing files in the application settings directory.
    /// </summary>
    public FileScoreboardStore()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileScoreboardStore"/> class
    /// storing files in a specific directory (used by tests).
    /// </summary>
    /// <param name="directory">Directory to store scoreboard files in.</param>
    public FileScoreboardStore(string directory)
    {
      this.directoryOverride = directory;
    }

    /// <inheritdoc />
    public IReadOnlyList<ScoreEntry> Load(string gameId)
    {
      string path = this.GetFilePath(gameId);

      if (!File.Exists(path))
      {
        return new List<ScoreEntry>();
      }

      try
      {
        return JsonSerializer.Deserialize<List<ScoreEntry>>(File.ReadAllText(path)) ?? new List<ScoreEntry>();
      }
      catch (JsonException)
      {
        // A corrupt scoreboard file must not prevent play; it is treated as empty.
        return new List<ScoreEntry>();
      }
    }

    /// <inheritdoc />
    public void Save(string gameId, IReadOnlyList<ScoreEntry> entries)
    {
      string path = this.GetFilePath(gameId);
      string? directoryPath = Path.GetDirectoryName(path);

      if (directoryPath is null)
      {
        return;
      }

      Directory.CreateDirectory(directoryPath);
      File.WriteAllText(path, JsonSerializer.Serialize(entries, new JsonSerializerOptions { WriteIndented = true }));
    }

    /// <summary>
    /// Gets the scoreboard file path for a game id.
    /// </summary>
    /// <param name="gameId">Id of the game.</param>
    /// <returns>Full path of the game's scoreboard file.</returns>
    private string GetFilePath(string gameId)
    {
      string fileName = string.Format(FileNameFormat, gameId.ToLowerInvariant());

      if (this.directoryOverride is null)
      {
        return SettingsPaths.GetPath(fileName);
      }

      return Path.Combine(this.directoryOverride, fileName);
    }
  }
}
