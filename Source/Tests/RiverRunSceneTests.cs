namespace SoundArcade.Tests
{
  using System;
  using System.Collections.Generic;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Colors;
  using SoundArcade.Domain.RiverRun.Game;
  using SoundArcade.Domain.RiverRun.Models;
  using SoundArcade.Domain.RiverRun.Models.Enum;
  using SoundArcade.Domain.RiverRun.Scene;
  using SoundArcade.Domain.Services;
  using SoundArcade.Tests.Mock;
  using Xunit;

  /// <summary>
  /// Tests for the RiverRun scene overlays.
  /// </summary>
  public sealed class RiverRunSceneTests
  {
    private const string GameId = "river-run";
    private const float FrameTime = 0.016f;

    private readonly MockInput input = new MockInput();
    private readonly MockTts tts = new MockTts();
    private readonly MockScoreboardStore store = new MockScoreboardStore();
    private readonly RiverRunScene scene;

    /// <summary>
    /// Initializes a new instance of the <see cref="RiverRunSceneTests"/> class.
    /// </summary>
    public RiverRunSceneTests()
    {
      this.scene = new RiverRunScene(
        tts: this.tts,
        audio: new MockAudio(),
        input: this.input,
        renderer: new MockRenderer(),
        theme: new Theme(),
        sceneManager: new SceneManager(),
        scoreboardStore: this.store);
    }

    /// <summary>
    /// Entering the scene announces the start menu instead of starting a run.
    /// </summary>
    [Fact]
    public void OnEnter_ShowsStartMenuWithoutStartingRun()
    {
      this.scene.OnEnter();

      Assert.Equal("Play", this.tts.LastSpokenText);

      this.scene.Update(0.016f);

      Assert.Equal("Play", this.tts.LastSpokenText);
    }

    /// <summary>
    /// Selecting Play starts the run.
    /// </summary>
    [Fact]
    public void StartMenu_PlayStartsRun()
    {
      this.scene.OnEnter();

      this.input.Press(Input.Enter);
      this.scene.Update(0.016f);

      Assert.Equal("Run started", this.tts.LastSpokenText);
    }

    /// <summary>
    /// The start menu offers Play, Scoreboard and Back in order.
    /// </summary>
    [Fact]
    public void StartMenu_AnnouncesOptionsInOrder()
    {
      this.scene.OnEnter();

      this.input.Press(Input.Down);
      this.scene.Update(0.016f);
      Assert.Equal("Scoreboard", this.tts.LastSpokenText);

      this.input.Press(Input.Down);
      this.scene.Update(0.016f);
      Assert.Equal("Back", this.tts.LastSpokenText);
    }

    /// <summary>
    /// Losing all lives shows the end-game menu, announced after the game-over speech.
    /// </summary>
    [Fact]
    public void GameOver_ShowsEndGameMenuAfterGameOverSpeech()
    {
      this.StartRunAndLose();

      int gameOverIndex = this.tts.SpokenTexts.FindIndex(text => text.StartsWith("Game over"));
      int menuIndex = this.tts.SpokenTexts.IndexOf("End game menu");

      Assert.True(gameOverIndex >= 0);
      Assert.True(menuIndex > gameOverIndex);
      Assert.Equal("Try again", this.tts.LastSpokenText);
    }

    /// <summary>
    /// The end-game menu offers Try again, Submit score and Main Menu in order.
    /// </summary>
    [Fact]
    public void EndGameMenu_AnnouncesOptionsInOrder()
    {
      this.StartRunAndLose();

      this.input.Press(Input.Down);
      this.scene.Update(FrameTime);
      Assert.Equal("Submit score", this.tts.LastSpokenText);

      this.input.Press(Input.Down);
      this.scene.Update(FrameTime);
      Assert.Equal("Main Menu", this.tts.LastSpokenText);
    }

    /// <summary>
    /// Try again starts a fresh run.
    /// </summary>
    [Fact]
    public void EndGameMenu_TryAgainStartsNewRun()
    {
      this.StartRunAndLose();

      this.input.Press(Input.Enter);
      this.scene.Update(FrameTime);

      Assert.Equal("Run started", this.tts.LastSpokenText);
    }

    /// <summary>
    /// Submitting with a confirmed name saves the score and confirms via speech,
    /// and Submit score is not offered again for the same run.
    /// </summary>
    [Fact]
    public void SubmitScore_ConfirmedNameSavesScoreOnce()
    {
      this.StartRunAndLose();
      this.OpenSubmitScore();

      this.input.Type("Zoe");
      this.scene.Update(FrameTime);
      this.input.Press(Input.Enter);
      this.scene.Update(FrameTime);

      ScoreEntry saved = Assert.Single(this.store.Load(GameId));
      Assert.Equal("Zoe", saved.Name);
      Assert.Contains("Score saved", this.tts.SpokenTexts);

      this.input.Press(Input.Down);
      this.scene.Update(FrameTime);
      Assert.Equal("Main Menu", this.tts.LastSpokenText);
    }

    /// <summary>
    /// Cancelling name entry saves nothing and returns to the end-game menu.
    /// </summary>
    [Fact]
    public void SubmitScore_CancelSavesNothing()
    {
      this.StartRunAndLose();
      this.OpenSubmitScore();

      this.input.Press(Input.Back);
      this.scene.Update(FrameTime);

      Assert.Empty(this.store.Load(GameId));
      Assert.Equal("Try again", this.tts.LastSpokenText);
    }

    /// <summary>
    /// Opening the name entry announces the prompt.
    /// </summary>
    [Fact]
    public void SubmitScore_OpensNameEntryWithPrompt()
    {
      this.StartRunAndLose();
      this.OpenSubmitScore();

      Assert.Equal("Enter your name", this.tts.LastSpokenText);
    }

    /// <summary>
    /// The scoreboard overlay lists entries by rank and returns to the start menu on Escape.
    /// </summary>
    [Fact]
    public void Scoreboard_AnnouncesRankedEntriesAndReturns()
    {
      this.store.Save(GameId, new List<ScoreEntry>
      {
        new ScoreEntry("Alice", 42, DateTimeOffset.UnixEpoch),
        new ScoreEntry("Bob", 17, DateTimeOffset.UnixEpoch)
      });

      this.OpenScoreboard();
      Assert.Equal("Rank 1, Alice, 42", this.tts.LastSpokenText);

      this.input.Press(Input.Down);
      this.scene.Update(FrameTime);
      Assert.Equal("Rank 2, Bob, 17", this.tts.LastSpokenText);

      this.input.Press(Input.Down);
      this.scene.Update(FrameTime);
      Assert.Equal("Back", this.tts.LastSpokenText);

      this.input.Press(Input.Back);
      this.scene.Update(FrameTime);
      Assert.Equal("Play", this.tts.LastSpokenText);
    }

    /// <summary>
    /// An empty scoreboard is announced as empty.
    /// </summary>
    [Fact]
    public void Scoreboard_EmptyIsAnnounced()
    {
      this.OpenScoreboard();

      Assert.Contains("Scoreboard is empty", this.tts.SpokenTexts);
      Assert.Equal("Back", this.tts.LastSpokenText);
    }

    /// <summary>
    /// Starts a run and collides with obstacles until the run ends.
    /// </summary>
    private void StartRunAndLose()
    {
      this.scene.OnEnter();
      this.input.Press(Input.Enter);
      this.scene.Update(FrameTime);

      RiverRunSession session = this.scene.Game.Session;

      while (session.State == SessionState.Playing)
      {
        session.QueueObstacle(RunConstants.LaneX.Center, session.Player.Position.Z);
        this.scene.Update(FrameTime);
      }
    }

    /// <summary>
    /// Selects Submit score in the end-game menu.
    /// </summary>
    private void OpenSubmitScore()
    {
      this.input.Press(Input.Down);
      this.scene.Update(FrameTime);
      this.input.Press(Input.Enter);
      this.scene.Update(FrameTime);
    }

    /// <summary>
    /// Opens the scoreboard from the start menu.
    /// </summary>
    private void OpenScoreboard()
    {
      this.scene.OnEnter();
      this.input.Press(Input.Down);
      this.scene.Update(FrameTime);
      this.input.Press(Input.Enter);
      this.scene.Update(FrameTime);
    }
  }
}
