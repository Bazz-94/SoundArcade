namespace SoundArcade.Tests
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using SoundArcade.Abstractions;
  using SoundArcade.Infrastructure.Windows;
  using Xunit;

  /// <summary>
  /// Tests for <see cref="FileScoreboardStore"/> persistence.
  /// </summary>
  public sealed class FileScoreboardStoreTests : IDisposable
  {
    private const string GameId = "riverrun";

    private readonly string directory;
    private readonly FileScoreboardStore store;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileScoreboardStoreTests"/> class
    /// with a store rooted in a unique temporary directory.
    /// </summary>
    public FileScoreboardStoreTests()
    {
      this.directory = Path.Combine(Path.GetTempPath(), $"soundarcade-tests-{Guid.NewGuid():N}");
      this.store = new FileScoreboardStore(this.directory);
    }

    /// <summary>
    /// Saved entries load back identically.
    /// </summary>
    [Fact]
    public void SaveThenLoad_RoundTripsEntries()
    {
      List<ScoreEntry> entries = new List<ScoreEntry>
      {
        new ScoreEntry("Alice", 42, new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero)),
        new ScoreEntry("Bob", 17, new DateTimeOffset(2026, 2, 2, 8, 30, 0, TimeSpan.Zero))
      };

      this.store.Save(GameId, entries);

      Assert.Equal(entries, this.store.Load(GameId));
    }

    /// <summary>
    /// A missing file loads as an empty scoreboard.
    /// </summary>
    [Fact]
    public void Load_MissingFileReturnsEmpty()
    {
      Assert.Empty(this.store.Load(GameId));
    }

    /// <summary>
    /// Malformed JSON loads as an empty scoreboard without throwing.
    /// </summary>
    [Fact]
    public void Load_CorruptFileReturnsEmpty()
    {
      Directory.CreateDirectory(this.directory);
      File.WriteAllText(Path.Combine(this.directory, $"scoreboard-{GameId}.json"), "{ not valid json");

      Assert.Empty(this.store.Load(GameId));
    }

    /// <summary>
    /// Different game ids persist to independent files.
    /// </summary>
    [Fact]
    public void Save_GameIdsAreIndependent()
    {
      this.store.Save(GameId, new List<ScoreEntry> { new ScoreEntry("Alice", 1, DateTimeOffset.UnixEpoch) });

      Assert.Empty(this.store.Load("othergame"));
    }

    /// <inheritdoc />
    public void Dispose()
    {
      if (Directory.Exists(this.directory))
      {
        Directory.Delete(this.directory, true);
      }
    }
  }
}
