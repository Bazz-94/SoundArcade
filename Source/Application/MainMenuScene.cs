namespace SoundArcade.Application
{
  using System;
  using System.Collections.Generic;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Models;
  using static SoundArcade.Application.ArcadeShell;

  public sealed class MainMenuScene : MenuScene
  {
    private const float menuZ = 4.0f;

    private static readonly Menu Menu = new Menu(
      (int)MenuType.Main,
      "Main menu",
      [
        new MenuItem((int)MainMenuItem.StartRun, "Start Run"),
        new MenuItem((int)MainMenuItem.Settings, "Settings"),
        new MenuItem((int)MainMenuItem.Exit, "Exit")
      ]);

    private readonly Action startRunAction;
    private readonly Action settingsAction;
    private readonly Action exitAction;
    private readonly IReadOnlyDictionary<int, Action<MenuItem>> itemActions;

    public MainMenuScene(
      IInput input,
      ITts tts,
      IRenderer renderer,
      Color selectedColor,
      Color unselectedColor,
      Action startRunAction,
      Action settingsAction,
      Action exitAction) : base(Menu, input, tts, renderer, selectedColor, unselectedColor, menuZ)
    {
      this.startRunAction = startRunAction;
      this.settingsAction = settingsAction;
      this.exitAction = exitAction;
      this.itemActions = this.CreateMainMenuActions();
    }

    protected override IReadOnlyDictionary<int, Action<MenuItem>> GetItemActions()
    {
      return this.itemActions;
    }

    protected override void OnBackSelected()
    {
      this.exitAction();
    }

    private IReadOnlyDictionary<int, Action<MenuItem>> CreateMainMenuActions()
    {
      Dictionary<int, Action<MenuItem>> actions = new Dictionary<int, Action<MenuItem>>
      {
        [(int)MainMenuItem.StartRun] = _ => this.startRunAction(),
        [(int)MainMenuItem.Settings] = _ => this.settingsAction(),
        [(int)MainMenuItem.Exit] = _ => this.exitAction()
      };

      return actions;
    }
  }
}
