namespace SoundArcade.Domain.RiverRun
{
  /// <summary>
  /// Registers the RiverRun mini-game in the arcade catalog.
  /// </summary>
  public sealed class RiverRunGame : IGame
  {
    private const string IdentityId = "river-run";
    private const string IdentityDisplayName = "RiverRun";

    /// <summary>
    /// Gets the game identity metadata.
    /// </summary>
    public GameIdentity Identity { get; } = new GameIdentity(IdentityId, IdentityDisplayName);
  }
}
