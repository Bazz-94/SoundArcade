namespace SoundArcade.Tests
{
  using System.Collections.Generic;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.RiverRun.Models.Enum;
  using SoundArcade.Domain.RiverRun.Services;
  using SoundArcade.Tests.Mock;
  using Xunit;

  /// <summary>
  /// Tests for player input to command translation.
  /// </summary>
  public sealed class PlayerControllerTests
  {
    /// <summary>
    /// Verifies mapped actions produce the expected command ordering.
    /// </summary>
    [Fact]
    public void ReadCommands_returns_commands_for_pressed_inputs()
    {
      MockInput input = new MockInput();
      PlayerController controller = new PlayerController(input);

      input.Press(Input.Left);
      input.Press(Input.Right);
      input.Press(Input.Back);
      input.Press(Input.Enter);

      IReadOnlyList<RunCommand> commands = controller.ReadCommands();

      Assert.Collection(
        commands,
        command => Assert.Equal(RunCommand.MoveLeft, command),
        command => Assert.Equal(RunCommand.MoveRight, command),
        command => Assert.Equal(RunCommand.TogglePause, command),
        command => Assert.Equal(RunCommand.Restart, command));
    }

    /// <summary>
    /// Verifies no commands are produced when no mapped input is pressed.
    /// </summary>
    [Fact]
    public void ReadCommands_returns_empty_when_no_inputs_pressed()
    {
      MockInput input = new MockInput();
      PlayerController controller = new PlayerController(input);

      IReadOnlyList<RunCommand> commands = controller.ReadCommands();

      Assert.Empty(commands);
    }
  }
}
