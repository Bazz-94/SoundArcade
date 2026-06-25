namespace SoundArcade.Domain.RiverRun.Models
{
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
      /// Width of one lane in world units.
      /// </summary>
      public const float LaneWidth = 1.0f;

      /// <summary>
      /// Left lane X coordinate.
      /// </summary>
      public const float Left = -1.0f;

      /// <summary>
      /// Center lane X coordinate.
      /// </summary>
      public const float Center = 0.0f;

      /// <summary>
      /// Right lane X coordinate.
      /// </summary>
      public const float Right = 1.0f;
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
      /// Lane cue volume.
      /// </summary>
      public const float LaneCue = 0.7f;

      /// <summary>
      /// Lane change volume.
      /// </summary>
      public const float LaneChange = 0.75f;

      /// <summary>
      /// Score milestone volume.
      /// </summary>
      public const float ScoreMilestone = 0.8f;

      /// <summary>
      /// Obstacle spawn volume.
      /// </summary>
      public const float ObstacleSpawn = 0.9f;

      /// <summary>
      /// Loudest volume for obstacle and pickup approach noise cues, used when the object is right next to the player.
      /// </summary>
      public const float ApproachNoiseMax = 0.6f;

      /// <summary>
      /// Quietest volume for obstacle and pickup approach noise cues, used at the edge of the approach noise radius.
      /// </summary>
      public const float ApproachNoiseMin = 0.15f;

      /// <summary>
      /// Volume for the ambient river noise either side of the player.
      /// </summary>
      public const float RiverAmbient = 0.4f;

      /// <summary>
      /// Volume for pickup collection feedback.
      /// </summary>
      public const float PickupCollected = 0.9f;
    }
  }
}
