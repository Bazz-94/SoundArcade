namespace SoundArcade.Domain.RiverRun.Models;

/// <summary>
/// Configurable settings for RiverRun gameplay pacing, scoring, and collisions.
/// </summary>
/// <param name="StartingLives">Lives available when a run starts.</param>
/// <param name="StartingSpawnIntervalSeconds">Initial time between obstacle spawns.</param>
/// <param name="MinimumSpawnIntervalSeconds">Lower bound for spawn interval.</param>
/// <param name="SpawnIntervalDecayPerSecond">Per-second reduction of spawn interval.</param>
/// <param name="StartingObstacleSpeed">Initial obstacle speed.</param>
/// <param name="ObstacleSpeedGainPerSecond">Per-second increase in obstacle speed.</param>
/// <param name="SpawnZ">Initial spawn Z position.</param>
/// <param name="CollisionZWindow">Absolute Z-distance from player used for collision checks.</param>
/// <param name="ScoringPerSecond">Base score gain per second while playing.</param>
/// <param name="DodgeBonus">Bonus score gained when an obstacle is successfully dodged.</param>
/// <param name="ScoreAnnouncementStep">Milestone interval for score announcements.</param>
public sealed record RunSettings(
  int StartingLives,
  float StartingSpawnIntervalSeconds,
  float MinimumSpawnIntervalSeconds,
  float SpawnIntervalDecayPerSecond,
  float StartingObstacleSpeed,
  float ObstacleSpeedGainPerSecond,
  float SpawnZ,
  float CollisionZWindow,
  float ScoringPerSecond,
  int DodgeBonus,
  int ScoreAnnouncementStep)
{
  /// <summary>
  /// Gets default gameplay settings for RiverRun.
  /// </summary>
  public static RunSettings Default { get; } = new RunSettings(
    StartingLives: 3,
    StartingSpawnIntervalSeconds: 1.8f,
    MinimumSpawnIntervalSeconds: 0.55f,
    SpawnIntervalDecayPerSecond: 0.05f,
    StartingObstacleSpeed: 6.0f,
    ObstacleSpeedGainPerSecond: 0.4f,
    SpawnZ: 22.0f,
    CollisionZWindow: 0.7f,
    ScoringPerSecond: 18.0f,
    DodgeBonus: 10,
    ScoreAnnouncementStep: 100);
}
