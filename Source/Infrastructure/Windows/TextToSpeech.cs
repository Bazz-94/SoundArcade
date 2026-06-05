namespace SoundArcade.Infrastructure.Windows
{
  using System.Speech.Synthesis;
  using SoundArcade.Abstractions;

  public class TextToSpeech : ITts
  {
    private readonly SpeechSynthesizer synth = new();

    public void Speak(string text)
        => synth.Speak(text);

    public void SpeakAsync(string text)
        => synth.SpeakAsync(text);

    public void Stop()
        => synth.SpeakAsyncCancelAll();
  }
}
