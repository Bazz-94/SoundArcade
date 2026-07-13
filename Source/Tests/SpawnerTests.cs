namespace SoundArcade.Tests
{
  using System;
  using System.Collections.Generic;
  using System.Numerics;
  using SoundArcade.Domain.Colors;
  using SoundArcade.Domain.RiverRun.Models;
  using SoundArcade.Domain.RiverRun.Models.GameObjects;
  using SoundArcade.Domain.RiverRun.Services;
  using Xunit;

  /// <summary>
  /// Tests for spawn track advancement, spacing, weighted factory selection, and reset.
  /// </summary>
  public sealed class SpawnerTests
  {
    private const int RandomSeed = 1234;
    private const float SpawnZ = 10.0f;

    private static readonly Theme Theme = new Theme();

    private static Func<Vector3, GameObject> ObstacleFactory =>
      position => new Obstacle(position, Theme.ColorPalette.Secondary);

    private static Func<Vector3, GameObject> PickupFactory =>
      position => new Pickup(position, Theme.ColorPalette.Accent);

    /// <summary>
    /// Verifies objects spawn ahead of the player as the player advances in Z.
    /// </summary>
    [Fact]
    public void Update_spawns_objects_ahead_of_the_player()
    {
      Spawner spawner = new Spawner(
        [(ObstacleFactory, 1.0f)],
        SpawnZ,
        spawnDistanceMin: 5.0f,
        spawnDistanceMax: 5.0f,
        new Random(RandomSeed));

      IReadOnlyList<GameObject> spawned = spawner.Update(playerZ: 0.0f);

      Assert.NotEmpty(spawned);

      foreach (GameObject spawnedObject in spawned)
      {
        Assert.True(spawnedObject.Position.Z > SpawnZ);
      }
    }

    /// <summary>
    /// Verifies no objects spawn while the spawn track is still far enough ahead of the player.
    /// </summary>
    [Fact]
    public void Update_spawns_nothing_while_the_track_is_ahead()
    {
      Spawner spawner = new Spawner(
        [(ObstacleFactory, 1.0f)],
        SpawnZ,
        spawnDistanceMin: 5.0f,
        spawnDistanceMax: 5.0f,
        new Random(RandomSeed));

      spawner.Update(playerZ: 0.0f);

      Assert.Empty(spawner.Update(playerZ: 0.0f));
    }

    /// <summary>
    /// Verifies consecutive spawns respect the configured min/max spacing.
    /// </summary>
    [Fact]
    public void Update_spaces_consecutive_spawns_within_configured_bounds()
    {
      const float SpacingMin = 4.0f;
      const float SpacingMax = 8.0f;

      Spawner spawner = new Spawner(
        [(ObstacleFactory, 1.0f)],
        SpawnZ,
        SpacingMin,
        SpacingMax,
        new Random(RandomSeed));

      List<GameObject> spawned = new List<GameObject>();

      foreach (float playerZ in new[] { 0.0f, 20.0f, 40.0f, 60.0f, 80.0f })
      {
        spawned.AddRange(spawner.Update(playerZ));
      }

      Assert.True(spawned.Count >= 2);

      foreach (int index in System.Linq.Enumerable.Range(1, spawned.Count - 1))
      {
        float spacing = spawned[index].Position.Z - spawned[index - 1].Position.Z;

        Assert.InRange(spacing, SpacingMin, SpacingMax);
      }
    }

    /// <summary>
    /// Verifies every spawn lands on one of the three lane X positions.
    /// </summary>
    [Fact]
    public void Update_places_spawns_on_a_valid_lane()
    {
      Spawner spawner = new Spawner(
        [(ObstacleFactory, 1.0f)],
        SpawnZ,
        spawnDistanceMin: 4.0f,
        spawnDistanceMax: 8.0f,
        new Random(RandomSeed));

      foreach (float playerZ in new[] { 0.0f, 20.0f, 40.0f, 60.0f, 80.0f })
      {
        foreach (GameObject spawnedObject in spawner.Update(playerZ))
        {
          Assert.Contains(
            spawnedObject.Position.X,
            new[] { RunConstants.LaneX.Left, RunConstants.LaneX.Center, RunConstants.LaneX.Right });
        }
      }
    }

    /// <summary>
    /// Verifies a single-factory spawner only produces that factory's object type.
    /// </summary>
    [Fact]
    public void Update_uses_the_only_registered_factory()
    {
      Spawner spawner = new Spawner(
        [(PickupFactory, 1.0f)],
        SpawnZ,
        spawnDistanceMin: 4.0f,
        spawnDistanceMax: 8.0f,
        new Random(RandomSeed));

      foreach (float playerZ in new[] { 0.0f, 20.0f, 40.0f })
      {
        foreach (GameObject spawnedObject in spawner.Update(playerZ))
        {
          Assert.IsType<Pickup>(spawnedObject);
        }
      }
    }

    /// <summary>
    /// Verifies weighted selection produces both object types over many spawns.
    /// </summary>
    [Fact]
    public void Update_mixes_object_types_with_balanced_weights()
    {
      Spawner spawner = new Spawner(
        [(ObstacleFactory, 0.5f), (PickupFactory, 0.5f)],
        SpawnZ,
        spawnDistanceMin: 1.0f,
        spawnDistanceMax: 1.0f,
        new Random(RandomSeed));

      List<GameObject> spawned = new List<GameObject>();

      foreach (float playerZ in new[] { 0.0f, 50.0f, 100.0f, 150.0f, 200.0f })
      {
        spawned.AddRange(spawner.Update(playerZ));
      }

      Assert.Contains(spawned, spawnedObject => spawnedObject is Obstacle);
      Assert.Contains(spawned, spawnedObject => spawnedObject is Pickup);
    }

    /// <summary>
    /// Verifies resetting the spawner restarts the spawn track at the initial spawn Z.
    /// </summary>
    [Fact]
    public void Reset_restarts_the_spawn_track()
    {
      const float SpacingMax = 8.0f;

      Spawner spawner = new Spawner(
        [(ObstacleFactory, 1.0f)],
        SpawnZ,
        spawnDistanceMin: 4.0f,
        SpacingMax,
        new Random(RandomSeed));

      spawner.Update(playerZ: 0.0f);
      spawner.Update(playerZ: 100.0f);
      spawner.Reset();

      IReadOnlyList<GameObject> spawned = spawner.Update(playerZ: 0.0f);

      Assert.NotEmpty(spawned);
      Assert.True(spawned[0].Position.Z <= SpawnZ + SpacingMax);
    }

    /// <summary>
    /// Verifies constructing a spawner without factories is rejected.
    /// </summary>
    [Fact]
    public void Constructor_rejects_empty_factory_list()
    {
      Assert.Throws<ArgumentException>(() => new Spawner(
        [],
        SpawnZ,
        spawnDistanceMin: 4.0f,
        spawnDistanceMax: 8.0f,
        new Random(RandomSeed)));
    }

    /// <summary>
    /// Verifies constructing a spawner with a non-positive factory weight is rejected.
    /// </summary>
    [Fact]
    public void Constructor_rejects_non_positive_weight()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() => new Spawner(
        [(ObstacleFactory, 0.0f)],
        SpawnZ,
        spawnDistanceMin: 4.0f,
        spawnDistanceMax: 8.0f,
        new Random(RandomSeed)));
    }
  }
}
