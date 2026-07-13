namespace SoundArcade.Tests.Mock
{
  using System;
  using SoundArcade.Abstractions;

  /// <summary>
  /// Mock <see cref="ITts"/> test double recording the last spoken text and volume.
  /// </summary>
  public sealed class MockTts : ITts
  {
    public event EventHandler? SpeakStarted
    {
      add
      {
      }

      remove
      {
      }
    }

    public event EventHandler? SpeakCompleted
    {
      add
      {
      }

      remove
      {
      }
    }

    public float Volume { get; private set; } = 1.0f;

    public string? LastSpokenText { get; private set; }

    public void Speak(string text)
    {
      this.LastSpokenText = text;
    }

    public void SpeakAsync(string text)
    {
      this.LastSpokenText = text;
    }

    public void Stop()
    {
    }

    public void SetVolume(float volume)
    {
      this.Volume = volume;
    }
  }
}
