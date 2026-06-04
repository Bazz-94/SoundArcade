using System;
using SoundArcade.Abstraction;
using SoundArcade.Infrastructure.Windows;

namespace SoundArcade.Desktop
{
  internal class Program
  {
    static void Main(string[] args)
    {
      ITextToSpeech tts = new TextToSpeech();
      tts.Speak("Hello, World!");
      Console.WriteLine("Hello, World!");
    }
  }
}