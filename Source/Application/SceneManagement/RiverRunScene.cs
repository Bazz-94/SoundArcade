namespace SoundArcade.Application.SceneManagement
{
  using System;
  using System.Numerics;
  using SoundArcade.Abstractions;
  using SoundArcade.Application.Game;
  using SoundArcade.Domain.RiverRun.Models;

  /// <summary>
  /// Gameplay scene for RiverRun run simulation and rendering.
  /// </summary>
  public sealed class RiverRunScene : IScene
  {
    private readonly GameLoop gameLoop;
    private readonly IRenderer renderer;
    private readonly Action onPaused;
    private readonly Color laneColor;
    private readonly Color playerColor;
    private readonly Color obstacleColor;
    private readonly Color hudLivesColor;
    private readonly Color hudScoreColor;

    /// <summary>
    /// Initializes a new instance of the <see cref="RiverRunScene"/> class.
    /// </summary>
    /// <param name="gameLoop">Game loop service.</param>
    /// <param name="renderer">Renderer abstraction.</param>
    /// <param name="onPaused">Callback invoked when run transitions to paused.</param>
    /// <param name="laneColor">Lane color.</param>
    /// <param name="playerColor">Player color.</param>
    /// <param name="obstacleColor">Obstacle color.</param>
    /// <param name="hudLivesColor">HUD lives color.</param>
    /// <param name="hudScoreColor">HUD score color.</param>
    public RiverRunScene(
      GameLoop gameLoop,
      IRenderer renderer,
      Action onPaused,
      Color laneColor,
      Color playerColor,
      Color obstacleColor,
      Color hudLivesColor,
      Color hudScoreColor)
    {
      this.gameLoop = gameLoop;
      this.renderer = renderer;
      this.onPaused = onPaused;
      this.laneColor = laneColor;
      this.playerColor = playerColor;
      this.obstacleColor = obstacleColor;
      this.hudLivesColor = hudLivesColor;
      this.hudScoreColor = hudScoreColor;
    }

    /// <inheritdoc />
    public void OnEnter()
    {
    }

    /// <inheritdoc />
    public void OnExit()
    {
    }

    /// <inheritdoc />
    public void Update(float deltaTime)
    {
      gameLoop.Tick(deltaTime);

      if (gameLoop.Session.State == RunState.Paused)
      {
        onPaused();
      }
    }

    /// <inheritdoc />
    public void Render()
    {
      this.RenderRunWorld();
      this.RenderHud();
    }

    private void RenderRunWorld()
    {
      float laneStartZ = gameLoop.Session.Player.Position.Z - 2.0f;
      float laneEndZ = laneStartZ + 28.0f;

      renderer.DrawLine(new Vector3(RunConstants.LaneX.Left, RunConstants.GroundY, laneStartZ), new Vector3(RunConstants.LaneX.Left, RunConstants.GroundY, laneEndZ), laneColor);
      renderer.DrawLine(new Vector3(RunConstants.LaneX.Center, RunConstants.GroundY, laneStartZ), new Vector3(RunConstants.LaneX.Center, RunConstants.GroundY, laneEndZ), laneColor);
      renderer.DrawLine(new Vector3(RunConstants.LaneX.Right, RunConstants.GroundY, laneStartZ), new Vector3(RunConstants.LaneX.Right, RunConstants.GroundY, laneEndZ), laneColor);

      renderer.DrawSphere(gameLoop.Session.Player.Position, 0.35f, playerColor);

      foreach (RunObstacle obstacle in gameLoop.Session.Obstacles)
      {
        renderer.DrawBox(obstacle.Position, new Vector3(0.6f, 0.6f, 0.6f), obstacleColor);
      }
    }

    private void RenderHud()
    {
      int lives = Math.Max(0, gameLoop.Session.Lives);
      int score = Math.Max(0, gameLoop.Session.Score);

      for (int i = 0; i < lives; i++)
      {
        renderer.DrawSphere(new Vector3(-3.5f + (i * 0.45f), 5.8f, 6.5f), 0.12f, hudLivesColor);
      }

      renderer.DrawText(new Vector3(1.0f, 5.6f, 6.5f), $"Score: {score}", 24, hudScoreColor);
    }
  }
}
