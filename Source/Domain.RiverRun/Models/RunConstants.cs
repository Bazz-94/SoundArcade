namespace SoundArcade.Domain.RiverRun.Models;

/// <summary>
/// Centralized constants used by the RiverRun domain model.
/// </summary>
public static class RunConstants
{
  /// <summary>
  /// Lane index constants used by the three-lane gameplay model.
  /// </summary>
  public static class Lane
  {
    /// <summary>
    /// Left lane index.
    /// </summary>
    public const int Left = 0;

    /// <summary>
    /// Center lane index.
    /// </summary>
    public const int Center = 1;

    /// <summary>
    /// Right lane index.
    /// </summary>
    public const int Right = 2;
  }

  /// <summary>
  /// X positions mapped to lane indices.
  /// </summary>
  public static class LaneX
  {
    /// <summary>
    /// Left lane X coordinate.
    /// </summary>
    public const float Left = -2.0f;

    /// <summary>
    /// Center lane X coordinate.
    /// </summary>
    public const float Center = 0.0f;

    /// <summary>
    /// Right lane X coordinate.
    /// </summary>
    public const float Right = 2.0f;
  }

  /// <summary>
  /// Shared Y plane used by the core gameplay loop.
  /// </summary>
  public const float GroundY = 0.0f;

  /// <summary>
  /// Z threshold where an obstacle is considered safely passed.
  /// </summary>
  public const float PassedObstacleZ = -1.0f;

  /// <summary>
  /// Sound IDs emitted by the gameplay loop.
  /// </summary>
  public static class SoundId
  {
    /// <summary>
    /// Sound played when a run starts.
    /// </summary>
    public const string RunStart = "run-start";

    /// <summary>
    /// Sound played for lane centering at run start.
    /// </summary>
    public const string LaneCenter = "lane-center";

    /// <summary>
    /// Sound played on pause.
    /// </summary>
    public const string Pause = "pause";

    /// <summary>
    /// Sound played on resume.
    /// </summary>
    public const string Resume = "resume";

    /// <summary>
    /// Sound played when the player changes lane.
    /// </summary>
    public const string LaneChange = "lane-change";

    /// <summary>
    /// Sound played when an obstacle spawns.
    /// </summary>
    public const string ObstacleSpawn = "obstacle-spawn";

    /// <summary>
    /// Sound played on collision.
    /// </summary>
    public const string Collision = "collision";

    /// <summary>
    /// Sound played at score milestones.
    /// </summary>
    public const string ScoreMilestone = "score-milestone";

    /// <summary>
    /// Sound played on game over.
    /// </summary>
    public const string GameOver = "game-over";
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
  }
}