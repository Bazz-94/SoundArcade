namespace SoundArcade.Abstractions
{
  using System;
  using System.Collections.Generic;

  /// <summary>
  /// A single persisted scoreboard entry.
  /// </summary>
  /// <param name="Name">Player name the score was saved under.</param>
  /// <param name="Score">Score achieved in the run.</param>
  /// <param name="AchievedAt">Moment the score was achieved, used to break ties.</param>
  public sealed record ScoreEntry(string Name, int Score, DateTimeOffset AchievedAt);

  /// <summary>
  /// Provides persistence for per-game scoreboards, keyed by game id.
  /// </summary>
  public interface IScoreboardStore
  {
    /// <summary>
    /// Loads the persisted entries for a game.
    /// </summary>
    /// <param name="gameId">Id of the game whose scoreboard to load.</param>
    /// <returns>Persisted entries, empty when nothing is stored.</returns>
    IReadOnlyList<ScoreEntry> Load(string gameId);

    /// <summary>
    /// Saves the entries for a game, replacing any previously stored entries.
    /// </summary>
    /// <param name="gameId">Id of the game whose scoreboard to save.</param>
    /// <param name="entries">Entries to persist.</param>
    void Save(string gameId, IReadOnlyList<ScoreEntry> entries);
  }
}
