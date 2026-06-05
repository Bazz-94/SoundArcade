namespace SoundArcade.Domain.RiverRun.Models;

/// <summary>
/// Represents one active obstacle in world space.
/// </summary>
/// <param name="Lane">Lane index occupied by the obstacle.</param>
/// <param name="Z">Forward/backward world position.</param>
/// <param name="Speed">Obstacle movement speed along the Z axis.</param>
public readonly record struct RunObstacle(int Lane, float Z, float Speed);
