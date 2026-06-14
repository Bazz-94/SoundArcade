namespace SoundArcade.Domain.Colors
{
  using SoundArcade.Abstractions;

  public record ColorPalette
  {
    public readonly MenuColors Menu = new();
  }

  public record MenuColors
  {
    public Color Background = new Color(ColorsHex.Black);
    public Color MenuItem = new(ColorsHex.MidnightBlue);
    public Color Text = new(ColorsHex.Pink);
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
