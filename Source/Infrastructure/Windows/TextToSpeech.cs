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
    private readonly SpeechSynthesizer synth = new();

    /// <inheritdoc />
    public event EventHandler? SpeakStarted;

    /// <inheritdoc />
    public event EventHandler? SpeakCompleted;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextToSpeech"/> class.
    /// </summary>
    public TextToSpeech()
    {
      synth.SpeakStarted += this.OnSpeakStarted;
      synth.SpeakCompleted += this.OnSpeakCompleted;
    }

    /// <summary>
    /// Releases resources used by the instance.
    /// </summary>
    public void Dispose()
    {
      synth.SpeakStarted -= this.OnSpeakStarted;
      synth.SpeakCompleted -= this.OnSpeakCompleted;
      this.Stop();
      synth.Dispose();
    }

    /// <summary>
    /// Speaks text synchronously.
    /// </summary>
    /// <param name="text">Text to speak.</param>
    public void Speak(string text)
    {
      synth.Speak(text);
    }

    /// <summary>
    /// Speaks text asynchronously.
    /// </summary>
    /// <param name="text">Text to speak.</param>
    public void SpeakAsync(string text)
    {
      synth.SpeakAsync(text);
    }

    /// <summary>
    /// Stops all queued and active speech.
    /// </summary>
    public void Stop()
    {
      synth.SpeakAsyncCancelAll();
    }

    /// <inheritdoc />
    public void SetVolume(float volume)
    {
      int volumePercent = (int)Math.Round(Math.Clamp(volume, 0.0f, 1.0f) * 100.0f);
      synth.Volume = volumePercent;
    }

    /// <summary>
    /// Raises the <see cref="SpeakStarted"/> event.
    /// </summary>
    /// <param name="sender">Event source.</param>
    /// <param name="e">Event data.</param>
    private void OnSpeakStarted(object? sender, SpeakStartedEventArgs e)
    {
      this.SpeakStarted?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Raises the <see cref="SpeakCompleted"/> event.
    /// </summary>
    /// <param name="sender">Event source.</param>
    /// <param name="e">Event data.</param>
    private void OnSpeakCompleted(object? sender, SpeakCompletedEventArgs e)
    {
      this.SpeakCompleted?.Invoke(this, EventArgs.Empty);
    }
  }
}
