namespace SoundArcade.Domain.RiverRun.Scene
{
  using System;
  using System.Collections.Generic;
  using System.Numerics;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain;
  using SoundArcade.Domain.Models;
  using SoundArcade.Domain.RiverRun.Game;
  using SoundArcade.Domain.RiverRun.Models;
  using SoundArcade.Domain.RiverRun.Services;

  /// <summary>
  /// Gameplay scene for RiverRun run simulation and rendering.
  /// </summary>
  public sealed class RiverRunScene : IScene
  {
    private const float PauseMenuStartY = 3.0f;
    private const float PauseMenuItemSpacing = 0.8f;
    private const int PauseMenuItemFontSize = 22;
    private const float PauseMenuItemTextZOffset = 0.16f;
    private const float PauseMenuZ = 1.5f;

    private static readonly Vector3 PauseMenuItemSize = new Vector3(2.4f, 0.28f, 0.28f);
    private static readonly Color PauseMenuSelectedColor = new Color(Colors.Pink);
    private static readonly Color PauseMenuUnselectedColor = new Color(Colors.Teal);

    private readonly GameLoop gameLoop;
    private readonly Menu pauseMenu;
    private readonly IReadOnlyDictionary<int, Action<MenuItem>> pauseMenuActions;
    private readonly IInput input;
    private readonly ITts tts;
    private readonly IRenderer renderer;
    private readonly Action onMainMenuRequested;
    private readonly Color laneColor;
    private readonly Color playerColor;
    private readonly Color obstacleColor;
    private readonly Color hudLivesColor;
    private readonly Color hudScoreColor;
    private bool isPaused;

    /// <summary>
    /// Initializes a new instance of the <see cref="RiverRunScene"/> class.
    /// </summary>
    /// <param name="tts">Text-to-speech abstraction.</param>
    /// <param name="audio">Audio abstraction.</param>
    /// <param name="input">Input abstraction.</param>
    /// <param name="renderer">Renderer abstraction.</param>
    /// <param name="onMainMenuRequested">Callback invoked when the scene should return to the main menu.</param>
    /// <param name="laneColor">Lane color.</param>
    /// <param name="playerColor">Player color.</param>
    /// <param name="obstacleColor">Obstacle color.</param>
    /// <param name="hudLivesColor">HUD lives color.</param>
    /// <param name="hudScoreColor">HUD score color.</param>
    public RiverRunScene(
      ITts tts,
      IAudio audio,
      IInput input,
      IRenderer renderer,
      Action onMainMenuRequested,
      Color laneColor,
      Color playerColor,
      Color obstacleColor,
      Color hudLivesColor,
      Color hudScoreColor)
    {
      gameLoop = new GameLoop(new PlayerController(input), tts, audio, new RiverRunSession(new RiverRunSettings()));
      pauseMenu = this.CreatePauseMenu();
      pauseMenuActions = this.CreatePauseMenuActions();
      this.input = input;
      this.tts = tts;
      this.renderer = renderer;
      this.onMainMenuRequested = onMainMenuRequested;
      this.laneColor = laneColor;
      this.playerColor = playerColor;
      this.obstacleColor = obstacleColor;
      this.hudLivesColor = hudLivesColor;
      this.hudScoreColor = hudScoreColor;
    }

    /// <inheritdoc />
    public void OnEnter()
    {
      isPaused = false;
      gameLoop.Start();
    }

    /// <inheritdoc />
    public void OnExit()
    {
    }

    /// <inheritdoc />
    public void Update(float deltaTime)
    {
      if (isPaused)
      {
        this.UpdatePauseMenu();
        return;
      }

      gameLoop.Tick(deltaTime);

      if (gameLoop.Session.State == SessionState.Paused)
      {
        isPaused = true;
        this.OnPauseMenuEnter();
      }
    }

    /// <inheritdoc />
    public void Render()
    {
      this.RenderWorld();
      this.RenderHud();

      if (isPaused)
      {
        this.RenderPauseMenu();
      }
    }

    private Menu CreatePauseMenu()
    {
      return new Menu(
        (int)PauseMenuType.Pause,
        "Pause menu",
        [
          new MenuItem((int)PauseMenuItem.Resume, "Resume"),
          new MenuItem((int)PauseMenuItem.MainMenu, "Main Menu")
        ]);
    }

    private IReadOnlyDictionary<int, Action<MenuItem>> CreatePauseMenuActions()
    {
      Dictionary<int, Action<MenuItem>> actions = new Dictionary<int, Action<MenuItem>>
      {
        [(int)PauseMenuItem.Resume] = this.OnPauseMenuResumeSelected,
        [(int)PauseMenuItem.MainMenu] = this.OnPauseMenuMainMenuSelected
      };

      return actions;
    }

    private void OnPauseMenuEnter()
    {
      pauseMenu.SelectFirst();
      tts.SpeakAsync(pauseMenu.DisplayText);
      tts.SpeakAsync(pauseMenu.SelectedItem.DisplayText);
    }

    private void UpdatePauseMenu()
    {
      bool selectionChanged = false;

      if (input.InputPressed(Input.Up))
      {
        pauseMenu.MovePrevious();
        selectionChanged = true;
      }

      if (input.InputPressed(Input.Down))
      {
        pauseMenu.MoveNext();
        selectionChanged = true;
      }

      if (selectionChanged)
      {
        tts.SpeakAsync(pauseMenu.SelectedItem.DisplayText);
      }

      if (input.InputPressed(Input.Back))
      {
        this.ResumeRunFromPause();
        return;
      }

      if (input.InputPressed(Input.Enter))
      {
        this.OnPauseMenuSelected(pauseMenu.SelectedItem);
        return;
      }
    }

    private void OnPauseMenuSelected(MenuItem item)
    {
      if (!pauseMenuActions.TryGetValue(item.Id, out Action<MenuItem>? action))
      {
        throw new InvalidOperationException($"No action configured for pause menu item {item.Id}.");
      }

      action(item);
    }

    private void ResumeRunFromPause()
    {
      gameLoop.DispatchCommand(RunCommand.TogglePause);
      isPaused = false;
    }

    private void OnPauseMenuResumeSelected(MenuItem item)
    {
      this.ResumeRunFromPause();
    }

    private void OnPauseMenuMainMenuSelected(MenuItem item)
    {
      this.ResumeRunFromPause();
      onMainMenuRequested();
    }

    private void RenderPauseMenu()
    {
      int itemIndex = 0;

      foreach (MenuItem item in pauseMenu.Items)
      {
        float y = PauseMenuStartY - (itemIndex * PauseMenuItemSpacing);
        bool isSelected = itemIndex == pauseMenu.SelectedIndex;
        Color itemColor = isSelected ? PauseMenuSelectedColor : PauseMenuUnselectedColor;
        Color textColor = isSelected ? PauseMenuUnselectedColor : PauseMenuSelectedColor;

        renderer.DrawBox(new Vector3(0.0f, y, PauseMenuZ), PauseMenuItemSize, itemColor);
        renderer.DrawText(new Vector3(0.0f, y, PauseMenuZ + PauseMenuItemTextZOffset), item.DisplayText, PauseMenuItemFontSize, textColor);
        itemIndex++;
      }
    }

    private void RenderWorld()
    {
      float laneStartZ = gameLoop.Session.Player.Position.Z - 2.0f;
      float laneEndZ = laneStartZ + 28.0f;

      renderer.DrawLine(new Vector3(RunConstants.LaneX.Left, RunConstants.GroundY, laneStartZ), new Vector3(RunConstants.LaneX.Left, RunConstants.GroundY, laneEndZ), laneColor);
      renderer.DrawLine(new Vector3(RunConstants.LaneX.Center, RunConstants.GroundY, laneStartZ), new Vector3(RunConstants.LaneX.Center, RunConstants.GroundY, laneEndZ), laneColor);
      renderer.DrawLine(new Vector3(RunConstants.LaneX.Right, RunConstants.GroundY, laneStartZ), new Vector3(RunConstants.LaneX.Right, RunConstants.GroundY, laneEndZ), laneColor);

      renderer.DrawSphere(gameLoop.Session.Player.Position, 0.35f, playerColor);

      foreach (RunObstacle obstacle in gameLoop.Session.Obstacles)
      {
        renderer.DrawBox(obstacle.Position, new Vector3(0.6f, 0.6f, 0.6f), obstacleColor);
      }
    }

    private void RenderHud()
    {
      int lives = Math.Max(0, gameLoop.Session.Lives);
      int score = Math.Max(0, gameLoop.Session.Score);

      for (int i = 0; i < lives; i++)
      {
        renderer.DrawSphere(new Vector3(-3.5f + (i * 0.45f), 5.8f, 6.5f), 0.12f, hudLivesColor);
      }

      renderer.DrawText(new Vector3(1.0f, 5.6f, 6.5f), $"Score: {score}", 24, hudScoreColor);
    }

    private enum PauseMenuType
    {
      Pause
    }

    private enum PauseMenuItem
    {
      Resume,
      MainMenu
    }
  }
}
