namespace SoundArcade.Infrastructure.Windows
{
  using Raylib_cs;
  using SoundArcade.Abstractions;

  /// <summary>
  /// Raylib-backed implementation of window lifecycle and frame timing.
  /// </summary>
  public sealed class RaylibWindow : IWindow
  {
    private const int TargetFramesPerSecond = 60;

    /// <inheritdoc />
    public bool ShouldClose => Raylib.WindowShouldClose();

    /// <inheritdoc />
    public void Initialize(int width, int height, string title)
    {
      Raylib.InitWindow(width, height, title);
      Raylib.SetTargetFPS(TargetFramesPerSecond);
    }

    /// <inheritdoc />
    public float GetDeltaTime()
    {
      return Raylib.GetFrameTime();
    }

    /// <inheritdoc />
    public void BeginFrame()
    {
      Raylib.BeginDrawing();
    }

    /// <inheritdoc />
    public void EndFrame()
    {
      Raylib.EndDrawing();
    }

    /// <inheritdoc />
    public void Close()
    {
      if (Raylib.IsWindowReady())
      {
        Raylib.CloseWindow();
      }
    }
  }
}