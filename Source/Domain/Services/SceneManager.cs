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

    private ISceneFactory? sceneFactory;

    /// <summary>
    /// Gets or sets the factory used to build scenes requested by <see cref="SceneType"/>.
    /// Property-injected after construction to break the dependency cycle with the factory;
    /// may only be set once.
    /// </summary>
    public ISceneFactory? SceneFactory
    {
      get
      {
        return this.sceneFactory;
      }

      set
      {
        if (this.sceneFactory is not null)
        {
          throw new InvalidOperationException("SceneManager.SceneFactory may only be set once.");
        }

        this.sceneFactory = value;
      }
    }

    /// <summary>
    /// Raised when <see cref="SceneType.Exit"/> is requested instead of a scene transition.
    /// </summary>
    public event Action? ExitRequested;

    /// <summary>
    /// Changes the active scene to the one identified by <paramref name="sceneType"/>, or
    /// raises <see cref="ExitRequested"/> when <paramref name="sceneType"/> is <see cref="SceneType.Exit"/>.
    /// </summary>
    /// <param name="sceneType">Scene to activate.</param>
    public void ChangeScene(SceneType sceneType)
    {
      if (sceneType == SceneType.Exit)
      {
        this.ExitRequested?.Invoke();
        return;
      }

      if (this.SceneFactory is null)
      {
        throw new InvalidOperationException("SceneManager.SceneFactory must be set before requesting a scene change.");
      }

      this.ChangeScene(this.SceneFactory.CreateScene(sceneType));
    }

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
