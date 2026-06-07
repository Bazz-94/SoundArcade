using System;

namespace SoundArcade.Abstractions;

/// <summary>
/// Provides text-to-speech synthesis for accessibility announcements.
/// </summary>
public interface ITts
{
  /// <summary>
  /// Raised when speech begins.
  /// </summary>
  event EventHandler? SpeakStarted;

  /// <summary>
  /// Raised when speech ends or is cancelled.
  /// </summary>
  event EventHandler? SpeakCompleted;

  /// <summary>
  /// Speaks text synchronously.
  /// </summary>
  /// <param name="text">Text to speak.</param>
  void Speak(string text);

  /// <summary>
  /// Queues text for asynchronous speech.
  /// </summary>
  /// <param name="text">Text to speak.</param>
  void SpeakAsync(string text);

  /// <summary>
  /// Stops all pending and active speech.
  /// </summary>
  void Stop();
}
