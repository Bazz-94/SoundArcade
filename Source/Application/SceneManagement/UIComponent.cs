namespace SoundArcade.Application.SceneManagement
{
  /// <summary>
  /// Base type for UI components with an identity and display text.
  /// </summary>
  public abstract class UIComponent
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="UIComponent"/> class.
    /// </summary>
    /// <param name="id">Stable component identifier.</param>
    /// <param name="displayText">Display text announced to users.</param>
    protected UIComponent(int id, string displayText)
    {
      this.Id = id;
      this.DisplayText = displayText;
    }

    /// <summary>
    /// Gets the stable component identifier.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets the display text for this component.
    /// </summary>
    public string DisplayText { get; }
  }
}