namespace SoundArcade.Abstractions;

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
  Back
}

/// <summary>
/// Provides generic input polling for gameplay and menus using abstract in-game inputs.
/// Platforms implement this interface and map physical keys or buttons to the <see cref="Input"/> enum.
/// </summary>
public interface IInput
{
  /// <summary>
  /// Returns true when an input transitions to pressed this frame.
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
}
