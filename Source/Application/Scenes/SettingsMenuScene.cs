namespace SoundArcade.Application.Scenes
{
  using System;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Colors;
  using SoundArcade.Domain.Enums;
  using SoundArcade.Domain.Models;
  using SoundArcade.Domain.Services;

  /// <summary>
  /// Menu for adjusting and persisting application settings.
  /// </summary>
  public sealed class SettingsMenuScene : MenuScene
  {
    private static readonly float[] VolumeLevels = [0.2f, 0.4f, 0.6f, 0.8f, 1.0f];

    private AppSettings AppSettings { get; }
    private ISettingsStore SettingsStore { get; }
    private IInput SceneInput { get; }
    private ITts Tts { get; }
    private IAudio Audio { get; }

    /// <inheritdoc />
    protected override Menu Menu { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsMenuScene"/> class.
    /// </summary>
    /// <param name="audio">Audio abstraction.</param>
    /// <param name="appSettings">Application settings.</param>
    /// <param name="settingsStore">Settings persistence.</param>
    /// <param name="input">Input abstraction.</param>
    /// <param name="tts">Text-to-speech abstraction.</param>
    /// <param name="renderer">Renderer abstraction.</param>
    /// <param name="theme">Theme for colors.</param>
    /// <param name="sceneManager">Scene manager for transitions.</param>
    public SettingsMenuScene(
      IAudio audio,
      AppSettings appSettings,
      ISettingsStore settingsStore,
      IInput input,
      ITts tts,
      IRenderer renderer,
      Theme theme,
      SceneManager sceneManager)
      : base(input, sceneManager)
    {
      this.AppSettings = appSettings;
      this.SettingsStore = settingsStore;
      this.SceneInput = input;
      this.Tts = tts;
      this.Audio = audio;

      this.Menu = new SettingsMenu(
        input,
        tts,
        renderer,
        id: (int)MenuType.Settings,
        controls: [
          new ControlItem(theme, (int)SettingsMenuItem.MasterVolume, MenuText.MasterVolumeLabel, VolumeLevels, appSettings.MasterVolume, this.ApplyMasterVolume),
          new ControlItem(theme, (int)SettingsMenuItem.TtsVolume, MenuText.TtsVolumeLabel, VolumeLevels, appSettings.TtsVolume, this.ApplyTtsVolume)
        ],
        backItem: new MenuItem(theme, (int)SettingsMenuItem.Back, MenuText.BackLabel, this.OnBackSelected),
        theme: theme,
        menuTitle: MenuText.SettingsMenuTitle);
    }

    /// <summary>
    /// Returns to the main menu.
    /// </summary>
    public override void OnBackSelected()
    {
      this.SceneManager.ChangeScene(SceneType.MainMenu);
    }

    /// <summary>
    /// Persists the settings when the scene is left.
    /// </summary>
    public override void OnExit()
    {
      this.SettingsStore.Save(this.AppSettings);
      this.SceneInput.SaveMappings();
    }

    private void ApplyMasterVolume(float volume)
    {
      this.AppSettings.SetMasterVolume(volume);
      this.Audio.SetMasterVolume(volume);

      this.Tts.Stop();
      this.Tts.SpeakAsync(string.Format(MenuText.MasterVolumeAnnouncementFormat, VolumePercent(volume)));
    }

    private void ApplyTtsVolume(float volume)
    {
      this.AppSettings.SetTtsVolume(volume);
      this.Tts.Stop();
      this.Tts.SetVolume(volume);

      this.Tts.SpeakAsync(string.Format(MenuText.TtsVolumeAnnouncementFormat, VolumePercent(volume)));
    }

    private static int VolumePercent(float volume)
    {
      return (int)MathF.Round(volume * 100.0f);
    }

    private enum SettingsMenuItem
    {
      MasterVolume,
      TtsVolume,
      Back
    }
  }
}
