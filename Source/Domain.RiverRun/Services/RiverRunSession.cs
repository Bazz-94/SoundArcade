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
    private const int HudMarginX = 20;
    private const int HudMarginY = 20;
    private const int HudFontSize = 24;
    private const int HudLineHeight = 30;

    private float ScoreRemainder { get; set; }
    private int NextScoreAnnouncement { get; set; }
    public IReadOnlyList<Obstacle> Obstacles => this.obstacles;
    public IReadOnlyList<Pickup> Pickups => this.pickups;
    private readonly List<Obstacle> obstacles = [];
    private readonly List<Pickup> pickups = [];
    private float NextRiverNoiseAt { get; set; }
    private bool ObstacleNoiseZoneOccupied { get; set; }
    private bool PickupNoiseZoneOccupied { get; set; }

    private Spawner Spawner { get; }
    private readonly Func<Vector3, Obstacle> obstacleFactory;
    private readonly Func<Vector3, Pickup> pickupFactory;
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
      this.obstacleFactory = position => new Obstacle(position, this.Theme.ColorPalette.Secondary);
      this.pickupFactory = position => new Pickup(position, this.Theme.ColorPalette.Accent);
      this.Spawner = new Spawner(
        [
          (this.obstacleFactory, 1.0f - this.Settings.PickupSpawnChance),
          (this.pickupFactory, this.Settings.PickupSpawnChance)
        ],
        this.Settings.SpawnZ,
        this.Settings.SpawnDistanceMin,
        this.Settings.SpawnDistanceMax,
        random);

      this.Player = new Player(
        this.Theme.ColorPalette.Tertiary,
        new Vector3(RunConstants.LaneX.Center, RunConstants.GroundY, 0.0f),
        this.Settings.StartingPlayerSpeed,
        this.Settings.PlayerSpeedIncreasePerZUnit,
        this.Settings.MaxPlayerSpeedIncrease);
      this.Lives = this.Settings.StartingLives;
      this.NextScoreAnnouncement = this.Settings.ScoreAnnouncementStep;
      this.State = SessionState.GameOver;
      this.laneColor = this.Theme.ColorPalette.Primary;
      this.hudLivesColor = this.Theme.ColorPalette.Accent;
      this.hudScoreColor = this.Theme.ColorPalette.Accent;
    }

    /// <summary>
    /// Starts a fresh run.
    /// </summary>
    /// <returns>Events that announce run startup state.</returns>
    public IReadOnlyList<RunEvent> Start()
    {
      this.obstacles.Clear();
      this.pickups.Clear();
      this.Spawner.Reset();
      this.ElapsedSeconds = 0.0f;
      this.ScoreRemainder = 0.0f;
      this.NextRiverNoiseAt = 0.0f;
      this.ObstacleNoiseZoneOccupied = false;
      this.PickupNoiseZoneOccupied = false;
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

      IReadOnlyList<GameObject> spawnedObjects = this.Spawner.Update(this.Player.Position.Z);

      foreach (GameObject spawnedObject in spawnedObjects)
      {
        switch (spawnedObject)
        {
          case Obstacle obstacle:
            this.obstacles.Add(obstacle);
            break;
          case Pickup pickup:
            this.pickups.Add(pickup);
            break;
          default:
            throw new InvalidOperationException($"Unhandled spawned object type {spawnedObject.GetType().Name}.");
        }
      }

      this.Player.Advance(deltaTimeSeconds);

      bool obstacleZoneOccupied = false;

      for (int i = 0; i < this.obstacles.Count; i++)
      {
        Obstacle obstacle = this.obstacles[i];

        bool collides = ProximityCollision.IsWithinBuffer(obstacle.Position, this.Player.Position, this.Settings.CollisionRadius);

        if (collides)
        {
          this.obstacles.RemoveAt(i);
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

        if (obstacleDistanceAhead > 0.0f && obstacleDistanceAhead <= this.Settings.ApproachNoiseRadius)
        {
          obstacleZoneOccupied = true;

          if (this.ElapsedSeconds >= obstacle.NextNoiseAt)
          {
            float volume = ComputeApproachNoiseVolume(obstacleDistanceAhead, this.Settings.ApproachNoiseRadius);
            float pitch = ComputeApproachNoisePitch(obstacleDistanceAhead, this.Settings.ApproachNoiseRadius);
            events.Add(new PlaySoundEvent(RunConstants.SoundId.ObstacleNoise, obstacle.Position, volume, pitch));
            obstacle.NextNoiseAt = this.ElapsedSeconds + this.Settings.ApproachNoiseInterval;
          }
        }

        if (obstacle.Position.Z < this.Player.Position.Z)
        {
          this.obstacles.RemoveAt(i);
          continue;
        }
      }

      // Silence the obstacle approach cue the moment the lane ahead clears, so the player can tell
      // an adjacent lane is safe rather than hearing the previous cue's tail ring out.
      if (this.ObstacleNoiseZoneOccupied && !obstacleZoneOccupied)
      {
        events.Add(new StopSoundEvent(RunConstants.SoundId.ObstacleNoise));
      }

      this.ObstacleNoiseZoneOccupied = obstacleZoneOccupied;

      if (this.State == SessionState.GameOver)
      {
        return events;
      }

      bool pickupZoneOccupied = false;

      for (int i = 0; i < this.pickups.Count; i++)
      {
        Pickup pickup = this.pickups[i];

        bool collected = ProximityCollision.IsWithinBuffer(pickup.Position, this.Player.Position, this.Settings.CollisionRadius);

        if (collected)
        {
          this.pickups.RemoveAt(i);
          this.Score += this.Settings.PickupScoreBonus;
          events.Add(new TextToSpeechEvent($"{RunConstants.Speech.PickupPrefix} {this.Settings.PickupScoreBonus}"));
          events.Add(new PlaySoundEvent(RunConstants.SoundId.Reward, pickup.Position, RunConstants.Volume.PickupCollected));
          continue;
        }

        float pickupDistanceAhead = pickup.Position.Z - this.Player.Position.Z;

        if (pickupDistanceAhead > 0.0f && pickupDistanceAhead <= this.Settings.ApproachNoiseRadius)
        {
          pickupZoneOccupied = true;

          if (this.ElapsedSeconds >= pickup.NextNoiseAt)
          {
            float volume = ComputeApproachNoiseVolume(pickupDistanceAhead, this.Settings.ApproachNoiseRadius);
            events.Add(new PlaySoundEvent(RunConstants.SoundId.RewardNoise, pickup.Position, volume));
            pickup.NextNoiseAt = this.ElapsedSeconds + this.Settings.ApproachNoiseInterval;
          }
        }

        if (pickup.Position.Z < this.Player.Position.Z)
        {
          this.pickups.RemoveAt(i);
          continue;
        }
      }

      // Silence the pickup approach cue once no pickup remains ahead within range.
      if (this.PickupNoiseZoneOccupied && !pickupZoneOccupied)
      {
        events.Add(new StopSoundEvent(RunConstants.SoundId.RewardNoise));
      }

      this.PickupNoiseZoneOccupied = pickupZoneOccupied;

      if (this.ElapsedSeconds >= this.NextRiverNoiseAt)
      {
        this.NextRiverNoiseAt = this.ElapsedSeconds + this.Settings.RiverNoiseInterval;

        Vector3 leftRiverPosition = new Vector3(RunConstants.LaneX.Left + this.Settings.RiverNoiseLaneOffset, RunConstants.GroundY, this.Player.Position.Z);
        Vector3 rightRiverPosition = new Vector3(RunConstants.LaneX.Right - this.Settings.RiverNoiseLaneOffset, RunConstants.GroundY, this.Player.Position.Z);

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

      renderer.DrawLine(new Vector3(RunConstants.LaneX.Left, RunConstants.GroundY, laneStartZ), new Vector3(RunConstants.LaneX.Left, RunConstants.GroundY, laneEndZ), this.laneColor);
      renderer.DrawLine(new Vector3(RunConstants.LaneX.Center, RunConstants.GroundY, laneStartZ), new Vector3(RunConstants.LaneX.Center, RunConstants.GroundY, laneEndZ), this.laneColor);
      renderer.DrawLine(new Vector3(RunConstants.LaneX.Right, RunConstants.GroundY, laneStartZ), new Vector3(RunConstants.LaneX.Right, RunConstants.GroundY, laneEndZ), this.laneColor);

      this.Player.Render(renderer);

      foreach (Obstacle obstacle in this.obstacles)
      {
        obstacle.Render(renderer);
      }

      foreach (Pickup pickup in this.pickups)
      {
        pickup.Render(renderer);
      }
    }

    public void RenderHud(IRenderer renderer)
    {
      int lives = Math.Max(0, this.Lives);
      int score = Math.Max(0, this.Score);

      renderer.DrawScreenText(HudMarginX, HudMarginY, $"Score: {score}", HudFontSize, this.hudScoreColor);
      renderer.DrawScreenText(HudMarginX, HudMarginY + HudLineHeight, $"Lives: {lives}", HudFontSize, this.hudLivesColor);
    }

    /// <summary>
    /// Adds a deterministic obstacle for tests or scripted scenarios.
    /// </summary>
    /// <param name="lane">Target lane index.</param>
    /// <param name="z">Initial Z position.</param>
    public void QueueObstacle(float lane, float z)
    {
      if (lane is < RunConstants.LaneX.Right or > RunConstants.LaneX.Left)
      {
        throw new ArgumentOutOfRangeException(nameof(lane));
      }
      Vector3 position = new Vector3(lane, RunConstants.GroundY, z);
      this.obstacles.Add(this.obstacleFactory(position));
    }

    /// <summary>
    /// Adds a deterministic pickup for tests or scripted scenarios.
    /// </summary>
    /// <param name="lane">Target lane index.</param>
    /// <param name="z">Initial Z position.</param>
    public void QueuePickup(float lane, float z)
    {
      if (lane is < RunConstants.LaneX.Right or > RunConstants.LaneX.Left)
      {
        throw new ArgumentOutOfRangeException(nameof(lane));
      }
      Vector3 position = new Vector3(lane, RunConstants.GroundY, z);
      this.pickups.Add(this.pickupFactory(position));
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

    /// <summary>
    /// Scales approach noise pitch so it rises only over the final stretch of the approach radius,
    /// giving a last-moment distance cue on top of the volume ramp.
    /// </summary>
    /// <param name="distanceAhead">Forward distance from the player to the object.</param>
    /// <param name="radius">Distance at which approach noise starts being audible.</param>
    private static float ComputeApproachNoisePitch(float distanceAhead, float radius)
    {
      float rampRadius = radius * RunConstants.Pitch.ObstacleNoiseRampFraction;

      if (rampRadius <= 0.0f)
      {
        return RunConstants.Pitch.ObstacleNoiseNear;
      }

      float proximity = 1.0f - Math.Clamp(distanceAhead / rampRadius, 0.0f, 1.0f);
      return RunConstants.Pitch.ObstacleNoiseFar + (proximity * (RunConstants.Pitch.ObstacleNoiseNear - RunConstants.Pitch.ObstacleNoiseFar));
    }
  }
}
