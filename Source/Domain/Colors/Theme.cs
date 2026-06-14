namespace SoundArcade.Domain.Colors
{
  using SoundArcade.Abstractions;

  public record Theme
  {
    public readonly ColorPalette ColorPalette = new();
    public int FontSize { get; set; } = 22;
  }

  public record ColorPalette
  {
    public Color Background = new Color(ColorsHex.Black);
    public Color Accent = new(ColorsHex.Pink);
    public Color Primary = new(ColorsHex.MidnightBlue);
    public Color Secondary = new(ColorsHex.Purple);
    public Color Tertiary = new(ColorsHex.Teal);
  }

  public static class ColorsHex
  {
    public const string Black = "#0B1026";
    public const string MidnightBlue = "#2b2d42";
    public const string Purple = "#9B5DE5";
    public const string Pink = "#F15BB5";
    public const string Teal = "#00F5D4";
  }
}
