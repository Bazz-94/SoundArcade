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
    /// <summary>
    /// Gets the action to execute when the item is pressed.
    /// </summary>
    public Action OnPressed { get; }

    /// <summary>
    /// Gets the text color.
    /// </summary>
    public Color TextColor { get; }

    /// <summary>
    /// Gets the horizontal screen center of the item.
    /// </summary>
    public int X { get; private set; }

    /// <summary>
    /// Gets the vertical screen center of the item.
    /// </summary>
    public int Y { get; private set; }

    /// <summary>
    /// Gets the item box width in pixels.
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    /// Gets the item box height in pixels.
    /// </summary>
    public int Height { get; private set; }

    /// <summary>
    /// Gets the label font size.
    /// </summary>
    public int FontSize { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuItem"/> class.
    /// </summary>
    /// <param name="theme">Theme for colors and font size.</param>
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
    /// Positions and sizes the item on screen.
    /// </summary>
    /// <param name="x">Horizontal screen center.</param>
    /// <param name="y">Vertical screen center.</param>
    /// <param name="width">Item box width in pixels.</param>
    /// <param name="height">Item box height in pixels.</param>
    public void SetLayout(int x, int y, int width, int height)
    {
      this.X = x;
      this.Y = y;
      this.Width = width;
      this.Height = height;
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
