namespace SoundArcade.Domain.RiverRun.Game
{
  using System.Collections.Generic;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.RiverRun.Models;
  using SoundArcade.Domain.RiverRun.Services;

  /// <summary>
  /// Coordinates RiverRun input polling and dispatches domain events to audio and TTS services.
  /// </summary>
  public sealed class GameLoop
  {
    private readonly PlayerController playerController;
    private readonly ITts tts;
    private readonly IAudio audio;
    private readonly RiverRunSession session;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameLoop"/> class.
    /// </summary>
    /// <param name="playerController">Player controller that maps input to domain commands.</param>
    /// <param name="tts">Text-to-speech abstraction for spoken feedback.</param>
    /// <param name="audio">Audio abstraction for non-speech cues.</param>
    /// <param name="session">Domain session that contains gameplay state and rules.</param>
    public GameLoop(PlayerController playerController, ITts tts, IAudio audio, RiverRunSession session)
    {
      this.playerController = playerController;
      this.tts = tts;
      this.audio = audio;
      this.session = session;
    }

    /// <summary>
    /// Gets the active RiverRun session.
    /// </summary>
    public RiverRunSession Session => session;

    /// <summary>
    /// Starts a new run and emits initial events.
    /// </summary>
    public void Start()
    {
      this.EmitEvents(session.Start());
      this.UpdateAudioListenerPosition();
    }

    /// <summary>
    /// Processes one frame of input and domain updates.
    /// </summary>
    /// <param name="deltaTimeSeconds">Elapsed frame time in seconds.</param>
    public void Tick(float deltaTimeSeconds)
    {
      IReadOnlyList<RunCommand> commands = playerController.ReadCommands();

      foreach (RunCommand command in commands)
      {
        this.EmitEvents(session.HandleCommand(command));
      }

      this.EmitEvents(session.Update(deltaTimeSeconds));
      this.UpdateAudioListenerPosition();
      audio.Update();
    }

    /// <summary>
    /// Dispatches one explicit command to the domain session.
    /// </summary>
    /// <param name="command">Command to dispatch.</param>
    public void DispatchCommand(RunCommand command)
    {
      this.EmitEvents(session.HandleCommand(command));
      this.UpdateAudioListenerPosition();
      audio.Update();
    }

    private void EmitEvents(IReadOnlyList<RunEvent> events)
    {
      foreach (RunEvent gameEvent in events)
      {
        if (gameEvent is TextToSpeechEvent textToSpeechEvent)
        {
          tts.SpeakAsync(textToSpeechEvent.Text);
        }

        if (gameEvent is PlaySoundEvent playSoundEvent)
        {
          if (playSoundEvent.Position.HasValue)
          {
            audio.PlaySoundAt(playSoundEvent.SoundId, playSoundEvent.Position.Value, playSoundEvent.Volume);
          }
          else
          {
            audio.PlaySound(playSoundEvent.SoundId, playSoundEvent.Volume);
          }
        }
      }
    }

    /// <summary>
    /// Syncs the audio listener with the current player position.
    /// </summary>
    private void UpdateAudioListenerPosition()
    {
      audio.SetListenerPosition(session.Player.Position);
    }
  }
}
