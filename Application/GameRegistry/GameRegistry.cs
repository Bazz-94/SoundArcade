using System.Collections.Generic;
using System.Linq;
using SoundArcade.Domain;

namespace SoundArcade.Application.GameRegistry;

public sealed class GameRegistry
{
  private readonly IReadOnlyList<IGame> games;

  public GameRegistry(IEnumerable<IGame> games)
  {
    this.games = games.ToArray();
  }

  public IReadOnlyList<IGame> Games
    => games;
}
