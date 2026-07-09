namespace SoundArcade.Domain.Models
{
  /// <summary>
  /// Contract for a scene that can be entered, updated, and rendered.
  /// </summary>
  public interface IScene
  {
    /// <summary>
    /// Called when the scene becomes active.
    /// </summary>
    void OnEnter();

    /// <summary>
    /// Called when the scene is no longer active.
    /// </summary>
    void OnExit();

    /// <summary>
    /// Updates the scene by one frame.
    /// </summary>
    /// <param name="deltaTime">Frame delta time in seconds.</param>
    void Update(float deltaTime);

    /// <summary>
    /// Renders the scene.
    /// </summary>
    void Render();

    /// <summary>
    /// Called when the back action is pressed.
    /// </summary>
    void OnBackSelected();
  }
}
