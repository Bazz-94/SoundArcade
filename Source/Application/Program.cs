namespace SoundArcade.Application
{
  using System;
  using Microsoft.Extensions.DependencyInjection;
  using SoundArcade.Application.DependencyInjection;

  internal class Program
  {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
    private static void Main(string[] args)
    {
      ServiceCollection services = new ServiceCollection();
      services.AddSoundArcade();
      services.AddRiverRun();

      using ServiceProvider serviceProvider = services.BuildServiceProvider();
      GameRegistry registry = serviceProvider.GetRequiredService<GameRegistry>();
      ArcadeShell shell = serviceProvider.GetRequiredService<ArcadeShell>();

      Console.WriteLine($"Registered {registry.Games.Count} game(s).");
      shell.Run();
    }
  }
}
