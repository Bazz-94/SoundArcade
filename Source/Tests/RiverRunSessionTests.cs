namespace SoundArcade.Tests
{
  using System.Collections.Generic;
  using System.Linq;
  using SoundArcade.Domain.Colors;
  using SoundArcade.Domain.RiverRun.Game;
  using SoundArcade.Domain.RiverRun.Models;
  using SoundArcade.Domain.RiverRun.Models.Enum;
  using SoundArcade.Domain.RiverRun.Models.Events;
  using Xunit;

  /// <summary>
  /// Tests for RiverRun session domain behavior.
  /// </summary>
  public sealed class RiverRunSessionTests
  {
    private const int RandomSeed = 1;
    private const float OneSecond = 1.0f;
    private const float FrameDelta = 0.016f;
    private const int HighAnnouncementStep = 1_000;

    /// <summary>
    /// Verifies score increases over time while the session is playing.
    /// </summary>
    [Fact]
    public void Update_increases_score_while_playing()
    {
      RiverRunSettings settings = new(
        scoringPerSecond: 50.0f,
        scoreAnnouncementStep: HighAnnouncementStep);

      RiverRunSession session = new RiverRunSession(new Theme(), settings, new System.Random(RandomSeed));
      session.Start();

      session.Update(OneSecond);

      Assert.True(session.Score >= 50);
    }

    /// <summary>
    /// Verifies player speed increases as distance is traveled and respects the cap.
    /// </summary>
    [Fact]
    public void Update_increases_player_speed_with_distance_and_caps_it()
    {
      RiverRunSettings settings = new(
        startingPlayerSpeed: 2.0f,
        playerSpeedIncreasePerZUnit: 1.0f,
        maxPlayerSpeedIncrease: 1.0f,
        scoreAnnouncementStep: HighAnnouncementStep);

      RiverRunSession session = new RiverRunSession(new Theme(), settings, new System.Random(RandomSeed));
      session.Start();

      float startingSpeed = session.Player.Speed;

      session.Update(OneSecond);

      Assert.Equal(startingSpeed + settings.MaxPlayerSpeedIncrease, session.Player.Speed);
      Assert.True(session.Player.Speed > startingSpeed);

      session.Update(OneSecond);

      Assert.Equal(startingSpeed + settings.MaxPlayerSpeedIncrease, session.Player.Speed);
    }

    /// <summary>
    /// Verifies obstacle spawning follows player Z progress rather than elapsed time.
    /// </summary>
    [Fact]
    public void Update_spawns_obstacles_as_player_advances_in_z()
    {
      RiverRunSettings settings = new(
        startingPlayerSpeed: 1.0f,
        playerSpeedIncreasePerZUnit: 0.0f,
        maxPlayerSpeedIncrease: 0.0f,
        spawnZ: 1.0f,
        spawnDistanceMin: 1.0f,
        spawnDistanceMax: 1.0f,
        scoreAnnouncementStep: HighAnnouncementStep);

      RiverRunSession session = new RiverRunSession(new Theme(), settings, new System.Random(RandomSeed));
      session.Start();

      session.Update(OneSecond);

      Assert.Single(session.Obstacles);
    }

    /// <summary>
    /// Verifies pause command toggles between playing and paused states.
    /// </summary>
    [Fact]
    public void TogglePause_transitions_between_playing_and_paused()
    {
      RiverRunSession session = new RiverRunSession(new Theme(), new RiverRunSettings(), new System.Random(RandomSeed));
      session.Start();

      IReadOnlyList<RunEvent> pauseEvents = session.HandleCommand(RunCommand.TogglePause);

      Assert.Equal(SessionState.Paused, session.State);
      Assert.Contains(pauseEvents.OfType<TextToSpeechEvent>(), x => x.Text == RunConstants.Speech.Paused);

      IReadOnlyList<RunEvent> resumeEvents = session.HandleCommand(RunCommand.TogglePause);

      Assert.Equal(SessionState.Playing, session.State);
      Assert.Contains(resumeEvents.OfType<TextToSpeechEvent>(), x => x.Text == RunConstants.Speech.Resumed);
    }

    /// <summary>
    /// Verifies repeated collisions consume lives and transition to game over.
    /// </summary>
    [Fact]
    public void Collision_until_no_lives_reaches_game_over()
    {
      RiverRunSettings settings = new(
        startingLives: 2,
        collisionRadius: 0.5f,
        scoringPerSecond: 0.0f,
        scoreAnnouncementStep: HighAnnouncementStep);

      RiverRunSession session = new RiverRunSession(new Theme(), settings, new System.Random(RandomSeed));
      session.Start();

      session.QueueObstacle(lane: RunConstants.LaneX.Center, z: 0.0f);
      session.Update(FrameDelta);

      Assert.Equal(SessionState.Playing, session.State);
      Assert.Equal(1, session.Lives);

      session.QueueObstacle(lane: RunConstants.LaneX.Center, z: 0.0f);
      IReadOnlyList<RunEvent> events = session.Update(FrameDelta);

      Assert.Equal(SessionState.GameOver, session.State);
      Assert.Equal(0, session.Lives);
      Assert.Contains(events.OfType<TextToSpeechEvent>(), x => x.Text.StartsWith(RunConstants.Speech.GameOverPrefix));
    }

    /// <summary>
    /// Verifies a single collision costs one life but does not end the run while lives remain.
    /// </summary>
    [Fact]
    public void Collision_with_default_settings_costs_one_life_and_continues()
    {
      RiverRunSettings settings = new(
        collisionRadius: 0.5f,
        scoringPerSecond: 0.0f,
        scoreAnnouncementStep: HighAnnouncementStep);

      RiverRunSession session = new RiverRunSession(new Theme(), settings, new System.Random(RandomSeed));
      session.Start();

      session.QueueObstacle(lane: RunConstants.LaneX.Center, z: 0.0f);
      session.Update(FrameDelta);

      Assert.Equal(SessionState.Playing, session.State);
      Assert.Equal(2, session.Lives);
    }

    /// <summary>
    /// Verifies collecting a pickup awards the configured score bonus and removes it from the world.
    /// </summary>
    [Fact]
    public void Update_collects_pickup_and_awards_bonus()
    {
      RiverRunSettings settings = new(
        collisionRadius: 0.5f,
        scoringPerSecond: 0.0f,
        pickupScoreBonus: 25,
        scoreAnnouncementStep: HighAnnouncementStep);

      RiverRunSession session = new RiverRunSession(new Theme(), settings, new System.Random(RandomSeed));
      session.Start();

      session.QueuePickup(lane: RunConstants.LaneX.Center, z: 0.0f);
      IReadOnlyList<RunEvent> events = session.Update(FrameDelta);

      Assert.Equal(25, session.Score);
      Assert.DoesNotContain(session.Pickups, pickup => pickup.Position.Z == 0.0f);
      Assert.Contains(events.OfType<PlaySoundEvent>(), x => x.SoundId == RunConstants.SoundId.Reward);
    }

    /// <summary>
    /// Verifies an approaching obstacle emits a positional approach-noise cue.
    /// </summary>
    [Fact]
    public void Update_emits_approach_noise_for_nearby_obstacle()
    {
      RiverRunSettings settings = new(
        startingPlayerSpeed: 0.0f,
        playerSpeedIncreasePerZUnit: 0.0f,
        maxPlayerSpeedIncrease: 0.0f,
        approachNoiseRadius: 10.0f,
        scoringPerSecond: 0.0f,
        scoreAnnouncementStep: HighAnnouncementStep);

      RiverRunSession session = new RiverRunSession(new Theme(), settings, new System.Random(RandomSeed));
      session.Start();

      session.QueueObstacle(lane: RunConstants.LaneX.Center, z: 5.0f);
      IReadOnlyList<RunEvent> events = session.Update(FrameDelta);

      Assert.Contains(events.OfType<PlaySoundEvent>(), x => x.SoundId == RunConstants.SoundId.ObstacleNoise);
    }

    /// <summary>
    /// Verifies the obstacle approach cue is stopped once the obstacle passes and the lane ahead clears.
    /// </summary>
    [Fact]
    public void Update_stops_obstacle_noise_after_the_lane_ahead_clears()
    {
      RiverRunSettings settings = new(
        startingPlayerSpeed: 5.0f,
        playerSpeedIncreasePerZUnit: 0.0f,
        maxPlayerSpeedIncrease: 0.0f,
        approachNoiseRadius: 10.0f,
        collisionRadius: 0.5f,
        scoringPerSecond: 0.0f,
        scoreAnnouncementStep: HighAnnouncementStep);

      RiverRunSession session = new RiverRunSession(new Theme(), settings, new System.Random(RandomSeed));
      session.Start();

      // Place the obstacle in an adjacent lane so the player passes it without colliding.
      session.QueueObstacle(lane: RunConstants.LaneX.Left, z: 3.0f);
      session.Update(FrameDelta);

      IReadOnlyList<RunEvent> events = session.Update(OneSecond);

      Assert.DoesNotContain(session.Obstacles, obstacle => obstacle.Position.Z == 3.0f);
      Assert.Contains(events.OfType<StopSoundEvent>(), x => x.SoundId == RunConstants.SoundId.ObstacleNoise);
    }

    /// <summary>
    /// Verifies approach noise volume increases as an obstacle gets closer to the player.
    /// </summary>
    [Fact]
    public void Update_obstacle_approach_noise_grows_louder_as_distance_shrinks()
    {
      RiverRunSettings farSettings = new(
        startingPlayerSpeed: 0.0f,
        playerSpeedIncreasePerZUnit: 0.0f,
        maxPlayerSpeedIncrease: 0.0f,
        approachNoiseRadius: 10.0f,
        scoringPerSecond: 0.0f,
        scoreAnnouncementStep: HighAnnouncementStep);

      RiverRunSession farSession = new RiverRunSession(new Theme(), farSettings, new System.Random(RandomSeed));
      farSession.Start();
      farSession.QueueObstacle(lane: RunConstants.LaneX.Center, z: 9.0f);
      PlaySoundEvent farNoise = farSession.Update(FrameDelta).OfType<PlaySoundEvent>()
        .Single(x => x.SoundId == RunConstants.SoundId.ObstacleNoise);

      RiverRunSession nearSession = new RiverRunSession(new Theme(), farSettings, new System.Random(RandomSeed));
      nearSession.Start();
      nearSession.QueueObstacle(lane: RunConstants.LaneX.Center, z: 2.0f);
      PlaySoundEvent nearNoise = nearSession.Update(FrameDelta).OfType<PlaySoundEvent>()
        .Single(x => x.SoundId == RunConstants.SoundId.ObstacleNoise);

      Assert.True(nearNoise.Volume > farNoise.Volume);
    }

    /// <summary>
    /// Verifies approach noise volume uses straight-line distance, so an obstacle in an adjacent
    /// lane is quieter than one in the player's lane at the same forward distance.
    /// </summary>
    [Fact]
    public void Update_obstacle_approach_noise_uses_straight_line_distance()
    {
      RiverRunSettings settings = new(
        startingPlayerSpeed: 0.0f,
        playerSpeedIncreasePerZUnit: 0.0f,
        maxPlayerSpeedIncrease: 0.0f,
        approachNoiseRadius: 10.0f,
        scoringPerSecond: 0.0f,
        scoreAnnouncementStep: HighAnnouncementStep);

      RiverRunSession sameLaneSession = new RiverRunSession(new Theme(), settings, new System.Random(RandomSeed));
      sameLaneSession.Start();
      sameLaneSession.QueueObstacle(lane: RunConstants.LaneX.Center, z: 5.0f);
      PlaySoundEvent sameLaneNoise = sameLaneSession.Update(FrameDelta).OfType<PlaySoundEvent>()
        .Single(x => x.SoundId == RunConstants.SoundId.ObstacleNoise);

      RiverRunSession adjacentLaneSession = new RiverRunSession(new Theme(), settings, new System.Random(RandomSeed));
      adjacentLaneSession.Start();
      adjacentLaneSession.QueueObstacle(lane: RunConstants.LaneX.Left, z: 5.0f);
      PlaySoundEvent adjacentLaneNoise = adjacentLaneSession.Update(FrameDelta).OfType<PlaySoundEvent>()
        .Single(x => x.SoundId == RunConstants.SoundId.ObstacleNoise);

      Assert.True(adjacentLaneNoise.Volume < sameLaneNoise.Volume);
    }

    /// <summary>
    /// Verifies obstacle pitch stays at base outside the ramp zone and only rises once the obstacle
    /// is within the final fraction of the approach radius.
    /// </summary>
    [Fact]
    public void Update_obstacle_approach_noise_rises_in_pitch_only_in_the_final_stretch()
    {
      // Radius 10 with a 0.25 ramp fraction means pitch is flat beyond 2.5 units and ramps within it.
      RiverRunSettings settings = new(
        startingPlayerSpeed: 0.0f,
        playerSpeedIncreasePerZUnit: 0.0f,
        maxPlayerSpeedIncrease: 0.0f,
        approachNoiseRadius: 10.0f,
        scoringPerSecond: 0.0f,
        scoreAnnouncementStep: HighAnnouncementStep);

      RiverRunSession farSession = new RiverRunSession(new Theme(), settings, new System.Random(RandomSeed));
      farSession.Start();
      farSession.QueueObstacle(lane: RunConstants.LaneX.Center, z: 9.0f);
      PlaySoundEvent farNoise = farSession.Update(FrameDelta).OfType<PlaySoundEvent>()
        .Single(x => x.SoundId == RunConstants.SoundId.ObstacleNoise);

      RiverRunSession nearSession = new RiverRunSession(new Theme(), settings, new System.Random(RandomSeed));
      nearSession.Start();
      nearSession.QueueObstacle(lane: RunConstants.LaneX.Center, z: 1.0f);
      PlaySoundEvent nearNoise = nearSession.Update(FrameDelta).OfType<PlaySoundEvent>()
        .Single(x => x.SoundId == RunConstants.SoundId.ObstacleNoise);

      Assert.Equal(RunConstants.Pitch.ObstacleNoiseFar, farNoise.Pitch);
      Assert.True(nearNoise.Pitch > farNoise.Pitch);
    }
  }
}
