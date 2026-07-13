namespace SoundArcade.Tests
{
  using System;
  using System.Numerics;
  using SoundArcade.Domain.Colors;
  using SoundArcade.Domain.RiverRun.Models;
  using SoundArcade.Domain.RiverRun.Models.Enum;
  using SoundArcade.Domain.RiverRun.Models.GameObjects;
  using Xunit;

  /// <summary>
  /// Tests for player lane movement, forward advancement, and command handling.
  /// </summary>
  public sealed class PlayerTests
  {
    private const float StartingSpeed = 2.0f;

    private static Player CreatePlayer(float laneX = RunConstants.LaneX.Center)
    {
      return new Player(
        new Theme().ColorPalette.Tertiary,
        new Vector3(laneX, RunConstants.GroundY, 0.0f),
        StartingSpeed,
        speedIncreasePerZUnit: 0.0f,
        maxSpeedIncrease: 0.0f);
    }

    /// <summary>
    /// Verifies moving left from the center lane lands on the left lane.
    /// </summary>
    [Fact]
    public void MoveLeft_moves_one_lane_left()
    {
      Player player = CreatePlayer();

      player.MoveLeft();

      Assert.Equal(RunConstants.LaneX.Left, player.Position.X);
    }

    /// <summary>
    /// Verifies moving left from the outer left lane does nothing.
    /// </summary>
    [Fact]
    public void MoveLeft_is_ignored_on_the_left_lane()
    {
      Player player = CreatePlayer(RunConstants.LaneX.Left);

      player.MoveLeft();

      Assert.Equal(RunConstants.LaneX.Left, player.Position.X);
    }

    /// <summary>
    /// Verifies moving right from the center lane lands on the right lane.
    /// </summary>
    [Fact]
    public void MoveRight_moves_one_lane_right()
    {
      Player player = CreatePlayer();

      player.MoveRight();

      Assert.Equal(RunConstants.LaneX.Right, player.Position.X);
    }

    /// <summary>
    /// Verifies moving right from the outer right lane does nothing.
    /// </summary>
    [Fact]
    public void MoveRight_is_ignored_on_the_right_lane()
    {
      Player player = CreatePlayer(RunConstants.LaneX.Right);

      player.MoveRight();

      Assert.Equal(RunConstants.LaneX.Right, player.Position.X);
    }

    /// <summary>
    /// Verifies advancing moves the player forward by speed multiplied by delta time.
    /// </summary>
    [Fact]
    public void Advance_moves_forward_by_speed_times_delta()
    {
      Player player = CreatePlayer();

      player.Advance(deltaTimeSeconds: 0.5f);

      Assert.Equal(StartingSpeed * 0.5f, player.Position.Z);
    }

    /// <summary>
    /// Verifies lane movement preserves the player's forward progress.
    /// </summary>
    [Fact]
    public void MoveLeft_preserves_z_position()
    {
      Player player = CreatePlayer();
      player.Advance(deltaTimeSeconds: 1.0f);

      float zBeforeMove = player.Position.Z;
      player.MoveLeft();

      Assert.Equal(zBeforeMove, player.Position.Z);
    }

    /// <summary>
    /// Verifies movement commands are dispatched to the matching move method.
    /// </summary>
    [Fact]
    public void HandleCommand_applies_movement_commands()
    {
      Player player = CreatePlayer();

      player.HandleCommand(RunCommand.MoveLeft);
      Assert.Equal(RunConstants.LaneX.Left, player.Position.X);

      player.HandleCommand(RunCommand.MoveRight);
      Assert.Equal(RunConstants.LaneX.Center, player.Position.X);
    }

    /// <summary>
    /// Verifies non-player commands are rejected.
    /// </summary>
    [Fact]
    public void HandleCommand_rejects_non_player_commands()
    {
      Player player = CreatePlayer();

      Assert.Throws<ArgumentException>(() => player.HandleCommand(RunCommand.TogglePause));
    }
  }
}
