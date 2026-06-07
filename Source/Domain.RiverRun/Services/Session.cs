namespace SoundArcade.Domain.RiverRun.Services
{
  using System;
  using System.Collections.Generic;
  using System.Numerics;
  using SoundArcade.Domain.RiverRun.Models;
  using SoundArcade.Domain.Services;

  /// <summary>
  /// Represents one RiverRun gameplay session and enforces run state, scoring, and collision rules.
  /// </summary>
  public sealed class Session
  {
    private readonly RunSettings settings;
    private readonly ObstacleSpawner spawner;
    private readonly List<RunObstacle> obstacles = [];
    private float ScoreRemainder { get; set; }
    private int NextScoreAnnouncement { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Session"/> class.
    /// </summary>
    /// <param name="settings">Gameplay tuning settings.</param>
    /// <param name="random">Optional random source used by spawning services.</param>
    public Session(RunSettings settings, Random? random = null)
    {
      this.settings = settings;
      spawner = new ObstacleSpawner(this.settings, random);

      this.Player = new Player(
        new Vector3(RunConstants.LaneX.Center, RunConstants.GroundY, 0.0f),
        this.settings.StartingPlayerSpeed,
        this.settings.PlayerSpeedIncreasePerZUnit,
        this.settings.MaxPlayerSpeedIncrease);
      this.Lives = this.settings.StartingLives;
      this.NextScoreAnnouncement = this.settings.ScoreAnnouncementStep;
      this.State = RunState.GameOver;
    }

    /// <summary>
    /// Gets the current high-level run state.
    /// </summary>
    public RunState State { get; private set; }

    /// <summary>
    /// Gets the player actor.
    /// </summary>
    public Player Player { get; private set; }

    /// <summary>
    /// Gets the remaining player lives.
    /// </summary>
    public int Lives { get; private set; }

    /// <summary>
    /// Gets the current score.
    /// </summary>
    public int Score { get; private set; }

    /// <summary>
    /// Gets elapsed run time in seconds.
    /// </summary>
    public float ElapsedSeconds { get; private set; }

    /// <summary>
    /// Gets active obstacles currently tracked in world space.
    /// </summary>
    public IReadOnlyList<RunObstacle> Obstacles => obstacles;

    /// <summary>
    /// Starts a fresh run.
    /// </summary>
    /// <returns>Events that announce run startup state.</returns>
    public IReadOnlyList<RunEvent> Start()
    {
      obstacles.Clear();
      spawner.Reset();
      this.ElapsedSeconds = 0.0f;
      this.ScoreRemainder = 0.0f;
      this.Score = 0;
      this.Player = new Player(
        new Vector3(RunConstants.LaneX.Center, RunConstants.GroundY, 0.0f),
        settings.StartingPlayerSpeed,
        settings.PlayerSpeedIncreasePerZUnit,
        settings.MaxPlayerSpeedIncrease);
      this.Lives = settings.StartingLives;
      this.State = RunState.Playing;
      this.NextScoreAnnouncement = settings.ScoreAnnouncementStep;

      return
      [
        new TextToSpeechEvent(RunConstants.Speech.RunStarted),
        new PlaySoundEvent(RunConstants.SoundId.RunStart)
      ];
    }

    /// <summary>
    /// Applies a player command to the session state.
    /// </summary>
    /// <param name="command">Player command to process.</param>
    /// <returns>Events emitted by command handling.</returns>
    public IReadOnlyList<RunEvent> HandleCommand(RunCommand command)
    {
      List<RunEvent> events = new List<RunEvent>();

      switch (this.State)
      {
        case RunState.Playing:
          switch (command)
          {
            case RunCommand.TogglePause:
              this.State = RunState.Paused;
              events.Add(new TextToSpeechEvent(RunConstants.Speech.Paused));
              events.Add(new PlaySoundEvent(RunConstants.SoundId.Pause));
              break;
            case RunCommand.MoveLeft:
            case RunCommand.MoveRight:
              {
                this.Player.HandleCommand(command);
                break;
              }
          }
          break;

        case RunState.Paused:
          switch (command)
          {
            case RunCommand.TogglePause:
              this.State = RunState.Playing;
              events.Add(new TextToSpeechEvent(RunConstants.Speech.Resumed));
              events.Add(new PlaySoundEvent(RunConstants.SoundId.Resume));
              break;
          }
          break;
        case RunState.GameOver:
          switch (command)
          {
            case RunCommand.Restart:
              return this.Start();
          }
          break;

        default:
          throw new InvalidOperationException($"Unhandled run state {this.State}.");
      }

      return events;
    }

    /// <summary>
    /// Advances gameplay simulation by one frame.
    /// </summary>
    /// <param name="deltaTimeSeconds">Frame delta in seconds.</param>
    /// <returns>Events emitted during frame simulation.</returns>
    public IReadOnlyList<RunEvent> Update(float deltaTimeSeconds)
    {
      if (this.State != RunState.Playing)
      {
        return Array.Empty<RunEvent>();
      }

      List<RunEvent> events = [];

      this.ElapsedSeconds += deltaTimeSeconds;
      this.ScoreRemainder += settings.ScoringPerSecond * deltaTimeSeconds;

      if (this.ScoreRemainder >= 1.0f)
      {
        int earned = (int)this.ScoreRemainder;
        this.Score += earned;
        this.ScoreRemainder -= earned;

        while (this.Score >= this.NextScoreAnnouncement)
        {
          events.Add(new TextToSpeechEvent(
            $"{RunConstants.Speech.ScorePrefix} {this.NextScoreAnnouncement}"));
          this.NextScoreAnnouncement += settings.ScoreAnnouncementStep;
        }
      }

      IReadOnlyList<RunObstacle> spawned = spawner.Update(this.Player.Position.Z);

      foreach (RunObstacle spawnedObstacle in spawned)
      {
        obstacles.Add(spawnedObstacle);
      }

      this.Player.Advance(deltaTimeSeconds);

      for (int i = 0; i < obstacles.Count; i++)
      {
        RunObstacle obstacle = obstacles[i];

        bool collides = ProximityCollision.IsWithinBuffer(obstacle.Position, this.Player.Position, settings.CollisionRadius);

        if (collides)
        {
          obstacles.RemoveAt(i);
          this.Lives--;
          events.Add(new TextToSpeechEvent($"Hit. {this.Lives} {RunConstants.Speech.LivesLeftSuffix}"));
          events.Add(new PlaySoundEvent(RunConstants.SoundId.Collision, null));

          if (this.Lives <= 0)
          {
            this.State = RunState.GameOver;
            events.Add(new TextToSpeechEvent($"{RunConstants.Speech.GameOverPrefix} {this.Score}"));
            events.Add(new PlaySoundEvent(RunConstants.SoundId.GameOver, null));
            break;
          }

          continue;
        }

        if (obstacle.Position.Z < this.Player.Position.Z)
        {
          obstacles.RemoveAt(i);
          continue;
        }
      }

      return events;
    }

    /// <summary>
    /// Adds a deterministic obstacle for tests or scripted scenarios.
    /// </summary>
    /// <param name="lane">Target lane index.</param>
    /// <param name="z">Initial Z position.</param>
    public void QueueObstacle(float lane, float z)
    {
      if (lane is < RunConstants.LaneX.Left or > RunConstants.LaneX.Right)
      {
        throw new ArgumentOutOfRangeException(nameof(lane));
      }
      Vector3 position = new Vector3(lane, RunConstants.GroundY, z);
      obstacles.Add(new RunObstacle(position));
    }

    /// <summary>
    /// Adds a deterministic obstacle using an explicit world position.
    /// </summary>
    /// <param name="position">Obstacle world position.</param>
    public void QueueObstacle(Vector3 position)
    {
      obstacles.Add(new RunObstacle(position));
    }
  }
}
