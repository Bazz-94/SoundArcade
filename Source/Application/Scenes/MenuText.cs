namespace SoundArcade.Application.Scenes
{
  /// <summary>
  /// Display and spoken strings for the application menus.
  /// </summary>
  public static class MenuText
  {
    /// <summary>Main menu title.</summary>
    public const string MainMenuTitle = "Sound Arcade";

    /// <summary>Play menu item label.</summary>
    public const string PlayLabel = "Play";

    /// <summary>Settings menu item label.</summary>
    public const string SettingsLabel = "Settings";

    /// <summary>Exit menu item label.</summary>
    public const string ExitLabel = "Exit";

    /// <summary>Game selection menu title.</summary>
    public const string GameSelectionTitle = "Select a Game";

    /// <summary>Back menu item label.</summary>
    public const string BackLabel = "Back";

    /// <summary>Settings menu title.</summary>
    public const string SettingsMenuTitle = "Settings Menu";

    /// <summary>Master volume menu item label.</summary>
    public const string MasterVolumeLabel = "Game Volume";

    /// <summary>Text-to-speech volume menu item label.</summary>
    public const string TtsVolumeLabel = "Text to Speech Volume";

    /// <summary>Spoken announcement format for the master volume; {0} is the percent value.</summary>
    public const string MasterVolumeAnnouncementFormat = "{0} game volume";

    /// <summary>Spoken announcement format for the text-to-speech volume; {0} is the percent value.</summary>
    public const string TtsVolumeAnnouncementFormat = "{0} text to speech volume";
  }
}
