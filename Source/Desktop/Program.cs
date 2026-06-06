using System;
using SoundArcade.Application.GameRegistry;
using SoundArcade.Domain;
using SoundArcade.Domain.RiverRun;

namespace SoundArcade.Desktop;

internal class Program
{
  private static void Main(string[] args)
  {
    IGame[] games = [new RiverRunGame()];
    GameRegistry registry = new GameRegistry(games);

    Console.WriteLine($"Registered {registry.Games.Count} game(s).");
  }
}
