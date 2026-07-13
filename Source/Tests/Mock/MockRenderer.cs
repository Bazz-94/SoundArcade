namespace SoundArcade.Tests.Mock
{
  using System.Numerics;
  using SoundArcade.Abstractions;

  /// <summary>
  /// No-op <see cref="IRenderer"/> test double reporting a fixed 1280x720 screen.
  /// </summary>
  public sealed class MockRenderer : IRenderer
  {
    public void Clear(Color color)
    {
    }

    public void DrawBox(Vector3 center, Vector3 size, Color color)
    {
    }

    public void DrawSphere(Vector3 center, float radius, Color color)
    {
    }

    public void DrawLine(Vector3 start, Vector3 end, Color color)
    {
    }

    public void DrawPoint(Vector3 position, Color color)
    {
    }

    public void DrawText(Vector3 position, string text, int fontSize, Color color)
    {
    }

    public void DrawScreenText(int x, int y, string text, int fontSize, Color color)
    {
    }

    public void DrawScreenTextCentered(int x, int y, string text, int fontSize, Color color)
    {
    }

    public void DrawScreenBox(int x, int y, int width, int height, Color color)
    {
    }

    public int GetScreenWidth()
    {
      return 1280;
    }

    public int GetScreenHeight()
    {
      return 720;
    }

    public void SetCameraTarget(Vector3 focusPosition)
    {
    }

    public void ResetCamera()
    {
    }
  }
}
