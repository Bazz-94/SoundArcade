namespace SoundArcade.Domain.Services
{
  using System;
  using SoundArcade.Domain.Models;

  /// <summary>
  /// Manages active scene transitions and dispatch.
  /// </summary>
  public sealed class SceneManager
  {
    private IScene? ActiveScene { get; set; }

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

      if (ReferenceEquals(this.ActiveScene, scene))
      {
        return;
      }

      if (this.ActiveScene is not null)
      {
        this.ActiveScene.OnExit();
      }

      this.ActiveScene = scene;
      this.ActiveScene.OnEnter();
    }

    /// <summary>
    /// Updates the active scene.
    /// </summary>
    /// <param name="deltaTime">Frame delta time in seconds.</param>
    public void Update(float deltaTime)
    {
      if (this.ActiveScene is null)
      {
        return;
      }

      this.ActiveScene.Update(deltaTime);
    }

    /// <summary>
    /// Renders the active scene.
    /// </summary>
    public void Render()
    {
      if (this.ActiveScene is null)
      {
        return;
      }

      this.ActiveScene.Render();
    }
  }
}
