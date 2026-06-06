using System.Numerics;
using SoundArcade.Domain.Services;
using Xunit;

namespace SoundArcade.Tests;

/// <summary>
/// Tests for shared 3D point-proximity collision checks.
/// </summary>
public sealed class ProximityCollisionTests
{
  /// <summary>
  /// Verifies positions inside the default buffer are treated as a collision.
  /// </summary>
  [Fact]
  public void IsWithinBuffer_uses_default_buffer_of_one()
  {
    Vector3 firstPosition = new Vector3(0.0f, 0.0f, 0.0f);
    Vector3 secondPosition = new Vector3(0.6f, 0.0f, 0.0f);
    bool collides = ProximityCollision.IsWithinBuffer(firstPosition, secondPosition);

    Assert.True(collides);
  }

  /// <summary>
  /// Verifies positions outside the provided buffer are not treated as a collision.
  /// </summary>
  [Fact]
  public void IsWithinBuffer_returns_false_when_distance_exceeds_buffer()
  {
    Vector3 firstPosition = new Vector3(0.0f, 0.0f, 0.0f);
    Vector3 secondPosition = new Vector3(1.01f, 0.0f, 0.0f);
    bool collides = ProximityCollision.IsWithinBuffer(firstPosition, secondPosition, 1.0f);

    Assert.False(collides);
  }

  /// <summary>
  /// Verifies negative buffers are rejected.
  /// </summary>
  [Fact]
  public void IsWithinBuffer_throws_for_negative_buffer()
  {
    Vector3 firstPosition = new Vector3(0.0f, 0.0f, 0.0f);
    Vector3 secondPosition = new Vector3(0.0f, 0.0f, 0.0f);

    Assert.Throws<System.ArgumentOutOfRangeException>(() => ProximityCollision.IsWithinBuffer(firstPosition, secondPosition, -0.1f));
  }
}
