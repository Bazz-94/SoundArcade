namespace SoundArcade.Application
{
  using System;
  using SoundArcade.Abstractions;
  using SoundArcade.Application.Scenes;
  using SoundArcade.Domain;
  using SoundArcade.Domain.Models;
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

    private static readonly Color BackgroundColor = new Color(Colors.Black);
    private static readonly Color LaneColor = new Color(Colors.Teal);
    private static readonly Color PlayerColor = new Color(Colors.Purple);
    private static readonly Color ObstacleColor = new Color(Colors.Pink);
    private static readonly Color HudLivesColor = new Color(Colors.Pink);
    private static readonly Color HudScoreColor = new Color(Colors.Purple);
    private static readonly Color MenuSelectedColor = new Color(Colors.Pink);
    private static readonly Color MenuUnselectedColor = new Color(Colors.Teal);
    private static readonly Color SettingsValueColor = new Color(Colors.Purple);
    private bool shouldExit;

    private IWindow Window { get; set; }
    private IRenderer Renderer { get; set; }
    private IInput Input { get; set; }
    private ITts Tts { get; set; }
    private IAudio Audio { get; set; }
    private SceneManager SceneManager { get; set; }
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
    /// <param name="session">RiverRun session state.</param>
    public ArcadeShell(
      IWindow window,
      IRenderer renderer,
      IInput input,
      ITts tts,
      IAudio audio,
      ISettingsStore settingsStore)
    {
      this.Window = window;
      this.Renderer = renderer;
      this.Input = input;
      this.Tts = tts;
      this.Audio = audio;
      this.SettingsStore = settingsStore;
      this.SceneManager = new SceneManager();
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
        this.Renderer.Clear(BackgroundColor);
        this.SceneManager.Render();
        this.Window.EndFrame();
      }

      this.Window.Close();
    }

    private MenuScene CreateMainMenuScene()
    {
      return new MainMenuScene(
        input: this.Input,
        tts: this.Tts,
        renderer: this.Renderer,
        selectedColor: MenuSelectedColor,
        unselectedColor: MenuUnselectedColor,
        startRunAction: () => this.SceneManager.ChangeScene(this.CreateRunScene()),
        settingsAction: () => this.SceneManager.ChangeScene(this.CreateSettingsMenuScene()),
        exitAction: this.Exit);
    }

    private MenuScene CreateSettingsMenuScene()
    {
      return new SettingsMenuScene(
        audio: this.Audio,
        appSettings: this.AppSettings,
        settingsStore: this.SettingsStore,
        input: this.Input,
        selectedColor: MenuSelectedColor,
        unselectedColor: MenuUnselectedColor,
        settingsValueColor: SettingsValueColor,
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
