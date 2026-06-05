namespace SoundArcade.Infrastructure.Windows
{
  using System.Speech.Synthesis;
  using SoundArcade.Abstractions;

  /// <summary>
  /// Windows speech-synthesis implementation of <see cref="ITts"/>.
  /// </summary>
  public class TextToSpeech : ITts
  {
    private readonly SpeechSynthesizer synth = new SpeechSynthesizer();

    /// <summary>
    /// Speaks text synchronously.
    /// </summary>
    /// <param name="text">Text to speak.</param>
    public void Speak(string text)
    {
      this.synth.Speak(text);
    }

    /// <summary>
    /// Speaks text asynchronously.
    /// </summary>
    /// <param name="text">Text to speak.</param>
    public void SpeakAsync(string text)
    {
      this.synth.SpeakAsync(text);
    }

    /// <summary>
    /// Stops all queued and active speech.
    /// </summary>
    public void Stop()
    {
      this.synth.SpeakAsyncCancelAll();
    }
  }
}
