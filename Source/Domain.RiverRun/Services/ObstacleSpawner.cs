namespace SoundArcade.Domain.RiverRun.Services
{
  using System;
  using System.Collections.Generic;
  using System.Numerics;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.RiverRun.Models;

  /// <summary>
  /// Spawns RiverRun obstacles based on player forward progress.
  /// </summary>
  public sealed class ObstacleSpawner
  {
    private Color Color { get; }
    private readonly RiverRunSettings settings;
    private readonly Random random;
    private float NextSpawnZ { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObstacleSpawner"/> class.
    /// </summary>
    /// <param name="settings">Tuning values for spawn distance and obstacle spacing.</param>
    /// <param name="random">Optional random source used for lane selection.</param>
    public ObstacleSpawner(Color color, RiverRunSettings settings, Random? random = null)
    {
      this.Color = color;
      this.settings = settings;
      this.random = random ?? new Random();
      this.NextSpawnZ = this.settings.SpawnZ;
    }

    /// <summary>
    /// Advances the spawn track based on player Z and returns obstacles due for spawn this frame.
    /// </summary>
    /// <param name="playerZ">Current player Z position.</param>
    /// <returns>All obstacles due for spawn this frame.</returns>
    public IReadOnlyList<Obstacle> Update(float playerZ)
    {
      List<Obstacle> spawned = new List<Obstacle>();

      while (this.NextSpawnZ - playerZ <= settings.SpawnZ)
      {
        // Choose lane X from the three lane constants.
        float laneX;
        int laneIndex = random.Next(0, 3); // 0,1,2
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
        float spacing = settings.SpawnDistanceMin + (float)random.NextDouble() * (settings.SpawnDistanceMax - settings.SpawnDistanceMin);
        float spawnZ = this.NextSpawnZ + spacing;
        this.NextSpawnZ = spawnZ;

        spawned.Add(this.CreateRunObstacle(new Vector3(laneX, RunConstants.GroundY, spawnZ)));
      }

      return spawned;
    }

    public Obstacle CreateRunObstacle(Vector3 position)
    {
      return new Obstacle(position, this.Color);
    }

    /// <summary>
    /// Resets internal spawn timers for a fresh run.
    /// </summary>
    public void Reset()
    {
      this.NextSpawnZ = settings.SpawnZ;
    }
  }
}
