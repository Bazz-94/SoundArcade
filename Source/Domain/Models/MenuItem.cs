namespace SoundArcade.Domain.Models
{
  using System;

  /// <summary>
  /// Menu item component.
  /// </summary>
  public sealed class MenuItem : UIComponent
  {
    public Action OnPressed { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuItem"/> class.
    /// </summary>
    /// <param name="id">Stable item identifier.</param>
    /// <param name="displayText">Display text announced to users.</param>
    /// <param name="onPressed">Action to execute when the menu item is pressed.</param>
    public MenuItem(int id, string displayText, Action onPressed)
      : base(id, displayText)
    {
      this.OnPressed = onPressed;
    }
  }
}