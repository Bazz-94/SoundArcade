namespace SoundArcade.Abstractions;

public interface IWindow
{
  bool ShouldClose { get; }

  void Initialize(int width, int height, string title);
  float GetDeltaTime();
  void BeginFrame();
  void EndFrame();
  void Close();
}
