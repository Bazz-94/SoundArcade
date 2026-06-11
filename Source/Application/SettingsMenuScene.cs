namespace SoundArcade.Application
{
  using System;
  using System.Collections.Generic;
  using System.Numerics;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Models;
  using static SoundArcade.Application.ArcadeShell;

  public class SettingsMenuScene : MenuScene
  {
    private const float menuZ = 2.5f;
    private static readonly float[] VolumeLevels = [0.2f, 0.4f, 0.6f, 0.8f, 1.0f];
    private static readonly string[] MoveLeftOptions = ["Left", "A", "J"];
    private static readonly string[] MoveRightOptions = ["Right", "D", "L"];
    private static readonly string[] ActionOptions = ["Enter", "Space"];
    private static readonly string[] PauseOptions = ["Escape", "Backspace", "P"];

    private AppSettings AppSettings { get; set; }
    private ISettingsStore SettingsStore { get; set; }
    private IAudio Audio { get; set; }
    private readonly Action backAction;
    private readonly IReadOnlyDictionary<int, Action<MenuItem>> itemActions;

    private static readonly Menu Menu = new Menu(
      (int)MenuType.Settings,
      "Settings",
      [
        new MenuItem((int)SettingsMenuItem.MasterVolume, "Master Volume"),
        new MenuItem((int)SettingsMenuItem.MoveLeft, "Move Left"),
        new MenuItem((int)SettingsMenuItem.MoveRight, "Move Right"),
        new MenuItem((int)SettingsMenuItem.Action, "Action"),
        new MenuItem((int)SettingsMenuItem.Pause, "Pause"),
        new MenuItem((int)SettingsMenuItem.ResetDefaults, "Reset Defaults"),
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
      Action backAction) : base(Menu, input, tts, renderer, selectedColor, unselectedColor, menuZ)
    {
      this.AppSettings = appSettings;
      this.SettingsStore = settingsStore;
      this.Audio = audio;
      this.backAction = backAction;
      this.itemActions = this.CreateSettingsMenuActions();
      this.SettingsValueColor = settingsValueColor;
    }

    private Color SettingsValueColor { get; set; }

    protected override IReadOnlyDictionary<int, Action<MenuItem>> GetItemActions()
    {
      return this.itemActions;
    }

    protected override void OnBackSelected()
    {
      this.PersistSettings();
      this.backAction();
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
        [(int)SettingsMenuItem.MoveLeft] = this.OnSettingsMoveLeftSelected,
        [(int)SettingsMenuItem.MoveRight] = this.OnSettingsMoveRightSelected,
        [(int)SettingsMenuItem.Action] = this.OnSettingsActionSelected,
        [(int)SettingsMenuItem.Pause] = this.OnSettingsPauseSelected,
        [(int)SettingsMenuItem.ResetDefaults] = _ => OnSettingsResetDefaultsSelected(this.Input, this.Tts),
        [(int)SettingsMenuItem.Back] = _ => this.OnBackSelected()
      };

      return actions;
    }

    private void OnSettingsMasterVolumeSelected(MenuItem item)
    {
      this.CycleVolume();
    }

    private void OnSettingsMoveLeftSelected(MenuItem item)
    {
      this.CycleMapping(Abstractions.Input.Left, MoveLeftOptions, item.DisplayText);
    }

    private void OnSettingsMoveRightSelected(MenuItem item)
    {
      this.CycleMapping(Abstractions.Input.Right, MoveRightOptions, item.DisplayText);
    }

    private void OnSettingsActionSelected(MenuItem item)
    {
      this.CycleMapping(Abstractions.Input.Enter, ActionOptions, item.DisplayText);
    }

    private void OnSettingsPauseSelected(MenuItem item)
    {
      this.CycleMapping(Abstractions.Input.Back, PauseOptions, item.DisplayText);
    }

    private static void RenderSettingsValues(IRenderer renderer, AppSettings appSettings, Color settingsValueColor)
    {
      int volumePercent = (int)MathF.Round(appSettings.MasterVolume * 100.0f);
      renderer.DrawText(new Vector3(3.0f, 3.1f, 2.7f), volumePercent.ToString(), 24, settingsValueColor);
    }

    private void PersistSettings()
    {
      this.SettingsStore.Save(this.AppSettings);
      this.Input.SaveMappings();
    }

    private void CycleVolume()
    {
      int currentIndex = 0;

      for (int i = 0; i < VolumeLevels.Length; i++)
      {
        if (Math.Abs(VolumeLevels[i] - this.AppSettings.MasterVolume) < 0.001f)
        {
          currentIndex = i;
          break;
        }
      }

      int nextIndex = WrapArrayIndex(currentIndex + 1, VolumeLevels.Length);
      this.AppSettings.MasterVolume = VolumeLevels[nextIndex];
      this.Audio.SetMasterVolume(this.AppSettings.MasterVolume);
      this.SettingsStore.Save(this.AppSettings);

      int percent = (int)MathF.Round(this.AppSettings.MasterVolume * 100.0f);
      this.Tts.SpeakAsync($"Volume {percent}");
    }

    private void CycleMapping(Input inputAction, string[] options, string label)
    {
      IReadOnlyDictionary<Input, string> mappings = this.Input.GetMappings();
      string current = mappings.TryGetValue(inputAction, out string? keyName) ? keyName : options[0];
      int currentIndex = 0;

      for (int i = 0; i < options.Length; i++)
      {
        if (string.Equals(options[i], current, StringComparison.OrdinalIgnoreCase))
        {
          currentIndex = i;
          break;
        }
      }

      int nextIndex = WrapArrayIndex(currentIndex + 1, options.Length);
      string nextKey = options[nextIndex];

      if (this.Input.TrySetMapping(inputAction, nextKey))
      {
        this.Input.SaveMappings();
        this.Tts.SpeakAsync($"{label} {nextKey}");
      }
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

    private static void OnSettingsResetDefaultsSelected(IInput input, ITts tts)
    {
      input.ResetMappingsToDefault();
      input.SaveMappings();
      tts.SpeakAsync("Input mappings reset");
    }

    private enum SettingsMenuItem
    {
      MasterVolume,
      MoveLeft,
      MoveRight,
      Action,
      Pause,
      ResetDefaults,
      Back
    }
  }
}
