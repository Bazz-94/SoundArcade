namespace SoundArcade.Domain.RiverRun.Models
{
  /// <summary>
  /// Configurable settings for RiverRun gameplay pacing, scoring, and collisions.
  /// </summary>
  public sealed record RiverRunSettings
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
    public float PickupSpawnDistanceMin { get; private set; }
    public float PickupSpawnDistanceMax { get; private set; }
    public float PickupSpawnZ { get; private set; }
    public int PickupScoreBonus { get; private set; }
    public float ApproachNoiseRadius { get; private set; }
    public float ApproachNoiseInterval { get; private set; }
    public float RiverNoiseInterval { get; private set; }
    public float RiverNoiseLaneOffset { get; private set; }

    /// <param name="StartingLives">Lives available when a run starts.</param>
    /// <param name="StartingPlayerSpeed">Speed at which the player starts moving.</param>
    /// <param name="PlayerSpeedIncreasePerZUnit">Speed gain applied per unit traveled on Z.</param>
    /// <param name="MaxPlayerSpeedIncrease">Maximum additional speed above the initial speed.</param>
    /// <param name="SpawnDistanceMin">Minimum distance between obstacle spawns.</param>
    /// <param name="SpawnDistanceMax">Maximum distance between obstacle spawns.</param>
    /// <param name="SpawnZ">Initial obstacle spawn Z position.</param>
    /// <param name="CollisionRadius">Absolute distance from player used for collision checks.</param>
    /// <param name="ScoringPerSecond">Base score gain per second while playing.</param>
    /// <param name="DodgeBonus">Bonus score gained when an obstacle is successfully dodged.</param>
    /// <param name="ScoreAnnouncementStep">Milestone interval for score announcements.</param>
    /// <param name="PickupSpawnDistanceMin">Minimum distance between pickup spawns.</param>
    /// <param name="PickupSpawnDistanceMax">Maximum distance between pickup spawns.</param>
    /// <param name="PickupSpawnZ">Initial pickup spawn Z position.</param>
    /// <param name="PickupScoreBonus">Score awarded when a pickup is collected.</param>
    /// <param name="ApproachNoiseRadius">Forward distance within which obstacles and pickups emit approach noise.</param>
    /// <param name="ApproachNoiseInterval">Minimum seconds between approach-noise cues for the same object.</param>
    /// <param name="RiverNoiseInterval">Seconds between ambient river noise cues on each side of the player.</param>
    /// <param name="RiverNoiseLaneOffset">World units beyond the outer lanes where river noise is positioned.</param>
    public RiverRunSettings(
      int StartingLives = 3,
      float StartingPlayerSpeed = 1.0f,
      float PlayerSpeedIncreasePerZUnit = 0.05f,
      float MaxPlayerSpeedIncrease = 3.0f,
      float SpawnDistanceMin = 4.0f,
      float SpawnDistanceMax = 8.0f,
      float SpawnZ = 22.0f,
      float CollisionRadius = 0.5f,
      float ScoringPerSecond = 1.0f,
      int ScoreAnnouncementStep = 100,
      float PickupSpawnDistanceMin = 6.0f,
      float PickupSpawnDistanceMax = 14.0f,
      float PickupSpawnZ = 22.0f,
      int PickupScoreBonus = 25,
      float ApproachNoiseRadius = 15.0f,
      float ApproachNoiseInterval = 0.75f,
      float RiverNoiseInterval = 2.0f,
      float RiverNoiseLaneOffset = 4.0f)
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
      this.ScoreAnnouncementStep = ScoreAnnouncementStep;
      this.PickupSpawnDistanceMin = PickupSpawnDistanceMin;
      this.PickupSpawnDistanceMax = PickupSpawnDistanceMax;
      this.PickupSpawnZ = PickupSpawnZ;
      this.PickupScoreBonus = PickupScoreBonus;
      this.ApproachNoiseRadius = ApproachNoiseRadius;
      this.ApproachNoiseInterval = ApproachNoiseInterval;
      this.RiverNoiseInterval = RiverNoiseInterval;
      this.RiverNoiseLaneOffset = RiverNoiseLaneOffset;
    }
  }
}
