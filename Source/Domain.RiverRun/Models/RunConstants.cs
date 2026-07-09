namespace SoundArcade.Domain.RiverRun.Models
{
  using SoundArcade.Abstractions;

  /// <summary>
  /// Centralized constants used by the RiverRun domain model.
  /// </summary>
  public static class RunConstants
  {
    /// <summary>
    /// X positions mapped to lane indices.
    /// </summary>
    public static class LaneX
    {
      /// <summary>
      /// Width of one lane in world units. This is also the lateral distance between adjacent lanes,
      /// so larger values spread the lanes further apart and sharpen the left/right stereo separation
      /// of obstacles and pickups. Keep <see cref="Left"/> and <see cref="Right"/> equal to ±this value.
      /// </summary>
      public const float LaneWidth = 3.0f;

      /// <summary>
      /// Left lane X coordinate. Positive X renders on the screen-left under the follow camera.
      /// </summary>
      public const float Left = LaneWidth;

      /// <summary>
      /// Center lane X coordinate.
      /// </summary>
      public const float Center = 0.0f;

      /// <summary>
      /// Right lane X coordinate. Negative X renders on the screen-right under the follow camera.
      /// </summary>
      public const float Right = -LaneWidth;
    }

    /// <summary>
    /// Shared Y plane used by the core gameplay loop.
    /// </summary>
    public const float GroundY = 0.0f;

    /// <summary>
    /// Sound IDs emitted by the gameplay loop.
    /// </summary>
    public static class SoundId
    {
      public const string RiverNoise = "river_noise";
      public const string ObstacleNoise = "obstacle_noise";
      public const string RewardNoise = "reward_noise";
      public const string Collision = "collision";
      public const string Reward = "reward";
    }

    /// <summary>
    /// Spoken labels used by gameplay events.
    /// </summary>
    public static class Speech
    {
      /// <summary>
      /// Spoken text for run start.
      /// </summary>
      public const string RunStarted = "Run started";

      /// <summary>
      /// Spoken text for pause.
      /// </summary>
      public const string Paused = "Paused";

      /// <summary>
      /// Spoken text for resume.
      /// </summary>
      public const string Resumed = "Resumed";

      /// <summary>
      /// Spoken prefix used for score milestone announcements.
      /// </summary>
      public const string ScorePrefix = "Score";

      /// <summary>
      /// Spoken suffix used when announcing remaining lives after a collision.
      /// </summary>
      public const string LivesLeftSuffix = "lives left";

      /// <summary>
      /// Spoken prefix used for game-over announcements.
      /// </summary>
      public const string GameOverPrefix = "Game over. Final score";

      /// <summary>
      /// Spoken prefix used when a pickup is collected.
      /// </summary>
      public const string PickupPrefix = "Pickup. Plus";
    }

    /// <summary>
    /// Shared event volumes.
    /// </summary>
    public static class Volume
    {
      /// <summary>
      /// Score milestone volume.
      /// </summary>
      public const float ScoreMilestone = 0.8f;

      /// <summary>
      /// Loudest volume for obstacle and pickup approach noise cues, used when the object is right next to the player.
      /// </summary>
      public const float ApproachNoiseMax = 0.6f;

      /// <summary>
      /// Quietest volume for obstacle and pickup approach noise cues, used at the edge of the approach noise radius.
      /// </summary>
      public const float ApproachNoiseMin = 0.01f;

      /// <summary>
      /// Volume for the ambient river noise either side of the player.
      /// </summary>
      public const float RiverAmbient = 0.01f;

      /// <summary>
      /// Volume for pickup collection feedback.
      /// </summary>
      public const float PickupCollected = 0.9f;
    }

    /// <summary>
    /// Per-asset gain multipliers applied at registration to normalize source files that were
    /// recorded at different loudness levels. Tune these so every cue sits at a comparable level;
    /// 1.0 leaves a file unchanged.
    /// </summary>
    public static class Gain
    {
      /// <summary>
      /// Gain for the obstacle collision sound.
      /// </summary>
      public const float Collision = 1.0f;

      /// <summary>
      /// Gain for the obstacle approach noise.
      /// </summary>
      public const float ObstacleNoise = 1.0f;

      /// <summary>
      /// Gain for the pickup collection sound.
      /// </summary>
      public const float Reward = 1.0f;

      /// <summary>
      /// Gain for the pickup approach noise. Generated at full level, so no boost is needed.
      /// </summary>
      public const float RewardNoise = 1.0f;

      /// <summary>
      /// Gain for the ambient river noise.
      /// </summary>
      public const float RiverNoise = 0.5f;
    }

    /// <summary>
    /// Profiles for procedurally generated sounds. Obstacle cues are synthesized at runtime
    /// instead of loaded from audio files; tune tone character here.
    /// </summary>
    public static class SoundProfiles
    {
      /// <summary>
      /// Obstacle approach noise: a short buzzy square tone. The session's volume and pitch
      /// ramps are applied on top of this base tone at playback time.
      /// </summary>
      public static readonly SoundProfile ObstacleNoise = new SoundProfile(Waveform.Square, 220.0f, 0.15f, Gain.ObstacleNoise);

      /// <summary>
      /// Obstacle collision impact: a longer, low raspy sawtooth tone.
      /// </summary>
      public static readonly SoundProfile Collision = new SoundProfile(Waveform.Sawtooth, 110.0f, 0.3f, Gain.Collision);

      /// <summary>
      /// Pickup approach noise: a short soft sine tone, brighter than the obstacle buzz so the
      /// two cues are distinguishable by timbre alone.
      /// </summary>
      public static readonly SoundProfile RewardNoise = new SoundProfile(Waveform.Sine, 660.0f, 0.15f, Gain.RewardNoise);

      /// <summary>
      /// Pickup collection feedback: a longer high sine chime.
      /// </summary>
      public static readonly SoundProfile Reward = new SoundProfile(Waveform.Sine, 880.0f, 0.3f, Gain.Reward);
    }

    /// <summary>
    /// Playback pitch multipliers, where 1.0 is the asset's base pitch.
    /// </summary>
    public static class Pitch
    {
      /// <summary>
      /// Pitch of the obstacle approach noise at the far edge of the approach radius.
      /// </summary>
      public const float ObstacleNoiseFar = 1.0f;

      /// <summary>
      /// Pitch of the obstacle approach noise when the obstacle is right next to the player. Higher
      /// than <see cref="ObstacleNoiseFar"/> so a rising pitch signals an obstacle closing in.
      /// </summary>
      public const float ObstacleNoiseNear = 1.25f;

      /// <summary>
      /// Fraction of the approach noise radius over which the obstacle pitch ramps up. The pitch stays
      /// at <see cref="ObstacleNoiseFar"/> until the obstacle is within this fraction of the radius,
      /// then rises to <see cref="ObstacleNoiseNear"/> as it reaches the player. 0.25 = last quarter.
      /// </summary>
      public const float ObstacleNoiseRampFraction = 0.25f;
    }
  }
}
