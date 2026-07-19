namespace SoundArcade.Tests.Mock
{
  using System.Collections.Generic;
  using SoundArcade.Abstractions;

  /// <summary>
  /// Mock <see cref="IScoreboardStore"/> test double keeping entries in memory per game id.
  /// </summary>
  public sealed class MockScoreboardStore : IScoreboardStore
  {
    private readonly Dictionary<string, IReadOnlyList<ScoreEntry>> scoreboards = new Dictionary<string, IReadOnlyList<ScoreEntry>>();

    public IReadOnlyList<ScoreEntry> Load(string gameId)
    {
      return this.scoreboards.TryGetValue(gameId, out IReadOnlyList<ScoreEntry>? entries) ? entries : new List<ScoreEntry>();
    }

    public void Save(string gameId, IReadOnlyList<ScoreEntry> entries)
    {
      this.scoreboards[gameId] = entries;
    }
  }
}
