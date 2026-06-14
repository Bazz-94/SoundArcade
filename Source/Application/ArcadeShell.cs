namespace SoundArcade.Application
{
  using System;
  using SoundArcade.Abstractions;
  using SoundArcade.Application.Scenes;
  using SoundArcade.Domain.Colors;
  using SoundArcade.Domain.RiverRun.Scene;
  using SoundArcade.Domain.Services;

  /// <summary>
  /// Hosts the desktop loop and delegates behavior to reusable scenes.
  /// </summary>
  public sealed class ArcadeShell
  {
    private const int WindowWidth = 1280;
    private const int WindowHeight = 720;
    private const string WindowTitle = "Sound Arcade";
    private const float MinimumVolume = 0.0f;
    private const float MaximumVolume = 1.0f;

    private static readonly Color LaneColor = new Color(ColorsHex.Teal);
    private static readonly Color PlayerColor = new Color(ColorsHex.Purple);
    private static readonly Color ObstacleColor = new Color(ColorsHex.Pink);
    private static readonly Color HudLivesColor = new Color(ColorsHex.Pink);
    private static readonly Color HudScoreColor = new Color(ColorsHex.Purple);
    private bool shouldExit;

    private IWindow Window { get; set; }
    private IRenderer Renderer { get; set; }
    private IInput Input { get; set; }
    private ITts Tts { get; set; }
    private IAudio Audio { get; set; }
    private SceneManager SceneManager { get; set; }
    public ColorPalette ColorPalette { get; }
    private AppSettings AppSettings { get; set; }
    private ISettingsStore SettingsStore { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArcadeShell"/> class.
    /// </summary>
    /// <param name="window">Window abstraction.</param>
    /// <param name="renderer">Renderer abstraction.</param>
    /// <param name="input">Input abstraction.</param>
    /// <param name="tts">Text-to-speech abstraction.</param>
    /// <param name="audio">Audio abstraction.</param>
    /// <param name="settingsStore">Settings store abstraction.</param>
    /// <param name="sceneManager">Scene manager abstraction.</param>
    public ArcadeShell(
      IWindow window,
      IRenderer renderer,
      IInput input,
      ITts tts,
      IAudio audio,
      ISettingsStore settingsStore,
      SceneManager sceneManager,
      ColorPalette colorPalette)
    {
      this.Window = window;
      this.Renderer = renderer;
      this.Input = input;
      this.Tts = tts;
      this.Audio = audio;
      this.SettingsStore = settingsStore;
      this.SceneManager = sceneManager;
      this.ColorPalette = colorPalette;
      this.AppSettings = new AppSettings();
    }

    /// <summary>
    /// Runs the desktop application loop.
    /// </summary>
    public void Run()
    {
      this.Window.Initialize(WindowWidth, WindowHeight, WindowTitle);
      this.LoadSettings();
      this.SceneManager.ChangeScene(this.CreateMainMenuScene());

      while (!this.Window.ShouldClose && !shouldExit)
      {
        float deltaTime = this.Window.GetDeltaTime();
        this.SceneManager.Update(deltaTime);

        this.Window.BeginFrame();
        this.Renderer.Clear(this.ColorPalette.Menu.Background);
        this.SceneManager.Render();
        this.Window.EndFrame();
      }

      this.Window.Close();
    }

    private MainMenuScene CreateMainMenuScene()
    {
      return new MainMenuScene(
        input: this.Input,
        tts: this.Tts,
        renderer: this.Renderer,
        menuColors: this.ColorPalette.Menu,
        startRunAction: () => this.SceneManager.ChangeScene(this.CreateRunScene()),
        settingsAction: () => this.SceneManager.ChangeScene(this.CreateSettingsMenuScene()),
        exitAction: this.Exit);
    }

    private SettingsMenuScene CreateSettingsMenuScene()
    {
      return new SettingsMenuScene(
        audio: this.Audio,
        appSettings: this.AppSettings,
        settingsStore: this.SettingsStore,
        input: this.Input,
        menuColors: this.ColorPalette.Menu,
        settingsValueColor: this.ColorPalette.Menu.Text,
        tts: this.Tts,
        renderer: this.Renderer,
        backAction: () => this.SceneManager.ChangeScene(this.CreateMainMenuScene()));
    }

    private RiverRunScene CreateRunScene()
    {
      return new RiverRunScene(
        tts: this.Tts,
        audio: this.Audio,
        input: this.Input,
        renderer: this.Renderer,
        menuColors: this.ColorPalette.Menu,
        onMainMenuRequested: () => this.SceneManager.ChangeScene(this.CreateMainMenuScene()),
        laneColor: LaneColor,
        playerColor: PlayerColor,
        obstacleColor: ObstacleColor,
        hudLivesColor: HudLivesColor,
        hudScoreColor: HudScoreColor);
    }

    private void Exit()
    {
      shouldExit = true;
    }

    private void LoadSettings()
    {
      this.AppSettings = this.SettingsStore.Load();
      this.AppSettings.MasterVolume = Math.Clamp(this.AppSettings.MasterVolume, MinimumVolume, MaximumVolume);
      this.AppSettings.TtsVolume = Math.Clamp(this.AppSettings.TtsVolume, MinimumVolume, MaximumVolume);
      this.Audio.SetMasterVolume(this.AppSettings.MasterVolume);
      this.Tts.SetVolume(this.AppSettings.TtsVolume);
      this.Input.LoadMappings();
    }

    public enum MenuType
    {
      Main,
      Settings
    }

    public enum MainMenuItem
    {
      StartRun,
      Settings,
      Exit
    }
  }
}
