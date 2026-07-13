namespace SoundArcade.Domain.RiverRun.Models.GameObjects
{
  using System.Numerics;
  using SoundArcade.Abstractions;

  /// <summary>
  /// Represents one active pickup in world space.
  /// Pickups are stationary in world space; the player moves forward to collect them.
  /// </summary>
  public sealed class Pickup : GameObject
  {
    private const float RenderRadius = 0.25f;

    private Color Color { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="Pickup"/>.
    /// </summary>
    /// <param name="position">Initial world position for the pickup.</param>
    /// <param name="color">Color for rendering the pickup.</param>
    public Pickup(Vector3 position, Color color)
      : base(position, isCollidable: true)
    {
      this.Color = color;
    }

    /// <inheritdoc />
    public override void Render(IRenderer renderer)
    {
      renderer.DrawSphere(this.Position, RenderRadius, this.Color);
    }
  }
}
