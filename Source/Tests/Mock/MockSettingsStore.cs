namespace SoundArcade.Tests.Mock
{
  using SoundArcade.Abstractions;

  /// <summary>
  /// Mock <see cref="ISettingsStore"/> test double that records the last saved settings.
  /// </summary>
  public sealed class MockSettingsStore : ISettingsStore
  {
    public MockSettingsStore(AppSettings settings)
    {
      this.Settings = settings;
    }

    public AppSettings Settings { get; }

    public AppSettings? LastSavedSettings { get; private set; }

    public AppSettings Load()
    {
      return this.Settings;
    }

    public void Save(AppSettings settings)
    {
      this.LastSavedSettings = new AppSettings
      {
        MasterVolume = settings.MasterVolume,
        TtsVolume = settings.TtsVolume
      };
    }
  }
}
