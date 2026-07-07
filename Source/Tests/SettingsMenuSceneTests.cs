namespace SoundArcade.Tests
{
  using System;
  using System.Collections.Generic;
  using System.Numerics;
  using SoundArcade.Abstractions;
  using SoundArcade.Application.Scenes;
  using SoundArcade.Domain.Colors;
  using SoundArcade.Domain.Services;
  using Xunit;

  /// <summary>
  /// Tests for the settings menu scene.
  /// </summary>
  public sealed class SettingsMenuSceneTests
  {
    /// <summary>
    /// Verifies the TTS volume menu item updates and persists the speech volume.
    /// </summary>
    [Fact]
    public void Selecting_tts_volume_cycles_and_applies_speech_volume()
    {
      FakeAudio audio = new FakeAudio();
      FakeSettingsStore settingsStore = new FakeSettingsStore(new AppSettings
      {
        MasterVolume = 0.4f,
        TtsVolume = 0.4f
      });
      FakeInput input = new FakeInput();
      FakeTts tts = new FakeTts();
      FakeRenderer renderer = new FakeRenderer();
      SettingsMenuScene scene = new SettingsMenuScene(
        audio: audio,
        appSettings: settingsStore.Settings,
        settingsStore: settingsStore,
        input: input,
        tts: tts,
        renderer: renderer,
        theme: new Theme(),
        sceneManager: new SceneManager());

      scene.OnEnter();

      input.Press(Input.Down);
      scene.Update(0.016f);

      input.Press(Input.Enter);
      scene.Update(0.016f);

      Assert.Equal(0.6f, settingsStore.Settings.TtsVolume, precision: 3);
      Assert.Equal(0.6f, tts.Volume, precision: 3);
      Assert.Equal("60 text to speech volume", tts.LastSpokenText);
      Assert.Equal(1.0f, audio.MasterVolume, precision: 3);
      Assert.Equal(0.6f, settingsStore.LastSavedSettings!.TtsVolume, precision: 3);
    }

    private sealed class FakeAudio : IAudio
    {
      public float MasterVolume { get; private set; } = 1.0f;

      public void Update()
      {
      }

      public void PlaySound(string soundId, float volume, float pitch = 1.0f)
      {
      }

      public void PlaySoundAt(string soundId, Vector3 position, float volume, float pitch = 1.0f)
      {
      }

      public void StopSound(string soundId)
      {
      }

      public void SetListenerPosition(Vector3 position)
      {
      }

      public void SetMasterVolume(float volume)
      {
        this.MasterVolume = volume;
      }

      public void PlayMusic(string musicId, bool loop)
      {
      }

      public void PauseMusic()
      {
      }

      public void ResumeMusic()
      {
      }

      public void StopMusic()
      {
      }

      public void SetMusicVolume(float volume)
      {
      }

      public void RegisterSound(string soundId, string assetPath, float gain = 1.0f)
      {
      }
    }

    private sealed class FakeSettingsStore : ISettingsStore
    {
      public FakeSettingsStore(AppSettings settings)
      {
        this.Settings = settings;
      }

      public AppSettings Settings { get; }

      public AppSettings? LastSavedSettings { get; private set; }

      public AppSettings Load()
      {
        return this.Settings;
      }

      public void Save(AppSettings settings)
      {
        this.LastSavedSettings = new AppSettings
        {
          MasterVolume = settings.MasterVolume,
          TtsVolume = settings.TtsVolume
        };
      }
    }

    private sealed class FakeInput : IInput
    {
      private readonly HashSet<Input> pressed = new HashSet<Input>();

      public event EventHandler<InputPressedEventArgs>? Pressed
      {
        add
        {
        }

        remove
        {
        }
      }

      public bool InputPressed(Input input)
      {
        if (!pressed.Contains(input))
        {
          return false;
        }

        pressed.Remove(input);
        return true;
      }

      public bool InputDown(Input input)
      {
        return false;
      }

      public IReadOnlyDictionary<Input, string> GetMappings()
      {
        return new Dictionary<Input, string>();
      }

      public bool TrySetMapping(Input input, string keyName)
      {
        return false;
      }

      public void ResetMappingsToDefault()
      {
      }

      public void LoadMappings()
      {
      }

      public void SaveMappings()
      {
      }

      public void Press(Input input)
      {
        pressed.Add(input);
      }
    }

    private sealed class FakeRenderer : IRenderer
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

      public void SetCameraTarget(Vector3 focusPosition)
      {
      }

      public void ResetCamera()
      {
      }
    }

    private sealed class FakeTts : ITts
    {
      public event EventHandler? SpeakStarted
      {
        add
        {
        }

        remove
        {
        }
      }

      public event EventHandler? SpeakCompleted
      {
        add
        {
        }

        remove
        {
        }
      }

      public float Volume { get; private set; } = 1.0f;

      public string? LastSpokenText { get; private set; }

      public void Speak(string text)
      {
        this.LastSpokenText = text;
      }

      public void SpeakAsync(string text)
      {
        this.LastSpokenText = text;
      }

      public void Stop()
      {
      }

      public void SetVolume(float volume)
      {
        this.Volume = volume;
      }
    }
  }
}