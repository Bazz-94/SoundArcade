namespace SoundArcade.Domain.RiverRun.Models;

/// <summary>
/// High-level state for a RiverRun gameplay session.
/// </summary>
public enum RunState
{
  /// <summary>
  /// Gameplay simulation is running.
  /// </summary>
  Playing,

  /// <summary>
  /// Gameplay simulation is paused.
  /// </summary>
  Paused,

  /// <summary>
  /// The run has ended and awaits restart.
  /// </summary>
  GameOver
}
