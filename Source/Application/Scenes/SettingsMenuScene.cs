namespace SoundArcade.Application.Scenes
{
  using System;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Colors;
  using SoundArcade.Domain.Enums;
  using SoundArcade.Domain.Models;
  using SoundArcade.Domain.Services;

  public class SettingsMenuScene : IScene
  {
    private static readonly float[] VolumeLevels = [0.2f, 0.4f, 0.6f, 0.8f, 1.0f];

    private IInput Input { get; }
    private AppSettings AppSettings { get; }
    private ISettingsStore SettingsStore { get; }
    public ITts Tts { get; }
    private IAudio Audio { get; }
    private SceneManager SceneManager { get; }
    private Menu Menu { get; }

    public SettingsMenuScene(
      IAudio audio,
      AppSettings appSettings,
      ISettingsStore settingsStore,
      IInput input,
      ITts tts,
      IRenderer renderer,
      Theme theme,
      SceneManager sceneManager)
    {
      this.AppSettings = appSettings;
      this.SettingsStore = settingsStore;
      this.Input = input;
      this.Tts = tts;
      this.Audio = audio;
      this.SceneManager = sceneManager;

      this.Menu = new SettingsMenu(
        input,
        tts,
        renderer,
        id: (int)MenuType.Settings,
        controls: [
          new ControlItem(theme, (int)SettingsMenuItem.MasterVolume, "Game Volume", VolumeLevels, appSettings.MasterVolume, this.ApplyMasterVolume),
          new ControlItem(theme, (int)SettingsMenuItem.TtsVolume, "Text to Speech Volume", VolumeLevels, appSettings.TtsVolume, this.ApplyTtsVolume)
        ],
        backItem: new MenuItem(theme, (int)SettingsMenuItem.Back, "Back", this.OnBackSelected),
        theme: theme,
        menuTitle: "Settings Menu");
    }

    public void OnBackSelected()
    {
      this.SceneManager.ChangeScene(SceneType.MainMenu);
    }

    private void PersistSettings()
    {
      this.SettingsStore.Save(this.AppSettings);
      this.Input.SaveMappings();
    }

    private void ApplyMasterVolume(float volume)
    {
      this.AppSettings.MasterVolume = volume;
      this.Audio.SetMasterVolume(volume);

      this.Tts.Stop();
      this.Tts.SpeakAsync($"{(int)MathF.Round(volume * 100.0f)} game volume");
    }

    private void ApplyTtsVolume(float volume)
    {
      this.AppSettings.TtsVolume = volume;
      this.Tts.Stop();
      this.Tts.SetVolume(volume);

      this.Tts.SpeakAsync($"{(int)MathF.Round(volume * 100.0f)} text to speech volume");
    }

    public void OnEnter()
    {
      this.Menu.SelectFirstItem();
    }

    public void OnExit()
    {
      this.PersistSettings();
    }

    public void Update(float deltaTime)
    {
      this.Menu.Update();

      if (this.Input.InputPressed(Abstractions.Input.Back))
      {
        this.OnBackSelected();
      }
    }

    public void Render()
    {
      this.Menu.Render();
    }

    private enum SettingsMenuItem
    {
      MasterVolume,
      TtsVolume,
      ResetDefaults,
      Back
    }
  }
}
