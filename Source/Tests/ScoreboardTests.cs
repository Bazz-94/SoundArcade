namespace SoundArcade.Tests
{
  using System;
  using System.Collections.Generic;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Models;
  using Xunit;

  /// <summary>
  /// Tests for the <see cref="Scoreboard"/> top-10 rules.
  /// </summary>
  public sealed class ScoreboardTests
  {
    private static readonly DateTimeOffset BaseTime = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

    /// <summary>
    /// Added entries are ordered highest score first.
    /// </summary>
    [Fact]
    public void Add_OrdersHighestScoreFirst()
    {
      Scoreboard scoreboard = new Scoreboard(new List<ScoreEntry>());
      scoreboard.Add(new ScoreEntry("Low", 10, BaseTime));
      scoreboard.Add(new ScoreEntry("High", 30, BaseTime));
      scoreboard.Add(new ScoreEntry("Mid", 20, BaseTime));

      Assert.Equal(new List<string> { "High", "Mid", "Low" }, ScoreboardTests.Names(scoreboard));
    }

    /// <summary>
    /// The scoreboard keeps only the ten best entries; the lowest is dropped on overflow.
    /// </summary>
    [Fact]
    public void Add_TrimsToTopTen()
    {
      Scoreboard scoreboard = new Scoreboard(new List<ScoreEntry>());

      foreach (int score in new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })
      {
        scoreboard.Add(new ScoreEntry($"Player{score}", score, BaseTime));
      }

      scoreboard.Add(new ScoreEntry("Newcomer", 11, BaseTime));

      Assert.Equal(10, scoreboard.Entries.Count);
      Assert.Equal("Newcomer", scoreboard.Entries[0].Name);
      Assert.DoesNotContain(scoreboard.Entries, entry => entry.Name == "Player1");
    }

    /// <summary>
    /// An entry that does not beat the current top ten is not kept.
    /// </summary>
    [Fact]
    public void Add_TooLowEntryOnFullBoardIsDropped()
    {
      Scoreboard scoreboard = new Scoreboard(new List<ScoreEntry>());

      foreach (int score in new List<int> { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 })
      {
        scoreboard.Add(new ScoreEntry($"Player{score}", score, BaseTime));
      }

      scoreboard.Add(new ScoreEntry("TooLow", 5, BaseTime));

      Assert.Equal(10, scoreboard.Entries.Count);
      Assert.DoesNotContain(scoreboard.Entries, entry => entry.Name == "TooLow");
    }

    /// <summary>
    /// Equal scores are ordered by earliest achieved first.
    /// </summary>
    [Fact]
    public void Add_TiesBrokenByEarliestAchieved()
    {
      Scoreboard scoreboard = new Scoreboard(new List<ScoreEntry>());
      scoreboard.Add(new ScoreEntry("Later", 50, BaseTime.AddMinutes(5)));
      scoreboard.Add(new ScoreEntry("Earlier", 50, BaseTime));

      Assert.Equal(new List<string> { "Earlier", "Later" }, ScoreboardTests.Names(scoreboard));
    }

    /// <summary>
    /// Loaded entries are normalized: sorted and trimmed to the top ten.
    /// </summary>
    [Fact]
    public void Constructor_NormalizesLoadedEntries()
    {
      List<ScoreEntry> loaded = new List<ScoreEntry>();

      foreach (int score in new List<int> { 3, 11, 7, 1, 9, 5, 12, 2, 8, 6, 4 })
      {
        loaded.Add(new ScoreEntry($"Player{score}", score, BaseTime));
      }

      Scoreboard scoreboard = new Scoreboard(loaded);

      Assert.Equal(10, scoreboard.Entries.Count);
      Assert.Equal("Player12", scoreboard.Entries[0].Name);
      Assert.Equal("Player2", scoreboard.Entries[9].Name);
    }

    private static List<string> Names(Scoreboard scoreboard)
    {
      List<string> names = new List<string>();

      foreach (ScoreEntry entry in scoreboard.Entries)
      {
        names.Add(entry.Name);
      }

      return names;
    }
  }
}
