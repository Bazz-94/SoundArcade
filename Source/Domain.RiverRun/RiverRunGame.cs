using SoundArcade.Domain;

namespace SoundArcade.Domain.RiverRun;

public sealed class RiverRunGame : IGame
{
  public GameIdentity Identity { get; } = new("river-run", "RiverRun");
}
