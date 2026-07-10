namespace SoundArcade.Domain.Models
{
  using System;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Colors;

  /// <summary>
  /// Menu item component.
  /// </summary>
  public class MenuItem : UIComponent
  {
    public Action OnPressed { get; private set; }
    public Color TextColor { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int FontSize { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuItem"/> class.
    /// </summary>
    /// <param name="id">Stable item identifier.</param>
    /// <param name="displayText">Display text announced to users.</param>
    /// <param name="onPressed">Action to execute when the menu item is pressed.</param>
    public MenuItem(Theme theme, int id, string displayText, Action onPressed)
      : base(theme.ColorPalette.Primary, id, displayText)
    {
      this.OnPressed = onPressed;
      this.TextColor = theme.ColorPalette.Accent;
      this.FontSize = theme.FontSize;
    }

    /// <summary>
    /// Renders the item box and label.
    /// </summary>
    /// <param name="renderer">Renderer abstraction.</param>
    /// <param name="isSelected">Whether the item is currently selected.</param>
    public virtual void Render(IRenderer renderer, bool isSelected)
    {
      Color itemColor = isSelected ? this.TextColor : this.Color;
      Color textColor = isSelected ? this.Color : this.TextColor;
      renderer.DrawScreenBox(this.X, this.Y, this.Width, this.Height, itemColor);
      renderer.DrawScreenTextCentered(this.X, this.Y, this.DisplayText, this.FontSize, textColor);
    }
  }
}