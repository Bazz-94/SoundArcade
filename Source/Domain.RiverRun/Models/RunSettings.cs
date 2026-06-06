namespace SoundArcade.Domain.RiverRun.Models;

/// <summary>
/// Configurable settings for RiverRun gameplay pacing, scoring, and collisions.
/// </summary>
public sealed record RunSettings
{
  public int StartingLives { get; private set; }
  public float StartingPlayerSpeed { get; private set; }
  public float PlayerSpeedIncreasePerZUnit { get; private set; }
  public float MaxPlayerSpeedIncrease { get; private set; }
  public float SpawnDistanceMin { get; private set; }
  public float SpawnDistanceMax { get; private set; }
  public float SpawnZ { get; private set; }
  public float CollisionRadius { get; private set; }
  public float ScoringPerSecond { get; private set; }
  public int DodgeBonus { get; private set; }
  public int ScoreAnnouncementStep { get; private set; }


  /// <param name="StartingLives">Lives available when a run starts.</param>
  /// <param name="StartingPlayerSpeed">Speed at which the player starts moving.</param>
  /// <param name="PlayerSpeedIncreasePerZUnit">Speed gain applied per unit traveled on Z.</param>
  /// <param name="MaxPlayerSpeedIncrease">Maximum additional speed above the initial speed.</param>
  /// <param name="SpawnDistanceMin">Minimum distance between spawns.</param>
  /// <param name="SpawnDistanceMax">Maximum distance between spawns.</param>
  /// <param name="SpawnZ">Initial spawn Z position.</param>
  /// <param name="CollisionRadius">Absolute distance from player used for collision checks.</param>
  /// <param name="ScoringPerSecond">Base score gain per second while playing.</param>
  /// <param name="DodgeBonus">Bonus score gained when an obstacle is successfully dodged.</param>
  /// <param name="ScoreAnnouncementStep">Milestone interval for score announcements.</param>
  public RunSettings(
    int StartingLives = 3,
    float StartingPlayerSpeed = 1.0f,
    float PlayerSpeedIncreasePerZUnit = 0.05f,
    float MaxPlayerSpeedIncrease = 3.0f,
    float SpawnDistanceMin = 4.0f,
    float SpawnDistanceMax = 8.0f,
    float SpawnZ = 22.0f,
    float CollisionRadius = 1f,
    float ScoringPerSecond = 18.0f,
    int DodgeBonus = 10,
    int ScoreAnnouncementStep = 100)
  {
    this.StartingLives = StartingLives;
    this.StartingPlayerSpeed = StartingPlayerSpeed;
    this.PlayerSpeedIncreasePerZUnit = PlayerSpeedIncreasePerZUnit;
    this.MaxPlayerSpeedIncrease = MaxPlayerSpeedIncrease;
    this.SpawnDistanceMin = SpawnDistanceMin;
    this.SpawnDistanceMax = SpawnDistanceMax;
    this.SpawnZ = SpawnZ;
    this.CollisionRadius = CollisionRadius;
    this.ScoringPerSecond = ScoringPerSecond;
    this.DodgeBonus = DodgeBonus;
    this.ScoreAnnouncementStep = ScoreAnnouncementStep;
  }
}
