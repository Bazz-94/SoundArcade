namespace SoundArcade.Abstractions
{
  using System.Numerics;

  /// <summary>
  /// Provides primitive rendering operations for simple 3D visualization.
  /// </summary>
  public interface IRenderer
  {
    /// <summary>
    /// Clears the frame background.
    /// </summary>
    /// <param name="color">Background color.</param>
    void Clear(Color color);

    /// <summary>
    /// Draws a box primitive.
    /// </summary>
    /// <param name="center">Box center position.</param>
    /// <param name="size">Box size along each axis.</param>
    /// <param name="color">Box color.</param>
    void DrawBox(Vector3 center, Vector3 size, Color color);

    /// <summary>
    /// Draws a sphere primitive.
    /// </summary>
    /// <param name="center">Sphere center position.</param>
    /// <param name="radius">Sphere radius.</param>
    /// <param name="color">Sphere color.</param>
    void DrawSphere(Vector3 center, float radius, Color color);

    /// <summary>
    /// Draws a line primitive.
    /// </summary>
    /// <param name="start">Line start position.</param>
    /// <param name="end">Line end position.</param>
    /// <param name="color">Line color.</param>
    void DrawLine(Vector3 start, Vector3 end, Color color);

    /// <summary>
    /// Draws a point primitive.
    /// </summary>
    /// <param name="position">Point position.</param>
    /// <param name="color">Point color.</param>
    void DrawPoint(Vector3 position, Color color);

    /// <summary>
    /// Draws text at a world position.
    /// </summary>
    /// <param name="position">Text anchor position.</param>
    /// <param name="text">Text content.</param>
    /// <param name="fontSize">Font size in pixels.</param>
    /// <param name="color">Text color.</param>
    void DrawText(Vector3 position, string text, int fontSize, Color color);
  }
}
