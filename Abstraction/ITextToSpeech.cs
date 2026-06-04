namespace SoundArcade.Abstraction
{
  public interface ITextToSpeech
  {
    void Speak(string text);
    void SpeakAsync(string text);
    void Stop();
  }
}
