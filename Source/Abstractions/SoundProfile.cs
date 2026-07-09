namespace SoundArcade.Abstractions
{
  using System;

  /// <summary>
  /// Describes a procedurally generated sound: its waveform shape, tone frequency, and duration.
  /// Used to register synthesized sounds with <see cref="IAudio"/> instead of loading audio files.
  /// </summary>
  public sealed record SoundProfile
  {
    /// <summary>
    /// Gets the waveform shape used to generate the sound.
    /// </summary>
    public Waveform Waveform { get; }

    /// <summary>
    /// Gets the tone frequency in hertz.
    /// </summary>
    public float FrequencyHz { get; }

    /// <summary>
    /// Gets the sound duration in seconds.
    /// </summary>
    public float DurationSeconds { get; }

    /// <summary>
    /// Gets the per-sound volume multiplier applied to every playback, used to balance generated
    /// sounds against other cues. 1.0 leaves the generated amplitude unchanged.
    /// </summary>
    public float Gain { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SoundProfile"/> record.
    /// </summary>
    /// <param name="waveform">Waveform shape used to generate the sound.</param>
    /// <param name="frequencyHz">Tone frequency in hertz. Must be positive.</param>
    /// <param name="durationSeconds">Sound duration in seconds. Must be positive.</param>
    /// <param name="gain">Per-sound volume multiplier. Must not be negative.</param>
    public SoundProfile(Waveform waveform, float frequencyHz, float durationSeconds, float gain = 1.0f)
    {
      if (frequencyHz <= 0.0f)
      {
        throw new ArgumentOutOfRangeException(nameof(frequencyHz), "Frequency must be positive.");
      }

      if (durationSeconds <= 0.0f)
      {
        throw new ArgumentOutOfRangeException(nameof(durationSeconds), "Duration must be positive.");
      }

      if (gain < 0.0f)
      {
        throw new ArgumentOutOfRangeException(nameof(gain), "Gain must not be negative.");
      }

      this.Waveform = waveform;
      this.FrequencyHz = frequencyHz;
      this.DurationSeconds = durationSeconds;
      this.Gain = gain;
    }
  }
}
