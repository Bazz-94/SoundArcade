namespace SoundArcade.Application
{
  using System;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Colors;
  using SoundArcade.Domain.Models;
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
    private bool shouldExit;

    private IWindow Window { get; set; }
    private IRenderer Renderer { get; set; }
    private IInput Input { get; set; }
    private ITts Tts { get; set; }
    private IAudio Audio { get; set; }
    private SceneManager SceneManager { get; set; }
    public Theme Theme { get; }
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
      Theme theme)
    {
      this.Window = window;
      this.Renderer = renderer;
      this.Input = input;
      this.Tts = tts;
      this.Audio = audio;
      this.SettingsStore = settingsStore;
      this.SceneManager = sceneManager;
      this.Theme = theme;
      this.AppSettings = new AppSettings();
      this.SceneManager.ExitRequested += this.Exit;
    }

    /// <summary>
    /// Runs the desktop application loop.
    /// </summary>
    public void Run()
    {
      this.Window.Initialize(WindowWidth, WindowHeight, WindowTitle);
      this.LoadSettings();
      this.SceneManager.SceneFactory = new SceneFactory(
        input: this.Input,
        tts: this.Tts,
        renderer: this.Renderer,
        audio: this.Audio,
        settingsStore: this.SettingsStore,
        appSettings: this.AppSettings,
        theme: this.Theme,
        sceneManager: this.SceneManager);
      this.SceneManager.ChangeScene(SceneType.MainMenu);

      while (!this.Window.ShouldClose && !shouldExit)
      {
        float deltaTime = this.Window.GetDeltaTime();
        this.SceneManager.Update(deltaTime);

        this.Window.BeginFrame();
        this.Renderer.Clear(this.Theme.ColorPalette.Background);
        this.SceneManager.Render();
        this.Window.EndFrame();
      }

      this.Window.Close();
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
  }
}
