namespace SoundArcade.Application.Scenes
{
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Colors;
  using SoundArcade.Domain.Enums;
  using SoundArcade.Domain.Models;
  using SoundArcade.Domain.Services;

  public sealed class MainMenuScene : IScene
  {
    private Menu Menu { get; set; }
    public IInput Input { get; }
    private SceneManager SceneManager { get; }

    public MainMenuScene(
      IInput input,
      ITts tts,
      IRenderer renderer,
      Theme theme,
      SceneManager sceneManager)
    {
      this.SceneManager = sceneManager;
      this.Menu = new Menu(
      input,
      tts,
      renderer,
      (int)MenuType.Main,
      [
        new MenuItem(theme, (int)MainMenuItem.Play, "Play", () => this.SceneManager.ChangeScene(SceneType.GameSelectionMenu)),
        new MenuItem(theme, (int)MainMenuItem.Settings, "Settings", () => this.SceneManager.ChangeScene(SceneType.SettingsMenu)),
        new MenuItem(theme, (int)MainMenuItem.Exit, "Exit", () => this.SceneManager.ChangeScene(SceneType.Exit))
      ],
      theme,
      "Sound Arcade"
      );
      this.Input = input;
    }

    public void OnBackSelected()
    {
      this.SceneManager.ChangeScene(SceneType.Exit);
    }

    public void OnExit()
    {
    }

    public void Update(float deltaTime)
    {
      this.Menu.Update();

      if (this.Input.InputPressed(Abstractions.Input.Back))
      {
        this.OnBackSelected();
      }
    }

    public void Render()
    {
      this.Menu.Render();
    }

    public void OnEnter()
    {
      this.Menu.SelectFirstItem();
    }

    public enum MainMenuItem
    {
      Play,
      Settings,
      Exit
    }
  }
}
