using System.Numerics;

namespace SoundArcade.Domain.RiverRun.Models;

/// <summary>
/// Event for playing an audio asset, optionally at a 3D position.
/// </summary>
/// <param name="SoundId">Audio asset identifier.</param>
/// <param name="Position">Optional world position for positional playback.</param>
/// <param name="Volume">Requested playback volume.</param>
public sealed record PlaySoundEvent(string SoundId, Vector3? Position = null, float Volume = 1.0f) : RunEvent;
