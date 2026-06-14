namespace SoundArcade.Application.Scenes
{
  using System;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Colors;
  using SoundArcade.Domain.Models;
  using static SoundArcade.Application.ArcadeShell;

  public sealed class MainMenuScene : IScene
  {
    private Menu Menu { get; set; }
    public IInput Input { get; }
    private Action ExitAction { get; }

    public MainMenuScene(
      IInput input,
      ITts tts,
      IRenderer renderer,
      Theme theme,
      Action startRunAction,
      Action settingsAction,
      Action exitAction)
    {
      this.Menu = new Menu(
      input,
      tts,
      renderer,
      (int)MenuType.Main,
      [
        new MenuItem(theme, (int)MainMenuItem.StartRun, "Start", startRunAction),
        new MenuItem(theme, (int)MainMenuItem.Settings, "Settings", settingsAction),
        new MenuItem(theme, (int)MainMenuItem.Exit, "Exit", exitAction)
      ],
      theme,
      "Sound Arcade"
      );
      this.Input = input;
      this.ExitAction = exitAction;
    }

    public void OnBackSelected()
    {
      this.ExitAction();
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
  }
}
