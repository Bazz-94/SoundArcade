namespace SoundArcade.Domain.Models
{
  using System.Collections.Generic;
  using System.Linq;
  using SoundArcade.Abstractions;

  /// <summary>
  /// Top-10 scoreboard: entries ordered highest score first, ties broken by earliest achieved.
  /// </summary>
  public sealed class Scoreboard
  {
    private const int MaxEntries = 10;

    private readonly List<ScoreEntry> entries;

    /// <summary>
    /// Initializes a new instance of the <see cref="Scoreboard"/> class,
    /// normalizing the given entries by sorting and trimming to the top ten.
    /// </summary>
    /// <param name="entries">Previously persisted entries, in any order.</param>
    public Scoreboard(IReadOnlyList<ScoreEntry> entries)
    {
      this.entries = Scoreboard.Normalize(entries);
    }

    /// <summary>
    /// Gets the entries, highest score first, at most ten.
    /// </summary>
    public IReadOnlyList<ScoreEntry> Entries => this.entries;

    /// <summary>
    /// Adds an entry, keeping only the top ten after re-sorting.
    /// </summary>
    /// <param name="entry">Entry to add.</param>
    public void Add(ScoreEntry entry)
    {
      List<ScoreEntry> updated = new List<ScoreEntry>(this.entries) { entry };

      this.entries.Clear();
      this.entries.AddRange(Scoreboard.Normalize(updated));
    }

    /// <summary>
    /// Sorts entries highest score first (ties earliest first) and trims to the top ten.
    /// </summary>
    /// <param name="entries">Entries to normalize.</param>
    /// <returns>Sorted and trimmed entries.</returns>
    private static List<ScoreEntry> Normalize(IReadOnlyList<ScoreEntry> entries)
    {
      return entries
        .OrderByDescending(entry => entry.Score)
        .ThenBy(entry => entry.AchievedAt)
        .Take(MaxEntries)
        .ToList();
    }
  }
}
