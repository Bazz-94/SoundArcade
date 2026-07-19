namespace SoundArcade.Abstractions
{
  using System;
  using System.Collections.Generic;

  /// <summary>
  /// Represents abstracted in-game input actions that platforms must map to physical keys, buttons or gestures.
  /// </summary>
  public enum Input
  {
    /// <summary>
    /// Navigate up in menus or UI.
    /// </summary>
    Up,

    /// <summary>
    /// Navigate down in menus or UI.
    /// </summary>
    Down,

    /// <summary>
    /// Navigate left in menus or UI or move player left.
    /// </summary>
    Left,

    /// <summary>
    /// Navigate right in menus or UI or move player right.
    /// </summary>
    Right,

    /// <summary>
    /// Confirm / activate (Enter key or A button).
    /// </summary>
    Enter,

    /// <summary>
    /// Back / cancel (Escape key or B button).
    /// </summary>
    Back,

    /// <summary>
    /// Delete the last typed character during text entry (Backspace key).
    /// </summary>
    Backspace
  }

  /// <summary>
  /// Provides generic input polling for gameplay and menus using abstract in-game inputs.
  /// Platforms implement this interface and map physical keys or buttons to the <see cref="Input"/> enum.
  /// </summary>
  public interface IInput
  {
    /// <summary>
    /// Raised when a logical input is pressed this frame. Implementations raise this from
    /// <see cref="InputPressed"/> polls, so the event only fires for inputs a caller polled,
    /// and polling the same input twice in one frame raises it twice.
    /// </summary>
    event EventHandler<InputPressedEventArgs>? Pressed;

    /// <summary>
    /// Returns true when an input transitions to pressed this frame.
    /// A true result also raises <see cref="Pressed"/> as a side effect.
    /// </summary>
    /// <param name="input">Logical input action to query.</param>
    /// <returns>True when the input was pressed this frame.</returns>
    bool InputPressed(Input input);

    /// <summary>
    /// Returns true while an input remains down.
    /// </summary>
    /// <param name="input">Logical input action to query.</param>
    /// <returns>True when the input is currently down.</returns>
    bool InputDown(Input input);

    /// <summary>
    /// Drains and returns the characters typed since the previous call, in typed order.
    /// Each character is returned exactly once; call at most once per frame.
    /// </summary>
    /// <returns>Characters typed this frame, empty when none.</returns>
    IReadOnlyList<char> ReadTypedCharacters();

    /// <summary>
    /// Gets a read-only snapshot of current action-to-key mappings.
    /// </summary>
    /// <returns>Mapping of logical input actions to platform key names.</returns>
    IReadOnlyDictionary<Input, string> GetMappings();

    /// <summary>
    /// Attempts to remap a logical action to a platform key name.
    /// </summary>
    /// <param name="input">Logical action to remap.</param>
    /// <param name="keyName">Platform key name.</param>
    /// <returns>True when remapping succeeds.</returns>
    bool TrySetMapping(Input input, string keyName);

    /// <summary>
    /// Restores the default key mapping.
    /// </summary>
    void ResetMappingsToDefault();

    /// <summary>
    /// Loads key mappings from persistent storage.
    /// </summary>
    void LoadMappings();

    /// <summary>
    /// Saves key mappings to persistent storage.
    /// </summary>
    void SaveMappings();
  }

  /// <summary>
  /// Event data for pressed logical inputs.
  /// </summary>
  public sealed class InputPressedEventArgs : EventArgs
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="InputPressedEventArgs"/> class.
    /// </summary>
    /// <param name="input">Input action pressed this frame.</param>
    public InputPressedEventArgs(Input input)
    {
      this.Input = input;
    }

    /// <summary>
    /// Gets the input action pressed this frame.
    /// </summary>
    public Input Input { get; }
  }
}
