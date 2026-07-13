namespace SoundArcade.Infrastructure.Windows
{
  using System;
  using System.IO;

  /// <summary>
  /// Resolves file paths inside the application's per-user settings directory.
  /// </summary>
  internal static class SettingsPaths
  {
    private const string SettingsDirectoryName = "SoundArcade";

    /// <summary>
    /// Gets the full path of a settings file inside the application data directory.
    /// </summary>
    /// <param name="fileName">Settings file name.</param>
    /// <returns>The full settings file path.</returns>
    public static string GetPath(string fileName)
    {
      return Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        SettingsDirectoryName,
        fileName);
    }
  }
}
