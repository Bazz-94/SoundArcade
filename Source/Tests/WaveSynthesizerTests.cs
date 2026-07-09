namespace SoundArcade.Tests
{
  using SoundArcade.Abstractions;
  using SoundArcade.Infrastructure.Audio;
  using Xunit;

  /// <summary>
  /// Tests for procedural PCM sample generation.
  /// </summary>
  public sealed class WaveSynthesizerTests
  {
    private const float TestFrequencyHz = 220.0f;
    private const float TestDurationSeconds = 0.2f;

    /// <summary>
    /// Sample count must equal duration times sample rate.
    /// </summary>
    [Theory]
    [InlineData(Waveform.Sine)]
    [InlineData(Waveform.Square)]
    [InlineData(Waveform.Triangle)]
    [InlineData(Waveform.Sawtooth)]
    public void Synthesize_ProducesExpectedSampleCount(Waveform waveform)
    {
      short[] samples = WaveSynthesizer.Synthesize(new SoundProfile(waveform, TestFrequencyHz, TestDurationSeconds));

      Assert.Equal((int)(TestDurationSeconds * WaveSynthesizer.SampleRate), samples.Length);
    }

    /// <summary>
    /// The attack/release envelope must start and end at silence to avoid clicks.
    /// </summary>
    [Theory]
    [InlineData(Waveform.Sine)]
    [InlineData(Waveform.Square)]
    [InlineData(Waveform.Triangle)]
    [InlineData(Waveform.Sawtooth)]
    public void Synthesize_StartsAndEndsSilent(Waveform waveform)
    {
      short[] samples = WaveSynthesizer.Synthesize(new SoundProfile(waveform, TestFrequencyHz, TestDurationSeconds));

      Assert.Equal(0, samples[0]);
      Assert.Equal(0, samples[samples.Length - 1]);
    }

    /// <summary>
    /// A square wave must reach both strong positive and strong negative levels in its sustained body.
    /// </summary>
    [Fact]
    public void Synthesize_SquareWaveReachesBothPolarities()
    {
      short[] samples = WaveSynthesizer.Synthesize(new SoundProfile(Waveform.Square, TestFrequencyHz, TestDurationSeconds));

      short min = short.MaxValue;
      short max = short.MinValue;

      foreach (short sample in samples)
      {
        if (sample < min)
        {
          min = sample;
        }

        if (sample > max)
        {
          max = sample;
        }
      }

      Assert.True(max > short.MaxValue / 2, $"Expected strong positive peak, got {max}.");
      Assert.True(min < short.MinValue / 2, $"Expected strong negative peak, got {min}.");
    }

    /// <summary>
    /// The WAV output must carry a valid RIFF/WAVE header and one 16-bit sample pair per generated sample.
    /// </summary>
    [Fact]
    public void SynthesizeWav_ProducesValidRiffHeader()
    {
      SoundProfile profile = new SoundProfile(Waveform.Sine, TestFrequencyHz, TestDurationSeconds);
      byte[] wav = WaveSynthesizer.SynthesizeWav(profile);

      Assert.Equal((byte)'R', wav[0]);
      Assert.Equal((byte)'I', wav[1]);
      Assert.Equal((byte)'F', wav[2]);
      Assert.Equal((byte)'F', wav[3]);
      Assert.Equal((byte)'W', wav[8]);
      Assert.Equal((byte)'A', wav[9]);
      Assert.Equal((byte)'V', wav[10]);
      Assert.Equal((byte)'E', wav[11]);

      int headerByteCount = 44;
      Assert.Equal(headerByteCount + (WaveSynthesizer.Synthesize(profile).Length * sizeof(short)), wav.Length);
    }
  }
}
