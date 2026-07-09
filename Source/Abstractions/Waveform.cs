namespace SoundArcade.Abstractions
{
  /// <summary>
  /// Basic waveform shapes available for procedural sound generation.
  /// </summary>
  public enum Waveform
  {
    /// <summary>
    /// Smooth sine wave; soft, pure tone.
    /// </summary>
    Sine,

    /// <summary>
    /// Square wave; harsh, buzzy tone.
    /// </summary>
    Square,

    /// <summary>
    /// Triangle wave; mellow tone between sine and square.
    /// </summary>
    Triangle,

    /// <summary>
    /// Sawtooth wave; bright, raspy tone.
    /// </summary>
    Sawtooth
  }
}
