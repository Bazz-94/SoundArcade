namespace SoundArcade.Application.Scenes
{
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Models;
  using SoundArcade.Domain.Services;

  /// <summary>
  /// Base scene that drives a <see cref="Domain.Models.Menu"/> and handles the Back input.
  /// </summary>
  public abstract class MenuScene : IScene
  {
    private IInput Input { get; }

    /// <summary>
    /// Gets the scene manager used for scene transitions.
    /// </summary>
    protected SceneManager SceneManager { get; }

    /// <summary>
    /// Gets the menu driven by this scene.
    /// </summary>
    protected abstract Menu Menu { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuScene"/> class.
    /// </summary>
    /// <param name="input">Input abstraction.</param>
    /// <param name="sceneManager">Scene manager for transitions.</param>
    protected MenuScene(IInput input, SceneManager sceneManager)
    {
      this.Input = input;
      this.SceneManager = sceneManager;
    }

    /// <summary>
    /// Handles the Back input or Back menu item.
    /// </summary>
    public abstract void OnBackSelected();

    /// <inheritdoc />
    public virtual void OnEnter()
    {
      this.Menu.SelectFirstItem();
    }

    /// <inheritdoc />
    public virtual void OnExit()
    {
    }

    /// <inheritdoc />
    public void Update(float deltaTime)
    {
      this.Menu.Update();

      if (this.Input.InputPressed(Abstractions.Input.Back))
      {
        this.OnBackSelected();
      }
    }

    /// <inheritdoc />
    public void Render()
    {
      this.Menu.Render();
    }
  }
}
