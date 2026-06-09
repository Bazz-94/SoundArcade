namespace SoundArcade.Abstractions
{
  /// <summary>
  /// Provides window lifecycle and frame timing operations.
  /// </summary>
  public interface IWindow
  {
    /// <summary>
    /// Gets a value indicating whether the window should close.
    /// </summary>
    bool ShouldClose { get; }

    /// <summary>
    /// Initializes the native window.
    /// </summary>
    /// <param name="width">Window width in pixels.</param>
    /// <param name="height">Window height in pixels.</param>
    /// <param name="title">Window title text.</param>
    void Initialize(int width, int height, string title);

    /// <summary>
    /// Returns the frame delta time in seconds.
    /// </summary>
    /// <returns>Frame delta time in seconds.</returns>
    float GetDeltaTime();

    /// <summary>
    /// Begins frame rendering.
    /// </summary>
    void BeginFrame();

    /// <summary>
    /// Ends frame rendering.
    /// </summary>
    void EndFrame();

    /// <summary>
    /// Closes and releases window resources.
    /// </summary>
    void Close();
  }
}
