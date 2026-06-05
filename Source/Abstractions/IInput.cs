namespace SoundArcade.Abstractions;

/// <summary>
/// Provides keyboard input polling for gameplay and menus.
/// </summary>
public interface IInput
{
  /// <summary>
  /// Returns true when a key transitions to pressed this frame.
  /// </summary>
  /// <param name="key">Keyboard key to query.</param>
  /// <returns>True when the key was pressed this frame.</returns>
  bool IsKeyPressed(KeyboardKey key);

  /// <summary>
  /// Returns true while a key remains down.
  /// </summary>
  /// <param name="key">Keyboard key to query.</param>
  /// <returns>True when the key is currently down.</returns>
  bool IsKeyDown(KeyboardKey key);
}
