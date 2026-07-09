namespace SoundArcade.Application.Scenes
{
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Colors;
  using SoundArcade.Domain.Enums;
  using SoundArcade.Domain.Models;
  using SoundArcade.Domain.Services;

  public sealed class GameSelectionMenuScene : IScene
  {
    private Menu Menu { get; set; }
    public IInput Input { get; }
    private SceneManager SceneManager { get; }

    public GameSelectionMenuScene(
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
      (int)MenuType.GameSelection,
      [
        new MenuItem(theme, (int)GameSelectionMenuItem.RiverRun, "RiverRun", () => this.SceneManager.ChangeScene(SceneType.Run)),
        new MenuItem(theme, (int)GameSelectionMenuItem.Back, "Back", this.OnBackSelected)
      ],
      theme,
      "Select a Game"
      );
      this.Input = input;
    }

    public void OnBackSelected()
    {
      this.SceneManager.ChangeScene(SceneType.MainMenu);
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

    public enum GameSelectionMenuItem
    {
      RiverRun,
      Back
    }
  }
}
