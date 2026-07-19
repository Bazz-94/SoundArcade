namespace SoundArcade.Tests.Mock
{
  using System;
  using System.Collections.Generic;
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

    /// <summary>
    /// Gets every spoken text in order.
    /// </summary>
    public List<string> SpokenTexts { get; } = new List<string>();

    public void Speak(string text)
    {
      this.LastSpokenText = text;
      this.SpokenTexts.Add(text);
    }

    public void SpeakAsync(string text)
    {
      this.LastSpokenText = text;
      this.SpokenTexts.Add(text);
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
