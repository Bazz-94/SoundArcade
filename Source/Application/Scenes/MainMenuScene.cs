namespace SoundArcade.Application.Scenes
{
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Colors;
  using SoundArcade.Domain.Enums;
  using SoundArcade.Domain.Models;
  using SoundArcade.Domain.Services;

  /// <summary>
  /// Top-level menu offering play, settings, and exit.
  /// </summary>
  public sealed class MainMenuScene : MenuScene
  {
    /// <inheritdoc />
    protected override Menu Menu { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainMenuScene"/> class.
    /// </summary>
    /// <param name="input">Input abstraction.</param>
    /// <param name="tts">Text-to-speech abstraction.</param>
    /// <param name="renderer">Renderer abstraction.</param>
    /// <param name="theme">Theme for colors.</param>
    /// <param name="sceneManager">Scene manager for transitions.</param>
    public MainMenuScene(
      IInput input,
      ITts tts,
      IRenderer renderer,
      Theme theme,
      SceneManager sceneManager)
      : base(input, sceneManager)
    {
      this.Menu = new Menu(
        input,
        tts,
        renderer,
        (int)MenuType.Main,
        [
          new MenuItem(theme, (int)MainMenuItem.Play, MenuText.PlayLabel, () => this.SceneManager.ChangeScene(SceneType.GameSelectionMenu)),
          new MenuItem(theme, (int)MainMenuItem.Settings, MenuText.SettingsLabel, () => this.SceneManager.ChangeScene(SceneType.SettingsMenu)),
          new MenuItem(theme, (int)MainMenuItem.Exit, MenuText.ExitLabel, () => this.SceneManager.ChangeScene(SceneType.Exit))
        ],
        theme,
        MenuText.MainMenuTitle);
    }

    /// <summary>
    /// Exits the application.
    /// </summary>
    public override void OnBackSelected()
    {
      this.SceneManager.ChangeScene(SceneType.Exit);
    }

    private enum MainMenuItem
    {
      Play,
      Settings,
      Exit
    }
  }
}
