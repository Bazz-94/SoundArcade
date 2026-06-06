using System.Collections.Generic;
using System.Linq;
using SoundArcade.Domain.RiverRun.Models;
using SoundArcade.Domain.RiverRun.Services;
using Xunit;

namespace SoundArcade.Tests;

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
    RunSettings settings = new(
      ScoringPerSecond: 50.0f,
      ScoreAnnouncementStep: HighAnnouncementStep);

    RiverRunSession session = new RiverRunSession(settings, new System.Random(RandomSeed));
    session.Start();

    session.Update(OneSecond);

    Assert.True(session.Score >= 50);
  }

  /// <summary>
  /// Verifies pause command toggles between playing and paused states.
  /// </summary>
  [Fact]
  public void TogglePause_transitions_between_playing_and_paused()
  {
    RiverRunSession session = new RiverRunSession(new RunSettings(), new System.Random(RandomSeed));
    session.Start();

    IReadOnlyList<RunEvent> pauseEvents = session.HandleCommand(RunCommand.TogglePause);

    Assert.Equal(RunState.Paused, session.State);
    Assert.Contains(pauseEvents.OfType<TextToSpeechEvent>(), x => x.Text == RunConstants.Speech.Paused);

    IReadOnlyList<RunEvent> resumeEvents = session.HandleCommand(RunCommand.TogglePause);

    Assert.Equal(RunState.Playing, session.State);
    Assert.Contains(resumeEvents.OfType<TextToSpeechEvent>(), x => x.Text == RunConstants.Speech.Resumed);
  }

  /// <summary>
  /// Verifies repeated collisions consume lives and transition to game over.
  /// </summary>
  [Fact]
  public void Collision_until_no_lives_reaches_game_over()
  {
    RunSettings settings = new(
      StartingLives: 2,
      CollisionRadius: 0.5f,
      ScoringPerSecond: 0.0f,
      ScoreAnnouncementStep: HighAnnouncementStep);

    RiverRunSession session = new RiverRunSession(settings, new System.Random(RandomSeed));
    session.Start();

    session.QueueObstacle(lane: RunConstants.LaneX.Center, z: 0.0f);
    session.Update(FrameDelta);

    Assert.Equal(RunState.Playing, session.State);
    Assert.Equal(1, session.Lives);

    session.QueueObstacle(lane: RunConstants.LaneX.Center, z: 0.0f);
    IReadOnlyList<RunEvent> events = session.Update(FrameDelta);

    Assert.Equal(RunState.GameOver, session.State);
    Assert.Equal(0, session.Lives);
    Assert.Contains(events.OfType<TextToSpeechEvent>(), x => x.Text.StartsWith(RunConstants.Speech.GameOverPrefix));
  }
}
