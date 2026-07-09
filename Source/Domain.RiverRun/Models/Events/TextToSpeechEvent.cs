namespace SoundArcade.Domain.RiverRun.Models.Events
{
  /// <summary>
  /// Event for speaking a text message through the TTS engine.
  /// </summary>
  /// <param name="Text">Text to announce.</param>
  public sealed record TextToSpeechEvent(string Text) : RunEvent;
}
