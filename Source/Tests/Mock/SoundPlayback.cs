namespace SoundArcade.Tests.Mock
{
  using System.Numerics;
  using SoundArcade.Infrastructure.Audio;

  /// <summary>
  /// One captured playback with the spatial levels the backend would apply.
  /// </summary>
  /// <param name="SoundId">Audio asset identifier.</param>
  /// <param name="Position">Source world position, or null for non-positional playback.</param>
  /// <param name="RequestedVolume">Volume passed by the caller before gain and attenuation.</param>
  /// <param name="Pitch">Playback pitch multiplier.</param>
  /// <param name="EffectiveVolume">Volume after gain and distance attenuation, clamped to [0, 1].</param>
  /// <param name="Pan">Stereo pan where -1.0 is fully left and 1.0 is fully right.</param>
  /// <param name="LeftVolume">Left-ear level per <see cref="SpatialAudioMath.ComputeChannelVolumes"/>.</param>
  /// <param name="RightVolume">Right-ear level per <see cref="SpatialAudioMath.ComputeChannelVolumes"/>.</param>
  public sealed record SoundPlayback(
    string SoundId,
    Vector3? Position,
    float RequestedVolume,
    float Pitch,
    float EffectiveVolume,
    float Pan,
    float LeftVolume,
    float RightVolume);
}
