using System;
using System.Numerics;

namespace SoundArcade.Domain.RiverRun.Models;

/// <summary>
/// Player actor in the RiverRun world.
/// Inherits position and collidable behaviour from GameObject.
/// </summary>
public sealed class Player : GameObject
{
  /// <summary>
  /// Initializes a new instance of <see cref="Player"/>.
  /// </summary>
  /// <param name="position">Initial player world position.</param>
  /// <param name="collidable">Whether the player collides with obstacles. Defaults to true.</param>
  public Player(Vector3 position, bool collidable = true)
    : base(position, collidable)
  {
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
  /// Advances the player's Z position by speed * deltaTimeSeconds.
  /// </summary>
  /// <param name="deltaTimeSeconds">Frame delta in seconds.</param>
  /// <param name="speed">Forward speed in world units per second.</param>
  public void Advance(float deltaTimeSeconds, float speed)
  {
    this.Position = new Vector3(this.Position.X, this.Position.Y, this.Position.Z + (speed * deltaTimeSeconds));
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
}
