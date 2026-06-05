namespace SoundArcade.Infrastructure.Windows
{
  using System;
  using System.Speech.Synthesis;
  using SoundArcade.Abstractions;

  /// <summary>
  /// Windows speech-synthesis implementation of <see cref="ITts"/>.
  /// Sealed because it implements IDisposable without a finalizer; sealing
  /// prevents inheritance issues related to disposal.
  /// </summary>
  public sealed class TextToSpeech : ITts, IDisposable
  {
    private readonly SpeechSynthesizer synth = new SpeechSynthesizer();

    /// <summary>
    /// Releases resources used by the instance.
    /// </summary>
    public void Dispose()
    {
      this.Stop();
      this.synth.Dispose();
    }

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
