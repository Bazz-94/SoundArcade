namespace SoundArcade.Application
{
  using System;
  using System.Collections.Generic;
  using System.Numerics;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain;
  using SoundArcade.Domain.Models;
  using SoundArcade.Domain.RiverRun.Scene;
  using SoundArcade.Domain.Services;

  /// <summary>
  /// Hosts the desktop loop and delegates behavior to reusable scenes.
  /// </summary>
  public sealed class ArcadeShell
  {
    private const int WindowWidth = 1280;
    private const int WindowHeight = 720;
    private const string WindowTitle = "Sound Arcade";
    private const float MinimumVolume = 0.0f;
    private const float MaximumVolume = 1.0f;
    private const int ShortHexLength = 7;
    private const int LongHexLength = 9;

    private static readonly Color BackgroundColor = new Color(Colors.Black);
    private static readonly Color LaneColor = new Color(Colors.Teal);
    private static readonly Color PlayerColor = new Color(Colors.Purple);
    private static readonly Color ObstacleColor = new Color(Colors.Pink);
    private static readonly Color HudLivesColor = new Color(Colors.Pink);
    private static readonly Color HudScoreColor = new Color(Colors.Purple);
    private static readonly Color MenuSelectedColor = new Color(Colors.Pink);
    private static readonly Color MenuUnselectedColor = new Color(Colors.Teal);
    private static readonly Color SettingsValueColor = new Color(Colors.Purple);

    private static readonly float[] VolumeLevels = [0.2f, 0.4f, 0.6f, 0.8f, 1.0f];
    private static readonly string[] MoveLeftOptions = ["Left", "A", "J"];
    private static readonly string[] MoveRightOptions = ["Right", "D", "L"];
    private static readonly string[] ActionOptions = ["Enter", "Space"];
    private static readonly string[] PauseOptions = ["Escape", "Backspace", "P"];

    private readonly IWindow window;
    private readonly IRenderer renderer;
    private readonly IInput input;
    private readonly ITts tts;
    private readonly IAudio audio;
    private readonly ISettingsStore settingsStore;

    private readonly SceneManager sceneManager;

    private bool shouldExit;
    private AppSettings appSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArcadeShell"/> class.
    /// </summary>
    /// <param name="window">Window abstraction.</param>
    /// <param name="renderer">Renderer abstraction.</param>
    /// <param name="input">Input abstraction.</param>
    /// <param name="tts">Text-to-speech abstraction.</param>
    /// <param name="audio">Audio abstraction.</param>
    /// <param name="session">RiverRun session state.</param>
    public ArcadeShell(
      IWindow window,
      IRenderer renderer,
      IInput input,
      ITts tts,
      IAudio audio,
      ISettingsStore settingsStore)
    {
      this.window = window;
      this.renderer = renderer;
      this.input = input;
      this.tts = tts;
      this.audio = audio;
      this.settingsStore = settingsStore;

      sceneManager = new SceneManager();

      appSettings = new AppSettings();
    }

    /// <summary>
    /// Runs the desktop application loop.
    /// </summary>
    public void Run()
    {
      window.Initialize(WindowWidth, WindowHeight, WindowTitle);
      this.LoadSettings();
      sceneManager.ChangeScene(this.CreateMainMenuScene());

      while (!window.ShouldClose && !shouldExit)
      {
        float deltaTime = window.GetDeltaTime();
        sceneManager.Update(deltaTime);

        window.BeginFrame();
        renderer.Clear(BackgroundColor);
        sceneManager.Render();
        window.EndFrame();
      }

      this.PersistSettings();
      window.Close();
    }

    private IReadOnlyDictionary<int, Action<MenuItem>> CreateMainMenuActions()
    {
      Dictionary<int, Action<MenuItem>> actions = new Dictionary<int, Action<MenuItem>>
      {
        [(int)MainMenuItem.StartRun] = this.OnMainMenuStartRunSelected,
        [(int)MainMenuItem.Settings] = this.OnMainMenuSettingsSelected,
        [(int)MainMenuItem.Exit] = this.OnMainMenuExitSelected
      };

      return actions;
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
        [(int)SettingsMenuItem.ResetDefaults] = this.OnSettingsResetDefaultsSelected,
        [(int)SettingsMenuItem.Back] = _ => this.ReturnToMainMenu()
      };

      return actions;
    }

    private MenuScene CreateMainMenuScene()
    {
      return new MenuScene(
        menu: CreateMainMenu(),
        input: input,
        tts: tts,
        renderer: renderer,
        selectedColor: MenuSelectedColor,
        unselectedColor: MenuUnselectedColor,
        menuZ: 4.0f,
        itemActions: this.CreateMainMenuActions());
    }

    private MenuScene CreateSettingsMenuScene()
    {
      return new MenuScene(
        menu: CreateSettingsMenu(),
        input: input,
        tts: tts,
        renderer: renderer,
        selectedColor: MenuSelectedColor,
        unselectedColor: MenuUnselectedColor,
        menuZ: 2.5f,
        itemActions: this.CreateSettingsMenuActions(),
        backAction: this.ReturnToMainMenu,
        afterRender: this.RenderSettingsValues);
    }

    private RiverRunScene CreateRunScene()
    {
      return new RiverRunScene(
        tts: tts,
        audio: audio,
        input: input,
        renderer: renderer,
        onMainMenuRequested: this.ShowMainMenu,
        laneColor: LaneColor,
        playerColor: PlayerColor,
        obstacleColor: ObstacleColor,
        hudLivesColor: HudLivesColor,
        hudScoreColor: HudScoreColor);
    }

    private void OnMainMenuStartRunSelected(MenuItem item)
    {
      sceneManager.ChangeScene(this.CreateRunScene());
    }

    private void OnMainMenuSettingsSelected(MenuItem item)
    {
      sceneManager.ChangeScene(this.CreateSettingsMenuScene());
    }

    private void OnMainMenuExitSelected(MenuItem item)
    {
      shouldExit = true;
    }

    private void OnSettingsMasterVolumeSelected(MenuItem item)
    {
      this.CycleVolume();
    }

    private void OnSettingsMoveLeftSelected(MenuItem item)
    {
      this.CycleMapping(Input.Left, MoveLeftOptions, item.DisplayText);
    }

    private void OnSettingsMoveRightSelected(MenuItem item)
    {
      this.CycleMapping(Input.Right, MoveRightOptions, item.DisplayText);
    }

    private void OnSettingsActionSelected(MenuItem item)
    {
      this.CycleMapping(Input.Enter, ActionOptions, item.DisplayText);
    }

    private void OnSettingsPauseSelected(MenuItem item)
    {
      this.CycleMapping(Input.Back, PauseOptions, item.DisplayText);
    }

    private void OnSettingsResetDefaultsSelected(MenuItem item)
    {
      input.ResetMappingsToDefault();
      input.SaveMappings();
      tts.SpeakAsync("Input mappings reset");
    }

    private void ShowMainMenu()
    {
      sceneManager.ChangeScene(this.CreateMainMenuScene());
    }

    private void ReturnToMainMenu()
    {
      this.PersistSettings(); // TODO: move to onExit of settings scene.
      this.ShowMainMenu();
    }

    private void RenderSettingsValues()
    {
      int volumePercent = (int)MathF.Round(appSettings.MasterVolume * 100.0f);
      renderer.DrawText(new Vector3(3.0f, 3.1f, 2.7f), volumePercent.ToString(), 24, SettingsValueColor);
    }

    private void LoadSettings()
    {
      appSettings = settingsStore.Load();
      appSettings.MasterVolume = Math.Clamp(appSettings.MasterVolume, MinimumVolume, MaximumVolume);
      audio.SetMasterVolume(appSettings.MasterVolume);
      input.LoadMappings();
    }

    private void PersistSettings()
    {
      settingsStore.Save(appSettings);
      input.SaveMappings();
    }

    private void CycleVolume()
    {
      int currentIndex = 0;

      for (int i = 0; i < VolumeLevels.Length; i++)
      {
        if (Math.Abs(VolumeLevels[i] - appSettings.MasterVolume) < 0.001f)
        {
          currentIndex = i;
          break;
        }
      }

      int nextIndex = WrapArrayIndex(currentIndex + 1, VolumeLevels.Length);
      appSettings.MasterVolume = VolumeLevels[nextIndex];
      audio.SetMasterVolume(appSettings.MasterVolume);
      settingsStore.Save(appSettings);

      int percent = (int)MathF.Round(appSettings.MasterVolume * 100.0f);
      tts.SpeakAsync($"Volume {percent}");
    }

    private void CycleMapping(Input inputAction, string[] options, string label)
    {
      IReadOnlyDictionary<Input, string> mappings = input.GetMappings();
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

      if (input.TrySetMapping(inputAction, nextKey))
      {
        input.SaveMappings();
        tts.SpeakAsync($"{label} {nextKey}");
      }
    }

    private static Menu CreateMainMenu()
    {
      return new Menu(
        (int)MenuType.Main,
        "Main menu",
        [
          new MenuItem((int)MainMenuItem.StartRun, "Start Run"),
          new MenuItem((int)MainMenuItem.Settings, "Settings"),
          new MenuItem((int)MainMenuItem.Exit, "Exit")
        ]);
    }

    private static Menu CreateSettingsMenu()
    {
      return new Menu(
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

    private enum MenuType
    {
      Main,
      Settings
    }

    private enum MainMenuItem
    {
      StartRun,
      Settings,
      Exit
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
