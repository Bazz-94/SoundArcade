namespace SoundArcade.Domain.RiverRun.Scene
{
  using System;
  using System.IO;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Colors;
  using SoundArcade.Domain.Models;
  using SoundArcade.Domain.RiverRun.Game;
  using SoundArcade.Domain.RiverRun.Models;
  using SoundArcade.Domain.Services;

  /// <summary>
  /// Gameplay scene for RiverRun run simulation and rendering.
  /// </summary>
  public sealed class RiverRunScene : IScene
  {
    private const float PauseMenuZOffset = 2.5f;

    private Game Game { get; }
    private Menu Menu { get; }
    public IAudio Audio { get; }
    private IInput Input { get; }
    private IRenderer Renderer { get; }
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
    public RiverRunScene(
      ITts tts,
      IAudio audio,
      IInput input,
      IRenderer renderer,
      Theme theme,
      SceneManager sceneManager)
    {
      this.Game = new Game(renderer, tts, audio, theme, new PlayerController(input));
      this.Audio = audio;
      this.Input = input;
      this.Renderer = renderer;
      this.Theme = theme;
      this.SceneManager = sceneManager;
      this.Audio = audio;

      this.Menu = new Menu(
        input,
        tts,
        renderer,
        id: (int)PauseMenuType.Pause,
        menuTitle: "Pause Menu",
        theme: theme,
        items: [
          new MenuItem(theme, (int)PauseMenuItem.Resume, "Resume", this.ResumeRunFromPause),
          new MenuItem(theme, (int)PauseMenuItem.MainMenu, "Main Menu", this.OnPauseMenuMainMenuSelected)
        ]);

      string assetBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
      this.Audio.RegisterSound(RunConstants.SoundId.Collision, Path.Combine(assetBasePath, "Obstacle.mp3"));
      this.Audio.RegisterSound(RunConstants.SoundId.ObstacleNoise, Path.Combine(assetBasePath, "ObstacleNoise.mp3"));
      this.Audio.RegisterSound(RunConstants.SoundId.Reward, Path.Combine(assetBasePath, "Reward.mp3"));
      this.Audio.RegisterSound(RunConstants.SoundId.RewardNoise, Path.Combine(assetBasePath, "RewardNoise.mp3"));
      this.Audio.RegisterSound(RunConstants.SoundId.RiverNoise, Path.Combine(assetBasePath, "RiverNoise.mp3"));
    }

    /// <inheritdoc />
    public void OnEnter()
    {
      this.Game.Start();
    }

    /// <inheritdoc />
    public void OnExit()
    {
      this.Renderer.ResetCamera();
    }

    /// <inheritdoc />
    public void Update(float deltaTime)
    {
      if (this.Game.Session.State == SessionState.Paused)
      {
        this.Menu.Update();

        if (this.Input.InputPressed(Abstractions.Input.Back))
        {
          this.ResumeRunFromPause();
        }
        return;
      }

      this.Game.Tick(deltaTime);

      if (this.Game.Session.State == SessionState.Paused)
      {
        this.Menu.SelectFirstItem();
      }
    }

    /// <inheritdoc />
    public void Render()
    {
      this.Game.Render();

      if (this.Game.Session.State == SessionState.Paused)
      {
        this.Menu.MenuZ = this.Game.Session.Player.Position.Z + PauseMenuZOffset;
        this.Menu.Render();
      }
    }

    /// <inheritdoc />
    public void OnBackSelected()
    {
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
