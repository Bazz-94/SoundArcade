namespace SoundArcade.Domain.Models
{
  using System;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Colors;

  /// <summary>
  /// Outcome of a <see cref="NameEntry"/> update.
  /// </summary>
  public enum NameEntryStatus
  {
    /// <summary>
    /// Entry is still in progress.
    /// </summary>
    Editing,

    /// <summary>
    /// A non-empty name was confirmed; read it from <see cref="NameEntry.Name"/>.
    /// </summary>
    Confirmed,

    /// <summary>
    /// Entry was cancelled or confirmed empty; no name should be used.
    /// </summary>
    Cancelled
  }

  /// <summary>
  /// Text-entry component for player names: letters, digits and spaces only,
  /// capped at twelve characters, every edit echoed via text-to-speech.
  /// </summary>
  public sealed class NameEntry : UIComponent
  {
    private const int MaxLength = 12;
    private const string SpaceEcho = "space";
    private const string DeletedEchoFormat = "{0} deleted";
    private const int PromptOffsetY = 80;
    private const int CenterY = 350;

    private readonly IInput input;
    private readonly ITts tts;
    private readonly IRenderer renderer;
    private readonly Theme theme;

    /// <summary>
    /// Initializes a new instance of the <see cref="NameEntry"/> class.
    /// </summary>
    /// <param name="input">Input abstraction.</param>
    /// <param name="tts">Text-to-speech abstraction.</param>
    /// <param name="renderer">Renderer abstraction.</param>
    /// <param name="theme">Theme for colors.</param>
    /// <param name="id">Stable component identifier.</param>
    /// <param name="prompt">Prompt announced when the component opens.</param>
    public NameEntry(IInput input, ITts tts, IRenderer renderer, Theme theme, int id, string prompt)
      : base(theme.ColorPalette.Tertiary, id, prompt)
    {
      ArgumentNullException.ThrowIfNull(input);
      ArgumentNullException.ThrowIfNull(tts);
      ArgumentNullException.ThrowIfNull(renderer);

      this.input = input;
      this.tts = tts;
      this.renderer = renderer;
      this.theme = theme;
    }

    /// <summary>
    /// Gets the raw entered text, untrimmed.
    /// </summary>
    public string Text { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the trimmed name; empty when nothing usable was entered.
    /// </summary>
    public string Name => this.Text.Trim();

    /// <summary>
    /// Clears the text and announces the prompt.
    /// </summary>
    public void Open()
    {
      this.Text = string.Empty;
      this.tts.Stop();
      this.tts.SpeakAsync(this.DisplayText);
    }

    /// <summary>
    /// Handles typing, backspace, confirm and cancel for the current frame.
    /// </summary>
    /// <returns>The entry status after this frame's input.</returns>
    public NameEntryStatus Update()
    {
      foreach (char character in this.input.ReadTypedCharacters())
      {
        this.Append(character);
      }

      if (this.input.InputPressed(Abstractions.Input.Backspace))
      {
        this.DeleteLastCharacter();
      }

      if (this.input.InputPressed(Abstractions.Input.Enter))
      {
        return this.Name.Length > 0 ? NameEntryStatus.Confirmed : NameEntryStatus.Cancelled;
      }

      if (this.input.InputPressed(Abstractions.Input.Back))
      {
        return NameEntryStatus.Cancelled;
      }

      return NameEntryStatus.Editing;
    }

    /// <summary>
    /// Renders the prompt and the current text.
    /// </summary>
    public void Render()
    {
      int centerX = this.renderer.GetScreenWidth() / 2;

      this.renderer.DrawScreenTextCentered(centerX, CenterY - PromptOffsetY, this.DisplayText, this.theme.FontSize, this.Color);
      this.renderer.DrawScreenTextCentered(centerX, CenterY, this.Text, this.theme.FontSize, this.theme.ColorPalette.Accent);
    }

    /// <summary>
    /// Appends an allowed character within the length cap and echoes it.
    /// </summary>
    /// <param name="character">Typed character.</param>
    private void Append(char character)
    {
      if (this.Text.Length >= MaxLength || (!char.IsLetterOrDigit(character) && character != ' '))
      {
        return;
      }

      this.Text += character;
      this.Echo(character);
    }

    /// <summary>
    /// Removes the last character, echoing the deletion; empty text is ignored.
    /// </summary>
    private void DeleteLastCharacter()
    {
      if (this.Text.Length == 0)
      {
        return;
      }

      char removed = this.Text[^1];
      this.Text = this.Text[..^1];
      this.tts.Stop();
      this.tts.SpeakAsync(string.Format(DeletedEchoFormat, NameEntry.Spoken(removed)));
    }

    /// <summary>
    /// Echoes a single accepted character.
    /// </summary>
    /// <param name="character">Character to echo.</param>
    private void Echo(char character)
    {
      this.tts.Stop();
      this.tts.SpeakAsync(NameEntry.Spoken(character));
    }

    /// <summary>
    /// Gets the spoken form of a character; space is spoken as a word.
    /// </summary>
    /// <param name="character">Character to speak.</param>
    /// <returns>Speakable text for the character.</returns>
    private static string Spoken(char character)
    {
      return character == ' ' ? SpaceEcho : character.ToString();
    }
  }
}
