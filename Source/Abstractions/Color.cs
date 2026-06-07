namespace SoundArcade.Abstractions
{
  using System;
  public sealed class Color
  {
    public byte R { get; }
    public byte G { get; }
    public byte B { get; }
    public byte A { get; }

    public Color(byte r, byte g, byte b, byte a = 255)
    {
      this.R = r;
      this.G = g;
      this.B = b;
      this.A = a;
    }

    public Color(string hex)
    {
      if (string.IsNullOrWhiteSpace(hex))
      {
        throw new ArgumentException("Hex color cannot be empty.", nameof(hex));
      }

      hex = hex.TrimStart('#');

      switch (hex.Length)
      {
        case 6: // RRGGBB
          this.R = Convert.ToByte(hex.Substring(0, 2), 16);
          this.G = Convert.ToByte(hex.Substring(2, 2), 16);
          this.B = Convert.ToByte(hex.Substring(4, 2), 16);
          this.A = 255;
          break;

        case 8: // RRGGBBAA
          this.R = Convert.ToByte(hex.Substring(0, 2), 16);
          this.G = Convert.ToByte(hex.Substring(2, 2), 16);
          this.B = Convert.ToByte(hex.Substring(4, 2), 16);
          this.A = Convert.ToByte(hex.Substring(6, 2), 16);
          break;

        default:
          throw new ArgumentException(
              "Hex color must be in the format #RRGGBB or #RRGGBBAA.",
              nameof(hex));
      }
    }

    public override string ToString()
    {
      return $"#{this.R:X2}{this.G:X2}{this.B:X2}{this.A:X2}";
    }
  }
}
