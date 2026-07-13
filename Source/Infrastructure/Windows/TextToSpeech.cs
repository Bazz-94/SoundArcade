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
      this.synth.SpeakStarted += this.OnSpeakStarted;
      this.synth.SpeakCompleted += this.OnSpeakCompleted;
    }

    /// <summary>
    /// Releases resources used by the instance.
    /// </summary>
    public void Dispose()
    {
      this.synth.SpeakStarted -= this.OnSpeakStarted;
      this.synth.SpeakCompleted -= this.OnSpeakCompleted;
      this.Stop();
      this.synth.Dispose();
    }

    /// <inheritdoc />
    public void Speak(string text)
    {
      this.synth.Speak(text);
    }

    /// <inheritdoc />
    public void SpeakAsync(string text)
    {
      this.synth.SpeakAsync(text);
    }

    /// <inheritdoc />
    public void Stop()
    {
      this.synth.SpeakAsyncCancelAll();
    }

    /// <inheritdoc />
    public void SetVolume(float volume)
    {
      this.synth.Volume = (int)Math.Round(Math.Clamp(volume, 0.0f, 1.0f) * 100.0f);
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
