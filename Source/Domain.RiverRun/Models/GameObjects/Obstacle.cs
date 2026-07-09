namespace SoundArcade.Domain.RiverRun.Models.GameObjects
{
  using System.Numerics;
  using SoundArcade.Abstractions;

  /// <summary>
  /// Represents one active obstacle in world space.
  /// Obstacles are stationary in world space; the player moves forward.
  /// </summary>
  public sealed class Obstacle : GameObject
  {
    private const float RenderSize = 0.6f;

    private Color Color { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="Obstacle"/>.
    /// </summary>
    /// <param name="position">Initial world position for the obstacle.</param>
    /// <param name="color">Color for rendering the obstacle.</param>
    public Obstacle(Vector3 position, Color color)
      : base(position, isCollidable: true)
    {
      this.Color = color;
    }

    /// <inheritdoc />
    public override void Render(IRenderer renderer)
    {
      renderer.DrawBox(this.Position, new Vector3(RenderSize, RenderSize, RenderSize), this.Color);
    }
  }
}
