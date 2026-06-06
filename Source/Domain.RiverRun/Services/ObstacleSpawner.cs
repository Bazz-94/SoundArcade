using System;
using System.Collections.Generic;
using System.Numerics;
using SoundArcade.Domain.RiverRun.Models;

namespace SoundArcade.Domain.RiverRun.Services;

/// <summary>
/// Spawns RiverRun obstacles using a time-based difficulty curve.
/// </summary>
public sealed class ObstacleSpawner
{
  private readonly RunSettings settings;
  private readonly Random random;
  private float ElapsedSinceSpawn { get; set; }
  private float NextSpawnZ { get; set; }

  /// <summary>
  /// Initializes a new instance of the <see cref="ObstacleSpawner"/> class.
  /// </summary>
  /// <param name="settings">Tuning values for spawn cadence and obstacle speed.</param>
  /// <param name="random">Optional random source used for lane selection.</param>
  public ObstacleSpawner(RunSettings settings, Random? random = null)
  {
    this.settings = settings;
    this.random = random ?? new Random();
    this.ElapsedSinceSpawn = 0.0f;
    this.NextSpawnZ = this.settings.SpawnZ;
  }

  /// <summary>
  /// Advances spawn timers and returns all obstacles due for spawn this frame.
  /// </summary>
  /// <param name="deltaTimeSeconds">Frame delta in seconds.</param>
  /// <param name="runElapsedSeconds">Total elapsed run time in seconds.</param>
  /// <returns>All obstacles due for spawn this frame.</returns>
  public IReadOnlyList<RunObstacle> Update(float deltaTimeSeconds, float runElapsedSeconds)
  {
    this.ElapsedSinceSpawn += deltaTimeSeconds;

    float interval = MathF.Max(
      this.settings.MinimumSpawnIntervalSeconds,
      this.settings.StartingSpawnIntervalSeconds - (this.settings.SpawnIntervalDecayPerSecond * runElapsedSeconds));

    List<RunObstacle> spawned = new List<RunObstacle>();

    while (this.ElapsedSinceSpawn >= interval)
    {
      this.ElapsedSinceSpawn -= interval;

      // Choose lane X from the three lane constants.
      float laneX;
      int laneIndex = this.random.Next(0, 3); // 0,1,2
      switch (laneIndex)
      {
        case 0:
          laneX = RunConstants.LaneX.Left;
          break;
        case 1:
          laneX = RunConstants.LaneX.Center;
          break;
        default:
          laneX = RunConstants.LaneX.Right;
          break;
      }

      // Determine Z position using the rolling NextSpawnZ and a random spacing.
      float spacing = this.settings.SpawnDistanceMin + (float)this.random.NextDouble() * (this.settings.SpawnDistanceMax - this.settings.SpawnDistanceMin);
      float spawnZ = this.NextSpawnZ + spacing;
      this.NextSpawnZ = spawnZ;

      spawned.Add(new RunObstacle(new Vector3(laneX, RunConstants.GroundY, spawnZ)));
    }

    return spawned;
  }

  /// <summary>
  /// Resets internal spawn timers for a fresh run.
  /// </summary>
  public void Reset()
  {
    this.ElapsedSinceSpawn = 0.0f;
    this.NextSpawnZ = this.settings.SpawnZ;
  }
}
