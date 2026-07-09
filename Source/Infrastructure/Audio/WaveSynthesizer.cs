namespace SoundArcade.Infrastructure.Audio
{
  using System;
  using System.IO;
  using SoundArcade.Abstractions;

  /// <summary>
  /// Synthesizes PCM sample data for procedurally generated sounds described by a <see cref="SoundProfile"/>.
  /// Pure sample math with no audio-device dependency, so it is unit testable.
  /// </summary>
  public static class WaveSynthesizer
  {
    /// <summary>
    /// Output sample rate in hertz.
    /// </summary>
    public const int SampleRate = 44100;

    private const int ChannelCount = 1;
    private const int BitsPerSample = 16;

    /// <summary>
    /// Peak amplitude as a fraction of the 16-bit range, kept below 1.0 to leave headroom.
    /// </summary>
    private const float PeakAmplitude = 0.9f;

    /// <summary>
    /// Longest attack/release fade in seconds, used to avoid clicks at the sound boundaries.
    /// </summary>
    private const float MaxFadeSeconds = 0.01f;

    /// <summary>
    /// Fraction of the total duration that the attack/release fade may occupy at most, so very
    /// short sounds still keep an audible sustained body.
    /// </summary>
    private const float MaxFadeFractionOfDuration = 0.25f;

    /// <summary>
    /// Generates 16-bit mono PCM samples for a sound profile, with a short attack/release
    /// envelope so playback starts and ends without clicks.
    /// </summary>
    /// <param name="profile">Profile describing the waveform, frequency, and duration.</param>
    /// <returns>The generated samples.</returns>
    public static short[] Synthesize(SoundProfile profile)
    {
      int sampleCount = (int)(profile.DurationSeconds * SampleRate);
      short[] samples = new short[sampleCount];
      int fadeSampleCount = (int)(Math.Min(MaxFadeSeconds, profile.DurationSeconds * MaxFadeFractionOfDuration) * SampleRate);

      for (int i = 0; i < sampleCount; i++)
      {
        // Phase is the position within the current wavelength cycle, in the range [0, 1).
        float phase = (float)(i * (double)profile.FrequencyHz / SampleRate % 1.0);
        float value = ComputeWaveValue(profile.Waveform, phase);
        samples[i] = (short)(value * ComputeEnvelope(i, sampleCount, fadeSampleCount) * PeakAmplitude * short.MaxValue);
      }

      return samples;
    }

    /// <summary>
    /// Generates a complete in-memory WAV file (16-bit mono PCM) for a sound profile.
    /// </summary>
    /// <param name="profile">Profile describing the waveform, frequency, and duration.</param>
    /// <returns>The WAV file bytes.</returns>
    public static byte[] SynthesizeWav(SoundProfile profile)
    {
      short[] samples = Synthesize(profile);
      int dataByteCount = samples.Length * sizeof(short);
      int byteRate = SampleRate * ChannelCount * sizeof(short);

      using MemoryStream stream = new MemoryStream();
      using BinaryWriter writer = new BinaryWriter(stream);

      writer.Write("RIFF"u8);
      writer.Write(36 + dataByteCount);
      writer.Write("WAVE"u8);
      writer.Write("fmt "u8);
      writer.Write(16); // PCM format chunk size.
      writer.Write((short)1); // PCM format tag.
      writer.Write((short)ChannelCount);
      writer.Write(SampleRate);
      writer.Write(byteRate);
      writer.Write((short)(ChannelCount * sizeof(short))); // Block align.
      writer.Write((short)BitsPerSample);
      writer.Write("data"u8);
      writer.Write(dataByteCount);

      foreach (short sample in samples)
      {
        writer.Write(sample);
      }

      writer.Flush();
      return stream.ToArray();
    }

    /// <summary>
    /// Computes the raw waveform value in the range [-1, 1] for a phase position.
    /// </summary>
    /// <param name="waveform">Waveform shape.</param>
    /// <param name="phase">Position within the current wavelength cycle, in the range [0, 1).</param>
    /// <returns>The waveform value.</returns>
    private static float ComputeWaveValue(Waveform waveform, float phase)
    {
      switch (waveform)
      {
        case Waveform.Sine:
          return (float)Math.Sin(2.0 * Math.PI * phase);
        case Waveform.Square:
          return phase < 0.5f ? 1.0f : -1.0f;
        case Waveform.Triangle:
          return (4.0f * Math.Abs(phase - 0.5f)) - 1.0f;
        case Waveform.Sawtooth:
          return (2.0f * phase) - 1.0f;
        default:
          throw new ArgumentOutOfRangeException(nameof(waveform), $"Unhandled waveform {waveform}.");
      }
    }

    /// <summary>
    /// Computes the linear attack/release envelope multiplier for a sample, in the range [0, 1].
    /// </summary>
    /// <param name="sampleIndex">Index of the sample being generated.</param>
    /// <param name="sampleCount">Total number of samples.</param>
    /// <param name="fadeSampleCount">Number of samples in each of the attack and release fades.</param>
    /// <returns>The envelope multiplier.</returns>
    private static float ComputeEnvelope(int sampleIndex, int sampleCount, int fadeSampleCount)
    {
      if (fadeSampleCount <= 0)
      {
        return 1.0f;
      }

      if (sampleIndex < fadeSampleCount)
      {
        return sampleIndex / (float)fadeSampleCount;
      }

      int samplesFromEnd = sampleCount - 1 - sampleIndex;
      if (samplesFromEnd < fadeSampleCount)
      {
        return samplesFromEnd / (float)fadeSampleCount;
      }

      return 1.0f;
    }
  }
}
