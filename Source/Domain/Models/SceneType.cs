namespace SoundArcade.Domain.Models
{
  /// <summary>
  /// Identifies a scene that can be requested from the <see cref="Services.SceneManager"/>.
  /// </summary>
  public enum SceneType
  {
    MainMenu,
    GameSelectionMenu,
    SettingsMenu,
    Run,
    Exit
  }
}
