namespace SoundArcade.Domain.Services
{
  using SoundArcade.Domain.Models;

  /// <summary>
  /// Creates scene instances on demand for the <see cref="SceneManager"/>.
  /// </summary>
  public interface ISceneFactory
  {
    /// <summary>
    /// Creates the scene identified by <paramref name="sceneType"/>.
    /// </summary>
    /// <param name="sceneType">Scene to create.</param>
    IScene CreateScene(SceneType sceneType);
  }
}
