namespace SoundArcade.Application
{
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
    private bool shouldExit;

    private IWindow Window { get; }
    private IRenderer Renderer { get; }
    private IInput Input { get; }
    private ITts Tts { get; }
    private IAudio Audio { get; }
    private SceneManager SceneManager { get; }
    private ISceneFactory SceneFactory { get; }
    private Theme Theme { get; }
    private AppSettings AppSettings { get; }
    private ISettingsStore SettingsStore { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArcadeShell"/> class.
    /// </summary>
    /// <param name="window">Window abstraction.</param>
    /// <param name="renderer">Renderer abstraction.</param>
    /// <param name="input">Input abstraction.</param>
    /// <param name="tts">Text-to-speech abstraction.</param>
    /// <param name="audio">Audio abstraction.</param>
    /// <param name="settingsStore">Settings store abstraction.</param>
    /// <param name="appSettings">Application settings.</param>
    /// <param name="sceneManager">Scene manager abstraction.</param>
    /// <param name="sceneFactory">Factory that builds scenes.</param>
    /// <param name="theme">Theme for colors.</param>
    public ArcadeShell(
      IWindow window,
      IRenderer renderer,
      IInput input,
      ITts tts,
      IAudio audio,
      ISettingsStore settingsStore,
      AppSettings appSettings,
      SceneManager sceneManager,
      ISceneFactory sceneFactory,
      Theme theme)
    {
      this.Window = window;
      this.Renderer = renderer;
      this.Input = input;
      this.Tts = tts;
      this.Audio = audio;
      this.SettingsStore = settingsStore;
      this.AppSettings = appSettings;
      this.SceneManager = sceneManager;
      this.SceneFactory = sceneFactory;
      this.Theme = theme;
      this.SceneManager.ExitRequested += this.Exit;
    }

    /// <summary>
    /// Runs the desktop application loop.
    /// </summary>
    public void Run()
    {
      this.Window.Initialize(WindowWidth, WindowHeight, WindowTitle);
      this.LoadSettings();
      this.SceneManager.SceneFactory = this.SceneFactory;
      this.SceneManager.ChangeScene(SceneType.MainMenu);

      while (!this.Window.ShouldClose && !this.shouldExit)
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
      this.shouldExit = true;
    }

    private void LoadSettings()
    {
      this.AppSettings.CopyFrom(this.SettingsStore.Load());
      this.Audio.SetMasterVolume(this.AppSettings.MasterVolume);
      this.Tts.SetVolume(this.AppSettings.TtsVolume);
      this.Input.LoadMappings();
    }
  }
}
