namespace SoundArcade.Abstractions;

public readonly record struct RgbaColor(byte R, byte G, byte B, byte A)
{
  public static RgbaColor Opaque(byte r, byte g, byte b)
    => new(r, g, b, 255);
}
