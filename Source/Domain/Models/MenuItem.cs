namespace SoundArcade.Domain.Models
{
  /// <summary>
  /// Menu item component.
  /// </summary>
  public sealed class MenuItem : UIComponent
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MenuItem"/> class.
    /// </summary>
    /// <param name="id">Stable item identifier.</param>
    /// <param name="displayText">Display text announced to users.</param>
    public MenuItem(int id, string displayText)
      : base(id, displayText)
    {
    }
  }
}