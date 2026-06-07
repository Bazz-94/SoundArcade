namespace SoundArcade.Application.SceneManagement
{
  using System;
  using System.Collections.Generic;
  using System.Numerics;
  using SoundArcade.Abstractions;
  using SoundArcade.Application.Game;
  using SoundArcade.Domain;
  using SoundArcade.Domain.RiverRun.Models;

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
    private readonly GameLoop gameLoop;

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
    /// <param name="settingsStore">Settings persistence abstraction.</param>
    /// <param name="gameLoop">RiverRun game loop.</param>
    public ArcadeShell(
      IWindow window,
      IRenderer renderer,
      IInput input,
      ITts tts,
      IAudio audio,
      ISettingsStore settingsStore,
      GameLoop gameLoop)
    {
      this.window = window;
      this.renderer = renderer;
      this.input = input;
      this.tts = tts;
      this.audio = audio;
      this.settingsStore = settingsStore;
      this.gameLoop = gameLoop;

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

    private IReadOnlyDictionary<int, Action<MenuItem>> CreatePauseMenuActions()
    {
      Dictionary<int, Action<MenuItem>> actions = new Dictionary<int, Action<MenuItem>>
      {
        [(int)PauseMenuItem.Resume] = this.OnPauseMenuResumeSelected,
        [(int)PauseMenuItem.MainMenu] = this.OnPauseMenuMainMenuSelected
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
      Menu mainMenu = CreateMainMenu();

      return new MenuScene(
        mainMenu,
        input,
        tts,
        renderer,
        MenuSelectedColor,
        MenuUnselectedColor,
        4.0f,
        this.CreateMainMenuActions());
    }

    private MenuScene CreatePauseMenuScene()
    {
      Menu pauseMenu = CreatePauseMenu();

      return new MenuScene(
        pauseMenu,
        input,
        tts,
        renderer,
        MenuSelectedColor,
        MenuUnselectedColor,
        1.5f,
        this.CreatePauseMenuActions(),
        this.ResumeRunFromPause);
    }

    private MenuScene CreateSettingsMenuScene()
    {
      Menu settingsMenu = CreateSettingsMenu();

      return new MenuScene(
        settingsMenu,
        input,
        tts,
        renderer,
        MenuSelectedColor,
        MenuUnselectedColor,
        2.5f,
        this.CreateSettingsMenuActions(),
        this.ReturnToMainMenu,
        this.RenderSettingsValues);
    }

    private RiverRunScene CreateRunScene()
    {
      return new RiverRunScene(
        gameLoop,
        renderer,
        this.ShowPauseMenu,
        LaneColor,
        PlayerColor,
        ObstacleColor,
        HudLivesColor,
        HudScoreColor);
    }

    private void OnMainMenuStartRunSelected(MenuItem item)
    {
      gameLoop.StartRun();
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

    private void OnPauseMenuResumeSelected(MenuItem item)
    {
      this.ResumeRunFromPause();
    }

    private void OnPauseMenuMainMenuSelected(MenuItem item)
    {
      sceneManager.ChangeScene(this.CreateMainMenuScene());
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

    private void ShowPauseMenu()
    {
      sceneManager.ChangeScene(this.CreatePauseMenuScene());
    }

    private void ResumeRunFromPause()
    {
      gameLoop.DispatchCommand(RunCommand.TogglePause);
      sceneManager.ChangeScene(this.CreateRunScene());
    }

    private void ShowMainMenu()
    {
      sceneManager.ChangeScene(this.CreateMainMenuScene());
    }

    private void ReturnToMainMenu()
    {
      this.PersistSettings();
      this.ShowMainMenu();
    }

    private void ReturnToPauseMenu()
    {
      this.PersistSettings();
      sceneManager.ChangeScene(this.CreatePauseMenuScene());
    }

    private void RenderSettingsValues()
    {
      int volumePercent = (int)MathF.Round(appSettings.MasterVolume * 100.0f);
      this.DrawNumber(volumePercent, new Vector3(3.0f, 3.1f, 2.7f), 0.19f, SettingsValueColor);
    }

    private void DrawNumber(int value, Vector3 origin, float scale, Color color)
    {
      string text = value.ToString();

      for (int index = 0; index < text.Length; index++)
      {
        this.DrawDigit(text[index], new Vector3(origin.X + (index * scale * 1.5f), origin.Y, origin.Z), scale, color);
      }
    }

    private void DrawDigit(char digit, Vector3 origin, float scale, Color color)
    {
      bool[] segments = digit switch
      {
        '0' => [true, true, true, true, true, true, false],
        '1' => [false, true, true, false, false, false, false],
        '2' => [true, true, false, true, true, false, true],
        '3' => [true, true, true, true, false, false, true],
        '4' => [false, true, true, false, false, true, true],
        '5' => [true, false, true, true, false, true, true],
        '6' => [true, false, true, true, true, true, true],
        '7' => [true, true, true, false, false, false, false],
        '8' => [true, true, true, true, true, true, true],
        '9' => [true, true, true, true, false, true, true],
        _ => [false, false, false, false, false, false, false]
      };

      Vector3 topLeft = new Vector3(origin.X, origin.Y, origin.Z);
      Vector3 topRight = new Vector3(origin.X + scale, origin.Y, origin.Z);
      Vector3 midLeft = new Vector3(origin.X, origin.Y - scale, origin.Z);
      Vector3 midRight = new Vector3(origin.X + scale, origin.Y - scale, origin.Z);
      Vector3 bottomLeft = new Vector3(origin.X, origin.Y - (scale * 2.0f), origin.Z);
      Vector3 bottomRight = new Vector3(origin.X + scale, origin.Y - (scale * 2.0f), origin.Z);

      if (segments[0])
      {
        renderer.DrawLine(topLeft, topRight, color);
      }

      if (segments[1])
      {
        renderer.DrawLine(topRight, midRight, color);
      }

      if (segments[2])
      {
        renderer.DrawLine(midRight, bottomRight, color);
      }

      if (segments[3])
      {
        renderer.DrawLine(bottomLeft, bottomRight, color);
      }

      if (segments[4])
      {
        renderer.DrawLine(midLeft, bottomLeft, color);
      }

      if (segments[5])
      {
        renderer.DrawLine(topLeft, midLeft, color);
      }

      if (segments[6])
      {
        renderer.DrawLine(midLeft, midRight, color);
      }
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

    private static Menu CreatePauseMenu()
    {
      return new Menu(
        (int)MenuType.Pause,
        "Pause menu",
        [
          new MenuItem((int)PauseMenuItem.Resume, "Resume"),
          new MenuItem((int)PauseMenuItem.MainMenu, "Main Menu")
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

    private static Color ToColor(string hex)
    {
      if (string.IsNullOrWhiteSpace(hex))
      {
        throw new ArgumentException("Hex color cannot be empty.", nameof(hex));
      }

      if (!hex.StartsWith("#", StringComparison.Ordinal))
      {
        throw new ArgumentException("Hex color must start with '#'.", nameof(hex));
      }

      if (hex.Length != ShortHexLength && hex.Length != LongHexLength)
      {
        throw new ArgumentException("Hex color must be in #RRGGBB or #RRGGBBAA format.", nameof(hex));
      }

      byte red = Convert.ToByte(hex.Substring(1, 2), 16);
      byte green = Convert.ToByte(hex.Substring(3, 2), 16);
      byte blue = Convert.ToByte(hex.Substring(5, 2), 16);
      byte alpha = 255;

      if (hex.Length == LongHexLength)
      {
        alpha = Convert.ToByte(hex.Substring(7, 2), 16);
      }

      return new Color(red, green, blue, alpha);
    }

    private enum MenuType
    {
      Main,
      Pause,
      Settings
    }

    private enum MainMenuItem
    {
      StartRun,
      Settings,
      Exit
    }

    private enum PauseMenuItem
    {
      Resume,
      MainMenu
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
