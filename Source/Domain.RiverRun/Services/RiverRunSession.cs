using System;
using System.Collections.Generic;
using System.Numerics;
using SoundArcade.Domain.RiverRun.Models;

namespace SoundArcade.Domain.RiverRun.Services;

/// <summary>
/// Represents one RiverRun gameplay session and enforces run state, scoring, and collision rules.
/// </summary>
public sealed class RiverRunSession
{
  private readonly RunSettings settings;
  private readonly RunSpawner spawner;
  private readonly List<RunObstacle> obstacles = [];

  private float elapsedSeconds;
  private float scoreRemainder;
  private int nextScoreAnnouncement;

  /// <summary>
  /// Initializes a new instance of the <see cref="RiverRunSession"/> class.
  /// </summary>
  /// <param name="settings">Optional gameplay tuning settings.</param>
  /// <param name="random">Optional random source used by spawning services.</param>
  public RiverRunSession(RunSettings? settings = null, Random? random = null)
  {
    this.settings = settings ?? RunSettings.Default;
    spawner = new RunSpawner(this.settings, random);

    Lane = RunConstants.Lane.Center;
    Lives = this.settings.StartingLives;
    nextScoreAnnouncement = this.settings.ScoreAnnouncementStep;
    State = RunState.GameOver;
  }

  /// <summary>
  /// Gets the current high-level run state.
  /// </summary>
  public RunState State { get; private set; }

  /// <summary>
  /// Gets the player lane index.
  /// </summary>
  public int Lane { get; private set; }

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
  public float ElapsedSeconds => this.elapsedSeconds;

  /// <summary>
  /// Gets active obstacles currently tracked in world space.
  /// </summary>
  public IReadOnlyList<RunObstacle> Obstacles => this.obstacles;

  /// <summary>
  /// Starts a fresh run.
  /// </summary>
  /// <returns>Events that announce run startup state.</returns>
  public IReadOnlyList<RunEvent> Start()
  {
    this.obstacles.Clear();
    this.spawner.Reset();
    this.elapsedSeconds = 0.0f;
    this.scoreRemainder = 0.0f;
    this.Score = 0;
    this.Lane = RunConstants.Lane.Center;
    this.Lives = this.settings.StartingLives;
    this.State = RunState.Playing;
    this.nextScoreAnnouncement = this.settings.ScoreAnnouncementStep;

    return
    [
      new TextToSpeechEvent(RunConstants.Speech.RunStarted),
      new PlaySoundEvent(RunConstants.SoundId.RunStart),
      new PlaySoundEvent(
        RunConstants.SoundId.LaneCenter,
        new Vector3(RunConstants.LaneX.Center, RunConstants.GroundY, 0.0f),
        RunConstants.Volume.LaneCue)
    ];
  }

  /// <summary>
  /// Applies a player command to the session state.
  /// </summary>
  /// <param name="command">Player command to process.</param>
  /// <returns>Events emitted by command handling.</returns>
  public IReadOnlyList<RunEvent> HandleCommand(RunCommand command)
  {
    List<RunEvent> events = [];

    switch (command)
    {
      case RunCommand.TogglePause:
        if (State == RunState.Playing)
        {
          this.State = RunState.Paused;
          events.Add(new TextToSpeechEvent(RunConstants.Speech.Paused));
          events.Add(new PlaySoundEvent(RunConstants.SoundId.Pause));
        }
        else if (State == RunState.Paused)
        {
          this.State = RunState.Playing;
          events.Add(new TextToSpeechEvent(RunConstants.Speech.Resumed));
          events.Add(new PlaySoundEvent(RunConstants.SoundId.Resume));
        }
        break;

      case RunCommand.MoveLeft:
        if (State == RunState.Playing && Lane > RunConstants.Lane.Left)
        {
          this.Lane--;
          events.Add(CreateLaneChangeEvent());
        }
        break;

      case RunCommand.MoveRight:
        if (State == RunState.Playing && Lane < RunConstants.Lane.Right)
        {
          this.Lane++;
          events.Add(CreateLaneChangeEvent());
        }
        break;

      case RunCommand.Restart:
        if (State == RunState.GameOver)
        {
          return Start();
        }
        break;
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
    if (State != RunState.Playing)
    {
      return Array.Empty<RunEvent>();
    }

    List<RunEvent> events = [];

    this.elapsedSeconds += deltaTimeSeconds;
    this.scoreRemainder += this.settings.ScoringPerSecond * deltaTimeSeconds;

    if (this.scoreRemainder >= 1.0f)
    {
      int earned = (int)this.scoreRemainder;
      this.Score += earned;
      this.scoreRemainder -= earned;

      while (this.Score >= this.nextScoreAnnouncement)
      {
        events.Add(new TextToSpeechEvent(
          $"{RunConstants.Speech.ScorePrefix} {this.nextScoreAnnouncement}"));
        events.Add(new PlaySoundEvent(RunConstants.SoundId.ScoreMilestone, null, RunConstants.Volume.ScoreMilestone));
        this.nextScoreAnnouncement += this.settings.ScoreAnnouncementStep;
      }
    }

    IReadOnlyList<RunObstacle> spawned = this.spawner.Update(deltaTimeSeconds, this.elapsedSeconds);

    for (int i = 0; i < spawned.Count; i++)
    {
      RunObstacle spawnedObstacle = spawned[i];
      this.obstacles.Add(spawnedObstacle);
      events.Add(new PlaySoundEvent(
        RunConstants.SoundId.ObstacleSpawn,
        LaneToPosition(spawnedObstacle.Lane, spawnedObstacle.Z),
        RunConstants.Volume.ObstacleSpawn));
    }

    for (int i = this.obstacles.Count - 1; i >= 0; i--)
    {
      RunObstacle obstacle = this.obstacles[i];
      obstacle = obstacle with { Z = obstacle.Z - (obstacle.Speed * deltaTimeSeconds) };

      bool collides = obstacle.Lane == this.Lane && MathF.Abs(obstacle.Z) <= this.settings.CollisionZWindow;

      if (collides)
      {
        this.obstacles.RemoveAt(i);
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

      if (obstacle.Z < RunConstants.PassedObstacleZ)
      {
        this.obstacles.RemoveAt(i);
        this.Score += this.settings.DodgeBonus;
        continue;
      }

      this.obstacles[i] = obstacle;
    }

    return events;
  }

  /// <summary>
  /// Adds a deterministic obstacle for tests or scripted scenarios.
  /// </summary>
  /// <param name="lane">Target lane index.</param>
  /// <param name="z">Initial Z position.</param>
  /// <param name="speed">Obstacle speed.</param>
  public void QueueObstacle(int lane, float z, float speed)
  {
    if (lane is < RunConstants.Lane.Left or > RunConstants.Lane.Right)
    {
      throw new ArgumentOutOfRangeException(nameof(lane));
    }

    this.obstacles.Add(new RunObstacle(lane, z, speed));
  }

  private RunEvent CreateLaneChangeEvent()
  {
    return new PlaySoundEvent(RunConstants.SoundId.LaneChange, LaneToPosition(this.Lane, 0.0f), RunConstants.Volume.LaneChange);
  }

  private static Vector3 LaneToPosition(int lane, float z)
  {
    float x = lane switch
    {
      RunConstants.Lane.Left => RunConstants.LaneX.Left,
      RunConstants.Lane.Center => RunConstants.LaneX.Center,
      _ => RunConstants.LaneX.Right
    };

    return new Vector3(x, RunConstants.GroundY, z);
  }
}
