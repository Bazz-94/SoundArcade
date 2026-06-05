using System;
using System.Collections.Generic;
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
  /// Advances spawn timers and returns all obstacles due for spawn this frame.
  /// </summary>
  /// <param name="deltaTimeSeconds">Frame delta in seconds.</param>
  /// <param name="runElapsedSeconds">Total elapsed run time in seconds.</param>
  /// <returns>All obstacles due for spawn this frame.</returns>
  public IReadOnlyList<RunObstacle> Update(float deltaTimeSeconds, float runElapsedSeconds)
  {
    this.elapsedSinceSpawn += deltaTimeSeconds;

    float interval = MathF.Max(
      this.settings.MinimumSpawnIntervalSeconds,
      this.settings.StartingSpawnIntervalSeconds - (this.settings.SpawnIntervalDecayPerSecond * runElapsedSeconds));

    List<RunObstacle> spawned = [];

    while (this.elapsedSinceSpawn >= interval)
    {
      this.elapsedSinceSpawn -= interval;

      int lane = this.random.Next(RunConstants.Lane.Left, RunConstants.Lane.Right + 1);
      float speed = this.settings.StartingObstacleSpeed + (this.settings.ObstacleSpeedGainPerSecond * runElapsedSeconds);

      spawned.Add(new RunObstacle(lane, this.settings.SpawnZ, speed));
    }

    return spawned;
  }

  /// <summary>
  /// Resets internal spawn timers for a fresh run.
  /// </summary>
  public void Reset()
  {
    this.elapsedSinceSpawn = 0.0f;
  }
}
