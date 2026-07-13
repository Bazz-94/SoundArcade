namespace SoundArcade.Application.Scenes
{
  using System.Collections.Generic;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain;
  using SoundArcade.Domain.Colors;
  using SoundArcade.Domain.Enums;
  using SoundArcade.Domain.Models;
  using SoundArcade.Domain.Services;

  /// <summary>
  /// Menu listing every registered mini-game plus a back item.
  /// </summary>
  public sealed class GameSelectionMenuScene : MenuScene
  {
    /// <inheritdoc />
    protected override Menu Menu { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameSelectionMenuScene"/> class.
    /// </summary>
    /// <param name="input">Input abstraction.</param>
    /// <param name="tts">Text-to-speech abstraction.</param>
    /// <param name="renderer">Renderer abstraction.</param>
    /// <param name="theme">Theme for colors.</param>
    /// <param name="gameRegistry">Registry of games to present.</param>
    /// <param name="sceneManager">Scene manager for transitions.</param>
    public GameSelectionMenuScene(
      IInput input,
      ITts tts,
      IRenderer renderer,
      Theme theme,
      GameRegistry gameRegistry,
      SceneManager sceneManager)
      : base(input, sceneManager)
    {
      List<MenuItem> items = [];

      foreach (IGame game in gameRegistry.Games)
      {
        items.Add(new MenuItem(theme, items.Count, game.Identity.DisplayName, () => this.SceneManager.ChangeScene(SceneType.Run)));
      }

      items.Add(new MenuItem(theme, items.Count, MenuText.BackLabel, this.OnBackSelected));

      this.Menu = new Menu(
        input,
        tts,
        renderer,
        (int)MenuType.GameSelection,
        items,
        theme,
        MenuText.GameSelectionTitle);
    }

    /// <summary>
    /// Returns to the main menu.
    /// </summary>
    public override void OnBackSelected()
    {
      this.SceneManager.ChangeScene(SceneType.MainMenu);
    }
  }
}
