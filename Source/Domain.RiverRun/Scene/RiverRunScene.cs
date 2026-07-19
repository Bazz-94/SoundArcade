namespace SoundArcade.Domain.RiverRun.Scene
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Colors;
  using SoundArcade.Domain.Models;
  using SoundArcade.Domain.RiverRun;
  using SoundArcade.Domain.RiverRun.Game;
  using SoundArcade.Domain.RiverRun.Models;
  using SoundArcade.Domain.RiverRun.Models.Enum;
  using SoundArcade.Domain.RiverRun.Services;
  using SoundArcade.Domain.Services;

  /// <summary>
  /// Gameplay scene for RiverRun run simulation and rendering.
  /// </summary>
  public sealed class RiverRunScene : IScene
  {
    /// <summary>
    /// Gets the game coordinator driving the current session.
    /// </summary>
    public Game Game { get; }

    private Menu PauseMenu { get; }
    private Menu StartMenu { get; }
    private Menu? EndGameMenu { get; set; }
    private Menu? ScoreboardMenu { get; set; }
    private NameEntry NameEntryOverlay { get; }
    private Overlay ActiveOverlay { get; set; }
    private bool ScoreSubmitted { get; set; }
    private IScoreboardStore ScoreboardStore { get; }
    private ITts Tts { get; }

    /// <summary>
    /// Gets the audio abstraction used for sound registration and playback.
    /// </summary>
    public IAudio Audio { get; }

    private IInput Input { get; }
    private IRenderer Renderer { get; }

    /// <summary>
    /// Gets the color theme applied to the scene.
    /// </summary>
    public Theme Theme { get; }

    private SceneManager SceneManager { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RiverRunScene"/> class.
    /// </summary>
    /// <param name="tts">Text-to-speech abstraction.</param>
    /// <param name="audio">Audio abstraction.</param>
    /// <param name="input">Input abstraction.</param>
    /// <param name="renderer">Renderer abstraction.</param>
    /// <param name="theme">Color theme.</param>
    /// <param name="sceneManager">Scene manager used to request transitions.</param>
    /// <param name="scoreboardStore">Scoreboard persistence.</param>
    public RiverRunScene(
      ITts tts,
      IAudio audio,
      IInput input,
      IRenderer renderer,
      Theme theme,
      SceneManager sceneManager,
      IScoreboardStore scoreboardStore)
    {
      this.Game = new Game(renderer, tts, audio, theme, new PlayerController(input));
      this.Audio = audio;
      this.Input = input;
      this.Renderer = renderer;
      this.Theme = theme;
      this.SceneManager = sceneManager;
      this.ScoreboardStore = scoreboardStore;
      this.Tts = tts;
      this.NameEntryOverlay = new NameEntry(input, tts, renderer, theme, (int)MenuType.NameEntry, "Enter your name");

      this.PauseMenu = new Menu(
        input,
        tts,
        renderer,
        id: (int)MenuType.Pause,
        menuTitle: "Pause Menu",
        theme: theme,
        items: [
          new MenuItem(theme, (int)PauseMenuItem.Resume, "Resume", this.ResumeRunFromPause),
          new MenuItem(theme, (int)PauseMenuItem.MainMenu, "Main Menu", this.OnPauseMenuMainMenuSelected)
        ]);

      this.StartMenu = new Menu(
        input,
        tts,
        renderer,
        id: (int)MenuType.Start,
        menuTitle: "River Run",
        theme: theme,
        items: [
          new MenuItem(theme, (int)StartMenuItem.Play, "Play", this.StartRun),
          new MenuItem(theme, (int)StartMenuItem.Scoreboard, "Scoreboard", this.OnScoreboardSelected),
          new MenuItem(theme, (int)StartMenuItem.Back, "Back", this.OnStartMenuBackSelected)
        ]);

      string assetBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
      this.Audio.RegisterGeneratedSound(RunConstants.SoundId.Collision, RunConstants.SoundProfiles.Collision);
      this.Audio.RegisterGeneratedSound(RunConstants.SoundId.ObstacleNoise, RunConstants.SoundProfiles.ObstacleNoise);
      this.Audio.RegisterGeneratedSound(RunConstants.SoundId.Reward, RunConstants.SoundProfiles.Reward);
      this.Audio.RegisterGeneratedSound(RunConstants.SoundId.RewardNoise, RunConstants.SoundProfiles.RewardNoise);
      this.Audio.RegisterSound(RunConstants.SoundId.RiverNoise, Path.Combine(assetBasePath, "RiverNoise.mp3"), RunConstants.Gain.RiverNoise);
    }

    /// <inheritdoc />
    public void OnEnter()
    {
      this.ShowStartMenu();
    }

    /// <inheritdoc />
    public void OnExit()
    {
      this.Renderer.ResetCamera();
    }

    /// <inheritdoc />
    public void Update(float deltaTime)
    {
      switch (this.ActiveOverlay)
      {
        case Overlay.StartMenu:
          this.StartMenu.Update();

          if (this.Input.InputPressed(Abstractions.Input.Back))
          {
            this.OnStartMenuBackSelected();
          }
          return;

        case Overlay.Scoreboard:
          this.ScoreboardMenu!.Update();

          if (this.Input.InputPressed(Abstractions.Input.Back))
          {
            this.ShowStartMenu();
          }
          return;

        case Overlay.EndGameMenu:
          this.EndGameMenu!.Update();
          return;

        case Overlay.NameEntry:
          this.UpdateNameEntry();
          return;
      }

      if (this.Game.Session.State == SessionState.Paused)
      {
        this.PauseMenu.Update();

        if (this.Input.InputPressed(Abstractions.Input.Back))
        {
          this.ResumeRunFromPause();
        }
        return;
      }

      this.Game.Tick(deltaTime);

      if (this.Game.Session.State == SessionState.Paused)
      {
        this.PauseMenu.SelectFirstItem();
      }

      if (this.Game.Session.State == SessionState.GameOver)
      {
        // Queue the menu announcement so the game-over score announcement is not cut off.
        this.ShowEndGameMenu(interruptSpeech: false);
      }
    }

    /// <inheritdoc />
    public void Render()
    {
      switch (this.ActiveOverlay)
      {
        case Overlay.StartMenu:
          this.StartMenu.Render();
          return;

        case Overlay.Scoreboard:
          this.ScoreboardMenu!.Render();
          return;

        case Overlay.EndGameMenu:
          this.Game.Render();
          this.EndGameMenu!.Render();
          return;

        case Overlay.NameEntry:
          this.Game.Render();
          this.NameEntryOverlay.Render();
          return;
      }

      this.Game.Render();

      if (this.Game.Session.State == SessionState.Paused)
      {
        this.PauseMenu.Render();
      }
    }

    /// <inheritdoc />
    public void OnBackSelected()
    {
      switch (this.ActiveOverlay)
      {
        case Overlay.StartMenu:
          this.OnStartMenuBackSelected();
          return;

        case Overlay.Scoreboard:
          this.ShowStartMenu();
          return;
      }

      if (this.Game.Session.State == SessionState.Paused)
      {
        this.ResumeRunFromPause();
      }
    }

    private void ResumeRunFromPause()
    {
      this.Game.DispatchCommand(RunCommand.TogglePause);
    }

    private void OnPauseMenuMainMenuSelected()
    {
      this.SceneManager.ChangeScene(SceneType.MainMenu);
    }

    /// <summary>
    /// Shows and announces the start menu.
    /// </summary>
    private void ShowStartMenu()
    {
      this.ActiveOverlay = Overlay.StartMenu;
      this.StartMenu.SelectFirstItem();
    }

    /// <summary>
    /// Leaves the current overlay and starts a new run.
    /// </summary>
    private void StartRun()
    {
      this.ActiveOverlay = Overlay.Gameplay;
      this.ScoreSubmitted = false;
      this.Game.Start();
    }

    /// <summary>
    /// Opens the scoreboard overlay with the persisted entries as navigable items.
    /// </summary>
    private void OnScoreboardSelected()
    {
      IReadOnlyList<ScoreEntry> entries = new Scoreboard(this.ScoreboardStore.Load(RiverRunGame.Id)).Entries;
      List<MenuItem> items = new List<MenuItem>();

      foreach (ScoreEntry entry in entries)
      {
        int rank = items.Count + 1;
        items.Add(new MenuItem(this.Theme, rank, $"Rank {rank}, {entry.Name}, {entry.Score}", () => { }));
      }

      items.Add(new MenuItem(this.Theme, items.Count + 1, "Back", this.ShowStartMenu));

      this.ScoreboardMenu = new Menu(
        this.Input,
        this.Tts,
        this.Renderer,
        id: (int)MenuType.Scoreboard,
        menuTitle: entries.Count == 0 ? "Scoreboard is empty" : "Scoreboard",
        theme: this.Theme,
        items: items);

      this.ActiveOverlay = Overlay.Scoreboard;
      this.ScoreboardMenu.SelectFirstItem();
    }

    /// <summary>
    /// Shows and announces the end-game menu; Submit score is offered at most once per run.
    /// </summary>
    /// <param name="interruptSpeech">When false, the announcement queues after active speech such as the game-over score.</param>
    private void ShowEndGameMenu(bool interruptSpeech = true)
    {
      List<MenuItem> items = new List<MenuItem>
      {
        new MenuItem(this.Theme, (int)EndGameMenuItem.TryAgain, "Try again", this.StartRun)
      };

      if (!this.ScoreSubmitted)
      {
        items.Add(new MenuItem(this.Theme, (int)EndGameMenuItem.SubmitScore, "Submit score", this.OnSubmitScoreSelected));
      }

      items.Add(new MenuItem(this.Theme, (int)EndGameMenuItem.MainMenu, "Main Menu", this.OnPauseMenuMainMenuSelected));

      this.EndGameMenu = new Menu(
        this.Input,
        this.Tts,
        this.Renderer,
        id: (int)MenuType.EndGame,
        menuTitle: "End game menu",
        theme: this.Theme,
        items: items);

      this.ActiveOverlay = Overlay.EndGameMenu;
      this.EndGameMenu.SelectFirstItem(interruptSpeech);
    }

    /// <summary>
    /// Opens the name entry overlay for score submission.
    /// </summary>
    private void OnSubmitScoreSelected()
    {
      this.ActiveOverlay = Overlay.NameEntry;
      this.NameEntryOverlay.Open();
    }

    /// <summary>
    /// Advances name entry; a confirmed name saves the score, cancelling saves nothing.
    /// </summary>
    private void UpdateNameEntry()
    {
      switch (this.NameEntryOverlay.Update())
      {
        case NameEntryStatus.Confirmed:
          this.SaveScore(this.NameEntryOverlay.Name);
          this.ShowEndGameMenu(interruptSpeech: false);
          break;

        case NameEntryStatus.Cancelled:
          this.ShowEndGameMenu();
          break;
      }
    }

    /// <summary>
    /// Persists the finished run's score under the given name and confirms via speech.
    /// </summary>
    /// <param name="name">Player name to save the score under.</param>
    private void SaveScore(string name)
    {
      Scoreboard scoreboard = new Scoreboard(this.ScoreboardStore.Load(RiverRunGame.Id));
      scoreboard.Add(new ScoreEntry(name, this.Game.Session.Score, DateTimeOffset.UtcNow));
      this.ScoreboardStore.Save(RiverRunGame.Id, scoreboard.Entries);
      this.ScoreSubmitted = true;
      this.Tts.Stop();
      this.Tts.SpeakAsync("Score saved");
    }

    private void OnStartMenuBackSelected()
    {
      this.SceneManager.ChangeScene(SceneType.GameSelectionMenu);
    }

    /// <summary>
    /// Overlays swapped within the scene; gameplay is the no-overlay state.
    /// </summary>
    private enum Overlay
    {
      StartMenu,
      Gameplay,
      EndGameMenu,
      NameEntry,
      Scoreboard
    }

    private enum MenuType
    {
      Pause,
      Start,
      EndGame,
      Scoreboard,
      NameEntry
    }

    private enum PauseMenuItem
    {
      Resume,
      MainMenu
    }

    private enum StartMenuItem
    {
      Play,
      Scoreboard,
      Back
    }

    private enum EndGameMenuItem
    {
      TryAgain,
      SubmitScore,
      MainMenu
    }
  }
}
