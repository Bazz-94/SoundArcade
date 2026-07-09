namespace SoundArcade.Domain.RiverRun.Models.Events
{
  /// <summary>
  /// Event for stopping any currently playing instances of an audio asset, used to silence
  /// approach cues immediately once the lane ahead is clear.
  /// </summary>
  /// <param name="SoundId">Audio asset identifier to stop.</param>
  public sealed record StopSoundEvent(string SoundId) : RunEvent;
}
