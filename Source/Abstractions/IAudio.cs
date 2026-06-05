using System.Numerics;

namespace SoundArcade.Abstractions;

public interface IAudio
{
  void PlaySound(string soundId, float volume);
  void PlaySoundAt(string soundId, Vector3 position, float volume);
  void StopSound(string soundId);
  void SetListenerPosition(Vector3 position);
}
