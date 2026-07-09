namespace SoundArcade.Tests
{
  using System.Numerics;
  using SoundArcade.Abstractions;
  using SoundArcade.Infrastructure.Audio;
  using SoundArcade.Tests.Fakes;
  using Xunit;

  /// <summary>
  /// Tests for spatial playback levels: given a player (listener) position and a game
  /// object (source) position, the attenuation, pan and per-ear volumes must be predictable.
  /// </summary>
  public sealed class SpatialAudioTests
  {
    private const float TestFrequencyHz = 440.0f;
    private const float TestDurationSeconds = 0.1f;
    private const string TestSoundId = "test_cue";

    /// <summary>
    /// Attenuation must fall linearly with distance and clamp to the audible floor.
    /// </summary>
    [Theory]
    [InlineData(0.0f, 1.0f)] // At the listener: full volume.
    [InlineData(10.0f, 0.5f)] // Half the hear distance: half volume.
    [InlineData(20.0f, 0.2f)] // At the hear distance: clamped to the floor.
    [InlineData(50.0f, 0.2f)] // Beyond the hear distance: still the floor, never silent.
    public void ComputeAttenuation_FallsLinearlyAndClampsToFloor(float distance, float expectedAttenuation)
    {
      float attenuation = SpatialAudioMath.ComputeAttenuation(Vector3.Zero, new Vector3(0.0f, 0.0f, distance));

      Assert.Equal(expectedAttenuation, attenuation, precision: 3);
    }

    /// <summary>
    /// Sources on the screen-left (greater X than the listener) must pan negative (left ear),
    /// sources on the screen-right must pan positive, and the pan must clamp at full deflection.
    /// </summary>
    [Theory]
    [InlineData(0.0f, 0.0f)] // Directly ahead: centered.
    [InlineData(1.0f, -0.5f)] // One unit screen-left: half left.
    [InlineData(2.0f, -1.0f)] // At the pan width: fully left.
    [InlineData(5.0f, -1.0f)] // Beyond the pan width: clamped fully left.
    [InlineData(-1.0f, 0.5f)] // One unit screen-right: half right.
    [InlineData(-5.0f, 1.0f)] // Beyond the pan width: clamped fully right.
    public void ComputePan_MapsLateralOffsetToStereoPan(float sourceX, float expectedPan)
    {
      float pan = SpatialAudioMath.ComputePan(Vector3.Zero, new Vector3(sourceX, 0.0f, 0.0f));

      Assert.Equal(expectedPan, pan, precision: 3);
    }

    /// <summary>
    /// Given a player position, a game object position, the sound's gain and the requested
    /// volume, playback must produce the expected left- and right-ear levels.
    /// </summary>
    [Theory]
    // Object at the player, gain 1, volume 1: full volume in both ears.
    [InlineData(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f)]
    // Object 2 units screen-left: attenuation 0.9, pan fully left, right ear silent.
    [InlineData(0.0f, 0.0f, 0.0f, 2.0f, 0.0f, 0.0f, 1.0f, 1.0f, 0.9f, 0.0f)]
    // Object 1 unit screen-right, gain 2, volume 0.5: effective 0.95, half pan right.
    [InlineData(0.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f, 2.0f, 0.5f, 0.475f, 0.95f)]
    // Object 30 units ahead: attenuation floor 0.2, centered in both ears.
    [InlineData(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 30.0f, 1.0f, 1.0f, 0.2f, 0.2f)]
    // Player offset from origin, object 1 unit to the player's screen-left: pan follows relative offset.
    [InlineData(3.0f, 0.0f, 10.0f, 4.0f, 0.0f, 10.0f, 1.0f, 1.0f, 0.95f, 0.475f)]
    // Requested volume zero stays silent regardless of position and gain.
    [InlineData(0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 2.0f, 0.0f, 0.0f, 0.0f)]
    public void PlaySoundAt_ProducesExpectedLeftAndRightVolumes(
      float playerX,
      float playerY,
      float playerZ,
      float objectX,
      float objectY,
      float objectZ,
      float gain,
      float volume,
      float expectedLeftVolume,
      float expectedRightVolume)
    {
      MockAudio audio = new MockAudio();
      audio.RegisterGeneratedSound(TestSoundId, new SoundProfile(Waveform.Sine, TestFrequencyHz, TestDurationSeconds, gain));
      audio.SetListenerPosition(new Vector3(playerX, playerY, playerZ));

      audio.PlaySoundAt(TestSoundId, new Vector3(objectX, objectY, objectZ), volume);

      SoundPlayback playback = Assert.Single(audio.Playbacks);
      Assert.Equal(expectedLeftVolume, playback.LeftVolume, precision: 3);
      Assert.Equal(expectedRightVolume, playback.RightVolume, precision: 3);
    }

    /// <summary>
    /// The captured playback must expose the effective volume and pan the backend would apply.
    /// </summary>
    [Fact]
    public void PlaySoundAt_CapturesEffectiveVolumeAndPan()
    {
      MockAudio audio = new MockAudio();
      audio.RegisterGeneratedSound(TestSoundId, new SoundProfile(Waveform.Square, TestFrequencyHz, TestDurationSeconds, gain: 1.0f));
      audio.SetListenerPosition(Vector3.Zero);

      audio.PlaySoundAt(TestSoundId, new Vector3(1.0f, 0.0f, 4.0f), volume: 0.8f, pitch: 1.25f);

      SoundPlayback playback = Assert.Single(audio.Playbacks);
      float expectedAttenuation = 1.0f - (new Vector3(1.0f, 0.0f, 4.0f).Length() / SpatialAudioMath.MaximumHearDistance);
      Assert.Equal(TestSoundId, playback.SoundId);
      Assert.Equal(0.8f * expectedAttenuation, playback.EffectiveVolume, precision: 3);
      Assert.Equal(-0.5f, playback.Pan, precision: 3);
      Assert.Equal(1.25f, playback.Pitch, precision: 3);
    }

    /// <summary>
    /// Non-positional playback must apply the registered gain and stay centered.
    /// </summary>
    [Fact]
    public void PlaySound_AppliesGainWithoutPanning()
    {
      MockAudio audio = new MockAudio();
      audio.RegisterSound(TestSoundId, "unused.mp3", gain: 0.5f);

      audio.PlaySound(TestSoundId, volume: 0.8f);

      SoundPlayback playback = Assert.Single(audio.Playbacks);
      Assert.Equal(0.4f, playback.EffectiveVolume, precision: 3);
      Assert.Equal(0.0f, playback.Pan, precision: 3);
      Assert.Equal(playback.EffectiveVolume, playback.LeftVolume, precision: 3);
      Assert.Equal(playback.EffectiveVolume, playback.RightVolume, precision: 3);
      Assert.Null(playback.Position);
    }
  }
}
