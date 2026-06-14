namespace SoundArcade.Domain.RiverRun.Scene
{
  using System;
  using System.Numerics;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Colors;
  using SoundArcade.Domain.Models;
  using SoundArcade.Domain.RiverRun.Game;
  using SoundArcade.Domain.RiverRun.Models;
  using SoundArcade.Domain.RiverRun.Services;

  /// <summary>
  /// Gameplay scene for RiverRun run simulation and rendering.
  /// </summary>
  public sealed class RiverRunScene : IScene
  {
    private readonly GameLoop gameLoop;
    private readonly Menu pauseMenu;
    private readonly IInput input;
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
    /// <param name="menuColors">Pause menu color palette.</param>
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
      MenuColors menuColors,
      Action onMainMenuRequested,
      Color laneColor,
      Color playerColor,
      Color obstacleColor,
      Color hudLivesColor,
      Color hudScoreColor)
    {
      gameLoop = new GameLoop(new PlayerController(input), tts, audio, new RiverRunSession(new RiverRunSettings()));
      this.input = input;
      this.renderer = renderer;
      this.onMainMenuRequested = onMainMenuRequested;
      this.laneColor = laneColor;
      this.playerColor = playerColor;
      this.obstacleColor = obstacleColor;
      this.hudLivesColor = hudLivesColor;
      this.hudScoreColor = hudScoreColor;

      pauseMenu = new Menu(
        input,
        tts,
        renderer,
        id: (int)PauseMenuType.Pause,
        menuTitle: "Pause Menu",
        menuColors: menuColors,
        items: [
          new MenuItem((int)PauseMenuItem.Resume, "Resume", this.ResumeRunFromPause),
          new MenuItem((int)PauseMenuItem.MainMenu, "Main Menu", OnPauseMenuMainMenuSelected)
        ]);
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
        pauseMenu.SelectFirstItem();
      }
    }

    /// <inheritdoc />
    public void Render()
    {
      this.RenderWorld();
      this.RenderHud();

      if (isPaused)
      {
        pauseMenu.Render();
      }
    }

    /// <inheritdoc />
    public void OnBackSelected()
    {
      if (isPaused)
      {
        this.ResumeRunFromPause();
      }
    }

    private void UpdatePauseMenu()
    {
      pauseMenu.Update();

      if (input.InputPressed(Input.Back))
      {
        this.ResumeRunFromPause();
      }
    }

    private void ResumeRunFromPause()
    {
      gameLoop.DispatchCommand(RunCommand.TogglePause);
      isPaused = false;
    }

    private void OnPauseMenuMainMenuSelected()
    {
      this.ResumeRunFromPause();
      onMainMenuRequested();
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
