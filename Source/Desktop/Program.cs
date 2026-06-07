using System;
using Microsoft.Extensions.DependencyInjection;
using SoundArcade.Application.GameRegistry;
using SoundArcade.Desktop.DependencyInjection;

namespace SoundArcade.Desktop;

internal class Program
{
  private static void Main(string[] args)
  {
    ServiceCollection services = new ServiceCollection();
    services.AddSoundArcade();

    using ServiceProvider serviceProvider = services.BuildServiceProvider();
    GameRegistry registry = serviceProvider.GetRequiredService<GameRegistry>();

    Console.WriteLine($"Registered {registry.Games.Count} game(s).");
  }
}
