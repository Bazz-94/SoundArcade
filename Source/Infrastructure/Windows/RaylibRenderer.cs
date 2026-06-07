namespace SoundArcade.Infrastructure.Windows
{
  using System;
  using System.Numerics;
  using Raylib_cs;
  using SoundArcade.Abstractions;
  using Color = Raylib_cs.Color;

  /// <summary>
  /// Raylib-backed implementation of primitive rendering.
  /// </summary>
  public sealed class RaylibRenderer : IRenderer
  {
    private const float CameraFov = 20.0f;

    private readonly Camera3D camera;

    /// <summary>
    /// Initializes a new instance of the <see cref="RaylibRenderer"/> class.
    /// </summary>
    public RaylibRenderer()
    {
      camera = new Camera3D(
        new Vector3(0.0f, 18.0f, -14.0f),
        new Vector3(0.0f, 0.0f, 18.0f),
        new Vector3(0.0f, 1.0f, 0.0f),
        CameraFov,
        CameraProjection.Orthographic);
    }

    /// <inheritdoc />
    public void Clear(Abstractions.Color color)
    {
      Raylib.ClearBackground(ToRaylibColor(color));
    }

    /// <inheritdoc />
    public void DrawBox(Vector3 center, Vector3 size, Abstractions.Color color)
    {
      this.WithCamera(() => Raylib.DrawCube(center, size.X, size.Y, size.Z, ToRaylibColor(color)));
    }

    /// <inheritdoc />
    public void DrawSphere(Vector3 center, float radius, Abstractions.Color color)
    {
      this.WithCamera(() => Raylib.DrawSphere(center, radius, ToRaylibColor(color)));
    }

    /// <inheritdoc />
    public void DrawLine(Vector3 start, Vector3 end, Abstractions.Color color)
    {
      this.WithCamera(() => Raylib.DrawLine3D(start, end, ToRaylibColor(color)));
    }

    /// <inheritdoc />
    public void DrawPoint(Vector3 position, Abstractions.Color color)
    {
      this.WithCamera(() => Raylib.DrawSphere(position, 0.07f, ToRaylibColor(color)));
    }

    /// <inheritdoc />
    public void DrawText(Vector3 position, string text, int fontSize, Abstractions.Color color)
    {
      this.WithCamera(() =>
      {
        Vector2 screenPosition = Raylib.GetWorldToScreen(position, camera);
        int textWidth = Raylib.MeasureText(text, fontSize);
        int x = (int)screenPosition.X - (textWidth / 2);
        int y = (int)screenPosition.Y - (fontSize / 2);
        Raylib.DrawText(text, x, y, fontSize, ToRaylibColor(color));
      });
    }

    /// <summary>
    /// Executes draw work while the fixed camera is active.
    /// </summary>
    /// <param name="drawAction">Draw operation.</param>
    private void WithCamera(Action drawAction)
    {
      Raylib.BeginMode3D(camera);
      drawAction();
      Raylib.EndMode3D();
    }

    /// <summary>
    /// Converts an abstraction color into a Raylib color.
    /// </summary>
    /// <param name="color">Color to convert.</param>
    /// <returns>Converted color.</returns>
    private static Color ToRaylibColor(Abstractions.Color color)
    {
      return new Color(color.R, color.G, color.B, color.A);
    }
  }
}