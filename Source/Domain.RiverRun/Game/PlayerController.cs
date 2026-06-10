namespace SoundArcade.Domain.RiverRun.Game
{
  using System.Collections.Generic;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.RiverRun.Models;

  /// <summary>
  /// Translates logical input actions into RiverRun player commands.
  /// </summary>
  public sealed class PlayerController
  {
    private readonly IInput input;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerController"/> class.
    /// </summary>
    /// <param name="input">Input abstraction used to evaluate mapped player actions.</param>
    public PlayerController(IInput input)
    {
      this.input = input;
    }

    /// <summary>
    /// Collects all commands pressed during the current frame.
    /// </summary>
    /// <returns>Ordered list of player commands to dispatch to the domain session.</returns>
    public IReadOnlyList<RunCommand> ReadCommands()
    {
      List<RunCommand> commands = new List<RunCommand>();

      if (input.InputPressed(Input.Left))
      {
        commands.Add(RunCommand.MoveLeft);
      }

      if (input.InputPressed(Input.Right))
      {
        commands.Add(RunCommand.MoveRight);
      }

      if (input.InputPressed(Input.Back))
      {
        commands.Add(RunCommand.TogglePause);
      }

      if (input.InputPressed(Input.Enter))
      {
        commands.Add(RunCommand.Restart);
      }

      return commands;
    }
  }
}