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

    /// <summary>
    /// World-space radius of the sphere used to visualize a point.
    /// </summary>
    private const float PointRadius = 0.07f;

    private static readonly Vector3 DefaultCameraPosition = new Vector3(0.0f, 18.0f, -14.0f);
    private static readonly Vector3 DefaultCameraTarget = new Vector3(0.0f, 0.0f, 18.0f);

    private Camera3D camera;

    /// <summary>
    /// Initializes a new instance of the <see cref="RaylibRenderer"/> class.
    /// </summary>
    public RaylibRenderer()
    {
      this.camera = new Camera3D(
        DefaultCameraPosition,
        DefaultCameraTarget,
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
      this.WithCamera(() => Raylib.DrawSphere(position, PointRadius, ToRaylibColor(color)));
    }

    /// <inheritdoc />
    public void DrawText(Vector3 position, string text, int fontSize, Abstractions.Color color)
    {
      Vector2 screenPosition = Raylib.GetWorldToScreen(position, this.camera);
      int textWidth = Raylib.MeasureText(text, fontSize);
      int x = (int)screenPosition.X - (textWidth / 2);
      int y = (int)screenPosition.Y - (fontSize / 2);
      Raylib.DrawText(text, x, y, fontSize, ToRaylibColor(color));
    }

    /// <inheritdoc />
    public void DrawScreenText(int x, int y, string text, int fontSize, Abstractions.Color color)
    {
      Raylib.DrawText(text, x, y, fontSize, ToRaylibColor(color));
    }

    /// <inheritdoc />
    public void DrawScreenTextCentered(int x, int y, string text, int fontSize, Abstractions.Color color)
    {
      int textWidth = Raylib.MeasureText(text, fontSize);
      Raylib.DrawText(text, x - (textWidth / 2), y - (fontSize / 2), fontSize, ToRaylibColor(color));
    }

    /// <inheritdoc />
    public void DrawScreenBox(int x, int y, int width, int height, Abstractions.Color color)
    {
      Raylib.DrawRectangle(x - (width / 2), y - (height / 2), width, height, ToRaylibColor(color));
    }

    /// <inheritdoc />
    public int GetScreenWidth()
    {
      return Raylib.GetScreenWidth();
    }

    /// <inheritdoc />
    public int GetScreenHeight()
    {
      return Raylib.GetScreenHeight();
    }

    /// <inheritdoc />
    public void SetCameraTarget(Vector3 focusPosition)
    {
      this.camera.Position = DefaultCameraPosition + new Vector3(0.0f, 0.0f, focusPosition.Z);
      this.camera.Target = DefaultCameraTarget + new Vector3(0.0f, 0.0f, focusPosition.Z);
    }

    /// <inheritdoc />
    public void ResetCamera()
    {
      this.camera.Position = DefaultCameraPosition;
      this.camera.Target = DefaultCameraTarget;
    }

    /// <summary>
    /// Executes draw work while the fixed camera is active.
    /// </summary>
    /// <param name="drawAction">Draw operation.</param>
    private void WithCamera(Action drawAction)
    {
      Raylib.BeginMode3D(this.camera);
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
