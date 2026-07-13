namespace SoundArcade.Domain.Colors
{
  using SoundArcade.Abstractions;

  /// <summary>
  /// Visual theme shared by UI components.
  /// </summary>
  public record Theme
  {
    /// <summary>
    /// Gets the color palette used by UI components.
    /// </summary>
    public ColorPalette ColorPalette { get; } = new();

    /// <summary>
    /// Gets the base font size for UI text.
    /// </summary>
    public int FontSize { get; } = 22;
  }

  /// <summary>
  /// Named colors used by the theme.
  /// </summary>
  public record ColorPalette
  {
    /// <summary>
    /// Gets the screen background color.
    /// </summary>
    public Color Background { get; } = new(ColorsHex.Black);

    /// <summary>
    /// Gets the accent color for highlighted elements.
    /// </summary>
    public Color Accent { get; } = new(ColorsHex.Pink);

    /// <summary>
    /// Gets the primary color for component surfaces.
    /// </summary>
    public Color Primary { get; } = new(ColorsHex.MidnightBlue);

    /// <summary>
    /// Gets the secondary color.
    /// </summary>
    public Color Secondary { get; } = new(ColorsHex.Purple);

    /// <summary>
    /// Gets the tertiary color for titles and decorations.
    /// </summary>
    public Color Tertiary { get; } = new(ColorsHex.Teal);
  }

  /// <summary>
  /// Hex color codes used by the palette.
  /// </summary>
  public static class ColorsHex
  {
    /// <summary>Near-black navy.</summary>
    public const string Black = "#0B1026";

    /// <summary>Dark midnight blue.</summary>
    public const string MidnightBlue = "#2b2d42";

    /// <summary>Vivid purple.</summary>
    public const string Purple = "#9B5DE5";

    /// <summary>Bright pink.</summary>
    public const string Pink = "#F15BB5";

    /// <summary>Bright teal.</summary>
    public const string Teal = "#00F5D4";
  }
}
