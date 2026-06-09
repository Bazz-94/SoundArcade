namespace SoundArcade.Domain.RiverRun.Models
{
  using System.Numerics;

  /// <summary>
  /// Represents one active obstacle in world space.
  /// Obstacles are stationary in world space; the player moves forward.
  /// </summary>
  public sealed class RunObstacle : GameObject
  {
    /// <summary>
    /// Initializes a new instance of <see cref="RunObstacle"/>.
    /// </summary>
    /// <param name="position">Initial world position for the obstacle.</param>
    public RunObstacle(Vector3 position)
      : base(position, collidable: true)
    {
    }
  }
}
