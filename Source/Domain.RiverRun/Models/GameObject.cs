namespace SoundArcade.Domain.RiverRun.Models
{
  using System;
  using System.Numerics;
  using SoundArcade.Abstractions;

  /// <summary>
  /// Base class for all game objects with a world position and collidable flag.
  /// Validation is executed whenever the Position is changed.
  /// </summary>
  public abstract class GameObject
  {
    private Vector3 _position;

    /// <summary>
    /// Gets or sets the world position. Setting the position triggers validation.
    /// </summary>
    public Vector3 Position
    {
      get
      {
        return _position;
      }
      set
      {
        this.ValidatePosition(value);
        _position = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the object participates in collisions.
    /// </summary>
    public bool IsCollidable { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="GameObject"/>.
    /// </summary>
    /// <param name="position">Initial world position.</param>
    /// <param name="isCollidable">Whether the object is collidable.</param>
    protected GameObject(Vector3 position, bool isCollidable)
    {
      this.IsCollidable = isCollidable;
      this.Position = position;
    }

    /// <summary>
    /// Validates a position. Derived types may override to provide additional checks.
    /// Default validation ensures X is within lane bounds, and Y equals the ground plane.
    /// </summary>
    /// <param name="position">Position to validate.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when X is out of lane bounds or components are NaN/Infinity.</exception>
    /// <exception cref="ArgumentException">Thrown when Y does not equal the ground plane.</exception>
    protected virtual void ValidatePosition(Vector3 position)
    {
      switch (position.X)
      {
        case RunConstants.LaneX.Left:
        case RunConstants.LaneX.Center:
        case RunConstants.LaneX.Right:
          // Valid lane positions.
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(position), $"Position.X must on one of the game's lanes.");
      }

      if (position.Y != RunConstants.GroundY)
      {
        throw new ArgumentException($"Position.Y must equal the ground plane value {RunConstants.GroundY}.", nameof(position));
      }
    }

    public abstract void Render(IRenderer renderer);
  }
}
