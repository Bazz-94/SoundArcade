namespace SoundArcade.Domain.RiverRun.Models
{
  using System;
  using System.Numerics;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Models;

  /// <summary>
  /// Player actor in the RiverRun world.
  /// Inherits position and collidable behaviour from GameObject.
  /// </summary>
  public sealed class Player : GameObject, IPlayer
  {
    /// <summary>
    /// Gets the current forward movement speed in world units per second.
    /// </summary>
    public float Speed { get; private set; }
    public Color color { get; }

    private readonly float speedIncreasePerZUnit;
    private readonly float maxSpeed;

    /// <summary>
    /// Initializes a new instance of <see cref="Player"/>.
    /// </summary>
    /// <param name="position">Initial player world position.</param>
    /// <param name="speed">Initial forward movement speed.</param>
    /// <param name="speedIncreasePerZUnit">Speed gain applied per world unit traveled on Z.</param>
    /// <param name="maxSpeedIncrease">Maximum additional speed allowed above the initial speed.</param>
    /// <param name="collidable">Whether the player collides with obstacles. Defaults to true.</param>
    public Player(
      Color color,
      Vector3 position,
      float speed,
      float speedIncreasePerZUnit,
      float maxSpeedIncrease,
      bool collidable = true)
      : base(position, collidable)
    {
      this.color = color;
      this.Speed = speed;
      this.speedIncreasePerZUnit = speedIncreasePerZUnit;
      maxSpeed = speed + maxSpeedIncrease;
    }

    /// <summary>
    /// Attempts to move the player one lane to the left.
    /// </summary>
    public void MoveLeft()
    {
      float currentX = this.Position.X;

      if (currentX <= RunConstants.LaneX.Left)
      {
        return;
      }

      float newX = currentX - RunConstants.LaneX.LaneWidth;
      this.Position = new Vector3(newX, RunConstants.GroundY, this.Position.Z);
    }

    /// <summary>
    /// Attempts to move the player one lane to the right.
    /// </summary>
    public void MoveRight()
    {
      float currentX = this.Position.X;

      if (currentX >= RunConstants.LaneX.Right)
      {
        return;
      }

      float newX = currentX + RunConstants.LaneX.LaneWidth;
      this.Position = new Vector3(newX, RunConstants.GroundY, this.Position.Z);
    }

    /// <summary>
    /// Advances the player's Z position and increases speed based on distance traveled.
    /// </summary>
    /// <param name="deltaTimeSeconds">Frame delta in seconds.</param>
    public void Advance(float deltaTimeSeconds)
    {
      float distanceTravelled = this.Speed * deltaTimeSeconds;
      this.Position = new Vector3(this.Position.X, this.Position.Y, this.Position.Z + distanceTravelled);

      this.Speed = MathF.Min(maxSpeed, this.Speed + (distanceTravelled * speedIncreasePerZUnit));
    }

    /// <summary>
    /// Handles a <see cref="RunCommand"/> directed at the player.
    /// Any command not understood by the player will throw an <see cref="ArgumentException"/>.
    /// </summary>
    /// <param name="command">Command to apply to the player.</param>
    /// <exception cref="ArgumentException">Thrown when the command is not applicable to the player.</exception>
    public void HandleCommand(RunCommand command)
    {
      switch (command)
      {
        case RunCommand.MoveLeft:
          this.MoveLeft();
          break;
        case RunCommand.MoveRight:
          this.MoveRight();
          break;
        default:
          throw new ArgumentException($"Command {command} is not a player command.", nameof(command));
      }
    }

    public override void Render(IRenderer renderer)
    {
      renderer.DrawSphere(this.Position, 0.35f, this.color);
    }
  }
}
