namespace SoundArcade.Domain.RiverRun.Models
{
  /// <summary>
  /// Configurable settings for RiverRun gameplay pacing, scoring, and collisions.
  /// </summary>
  public sealed record RiverRunSettings
  {
    /// <summary>
    /// Gets the lives available when a run starts.
    /// </summary>
    public int StartingLives { get; private set; }

    /// <summary>
    /// Gets the speed at which the player starts moving.
    /// </summary>
    public float StartingPlayerSpeed { get; private set; }

    /// <summary>
    /// Gets the speed gain applied per unit traveled on Z.
    /// </summary>
    public float PlayerSpeedIncreasePerZUnit { get; private set; }

    /// <summary>
    /// Gets the maximum additional speed above the initial speed.
    /// </summary>
    public float MaxPlayerSpeedIncrease { get; private set; }

    /// <summary>
    /// Gets the minimum distance between object spawns.
    /// </summary>
    public float SpawnDistanceMin { get; private set; }

    /// <summary>
    /// Gets the maximum distance between object spawns.
    /// </summary>
    public float SpawnDistanceMax { get; private set; }

    /// <summary>
    /// Gets the distance ahead of the player at which objects spawn.
    /// </summary>
    public float SpawnZ { get; private set; }

    /// <summary>
    /// Gets the absolute distance from the player used for collision checks.
    /// </summary>
    public float CollisionRadius { get; private set; }

    /// <summary>
    /// Gets the base score gain per second while playing.
    /// </summary>
    public float ScoringPerSecond { get; private set; }

    /// <summary>
    /// Gets the milestone interval for score announcements.
    /// </summary>
    public int ScoreAnnouncementStep { get; private set; }

    /// <summary>
    /// Gets the probability (0..1) that a spawn slot produces a pickup instead of an obstacle.
    /// </summary>
    public float PickupSpawnChance { get; private set; }

    /// <summary>
    /// Gets the score awarded when a pickup is collected.
    /// </summary>
    public int PickupScoreBonus { get; private set; }

    /// <summary>
    /// Gets the forward distance within which obstacles and pickups emit approach noise.
    /// </summary>
    public float ApproachNoiseRadius { get; private set; }

    /// <summary>
    /// Gets the minimum seconds between approach-noise cues for the same object.
    /// </summary>
    public float ApproachNoiseInterval { get; private set; }

    /// <summary>
    /// Gets the seconds between ambient river noise cues on each side of the player.
    /// </summary>
    public float RiverNoiseInterval { get; private set; }

    /// <summary>
    /// Gets the world units beyond the outer lanes where river noise is positioned.
    /// </summary>
    public float RiverNoiseLaneOffset { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RiverRunSettings"/> record.
    /// </summary>
    /// <param name="startingLives">Lives available when a run starts.</param>
    /// <param name="startingPlayerSpeed">Speed at which the player starts moving.</param>
    /// <param name="playerSpeedIncreasePerZUnit">Speed gain applied per unit traveled on Z.</param>
    /// <param name="maxPlayerSpeedIncrease">Maximum additional speed above the initial speed.</param>
    /// <param name="spawnDistanceMin">Minimum distance between obstacle spawns.</param>
    /// <param name="spawnDistanceMax">Maximum distance between obstacle spawns.</param>
    /// <param name="spawnZ">Initial obstacle spawn Z position.</param>
    /// <param name="collisionRadius">Absolute distance from player used for collision checks.</param>
    /// <param name="scoringPerSecond">Base score gain per second while playing.</param>
    /// <param name="scoreAnnouncementStep">Milestone interval for score announcements.</param>
    /// <param name="pickupSpawnChance">Probability (0..1) that a spawn slot produces a pickup instead of an obstacle.</param>
    /// <param name="pickupScoreBonus">Score awarded when a pickup is collected.</param>
    /// <param name="approachNoiseRadius">Forward distance within which obstacles and pickups emit approach noise.</param>
    /// <param name="approachNoiseInterval">Minimum seconds between approach-noise cues for the same object.</param>
    /// <param name="riverNoiseInterval">Seconds between ambient river noise cues on each side of the player.</param>
    /// <param name="riverNoiseLaneOffset">World units beyond the outer lanes where river noise is positioned.</param>
    public RiverRunSettings(
      int startingLives = 3,
      float startingPlayerSpeed = 1.0f,
      float playerSpeedIncreasePerZUnit = 0.05f,
      float maxPlayerSpeedIncrease = 3.0f,
      float spawnDistanceMin = 4.0f,
      float spawnDistanceMax = 8.0f,
      float spawnZ = 22.0f,
      float collisionRadius = 0.5f,
      float scoringPerSecond = 1.0f,
      int scoreAnnouncementStep = 100,
      float pickupSpawnChance = 0.35f,
      int pickupScoreBonus = 25,
      float approachNoiseRadius = 15.0f,
      float approachNoiseInterval = 0.75f,
      float riverNoiseInterval = 2.0f,
      float riverNoiseLaneOffset = 4.0f)
    {
      this.StartingLives = startingLives;
      this.StartingPlayerSpeed = startingPlayerSpeed;
      this.PlayerSpeedIncreasePerZUnit = playerSpeedIncreasePerZUnit;
      this.MaxPlayerSpeedIncrease = maxPlayerSpeedIncrease;
      this.SpawnDistanceMin = spawnDistanceMin;
      this.SpawnDistanceMax = spawnDistanceMax;
      this.SpawnZ = spawnZ;
      this.CollisionRadius = collisionRadius;
      this.ScoringPerSecond = scoringPerSecond;
      this.ScoreAnnouncementStep = scoreAnnouncementStep;
      this.PickupSpawnChance = pickupSpawnChance;
      this.PickupScoreBonus = pickupScoreBonus;
      this.ApproachNoiseRadius = approachNoiseRadius;
      this.ApproachNoiseInterval = approachNoiseInterval;
      this.RiverNoiseInterval = riverNoiseInterval;
      this.RiverNoiseLaneOffset = riverNoiseLaneOffset;
    }
  }
}
