namespace SoundArcade.Tests
{
  using System;
  using System.Collections.Generic;
  using SoundArcade.Domain.Models;
  using SoundArcade.Domain.Services;
  using Xunit;

  /// <summary>
  /// Tests for scene transitions and dispatch.
  /// </summary>
  public sealed class SceneManagerTests
  {
    /// <summary>
    /// Fake scene recording lifecycle and frame calls.
    /// </summary>
    private sealed class FakeScene : IScene
    {
      public List<string> Calls { get; } = new List<string>();

      public void OnEnter()
      {
        this.Calls.Add(nameof(this.OnEnter));
      }

      public void OnExit()
      {
        this.Calls.Add(nameof(this.OnExit));
      }

      public void Update(float deltaTime)
      {
        this.Calls.Add(nameof(this.Update));
      }

      public void Render()
      {
        this.Calls.Add(nameof(this.Render));
      }

      public void OnBackSelected()
      {
        this.Calls.Add(nameof(this.OnBackSelected));
      }
    }

    /// <summary>
    /// Fake factory returning a fixed scene and recording requests.
    /// </summary>
    private sealed class FakeSceneFactory : ISceneFactory
    {
      public FakeScene Scene { get; } = new FakeScene();

      public SceneType? RequestedScene { get; private set; }

      public IScene CreateScene(SceneType sceneType)
      {
        this.RequestedScene = sceneType;
        return this.Scene;
      }
    }

    /// <summary>
    /// Verifies activating a scene calls its enter hook.
    /// </summary>
    [Fact]
    public void ChangeScene_enters_new_scene()
    {
      SceneManager manager = new SceneManager();
      FakeScene scene = new FakeScene();

      manager.ChangeScene(scene);

      Assert.Equal([nameof(IScene.OnEnter)], scene.Calls);
    }

    /// <summary>
    /// Verifies switching scenes exits the old scene before entering the new one.
    /// </summary>
    [Fact]
    public void ChangeScene_exits_previous_scene_before_entering_next()
    {
      SceneManager manager = new SceneManager();
      FakeScene first = new FakeScene();
      FakeScene second = new FakeScene();

      manager.ChangeScene(first);
      manager.ChangeScene(second);

      Assert.Equal([nameof(IScene.OnEnter), nameof(IScene.OnExit)], first.Calls);
      Assert.Equal([nameof(IScene.OnEnter)], second.Calls);
    }

    /// <summary>
    /// Verifies re-activating the active scene does nothing.
    /// </summary>
    [Fact]
    public void ChangeScene_ignores_already_active_scene()
    {
      SceneManager manager = new SceneManager();
      FakeScene scene = new FakeScene();

      manager.ChangeScene(scene);
      manager.ChangeScene(scene);

      Assert.Equal([nameof(IScene.OnEnter)], scene.Calls);
    }

    /// <summary>
    /// Verifies a scene-type change goes through the factory.
    /// </summary>
    [Fact]
    public void ChangeScene_by_type_uses_factory()
    {
      SceneManager manager = new SceneManager();
      FakeSceneFactory factory = new FakeSceneFactory();
      manager.SceneFactory = factory;

      manager.ChangeScene(SceneType.MainMenu);

      Assert.Equal(SceneType.MainMenu, factory.RequestedScene);
      Assert.Equal([nameof(IScene.OnEnter)], factory.Scene.Calls);
    }

    /// <summary>
    /// Verifies a scene-type change without a factory throws.
    /// </summary>
    [Fact]
    public void ChangeScene_by_type_without_factory_throws()
    {
      SceneManager manager = new SceneManager();

      Assert.Throws<InvalidOperationException>(() => manager.ChangeScene(SceneType.MainMenu));
    }

    /// <summary>
    /// Verifies the exit scene type raises the exit event instead of a transition.
    /// </summary>
    [Fact]
    public void ChangeScene_exit_raises_exit_requested()
    {
      SceneManager manager = new SceneManager();
      bool exitRequested = false;
      manager.ExitRequested += () => exitRequested = true;

      manager.ChangeScene(SceneType.Exit);

      Assert.True(exitRequested);
    }

    /// <summary>
    /// Verifies the factory may only be assigned once.
    /// </summary>
    [Fact]
    public void SceneFactory_may_only_be_set_once()
    {
      SceneManager manager = new SceneManager();
      manager.SceneFactory = new FakeSceneFactory();

      Assert.Throws<InvalidOperationException>(() => manager.SceneFactory = new FakeSceneFactory());
    }

    /// <summary>
    /// Verifies update and render dispatch to the active scene and are no-ops without one.
    /// </summary>
    [Fact]
    public void Update_and_render_dispatch_to_active_scene()
    {
      SceneManager manager = new SceneManager();
      manager.Update(0.016f);
      manager.Render();

      FakeScene scene = new FakeScene();
      manager.ChangeScene(scene);
      manager.Update(0.016f);
      manager.Render();

      Assert.Equal([nameof(IScene.OnEnter), nameof(IScene.Update), nameof(IScene.Render)], scene.Calls);
    }
  }
}
