namespace SoundArcade.Domain.Services
{
  using System;
  using System.Numerics;

  /// <summary>
  /// Provides shared 3D point-proximity collision checks for mini-games.
  /// </summary>
  public static class ProximityCollision
  {
    /// <summary>
    /// Default collision buffer when callers do not provide one.
    /// </summary>
    public const float DefaultBuffer = 1.0f;

    /// <summary>
    /// Returns true when two 3D positions are within the provided buffer distance.
    /// </summary>
    /// <param name="firstPosition">First world position.</param>
    /// <param name="secondPosition">Second world position.</param>
    /// <param name="buffer">Inclusive distance buffer used for hit detection.</param>
    /// <returns>True when the Euclidean distance is less than or equal to the buffer.</returns>
    public static bool IsWithinBuffer(Vector3 firstPosition, Vector3 secondPosition, float buffer = DefaultBuffer)
    {
      if (buffer < 0.0f)
      {
        throw new ArgumentOutOfRangeException(nameof(buffer));
      }

      return Vector3.Distance(firstPosition, secondPosition) <= buffer;
    }
  }
}
