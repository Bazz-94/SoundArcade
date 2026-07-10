namespace SoundArcade.Application
{
  using Microsoft.Extensions.DependencyInjection;
  using SoundArcade.Application.DependencyInjection;

  /// <summary>
  /// Executable entry point that wires the composition root and starts the shell.
  /// </summary>
  internal class Program
  {
    /// <summary>
    /// Builds the service provider and runs the arcade shell.
    /// </summary>
    private static void Main()
    {
      ServiceCollection services = new ServiceCollection();
      services.AddSoundArcade();
      services.AddRiverRun();

      using ServiceProvider serviceProvider = services.BuildServiceProvider();
      serviceProvider.GetRequiredService<ArcadeShell>().Run();
    }
  }
}
