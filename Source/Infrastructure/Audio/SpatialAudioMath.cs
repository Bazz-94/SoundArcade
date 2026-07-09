namespace SoundArcade.Infrastructure.Audio
{
  using System;
  using System.Numerics;

  /// <summary>
  /// Pure spatial playback math shared by the Raylib audio backend and test doubles.
  /// Keeping this free of Raylib types lets unit tests verify the exact levels a sound
  /// plays at for any listener and source position.
  /// </summary>
  public static class SpatialAudioMath
  {
    /// <summary>
    /// Distance at which positional playback reaches its quietest attenuation.
    /// </summary>
    public const float MaximumHearDistance = 20.0f;

    /// <summary>
    /// Attenuation floor so distance alone never silences a sound in range; the requested
    /// volume is free to go down to silence so callers can tune quiet cues like the ambient river.
    /// </summary>
    public const float MinimumAudibleVolume = 0.05f;

    /// <summary>
    /// Lateral distance, in world units, at which a source is panned fully to one ear.
    /// </summary>
    public const float PanWidth = 2.0f;

    /// <summary>
    /// Calculates volume attenuation for positional playback using linear distance falloff.
    /// </summary>
    /// <param name="listener">Listener position.</param>
    /// <param name="source">Sound source position.</param>
    /// <returns>An attenuation factor clamped to [<see cref="MinimumAudibleVolume"/>, 1].</returns>
    public static float ComputeAttenuation(Vector3 listener, Vector3 source)
    {
      float distance = Vector3.Distance(listener, source);
      float attenuation = 1.0f - (distance / MaximumHearDistance);
      return Math.Clamp(attenuation, MinimumAudibleVolume, 1.0f);
    }

    /// <summary>
    /// Calculates the stereo pan for positional playback, where -1.0 is fully left,
    /// 0.0 is center and 1.0 is fully right. A source on the screen-left has a greater
    /// X than the listener (positive offset), so it must map to a negative pan to be
    /// heard on the left.
    /// </summary>
    /// <param name="listener">Listener position.</param>
    /// <param name="source">Sound source position.</param>
    /// <returns>A pan value clamped to [-1, 1].</returns>
    public static float ComputePan(Vector3 listener, Vector3 source)
    {
      float lateralOffset = Math.Clamp((source.X - listener.X) / PanWidth, -1.0f, 1.0f);
      return -lateralOffset;
    }

    /// <summary>
    /// Calculates the effective playback volume for a positional sound: the requested
    /// volume scaled by the per-asset gain and the distance attenuation.
    /// </summary>
    /// <param name="volume">Requested playback volume.</param>
    /// <param name="gain">Per-asset gain multiplier.</param>
    /// <param name="listener">Listener position.</param>
    /// <param name="source">Sound source position.</param>
    /// <returns>The effective volume clamped to [0, 1].</returns>
    public static float ComputePlaybackVolume(float volume, float gain, Vector3 listener, Vector3 source)
    {
      return Math.Clamp(volume * gain * ComputeAttenuation(listener, source), 0.0f, 1.0f);
    }

    /// <summary>
    /// Splits an effective playback volume into per-ear levels using a linear pan law:
    /// at center pan both ears receive the full effective volume, and panning towards one
    /// side linearly reduces the opposite ear to silence. This is the reference model used
    /// by tests; the Raylib mixer applies its own pan law to the same pan value at runtime.
    /// </summary>
    /// <param name="effectiveVolume">Effective playback volume after gain and attenuation.</param>
    /// <param name="pan">Stereo pan where -1.0 is fully left and 1.0 is fully right.</param>
    /// <returns>The left and right ear volumes.</returns>
    public static (float Left, float Right) ComputeChannelVolumes(float effectiveVolume, float pan)
    {
      float left = Math.Clamp(1.0f - pan, 0.0f, 1.0f) * effectiveVolume;
      float right = Math.Clamp(1.0f + pan, 0.0f, 1.0f) * effectiveVolume;
      return (left, right);
    }
  }
}
