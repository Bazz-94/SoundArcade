namespace SoundArcade.Domain.RiverRun
{
  /// <summary>
  /// Registers the RiverRun mini-game in the arcade catalog.
  /// </summary>
  public sealed class RiverRunGame : IGame
  {
    /// <summary>
    /// Stable game id used for registration and per-game persistence such as scoreboards.
    /// </summary>
    public const string Id = "river-run";

    private const string IdentityDisplayName = "RiverRun";

    /// <summary>
    /// Gets the game identity metadata.
    /// </summary>
    public GameIdentity Identity { get; } = new GameIdentity(Id, IdentityDisplayName);
  }
}
