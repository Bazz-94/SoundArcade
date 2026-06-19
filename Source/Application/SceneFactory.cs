namespace SoundArcade.Application
{
  using System;
  using SoundArcade.Abstractions;
  using SoundArcade.Application.Scenes;
  using SoundArcade.Domain.Colors;
  using SoundArcade.Domain.Models;
  using SoundArcade.Domain.RiverRun.Scene;
  using SoundArcade.Domain.Services;

  /// <summary>
  /// Builds scenes with the dependencies they need, keyed by <see cref="SceneType"/>.
  /// </summary>
  public sealed class SceneFactory : ISceneFactory
  {
    private IInput Input { get; }
    private ITts Tts { get; }
    private IRenderer Renderer { get; }
    private IAudio Audio { get; }
    private ISettingsStore SettingsStore { get; }
    private AppSettings AppSettings { get; }
    private Theme Theme { get; }
    private SceneManager SceneManager { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneFactory"/> class.
    /// </summary>
    public SceneFactory(
      IInput input,
      ITts tts,
      IRenderer renderer,
      IAudio audio,
      ISettingsStore settingsStore,
      AppSettings appSettings,
      Theme theme,
      SceneManager sceneManager)
    {
      this.Input = input;
      this.Tts = tts;
      this.Renderer = renderer;
      this.Audio = audio;
      this.SettingsStore = settingsStore;
      this.AppSettings = appSettings;
      this.Theme = theme;
      this.SceneManager = sceneManager;
    }

    /// <inheritdoc />
    public IScene CreateScene(SceneType sceneType)
    {
      return sceneType switch
      {
        SceneType.MainMenu => this.CreateMainMenuScene(),
        SceneType.GameSelectionMenu => this.CreateGameSelectionMenuScene(),
        SceneType.SettingsMenu => this.CreateSettingsMenuScene(),
        SceneType.Run => this.CreateRunScene(),
        _ => throw new ArgumentOutOfRangeException(nameof(sceneType), sceneType, "Unknown scene type.")
      };
    }

    private IScene CreateMainMenuScene()
    {
      return new MainMenuScene(
        input: this.Input,
        tts: this.Tts,
        renderer: this.Renderer,
        theme: this.Theme,
        sceneManager: this.SceneManager);
    }

    private IScene CreateGameSelectionMenuScene()
    {
      return new GameSelectionMenuScene(
        input: this.Input,
        tts: this.Tts,
        renderer: this.Renderer,
        theme: this.Theme,
        sceneManager: this.SceneManager);
    }

    private IScene CreateSettingsMenuScene()
    {
      return new SettingsMenuScene(
        audio: this.Audio,
        appSettings: this.AppSettings,
        settingsStore: this.SettingsStore,
        input: this.Input,
        tts: this.Tts,
        renderer: this.Renderer,
        theme: this.Theme,
        sceneManager: this.SceneManager);
    }

    private IScene CreateRunScene()
    {
      return new RiverRunScene(
        tts: this.Tts,
        audio: this.Audio,
        input: this.Input,
        renderer: this.Renderer,
        theme: this.Theme,
        sceneManager: this.SceneManager);
    }
  }
}
