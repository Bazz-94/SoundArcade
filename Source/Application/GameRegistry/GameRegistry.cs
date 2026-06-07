namespace SoundArcade.Application.GameRegistry
{
  using System.Collections.Generic;
  using System.Linq;
  using SoundArcade.Domain;

  /// <summary>
  /// Stores registered mini-games for discovery and menu presentation.
  /// </summary>
  public sealed class GameRegistry
  {
    private readonly IReadOnlyList<IGame> games;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameRegistry"/> class.
    /// </summary>
    /// <param name="games">Games to expose through the registry.</param>
    public GameRegistry(IEnumerable<IGame> games)
    {
      this.games = games.ToArray();
    }

    /// <summary>
    /// Gets all registered games.
    /// </summary>
    public IReadOnlyList<IGame> Games
      => games;
  }
}
