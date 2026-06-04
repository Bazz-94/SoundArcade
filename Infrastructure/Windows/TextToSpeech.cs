namespace SoundArcade.Infrastructure.Windows
{
  using System.Speech.Synthesis;
  using SoundArcade.Abstraction;

  public class TextToSpeech : ITextToSpeech
  {
    private readonly SpeechSynthesizer synth = new();

    public void init()
    {
      PromptBuilder builder = new();
      Prompt prompt = new("Hello, World!");
    }

    public void Speak(string text)
        => synth.Speak(text);

    public void SpeakAsync(string text)
        => synth.SpeakAsync(text);

    public void Stop()
        => synth.SpeakAsyncCancelAll();
  }
}
