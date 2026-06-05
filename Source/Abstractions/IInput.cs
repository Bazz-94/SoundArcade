namespace SoundArcade.Abstractions;

public interface IInput
{
  bool IsKeyPressed(KeyboardKey key);
  bool IsKeyDown(KeyboardKey key);
}
