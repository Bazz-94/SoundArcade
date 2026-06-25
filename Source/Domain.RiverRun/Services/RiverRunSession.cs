namespace SoundArcade.Domain.RiverRun.Services
{
  using System;
  using System.Collections.Generic;
  using System.Numerics;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Colors;
  using SoundArcade.Domain.RiverRun.Models;
  using SoundArcade.Domain.Services;

  /// <summary>
  /// Represents one RiverRun gameplay session and enforces run state, scoring, and collision rules.
  /// </summary>
  public sealed class RiverRunSession
  {
    private float ScoreRemainder { get; set; }
    private int NextScoreAnnouncement { get; set; }
    public IReadOnlyList<Obstacle> Obstacles => obstacles;
    public IReadOnlyList<Pickup> Pickups => pickups;
    private readonly List<Obstacle> obstacles = [];
    private readonly List<Pickup> pickups = [];
    private float NextRiverNoiseAt { get; set; }

    private ObstacleSpawner Spawner { get; }
    private PickupSpawner PickupSpawner { get; }
    public Theme Theme { get; }
    private RiverRunSettings Settings { get; }
    public float ElapsedSeconds { get; private set; }
    public int Lives { get; private set; }
    public int Score { get; private set; }
    public Player Player { get; private set; }
    public SessionState State { get; private set; }

    private readonly Color laneColor;
    private readonly Color hudLivesColor;
    private readonly Color hudScoreColor;

    /// <summary>
    /// Initializes a new instance of the <see cref="RiverRunSession"/> class.
    /// </summary>
    /// <param name="settings">Gameplay tuning settings.</param>
    /// <param name="random">Optional random source used by spawning services.</param>
    public RiverRunSession(Theme theme, RiverRunSettings settings, Random? random = null)
    {
      this.Theme = theme;
      this.Settings = settings;
      this.Spawner = new ObstacleSpawner(this.Theme.ColorPalette.Secondary, this.Settings, random);
      this.PickupSpawner = new PickupSpawner(this.Theme.ColorPalette.Accent, this.Settings, random);

      this.Player = new Player(
        this.Theme.ColorPalette.Tertiary,
        new Vector3(RunConstants.LaneX.Center, RunConstants.GroundY, 0.0f),
        this.Settings.StartingPlayerSpeed,
        this.Settings.PlayerSpeedIncreasePerZUnit,
        this.Settings.MaxPlayerSpeedIncrease);
      this.Lives = this.Settings.StartingLives;
      this.NextScoreAnnouncement = this.Settings.ScoreAnnouncementStep;
      this.State = SessionState.GameOver;
      laneColor = this.Theme.ColorPalette.Primary;
      hudLivesColor = this.Theme.ColorPalette.Accent;
      hudScoreColor = this.Theme.ColorPalette.Accent;
    }

    /// <summary>
    /// Starts a fresh run.
    /// </summary>
    /// <returns>Events that announce run startup state.</returns>
    public IReadOnlyList<RunEvent> Start()
    {
      obstacles.Clear();
      pickups.Clear();
      this.Spawner.Reset();
      this.PickupSpawner.Reset();
      this.ElapsedSeconds = 0.0f;
      this.ScoreRemainder = 0.0f;
      this.NextRiverNoiseAt = 0.0f;
      this.Score = 0;
      this.Player = new Player(
        this.Theme.ColorPalette.Tertiary,
        new Vector3(RunConstants.LaneX.Center, RunConstants.GroundY, 0.0f),
        this.Settings.StartingPlayerSpeed,
        this.Settings.PlayerSpeedIncreasePerZUnit,
        this.Settings.MaxPlayerSpeedIncrease);
      this.Lives = this.Settings.StartingLives;
      this.State = SessionState.Playing;
      this.NextScoreAnnouncement = this.Settings.ScoreAnnouncementStep;

      return
      [
        new TextToSpeechEvent(RunConstants.Speech.RunStarted)
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
        case SessionState.Playing:
          switch (command)
          {
            case RunCommand.TogglePause:
              this.State = SessionState.Paused;
              events.Add(new TextToSpeechEvent(RunConstants.Speech.Paused));
              break;
            case RunCommand.MoveLeft:
            case RunCommand.MoveRight:
              {
                this.Player.HandleCommand(command);
                break;
              }
          }
          break;

        case SessionState.Paused:
          switch (command)
          {
            case RunCommand.TogglePause:
              this.State = SessionState.Playing;
              events.Add(new TextToSpeechEvent(RunConstants.Speech.Resumed));
              break;
          }
          break;
        case SessionState.GameOver:
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
      if (this.State != SessionState.Playing)
      {
        return Array.Empty<RunEvent>();
      }

      List<RunEvent> events = [];

      this.ElapsedSeconds += deltaTimeSeconds;
      this.ScoreRemainder += this.Settings.ScoringPerSecond * deltaTimeSeconds;

      if (this.ScoreRemainder >= 1.0f)
      {
        int earned = (int)this.ScoreRemainder;
        this.Score += earned;
        this.ScoreRemainder -= earned;

        while (this.Score >= this.NextScoreAnnouncement)
        {
          events.Add(new TextToSpeechEvent(
            $"{RunConstants.Speech.ScorePrefix} {this.NextScoreAnnouncement}"));
          this.NextScoreAnnouncement += this.Settings.ScoreAnnouncementStep;
        }
      }

      IReadOnlyList<Obstacle> spawnedObstacles = this.Spawner.Update(this.Player.Position.Z);

      foreach (Obstacle spawnedObstacle in spawnedObstacles)
      {
        obstacles.Add(spawnedObstacle);
      }

      IReadOnlyList<Pickup> spawnedPickups = this.PickupSpawner.Update(this.Player.Position.Z);

      foreach (Pickup spawnedPickup in spawnedPickups)
      {
        pickups.Add(spawnedPickup);
      }

      this.Player.Advance(deltaTimeSeconds);

      for (int i = 0; i < obstacles.Count; i++)
      {
        Obstacle obstacle = obstacles[i];

        bool collides = ProximityCollision.IsWithinBuffer(obstacle.Position, this.Player.Position, this.Settings.CollisionRadius);

        if (collides)
        {
          obstacles.RemoveAt(i);
          this.Lives--;
          events.Add(new TextToSpeechEvent($"Hit. {this.Lives} {RunConstants.Speech.LivesLeftSuffix}"));
          events.Add(new PlaySoundEvent(RunConstants.SoundId.Collision, null));

          if (this.Lives <= 0)
          {
            this.State = SessionState.GameOver;
            events.Add(new TextToSpeechEvent($"{RunConstants.Speech.GameOverPrefix} {this.Score}"));
            break;
          }

          continue;
        }

        float obstacleDistanceAhead = obstacle.Position.Z - this.Player.Position.Z;

        if (obstacleDistanceAhead > 0.0f
          && obstacleDistanceAhead <= this.Settings.ApproachNoiseRadius
          && this.ElapsedSeconds >= obstacle.NextNoiseAt)
        {
          float volume = ComputeApproachNoiseVolume(obstacleDistanceAhead, this.Settings.ApproachNoiseRadius);
          events.Add(new PlaySoundEvent(RunConstants.SoundId.ObstacleNoise, obstacle.Position, volume));
          obstacle.NextNoiseAt = this.ElapsedSeconds + this.Settings.ApproachNoiseInterval;
        }

        if (obstacle.Position.Z < this.Player.Position.Z)
        {
          obstacles.RemoveAt(i);
          continue;
        }
      }

      if (this.State == SessionState.GameOver)
      {
        return events;
      }

      for (int i = 0; i < pickups.Count; i++)
      {
        Pickup pickup = pickups[i];

        bool collected = ProximityCollision.IsWithinBuffer(pickup.Position, this.Player.Position, this.Settings.CollisionRadius);

        if (collected)
        {
          pickups.RemoveAt(i);
          this.Score += this.Settings.PickupScoreBonus;
          events.Add(new TextToSpeechEvent($"{RunConstants.Speech.PickupPrefix} {this.Settings.PickupScoreBonus}"));
          events.Add(new PlaySoundEvent(RunConstants.SoundId.Reward, pickup.Position, RunConstants.Volume.PickupCollected));
          continue;
        }

        float pickupDistanceAhead = pickup.Position.Z - this.Player.Position.Z;

        if (pickupDistanceAhead > 0.0f
          && pickupDistanceAhead <= this.Settings.ApproachNoiseRadius
          && this.ElapsedSeconds >= pickup.NextNoiseAt)
        {
          float volume = ComputeApproachNoiseVolume(pickupDistanceAhead, this.Settings.ApproachNoiseRadius);
          events.Add(new PlaySoundEvent(RunConstants.SoundId.RewardNoise, pickup.Position, volume));
          pickup.NextNoiseAt = this.ElapsedSeconds + this.Settings.ApproachNoiseInterval;
        }

        if (pickup.Position.Z < this.Player.Position.Z)
        {
          pickups.RemoveAt(i);
          continue;
        }
      }

      if (this.ElapsedSeconds >= this.NextRiverNoiseAt)
      {
        this.NextRiverNoiseAt = this.ElapsedSeconds + this.Settings.RiverNoiseInterval;

        Vector3 leftRiverPosition = new Vector3(RunConstants.LaneX.Left - this.Settings.RiverNoiseLaneOffset, RunConstants.GroundY, this.Player.Position.Z);
        Vector3 rightRiverPosition = new Vector3(RunConstants.LaneX.Right + this.Settings.RiverNoiseLaneOffset, RunConstants.GroundY, this.Player.Position.Z);

        events.Add(new PlaySoundEvent(RunConstants.SoundId.RiverNoise, leftRiverPosition, RunConstants.Volume.RiverAmbient));
        events.Add(new PlaySoundEvent(RunConstants.SoundId.RiverNoise, rightRiverPosition, RunConstants.Volume.RiverAmbient));
      }

      return events;
    }

    public void Render(IRenderer renderer)
    {
      renderer.SetCameraTarget(this.Player.Position);
      this.RenderHud(renderer);
      float laneStartZ = this.Player.Position.Z - 2.0f;
      float laneEndZ = laneStartZ + 28.0f;

      renderer.DrawLine(new Vector3(RunConstants.LaneX.Left, RunConstants.GroundY, laneStartZ), new Vector3(RunConstants.LaneX.Left, RunConstants.GroundY, laneEndZ), laneColor);
      renderer.DrawLine(new Vector3(RunConstants.LaneX.Center, RunConstants.GroundY, laneStartZ), new Vector3(RunConstants.LaneX.Center, RunConstants.GroundY, laneEndZ), laneColor);
      renderer.DrawLine(new Vector3(RunConstants.LaneX.Right, RunConstants.GroundY, laneStartZ), new Vector3(RunConstants.LaneX.Right, RunConstants.GroundY, laneEndZ), laneColor);

      this.Player.Render(renderer);

      foreach (Obstacle obstacle in obstacles)
      {
        obstacle.Render(renderer);
      }

      foreach (Pickup pickup in pickups)
      {
        pickup.Render(renderer);
      }
    }

    public void RenderHud(IRenderer renderer)
    {
      int lives = Math.Max(0, this.Lives);
      int score = Math.Max(0, this.Score);
      float hudZ = this.Player.Position.Z + 6.5f;

      for (int i = 0; i < lives; i++)
      {
        renderer.DrawSphere(new Vector3(-3.5f + (i * 0.45f), 5.8f, hudZ), 0.12f, hudLivesColor);
      }

      renderer.DrawText(new Vector3(1.0f, 5.6f, hudZ), $"Score: {score}", 24, hudScoreColor);
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
      obstacles.Add(this.Spawner.CreateRunObstacle(position));
    }

    /// <summary>
    /// Adds a deterministic obstacle using an explicit world position.
    /// </summary>
    /// <param name="position">Obstacle world position.</param>
    public void QueueObstacle(Vector3 position)
    {
      obstacles.Add(this.Spawner.CreateRunObstacle(position));
    }

    /// <summary>
    /// Adds a deterministic pickup for tests or scripted scenarios.
    /// </summary>
    /// <param name="lane">Target lane index.</param>
    /// <param name="z">Initial Z position.</param>
    public void QueuePickup(float lane, float z)
    {
      if (lane is < RunConstants.LaneX.Left or > RunConstants.LaneX.Right)
      {
        throw new ArgumentOutOfRangeException(nameof(lane));
      }
      Vector3 position = new Vector3(lane, RunConstants.GroundY, z);
      pickups.Add(this.PickupSpawner.CreatePickup(position));
    }

    /// <summary>
    /// Scales approach noise volume so it is loudest right next to the player and quietest at the edge of the approach radius.
    /// </summary>
    /// <param name="distanceAhead">Forward distance from the player to the object.</param>
    /// <param name="radius">Distance at which approach noise starts being audible.</param>
    private static float ComputeApproachNoiseVolume(float distanceAhead, float radius)
    {
      if (radius <= 0.0f)
      {
        return RunConstants.Volume.ApproachNoiseMax;
      }

      float proximity = 1.0f - Math.Clamp(distanceAhead / radius, 0.0f, 1.0f);
      return RunConstants.Volume.ApproachNoiseMin + (proximity * (RunConstants.Volume.ApproachNoiseMax - RunConstants.Volume.ApproachNoiseMin));
    }
  }
}
