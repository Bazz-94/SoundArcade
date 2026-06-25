namespace SoundArcade.Domain.RiverRun.Services
{
  using System;
  using System.Collections.Generic;
  using System.Numerics;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.RiverRun.Models;

  /// <summary>
  /// Spawns RiverRun pickups based on player forward progress.
  /// </summary>
  public sealed class PickupSpawner
  {
    private Color Color { get; }
    private readonly RiverRunSettings settings;
    private readonly Random random;
    private float NextSpawnZ { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PickupSpawner"/> class.
    /// </summary>
    /// <param name="color">Color used to render spawned pickups.</param>
    /// <param name="settings">Tuning values for spawn distance and lane selection.</param>
    /// <param name="random">Optional random source used for lane selection.</param>
    public PickupSpawner(Color color, RiverRunSettings settings, Random? random = null)
    {
      this.Color = color;
      this.settings = settings;
      this.random = random ?? new Random();
      this.NextSpawnZ = this.settings.PickupSpawnZ;
    }

    /// <summary>
    /// Advances the spawn track based on player Z and returns pickups due for spawn this frame.
    /// </summary>
    /// <param name="playerZ">Current player Z position.</param>
    /// <returns>All pickups due for spawn this frame.</returns>
    public IReadOnlyList<Pickup> Update(float playerZ)
    {
      List<Pickup> spawned = new List<Pickup>();

      while (this.NextSpawnZ - playerZ <= settings.PickupSpawnZ)
      {
        float laneX;
        int laneIndex = random.Next(0, 3);
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

        float spacing = settings.PickupSpawnDistanceMin + (float)random.NextDouble() * (settings.PickupSpawnDistanceMax - settings.PickupSpawnDistanceMin);
        float spawnZ = this.NextSpawnZ + spacing;
        this.NextSpawnZ = spawnZ;

        spawned.Add(this.CreatePickup(new Vector3(laneX, RunConstants.GroundY, spawnZ)));
      }

      return spawned;
    }

    public Pickup CreatePickup(Vector3 position)
    {
      return new Pickup(position, this.Color);
    }

    /// <summary>
    /// Resets internal spawn timers for a fresh run.
    /// </summary>
    public void Reset()
    {
      this.NextSpawnZ = settings.PickupSpawnZ;
    }
  }
}
