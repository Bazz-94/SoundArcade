using SoundArcade.Application.GameRegistry;
using SoundArcade.Domain;
using SoundArcade.Domain.RiverRun;
using Xunit;

namespace SoundArcade.Tests;

public sealed class GameRegistryTests
{
  [Fact]
  public void Registry_exposes_registered_games()
  {
    IGame[] games = [new RiverRunGame()];

    var registry = new GameRegistry(games);

    Assert.Single(registry.Games);
    Assert.Equal("river-run", registry.Games[0].Identity.Id);
  }
}
