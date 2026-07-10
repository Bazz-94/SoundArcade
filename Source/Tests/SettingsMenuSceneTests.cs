namespace SoundArcade.Tests
{
  using SoundArcade.Abstractions;
  using SoundArcade.Application.Scenes;
  using SoundArcade.Domain.Colors;
  using SoundArcade.Domain.Services;
  using SoundArcade.Tests.Mock;
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
      MockAudio audio = new MockAudio();
      MockSettingsStore settingsStore = new MockSettingsStore(new AppSettings
      {
        MasterVolume = 0.4f,
        TtsVolume = 0.4f
      });
      MockInput input = new MockInput();
      MockTts tts = new MockTts();
      MockRenderer renderer = new MockRenderer();
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
      Assert.Null(settingsStore.LastSavedSettings);

      scene.OnExit();

      Assert.Equal(0.6f, settingsStore.LastSavedSettings!.TtsVolume, precision: 3);
    }

    /// <summary>
    /// Verifies left and right arrows cycle the selected volume control down and up.
    /// </summary>
    [Fact]
    public void Left_and_right_arrows_cycle_selected_volume_control()
    {
      MockAudio audio = new MockAudio();
      MockSettingsStore settingsStore = new MockSettingsStore(new AppSettings
      {
        MasterVolume = 0.4f,
        TtsVolume = 0.4f
      });
      MockInput input = new MockInput();
      SettingsMenuScene scene = new SettingsMenuScene(
        audio: audio,
        appSettings: settingsStore.Settings,
        settingsStore: settingsStore,
        input: input,
        tts: new MockTts(),
        renderer: new MockRenderer(),
        theme: new Theme(),
        sceneManager: new SceneManager());

      scene.OnEnter();

      input.Press(Input.Right);
      scene.Update(0.016f);

      Assert.Equal(0.6f, settingsStore.Settings.MasterVolume, precision: 3);
      Assert.Equal(0.6f, audio.MasterVolume, precision: 3);

      input.Press(Input.Left);
      scene.Update(0.016f);

      Assert.Equal(0.4f, settingsStore.Settings.MasterVolume, precision: 3);
      Assert.Equal(0.4f, audio.MasterVolume, precision: 3);
    }
  }
}
