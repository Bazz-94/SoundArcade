namespace SoundArcade.Tests.Mock
{
  using System;
  using System.Collections.Generic;
  using SoundArcade.Abstractions;

  /// <summary>
  /// Mock <see cref="IInput"/> test double. Presses queued via <see cref="Press"/> are consumed
  /// by <see cref="InputPressed"/>, which also raises <see cref="Pressed"/>.
  /// </summary>
  public sealed class MockInput : IInput
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
      if (!this.pressed.Contains(input))
      {
        return false;
      }

      this.pressed.Remove(input);
      this.Pressed?.Invoke(this, new InputPressedEventArgs(input));
      return true;
    }

    public bool InputDown(Input input)
    {
      return this.pressed.Contains(input);
    }

    public IReadOnlyDictionary<Input, string> GetMappings()
    {
      return this.mappings;
    }

    public bool TrySetMapping(Input input, string keyName)
    {
      if (string.IsNullOrWhiteSpace(keyName))
      {
        return false;
      }

      this.mappings[input] = keyName;
      return true;
    }

    public void ResetMappingsToDefault()
    {
      this.mappings[Input.Up] = "Up";
      this.mappings[Input.Down] = "Down";
      this.mappings[Input.Left] = "Left";
      this.mappings[Input.Right] = "Right";
      this.mappings[Input.Enter] = "Enter";
      this.mappings[Input.Back] = "Escape";
    }

    public void LoadMappings()
    {
    }

    public void SaveMappings()
    {
    }

    /// <summary>
    /// Queues a press consumed by the next matching <see cref="InputPressed"/> call.
    /// </summary>
    /// <param name="input">Input to press.</param>
    public void Press(Input input)
    {
      this.pressed.Add(input);
    }
  }
}
