namespace SoundArcade.Tests
{
  using SoundArcade.Application.GameRegistry;
  using SoundArcade.Domain;
  using SoundArcade.Domain.RiverRun;
  using Xunit;

  /// <summary>
  /// Tests for game registry behavior.
  /// </summary>
  public sealed class GameRegistryTests
  {
    private const int FirstGameIndex = 0;
    private const string RiverRunGameId = "river-run";

    /// <summary>
    /// Verifies the registry returns all games passed during construction.
    /// </summary>
    [Fact]
    public void Registry_exposes_registered_games()
    {
      IGame[] games = [new RiverRunGame()];

      GameRegistry registry = new GameRegistry(games);

      Assert.Single(registry.Games);
      Assert.Equal(RiverRunGameId, registry.Games[FirstGameIndex].Identity.Id);
    }
  }
}
