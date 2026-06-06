using System.Collections.Generic;
using SoundArcade.Abstractions;
using SoundArcade.Domain.RiverRun.Models;
using SoundArcade.Domain.RiverRun.Services;

namespace SoundArcade.Application.GameLoop;

/// <summary>
/// Coordinates RiverRun input polling and dispatches domain events to audio and TTS services.
/// </summary>
public sealed class GameLoop
{
  private readonly IInput input;
  private readonly ITts tts;
  private readonly IAudio audio;
  private readonly Session session;

  /// <summary>
  /// Initializes a new instance of the <see cref="GameLoop"/> class.
  /// </summary>
  /// <param name="input">Input abstraction used for player commands.</param>
  /// <param name="tts">Text-to-speech abstraction for spoken feedback.</param>
  /// <param name="audio">Audio abstraction for non-speech cues.</param>
  /// <param name="session">Domain session that contains gameplay state and rules.</param>
  public GameLoop(IInput input, ITts tts, IAudio audio, Session session)
  {
    this.input = input;
    this.tts = tts;
    this.audio = audio;
    this.session = session;
  }

  /// <summary>
  /// Gets the active RiverRun session.
  /// </summary>
  public Session Session => this.session;

  /// <summary>
  /// Starts a new run and emits initial events.
  /// </summary>
  public void StartRun()
  {
    this.EmitEvents(this.session.Start());
  }

  /// <summary>
  /// Processes one frame of input and domain updates.
  /// </summary>
  /// <param name="deltaTimeSeconds">Elapsed frame time in seconds.</param>
  public void Tick(float deltaTimeSeconds)
  {
    if (this.input.InputPressed(Input.Left))
    {
      this.EmitEvents(this.session.HandleCommand(RunCommand.MoveLeft));
    }

    if (this.input.InputPressed(Input.Right))
    {
      this.EmitEvents(this.session.HandleCommand(RunCommand.MoveRight));
    }

    if (this.input.InputPressed(Input.Back))
    {
      this.EmitEvents(this.session.HandleCommand(RunCommand.TogglePause));
    }

    if (this.input.InputPressed(Input.Enter))
    {
      this.EmitEvents(this.session.HandleCommand(RunCommand.Restart));
    }

    this.EmitEvents(this.session.Update(deltaTimeSeconds));
  }

  private void EmitEvents(IReadOnlyList<RunEvent> events)
  {
    foreach (RunEvent gameEvent in events)
    {
      if (gameEvent is TextToSpeechEvent textToSpeechEvent)
      {
        this.tts.SpeakAsync(textToSpeechEvent.Text);
      }

      if (gameEvent is PlaySoundEvent playSoundEvent)
      {
        if (playSoundEvent.Position.HasValue)
        {
          this.audio.PlaySoundAt(playSoundEvent.SoundId, playSoundEvent.Position.Value, playSoundEvent.Volume);
        }
        else
        {
          this.audio.PlaySound(playSoundEvent.SoundId, playSoundEvent.Volume);
        }
      }
    }
  }
}
