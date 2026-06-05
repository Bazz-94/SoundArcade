namespace SoundArcade.Abstractions;

public interface ITts
{
  void Speak(string text);
  void SpeakAsync(string text);
  void Stop();
}
