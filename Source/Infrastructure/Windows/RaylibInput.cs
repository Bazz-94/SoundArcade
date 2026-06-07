namespace SoundArcade.Infrastructure.Windows
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Text.Json;
  using Raylib_cs;
  using SoundArcade.Abstractions;

  /// <summary>
  /// Raylib-backed implementation of logical action input with persisted key mappings.
  /// </summary>
  public sealed class RaylibInput : IInput
  {
    private const string SettingsDirectoryName = "SoundArcade";
    private const string SettingsFileName = "input-mappings.json";

    private static readonly IReadOnlyDictionary<Input, KeyboardKey> DefaultMappings =
      new Dictionary<Input, KeyboardKey>
      {
        [Input.Up] = KeyboardKey.Up,
        [Input.Down] = KeyboardKey.Down,
        [Input.Left] = KeyboardKey.Left,
        [Input.Right] = KeyboardKey.Right,
        [Input.Enter] = KeyboardKey.Enter,
        [Input.Back] = KeyboardKey.Escape
      };

    private readonly Dictionary<Input, KeyboardKey> mappings = new Dictionary<Input, KeyboardKey>();
    private readonly string settingsPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="RaylibInput"/> class.
    /// </summary>
    public RaylibInput()
    {
      string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      string settingsDirectoryPath = Path.Combine(appDataPath, SettingsDirectoryName);
      settingsPath = Path.Combine(settingsDirectoryPath, SettingsFileName);

      this.ResetMappingsToDefault();
      this.LoadMappings();
    }

    /// <inheritdoc />
    public event EventHandler<InputPressedEventArgs>? Pressed;

    /// <inheritdoc />
    public bool InputPressed(Input input)
    {
      KeyboardKey key = this.GetMappedKey(input);
      bool wasPressed = Raylib.IsKeyPressed(key);

      if (wasPressed)
      {
        this.Pressed?.Invoke(this, new InputPressedEventArgs(input));
      }

      return wasPressed;
    }

    /// <inheritdoc />
    public bool InputDown(Input input)
    {
      KeyboardKey key = this.GetMappedKey(input);
      return Raylib.IsKeyDown(key);
    }

    /// <inheritdoc />
    public IReadOnlyDictionary<Input, string> GetMappings()
    {
      Dictionary<Input, string> snapshot = new Dictionary<Input, string>();

      foreach (KeyValuePair<Input, KeyboardKey> mapping in mappings)
      {
        snapshot[mapping.Key] = mapping.Value.ToString();
      }

      return snapshot;
    }

    /// <inheritdoc />
    public bool TrySetMapping(Input input, string keyName)
    {
      if (string.IsNullOrWhiteSpace(keyName))
      {
        return false;
      }

      bool parsed = Enum.TryParse(keyName, true, out KeyboardKey parsedKey);

      if (!parsed)
      {
        return false;
      }

      mappings[input] = parsedKey;
      return true;
    }

    /// <inheritdoc />
    public void ResetMappingsToDefault()
    {
      mappings.Clear();

      foreach (KeyValuePair<Input, KeyboardKey> mapping in DefaultMappings)
      {
        mappings[mapping.Key] = mapping.Value;
      }
    }

    /// <inheritdoc />
    public void LoadMappings()
    {
      if (!File.Exists(settingsPath))
      {
        return;
      }

      string json = File.ReadAllText(settingsPath);
      InputMappingsFile? persistedMappings = JsonSerializer.Deserialize<InputMappingsFile>(json);

      if (persistedMappings?.Mappings is null)
      {
        return;
      }

      foreach (KeyValuePair<string, string> mapping in persistedMappings.Mappings)
      {
        bool parsedInput = Enum.TryParse(mapping.Key, true, out Input input);
        bool parsedKey = Enum.TryParse(mapping.Value, true, out KeyboardKey key);

        if (parsedInput && parsedKey)
        {
          mappings[input] = key;
        }
      }
    }

    /// <inheritdoc />
    public void SaveMappings()
    {
      string? settingsDirectoryPath = Path.GetDirectoryName(settingsPath);

      if (settingsDirectoryPath is null)
      {
        return;
      }

      Directory.CreateDirectory(settingsDirectoryPath);

      Dictionary<string, string> serializableMappings = new Dictionary<string, string>();

      foreach (KeyValuePair<Input, KeyboardKey> mapping in mappings)
      {
        serializableMappings[mapping.Key.ToString()] = mapping.Value.ToString();
      }

      InputMappingsFile payload = new InputMappingsFile
      {
        Mappings = serializableMappings
      };

      string json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
      File.WriteAllText(settingsPath, json);
    }

    private KeyboardKey GetMappedKey(Input input)
    {
      if (mappings.TryGetValue(input, out KeyboardKey key))
      {
        return key;
      }

      if (DefaultMappings.TryGetValue(input, out KeyboardKey defaultKey))
      {
        return defaultKey;
      }

      throw new ArgumentOutOfRangeException(nameof(input));
    }

    private sealed class InputMappingsFile
    {
      public Dictionary<string, string> Mappings { get; set; } = new Dictionary<string, string>();
    }
  }
}