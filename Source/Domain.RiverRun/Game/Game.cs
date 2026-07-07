namespace SoundArcade.Domain.RiverRun.Game
{
  using System.Collections.Generic;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Colors;
  using SoundArcade.Domain.RiverRun.Models;
  using SoundArcade.Domain.RiverRun.Services;

  /// <summary>
  /// Coordinates RiverRun input polling and dispatches domain events to audio and TTS services.
  /// </summary>
  public sealed class Game
  {
    private PlayerController PlayerController { get; }
    private ITts Tts { get; }
    private IAudio Audio { get; }
    public RiverRunSession Session { get; set; }
    private IRenderer Renderer { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Game"/> class.
    /// </summary>
    /// <param name="playerController">Player controller that maps input to domain commands.</param>
    /// <param name="tts">Text-to-speech abstraction for spoken feedback.</param>
    /// <param name="audio">Audio abstraction for non-speech cues.</param>
    /// <param name="session">Domain session that contains gameplay state and rules.</param>
    public Game(IRenderer renderer, ITts tts, IAudio audio, Theme theme, PlayerController playerController)
    {
      this.PlayerController = playerController;
      this.Tts = tts;
      this.Audio = audio;
      this.Session = new RiverRunSession(theme, new RiverRunSettings());
      this.Renderer = renderer;

      //laneColor = theme.Primary;
      //playerColor = theme.Tertiary;
      //obstacleColor = theme.Secondary;
      //hudLivesColor = theme.Accent;
      //hudScoreColor = theme.Accent;
    }

    /// <summary>
    /// Starts a new run and emits initial events.
    /// </summary>
    public void Start()
    {
      this.EmitEvents(this.Session.Start());
      this.UpdateAudioListenerPosition();
    }

    /// <summary>
    /// Processes one frame of input and domain updates.
    /// </summary>
    /// <param name="deltaTimeSeconds">Elapsed frame time in seconds.</param>
    public void Tick(float deltaTimeSeconds)
    {
      IReadOnlyList<RunCommand> commands = this.PlayerController.ReadCommands();

      foreach (RunCommand command in commands)
      {
        this.EmitEvents(this.Session.HandleCommand(command));
      }

      IReadOnlyList<RunEvent> updateEvents = this.Session.Update(deltaTimeSeconds);
      this.UpdateAudioListenerPosition();
      this.EmitEvents(updateEvents);
      this.Audio.Update();
    }

    /// <summary>
    /// Dispatches one explicit command to the domain session.
    /// </summary>
    /// <param name="command">Command to dispatch.</param>
    public void DispatchCommand(RunCommand command)
    {
      this.EmitEvents(this.Session.HandleCommand(command));
      this.UpdateAudioListenerPosition();
      this.Audio.Update();
    }

    private void EmitEvents(IReadOnlyList<RunEvent> events)
    {
      foreach (RunEvent gameEvent in events)
      {
        if (gameEvent is TextToSpeechEvent textToSpeechEvent)
        {
          this.Tts.SpeakAsync(textToSpeechEvent.Text);
        }

        if (gameEvent is PlaySoundEvent playSoundEvent)
        {
          if (playSoundEvent.Position.HasValue)
          {
            this.Audio.PlaySoundAt(playSoundEvent.SoundId, playSoundEvent.Position.Value, playSoundEvent.Volume, playSoundEvent.Pitch);
          }
          else
          {
            this.Audio.PlaySound(playSoundEvent.SoundId, playSoundEvent.Volume, playSoundEvent.Pitch);
          }
        }

        if (gameEvent is StopSoundEvent stopSoundEvent)
        {
          this.Audio.StopSound(stopSoundEvent.SoundId);
        }
      }
    }

    /// <summary>
    /// Syncs the audio listener with the current player position.
    /// </summary>
    private void UpdateAudioListenerPosition()
    {
      this.Audio.SetListenerPosition(this.Session.Player.Position);
    }

    public void Render()
    {
      this.Session.Render(this.Renderer);
    }
  }
}
