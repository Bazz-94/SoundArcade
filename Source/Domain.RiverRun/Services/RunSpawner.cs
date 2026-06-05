using System;
using SoundArcade.Domain.RiverRun.Models;

namespace SoundArcade.Domain.RiverRun.Services;

/// <summary>
/// Spawns RiverRun obstacles using a time-based difficulty curve.
/// </summary>
public sealed class RunSpawner
{
  private readonly RunSettings settings;
  private readonly Random random;
  private float elapsedSinceSpawn;

  /// <summary>
  /// Initializes a new instance of the <see cref="RunSpawner"/> class.
  /// </summary>
  /// <param name="settings">Tuning values for spawn cadence and obstacle speed.</param>
  /// <param name="random">Optional random source used for lane selection.</param>
  public RunSpawner(RunSettings settings, Random? random = null)
  {
    this.settings = settings;
    this.random = random ?? new Random();
    elapsedSinceSpawn = 0.0f;
  }

  /// <summary>
  /// Advances spawn timers and returns a new obstacle when the spawn interval elapses.
  /// </summary>
  /// <param name="deltaTimeSeconds">Frame delta in seconds.</param>
  /// <param name="runElapsedSeconds">Total elapsed run time in seconds.</param>
  /// <returns>A spawned obstacle, or <see langword="null"/> when no spawn is due this frame.</returns>
  public RunObstacle? Update(float deltaTimeSeconds, float runElapsedSeconds)
  {
    this.elapsedSinceSpawn += deltaTimeSeconds;

    float interval = MathF.Max(
      this.settings.MinimumSpawnIntervalSeconds,
      this.settings.StartingSpawnIntervalSeconds - (this.settings.SpawnIntervalDecayPerSecond * runElapsedSeconds));

    if (this.elapsedSinceSpawn < interval)
    {
      return null;
    }

    this.elapsedSinceSpawn = 0.0f;

    int lane = this.random.Next(RunConstants.Lane.Left, RunConstants.Lane.Right + 1);
    float speed = this.settings.StartingObstacleSpeed + (this.settings.ObstacleSpeedGainPerSecond * runElapsedSeconds);

    return new RunObstacle(lane, this.settings.SpawnZ, speed);
  }

  /// <summary>
  /// Resets internal spawn timers for a fresh run.
  /// </summary>
  public void Reset()
  {
    this.elapsedSinceSpawn = 0.0f;
  }
}
