using Microsoft.Extensions.DependencyInjection;
using SoundArcade.Abstractions;
using SoundArcade.Application.GameLoop;
using SoundArcade.Application.GameRegistry;
using SoundArcade.Domain;
using SoundArcade.Domain.RiverRun;
using SoundArcade.Domain.RiverRun.Models;
using SoundArcade.Domain.RiverRun.Services;
using SoundArcade.Infrastructure.Audio;
using SoundArcade.Infrastructure.Windows;

namespace SoundArcade.Desktop.DependencyInjection;

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
    services.AddSingleton<IGame, RiverRunGame>();
    services.AddSingleton<IInput, RaylibInput>();
    services.AddSingleton<ITts, TextToSpeech>();
    services.AddSingleton<RaylibAudio>(serviceProvider => new RaylibAudio(serviceProvider.GetRequiredService<ITts>()));
    services.AddSingleton<IAudio>(serviceProvider => serviceProvider.GetRequiredService<RaylibAudio>());
    services.AddSingleton<RunSettings>();
    services.AddSingleton<Session>();
    services.AddSingleton<PlayerController>();
    services.AddSingleton<GameLoop>();
    services.AddSingleton<GameRegistry>();

    return services;
  }
}
