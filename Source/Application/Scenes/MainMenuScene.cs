namespace SoundArcade.Application.Scenes
{
  using System;
  using System.Collections.Generic;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Models;
  using static SoundArcade.Application.ArcadeShell;

  public sealed class MainMenuScene : MenuScene
  {
    private static readonly Menu Menu = new Menu(
      (int)MenuType.Main,
      "Sound Arcade",
      [
        new MenuItem((int)MainMenuItem.StartRun, "Start"),
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
      Action exitAction) : base(Menu, input, tts, renderer, selectedColor, unselectedColor)
    {
      this.startRunAction = startRunAction;
      this.settingsAction = settingsAction;
      this.exitAction = exitAction;
      itemActions = this.CreateMainMenuActions();
    }

    protected override IReadOnlyDictionary<int, Action<MenuItem>> GetItemActions()
    {
      return itemActions;
    }

    protected override void OnBackSelected()
    {
      exitAction();
    }

    private IReadOnlyDictionary<int, Action<MenuItem>> CreateMainMenuActions()
    {
      Dictionary<int, Action<MenuItem>> actions = new Dictionary<int, Action<MenuItem>>
      {
        [(int)MainMenuItem.StartRun] = _ => startRunAction(),
        [(int)MainMenuItem.Settings] = _ => settingsAction(),
        [(int)MainMenuItem.Exit] = _ => exitAction()
      };

      return actions;
    }
  }
}
