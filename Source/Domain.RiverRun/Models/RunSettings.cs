using System;

namespace SoundArcade.Domain.RiverRun.Models;

/// <summary>
/// Configurable settings for RiverRun gameplay pacing, scoring, and collisions.
/// </summary>
public sealed record RunSettings
{
  /// <summary>
  /// Lives available when a run starts.
  /// </summary>
  public int StartingLives { get; init; }

  /// <summary>
  /// Initial time between obstacle spawns.
  /// </summary>
  public float StartingSpawnIntervalSeconds { get; init; }

  /// <summary>
  /// Lower bound for spawn interval.
  /// </summary>
  public float MinimumSpawnIntervalSeconds { get; init; }

  /// <summary>
  /// Per-second reduction of spawn interval.
  /// </summary>
  public float SpawnIntervalDecayPerSecond { get; init; }

  /// <summary>
  /// Initial obstacle speed.
  /// </summary>
  public float StartingObstacleSpeed { get; init; }

  /// <summary>
  /// Per-second increase in obstacle speed.
  /// </summary>
  public float ObstacleSpeedGainPerSecond { get; init; }

  /// <summary>
  /// Initial spawn Z position.
  /// </summary>
  public float SpawnZ { get; init; }

  /// <summary>
  /// Absolute Z-distance from player used for collision checks.
  /// </summary>
  public float CollisionZWindow { get; init; }

  /// <summary>
  /// Base score gain per second while playing.
  /// </summary>
  public float ScoringPerSecond { get; init; }

  /// <summary>
  /// Bonus score gained when an obstacle is successfully dodged.
  /// </summary>
  public int DodgeBonus { get; init; }

  /// <summary>
  /// Milestone interval for score announcements. Must be greater than or equal to 0.
  /// Setting a negative value will throw an <see cref="ArgumentOutOfRangeException"/>.
  /// </summary>
  public int ScoreAnnouncementStep
  {
    get => this.ScoreAnnouncementStep;
    init
    {
      if (value < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(ScoreAnnouncementStep), "ScoreAnnouncementStep must be greater than or equal to 0.");
      }
      this.ScoreAnnouncementStep = value;
    }
  }

  /// <summary>
  /// Initializes a new instance of <see cref="RunSettings"/>.
  /// </summary>
  public RunSettings(
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
    this.StartingLives = StartingLives;
    this.StartingSpawnIntervalSeconds = StartingSpawnIntervalSeconds;
    this.MinimumSpawnIntervalSeconds = MinimumSpawnIntervalSeconds;
    this.SpawnIntervalDecayPerSecond = SpawnIntervalDecayPerSecond;
    this.StartingObstacleSpeed = StartingObstacleSpeed;
    this.ObstacleSpeedGainPerSecond = ObstacleSpeedGainPerSecond;
    this.SpawnZ = SpawnZ;
    this.CollisionZWindow = CollisionZWindow;
    this.ScoringPerSecond = ScoringPerSecond;
    this.DodgeBonus = DodgeBonus;
    this.ScoreAnnouncementStep = ScoreAnnouncementStep;
  }

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
