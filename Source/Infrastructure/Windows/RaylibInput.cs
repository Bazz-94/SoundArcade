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
    private const string SettingsFileName = "input-mappings.json";

    private static readonly IReadOnlyDictionary<Input, KeyboardKey> DefaultMappings =
      new Dictionary<Input, KeyboardKey>
      {
        [Input.Up] = KeyboardKey.Up,
        [Input.Down] = KeyboardKey.Down,
        [Input.Left] = KeyboardKey.Left,
        [Input.Right] = KeyboardKey.Right,
        [Input.Enter] = KeyboardKey.Enter,
        [Input.Back] = KeyboardKey.Escape,
        [Input.Backspace] = KeyboardKey.Backspace
      };

    private readonly Dictionary<Input, KeyboardKey> mappings = new Dictionary<Input, KeyboardKey>();
    private readonly string settingsPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="RaylibInput"/> class.
    /// </summary>
    public RaylibInput()
    {
      this.settingsPath = SettingsPaths.GetPath(SettingsFileName);
      this.ResetMappingsToDefault();
      this.LoadMappings();
    }

    /// <inheritdoc />
    public event EventHandler<InputPressedEventArgs>? Pressed;

    /// <inheritdoc />
    public bool InputPressed(Input input)
    {
      bool wasPressed = Raylib.IsKeyPressed(this.GetMappedKey(input));

      if (wasPressed)
      {
        this.Pressed?.Invoke(this, new InputPressedEventArgs(input));
      }

      return wasPressed;
    }

    /// <inheritdoc />
    public bool InputDown(Input input)
    {
      return Raylib.IsKeyDown(this.GetMappedKey(input));
    }

    /// <inheritdoc />
    public IReadOnlyList<char> ReadTypedCharacters()
    {
      List<char> typedCharacters = new List<char>();

      for (int codepoint = Raylib.GetCharPressed(); codepoint > 0; codepoint = Raylib.GetCharPressed())
      {
        typedCharacters.Add((char)codepoint);
      }

      return typedCharacters;
    }

    /// <inheritdoc />
    public IReadOnlyDictionary<Input, string> GetMappings()
    {
      Dictionary<Input, string> snapshot = new Dictionary<Input, string>();

      foreach (KeyValuePair<Input, KeyboardKey> mapping in this.mappings)
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

      if (!parsed || !Enum.IsDefined(parsedKey) || parsedKey == KeyboardKey.Null)
      {
        return false;
      }

      this.mappings[input] = parsedKey;
      return true;
    }

    /// <inheritdoc />
    public void ResetMappingsToDefault()
    {
      this.mappings.Clear();

      foreach (KeyValuePair<Input, KeyboardKey> mapping in DefaultMappings)
      {
        this.mappings[mapping.Key] = mapping.Value;
      }
    }

    /// <inheritdoc />
    public void LoadMappings()
    {
      if (!File.Exists(this.settingsPath))
      {
        return;
      }

      InputMappingsFile? persistedMappings;
      try
      {
        persistedMappings = JsonSerializer.Deserialize<InputMappingsFile>(File.ReadAllText(this.settingsPath));
      }
      catch (JsonException)
      {
        // A corrupt or hand-edited mappings file must not prevent startup; defaults stay active.
        return;
      }

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
          this.mappings[input] = key;
        }
      }
    }

    /// <inheritdoc />
    public void SaveMappings()
    {
      string? settingsDirectoryPath = Path.GetDirectoryName(this.settingsPath);

      if (settingsDirectoryPath is null)
      {
        return;
      }

      Directory.CreateDirectory(settingsDirectoryPath);

      Dictionary<string, string> serializableMappings = new Dictionary<string, string>();

      foreach (KeyValuePair<Input, KeyboardKey> mapping in this.mappings)
      {
        serializableMappings[mapping.Key.ToString()] = mapping.Value.ToString();
      }

      InputMappingsFile payload = new InputMappingsFile
      {
        Mappings = serializableMappings
      };

      File.WriteAllText(this.settingsPath, JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }));
    }

    /// <summary>
    /// Gets the physical key mapped to a logical input.
    /// </summary>
    /// <param name="input">Logical input action.</param>
    /// <returns>The mapped keyboard key.</returns>
    private KeyboardKey GetMappedKey(Input input)
    {
      if (this.mappings.TryGetValue(input, out KeyboardKey key))
      {
        return key;
      }

      if (DefaultMappings.TryGetValue(input, out KeyboardKey defaultKey))
      {
        return defaultKey;
      }

      throw new ArgumentOutOfRangeException(nameof(input));
    }

    /// <summary>
    /// Serialization shape for the persisted key mappings file.
    /// </summary>
    private sealed class InputMappingsFile
    {
      /// <summary>
      /// Gets or sets the persisted action-to-key mappings.
      /// </summary>
      public Dictionary<string, string> Mappings { get; set; } = new Dictionary<string, string>();
    }
  }
}
