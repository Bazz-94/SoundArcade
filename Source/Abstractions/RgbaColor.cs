namespace SoundArcade.Abstractions;

/// <summary>
/// Represents an RGBA color with 8-bit channels.
/// </summary>
/// <param name="R">Red channel.</param>
/// <param name="G">Green channel.</param>
/// <param name="B">Blue channel.</param>
/// <param name="A">Alpha channel.</param>
public readonly record struct RgbaColor(byte R, byte G, byte B, byte A)
{
  /// <summary>
  /// Creates an opaque color from RGB channels.
  /// </summary>
  /// <param name="r">Red channel.</param>
  /// <param name="g">Green channel.</param>
  /// <param name="b">Blue channel.</param>
  /// <returns>An opaque color value.</returns>
  public static RgbaColor Opaque(byte r, byte g, byte b)
  {
    return new RgbaColor(r, g, b, 255);
  }
}
