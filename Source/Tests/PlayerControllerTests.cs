namespace SoundArcade.Tests
{
  using System;
  using System.Collections.Generic;
  using SoundArcade.Abstractions;
  using SoundArcade.Application.Game;
  using SoundArcade.Domain.RiverRun.Models;
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
      FakeInput input = new FakeInput();
      PlayerController controller = new PlayerController(input);

      input.SetPressed(Input.Left);
      input.SetPressed(Input.Right);
      input.SetPressed(Input.Back);
      input.SetPressed(Input.Enter);

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
      FakeInput input = new FakeInput();
      PlayerController controller = new PlayerController(input);

      IReadOnlyList<RunCommand> commands = controller.ReadCommands();

      Assert.Empty(commands);
    }

    private sealed class FakeInput : IInput
    {
      private readonly HashSet<Input> pressed = new HashSet<Input>();
      private readonly Dictionary<Input, string> mappings = new Dictionary<Input, string>
      {
        [Input.Up] = "Up",
        [Input.Down] = "Down",
        [Input.Left] = "Left",
        [Input.Right] = "Right",
        [Input.Enter] = "Enter",
        [Input.Back] = "Escape"
      };

      public event EventHandler<InputPressedEventArgs>? Pressed;

      public bool InputPressed(Input input)
      {
        bool isPressed = pressed.Contains(input);

        if (isPressed)
        {
          this.Pressed?.Invoke(this, new InputPressedEventArgs(input));
        }

        return isPressed;
      }

      public bool InputDown(Input input)
      {
        return pressed.Contains(input);
      }

      public IReadOnlyDictionary<Input, string> GetMappings()
      {
        return mappings;
      }

      public bool TrySetMapping(Input input, string keyName)
      {
        if (string.IsNullOrWhiteSpace(keyName))
        {
          return false;
        }

        mappings[input] = keyName;
        return true;
      }

      public void ResetMappingsToDefault()
      {
        mappings[Input.Up] = "Up";
        mappings[Input.Down] = "Down";
        mappings[Input.Left] = "Left";
        mappings[Input.Right] = "Right";
        mappings[Input.Enter] = "Enter";
        mappings[Input.Back] = "Escape";
      }

      public void LoadMappings()
      {
      }

      public void SaveMappings()
      {
      }

      public void SetPressed(Input input)
      {
        pressed.Add(input);
      }
    }
  }
}