namespace SoundArcade.Application.DependencyInjection
{
  using Microsoft.Extensions.DependencyInjection;
  using SoundArcade.Abstractions;
  using SoundArcade.Application;
  using SoundArcade.Domain;
  using SoundArcade.Domain.Colors;
  using SoundArcade.Domain.RiverRun;
  using SoundArcade.Domain.Services;
  using SoundArcade.Infrastructure.Audio;
  using SoundArcade.Infrastructure.Windows;

  /// <summary>
  /// Registers the desktop composition root for Sound Arcade.
  /// </summary>
  public static class ServiceCollectionExtensions
  {
    /// <summary>
    /// Adds the current game, PAL services, and infrastructure implementations.
    /// </summary>
    /// <param name="services">Service collection to configure.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddSoundArcade(this IServiceCollection services)
    {
      services.AddSingleton<IInput, RaylibInput>();
      services.AddSingleton<IWindow, RaylibWindow>();
      services.AddSingleton<IRenderer, RaylibRenderer>();
      services.AddSingleton<ISettingsStore, FileSettingsStore>();
      services.AddSingleton<IScoreboardStore>(new FileScoreboardStore());
      services.AddSingleton<ITts, TextToSpeech>();
      services.AddSingleton<RaylibAudio>(serviceProvider => new RaylibAudio(serviceProvider.GetRequiredService<ITts>()));
      services.AddSingleton<IAudio>(serviceProvider => serviceProvider.GetRequiredService<RaylibAudio>());
      services.AddSingleton<AppSettings>();
      services.AddSingleton<ArcadeShell>();
      services.AddSingleton<GameRegistry>();
      services.AddSingleton<SceneManager>();
      services.AddSingleton<ISceneFactory, SceneFactory>();
      services.AddSingleton<Theme>();

      return services;
    }

    /// <summary>
    /// Registers the RiverRun mini-game module.
    /// </summary>
    /// <param name="services">Service collection to configure.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddRiverRun(this IServiceCollection services)
    {
      services.AddSingleton<IGame, RiverRunGame>();
      return services;
    }
  }
}
