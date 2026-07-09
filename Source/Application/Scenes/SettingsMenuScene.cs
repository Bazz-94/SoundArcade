namespace SoundArcade.Application.Scenes
{
  using System;
  using System.Numerics;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Colors;
  using SoundArcade.Domain.Enums;
  using SoundArcade.Domain.Models;
  using SoundArcade.Domain.Services;

  public class SettingsMenuScene : IScene
  {
    private static readonly float[] VolumeLevels = [0.2f, 0.4f, 0.6f, 0.8f, 1.0f];
    private const float ValueTextX = -6.0f;
    private const float ValueTextZ = 2.7f;
    private const int ValueFontSize = 24;

    private IInput Input { get; }
    private AppSettings AppSettings { get; }
    private ISettingsStore SettingsStore { get; }
    public ITts Tts { get; }
    public IRenderer Renderer { get; }
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
      this.Renderer = renderer;
      this.Audio = audio;
      this.SceneManager = sceneManager;
      this.SettingsValueColor = theme.ColorPalette.Accent;

      this.Menu = new Menu(
      input,
      tts,
      renderer,
      theme: theme,
      id: (int)MenuType.Settings,
      menuTitle: "Settings Menu",
      items: [
        new MenuItem(theme, (int)SettingsMenuItem.MasterVolume, "Game Volume", this.CycleMasterVolume),
        new MenuItem(theme, (int)SettingsMenuItem.TtsVolume, "Text to Speech Volume", this.CycleTtsVolume),
        new MenuItem(theme, (int)SettingsMenuItem.Back, "Back", this.OnBackSelected)
      ]);
    }

    private Color SettingsValueColor { get; set; }

    public void OnBackSelected()
    {
      this.PersistSettings();
      this.SceneManager.ChangeScene(SceneType.MainMenu);
    }

    private void RenderSettingsValues(IRenderer renderer, AppSettings appSettings, Color settingsValueColor)
    {
      int masterVolumePercent = (int)MathF.Round(appSettings.MasterVolume * 100.0f);
      int ttsVolumePercent = (int)MathF.Round(appSettings.TtsVolume * 100.0f);

      renderer.DrawText(new Vector3(ValueTextX, this.Menu.Items[(int)SettingsMenuItem.MasterVolume].Position.Y, ValueTextZ), masterVolumePercent.ToString(), ValueFontSize, settingsValueColor);
      renderer.DrawText(new Vector3(ValueTextX, this.Menu.Items[(int)SettingsMenuItem.TtsVolume].Position.Y, ValueTextZ), ttsVolumePercent.ToString(), ValueFontSize, settingsValueColor);
    }

    private void PersistSettings()
    {
      this.SettingsStore.Save(this.AppSettings);
      this.Input.SaveMappings();
    }

    private void CycleMasterVolume()
    {
      this.AppSettings.MasterVolume = CycleVolumeLevel(this.AppSettings.MasterVolume);
      this.Audio.SetMasterVolume(this.AppSettings.MasterVolume);
      this.SettingsStore.Save(this.AppSettings);

      int percent = (int)MathF.Round(this.AppSettings.MasterVolume * 100.0f);
      this.Tts.Stop();
      this.Tts.SpeakAsync($"{percent} game volume");
    }

    private void CycleTtsVolume()
    {
      this.AppSettings.TtsVolume = CycleVolumeLevel(this.AppSettings.TtsVolume);
      this.Tts.Stop();
      this.Tts.SetVolume(this.AppSettings.TtsVolume);
      this.SettingsStore.Save(this.AppSettings);

      int percent = (int)MathF.Round(this.AppSettings.TtsVolume * 100.0f);
      this.Tts.Stop();
      this.Tts.SpeakAsync($"{percent} text to speech volume");
    }

    private static float CycleVolumeLevel(float currentVolume)
    {
      int currentIndex = 0;

      for (int i = 0; i < VolumeLevels.Length; i++)
      {
        if (Math.Abs(VolumeLevels[i] - currentVolume) < 0.001f)
        {
          currentIndex = i;
          break;
        }
      }

      int nextIndex = WrapArrayIndex(currentIndex + 1, VolumeLevels.Length);
      return VolumeLevels[nextIndex];
    }

    private static int WrapArrayIndex(int index, int length)
    {
      if (length <= 0)
      {
        return 0;
      }

      int wrapped = index % length;

      if (wrapped < 0)
      {
        wrapped += length;
      }

      return wrapped;
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
    }

    public void Render()
    {
      this.Menu.Render();
      this.RenderSettingsValues(this.Renderer, this.AppSettings, this.SettingsValueColor);
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
