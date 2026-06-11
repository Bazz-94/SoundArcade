namespace SoundArcade.Application.Scenes
{
  using System;
  using System.Collections.Generic;
  using System.Numerics;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Models;
  using static SoundArcade.Application.ArcadeShell;

  public class SettingsMenuScene : MenuScene
  {
    private static readonly float[] VolumeLevels = [0.2f, 0.4f, 0.6f, 0.8f, 1.0f];
    private const float ValueTextX = 3.0f;
    private const float MasterVolumeTextY = 3.1f;
    private const float TtsVolumeTextY = 2.2f;
    private const float ValueTextZ = 2.7f;
    private const int ValueFontSize = 24;

    private AppSettings AppSettings { get; set; }
    private ISettingsStore SettingsStore { get; set; }
    private IAudio Audio { get; set; }
    private readonly Action backAction;
    private readonly IReadOnlyDictionary<int, Action<MenuItem>> itemActions;

    private static readonly Menu Menu = new Menu(
      (int)MenuType.Settings,
      "Settings Menu",
      [
        new MenuItem((int)SettingsMenuItem.MasterVolume, "Game Volume"),
        new MenuItem((int)SettingsMenuItem.TtsVolume, "Text to Speech Volume"),
        new MenuItem((int)SettingsMenuItem.Back, "Back")
      ]);

    public SettingsMenuScene(
      IAudio audio,
      AppSettings appSettings,
      ISettingsStore settingsStore,
      IInput input,
      ITts tts,
      IRenderer renderer,
      Color selectedColor,
      Color unselectedColor,
      Color settingsValueColor,
      Action backAction) : base(Menu, input, tts, renderer, selectedColor, unselectedColor)
    {
      this.AppSettings = appSettings;
      this.SettingsStore = settingsStore;
      this.Audio = audio;
      this.backAction = backAction;
      itemActions = this.CreateSettingsMenuActions();
      this.SettingsValueColor = settingsValueColor;
    }

    private Color SettingsValueColor { get; set; }

    protected override IReadOnlyDictionary<int, Action<MenuItem>> GetItemActions()
    {
      return itemActions;
    }

    protected override void OnBackSelected()
    {
      this.PersistSettings();
      backAction();
    }

    protected override void AfterRender()
    {
      RenderSettingsValues(this.Renderer, this.AppSettings, this.SettingsValueColor);
    }

    private IReadOnlyDictionary<int, Action<MenuItem>> CreateSettingsMenuActions()
    {
      Dictionary<int, Action<MenuItem>> actions = new Dictionary<int, Action<MenuItem>>
      {
        [(int)SettingsMenuItem.MasterVolume] = this.OnSettingsMasterVolumeSelected,
        [(int)SettingsMenuItem.TtsVolume] = this.OnSettingsTtsVolumeSelected,
        [(int)SettingsMenuItem.Back] = _ => this.OnBackSelected()
      };

      return actions;
    }

    private void OnSettingsMasterVolumeSelected(MenuItem item)
    {
      this.CycleMasterVolume();
    }

    private void OnSettingsTtsVolumeSelected(MenuItem item)
    {
      this.CycleTtsVolume();
    }

    private static void RenderSettingsValues(IRenderer renderer, AppSettings appSettings, Color settingsValueColor)
    {
      int masterVolumePercent = (int)MathF.Round(appSettings.MasterVolume * 100.0f);
      int ttsVolumePercent = (int)MathF.Round(appSettings.TtsVolume * 100.0f);

      renderer.DrawText(new Vector3(ValueTextX, MasterVolumeTextY, ValueTextZ), masterVolumePercent.ToString(), ValueFontSize, settingsValueColor);
      renderer.DrawText(new Vector3(ValueTextX, TtsVolumeTextY, ValueTextZ), ttsVolumePercent.ToString(), ValueFontSize, settingsValueColor);
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

    private enum SettingsMenuItem
    {
      MasterVolume,
      TtsVolume,
      ResetDefaults,
      Back
    }
  }
}
