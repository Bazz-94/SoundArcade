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
    private Vector3 position;

    /// <summary>
    /// Gets the world position. Derived types update it through their own state-transition methods;
    /// setting the position triggers validation.
    /// </summary>
    public Vector3 Position
    {
      get
      {
        return this.position;
      }
      protected set
      {
        this.ValidatePosition(value);
        this.position = value;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the object participates in collisions.
    /// </summary>
    public bool IsCollidable { get; }

    /// <summary>
    /// Gets the elapsed-time threshold after which this object may emit another approach noise cue.
    /// </summary>
    public float NextNoiseAt { get; private set; }

    /// <summary>
    /// Schedules the next approach noise cue if the current one is due.
    /// </summary>
    /// <param name="elapsedSeconds">Total elapsed session time in seconds.</param>
    /// <param name="interval">Minimum seconds between cues for this object.</param>
    /// <returns>True when a cue is due and the next one was scheduled; otherwise false.</returns>
    public bool TryScheduleNoise(float elapsedSeconds, float interval)
    {
      if (elapsedSeconds < this.NextNoiseAt)
      {
        return false;
      }

      this.NextNoiseAt = elapsedSeconds + interval;
      return true;
    }

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
          throw new ArgumentOutOfRangeException(nameof(position), $"Position.X must be on one of the game's lanes.");
      }

      if (position.Y != RunConstants.GroundY)
      {
        throw new ArgumentException($"Position.Y must equal the ground plane value {RunConstants.GroundY}.", nameof(position));
      }
    }

    /// <summary>
    /// Renders the object using primitive shapes.
    /// </summary>
    /// <param name="renderer">Renderer abstraction to draw with.</param>
    public abstract void Render(IRenderer renderer);
  }
}
