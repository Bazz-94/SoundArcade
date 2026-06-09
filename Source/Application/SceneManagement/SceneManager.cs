namespace SoundArcade.Application.SceneManagement
{
  using System;

  /// <summary>
  /// Manages active scene transitions and dispatch.
  /// </summary>
  public sealed class SceneManager
  {
    private IScene? activeScene;

    /// <summary>
    /// Gets the active scene.
    /// </summary>
    public IScene? ActiveScene => activeScene;

    /// <summary>
    /// Changes the active scene.
    /// </summary>
    /// <param name="scene">Scene to activate.</param>
    public void ChangeScene(IScene scene)
    {
      if (scene is null)
      {
        throw new ArgumentNullException(nameof(scene));
      }

      if (ReferenceEquals(activeScene, scene))
      {
        return;
      }

      if (activeScene is not null)
      {
        activeScene.OnExit();
      }

      activeScene = scene;
      activeScene.OnEnter();
    }

    /// <summary>
    /// Updates the active scene.
    /// </summary>
    /// <param name="deltaTime">Frame delta time in seconds.</param>
    public void Update(float deltaTime)
    {
      if (activeScene is null)
      {
        return;
      }

      activeScene.Update(deltaTime);
    }

    /// <summary>
    /// Renders the active scene.
    /// </summary>
    public void Render()
    {
      if (activeScene is null)
      {
        return;
      }

      activeScene.Render();
    }
  }
}
