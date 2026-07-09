namespace SoundArcade.Domain.RiverRun.Services
{
  using System;
  using System.Collections.Generic;
  using System.Numerics;
  using SoundArcade.Domain.RiverRun.Models;
  using SoundArcade.Domain.RiverRun.Models.GameObjects;

  /// <summary>
  /// Spawns RiverRun world objects based on player forward progress.
  /// All object kinds share one spawn track, so every consecutive pair of spawned
  /// objects respects the same min/max spacing and can never overlap.
  /// </summary>
  public sealed class Spawner
  {
    private readonly IReadOnlyList<(Func<Vector3, GameObject> Factory, float Weight)> factories;
    private readonly float totalWeight;
    private readonly float spawnZ;
    private readonly float spawnDistanceMin;
    private readonly float spawnDistanceMax;
    private readonly Random random;
    private float NextSpawnZ { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Spawner"/> class.
    /// </summary>
    /// <param name="factories">Object factories with relative spawn weights; one is chosen per spawn slot.</param>
    /// <param name="spawnZ">Distance ahead of the player at which objects spawn.</param>
    /// <param name="spawnDistanceMin">Minimum spacing between consecutive spawns.</param>
    /// <param name="spawnDistanceMax">Maximum spacing between consecutive spawns.</param>
    /// <param name="random">Optional random source used for lane and factory selection.</param>
    public Spawner(
      IReadOnlyList<(Func<Vector3, GameObject> Factory, float Weight)> factories,
      float spawnZ,
      float spawnDistanceMin,
      float spawnDistanceMax,
      Random? random = null)
    {
      if (factories.Count == 0)
      {
        throw new ArgumentException("At least one factory is required.", nameof(factories));
      }

      this.factories = factories;

      float weightSum = 0.0f;
      foreach ((_, float weight) in factories)
      {
        if (weight <= 0.0f)
        {
          throw new ArgumentOutOfRangeException(nameof(factories), "Factory weights must be positive.");
        }
        weightSum += weight;
      }

      this.totalWeight = weightSum;
      this.spawnZ = spawnZ;
      this.spawnDistanceMin = spawnDistanceMin;
      this.spawnDistanceMax = spawnDistanceMax;
      this.random = random ?? new Random();
      this.NextSpawnZ = spawnZ;
    }

    /// <summary>
    /// Advances the spawn track based on player Z and returns objects due for spawn this frame.
    /// </summary>
    /// <param name="playerZ">Current player Z position.</param>
    /// <returns>All objects due for spawn this frame.</returns>
    public IReadOnlyList<GameObject> Update(float playerZ)
    {
      List<GameObject> spawned = new List<GameObject>();

      while (this.NextSpawnZ - playerZ <= this.spawnZ)
      {
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
        float spacing = this.spawnDistanceMin + ((float)this.random.NextDouble() * (this.spawnDistanceMax - this.spawnDistanceMin));
        float nextZ = this.NextSpawnZ + spacing;
        this.NextSpawnZ = nextZ;

        Func<Vector3, GameObject> factory = this.PickFactory();
        spawned.Add(factory(new Vector3(laneX, RunConstants.GroundY, nextZ)));
      }

      return spawned;
    }

    /// <summary>
    /// Resets internal spawn timers for a fresh run.
    /// </summary>
    public void Reset()
    {
      this.NextSpawnZ = this.spawnZ;
    }

    /// <summary>
    /// Picks one factory using weighted random selection.
    /// </summary>
    private Func<Vector3, GameObject> PickFactory()
    {
      float roll = (float)this.random.NextDouble() * this.totalWeight;

      foreach ((Func<Vector3, GameObject> factory, float weight) in this.factories)
      {
        if (roll < weight)
        {
          return factory;
        }
        roll -= weight;
      }

      return this.factories[this.factories.Count - 1].Factory;
    }
  }
}
