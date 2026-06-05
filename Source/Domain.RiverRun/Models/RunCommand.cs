namespace SoundArcade.Domain.RiverRun.Models;

/// <summary>
/// Player commands accepted by the RiverRun session.
/// </summary>
public enum RunCommand
{
  /// <summary>
  /// Move the player one lane to the left.
  /// </summary>
  MoveLeft,

  /// <summary>
  /// Move the player one lane to the right.
  /// </summary>
  MoveRight,

  /// <summary>
  /// Toggle between pause and play states.
  /// </summary>
  TogglePause,

  /// <summary>
  /// Restart the run when the game is over.
  /// </summary>
  Restart
}
