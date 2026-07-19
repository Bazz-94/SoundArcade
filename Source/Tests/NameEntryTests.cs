namespace SoundArcade.Tests
{
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Colors;
  using SoundArcade.Domain.Models;
  using SoundArcade.Tests.Mock;
  using Xunit;

  /// <summary>
  /// Tests for the <see cref="NameEntry"/> text-entry component.
  /// </summary>
  public sealed class NameEntryTests
  {
    private readonly MockInput input = new MockInput();
    private readonly MockTts tts = new MockTts();
    private readonly NameEntry nameEntry;

    /// <summary>
    /// Initializes a new instance of the <see cref="NameEntryTests"/> class.
    /// </summary>
    public NameEntryTests()
    {
      this.nameEntry = new NameEntry(this.input, this.tts, new MockRenderer(), new Theme(), 1, "Enter your name");
    }

    /// <summary>
    /// Typed letters and digits are appended and each character is echoed.
    /// </summary>
    [Fact]
    public void Update_AppendsAndEchoesTypedCharacters()
    {
      this.input.Type("a1");
      NameEntryStatus status = this.nameEntry.Update();

      Assert.Equal(NameEntryStatus.Editing, status);
      Assert.Equal("a1", this.nameEntry.Text);
      Assert.Equal("1", this.tts.LastSpokenText);
    }

    /// <summary>
    /// A typed space is accepted and echoed as the word "space".
    /// </summary>
    [Fact]
    public void Update_EchoesSpaceAsWord()
    {
      this.input.Type("a ");
      this.nameEntry.Update();

      Assert.Equal("a ", this.nameEntry.Text);
      Assert.Equal("space", this.tts.LastSpokenText);
    }

    /// <summary>
    /// Characters other than letters, digits and spaces are ignored.
    /// </summary>
    [Fact]
    public void Update_IgnoresDisallowedCharacters()
    {
      this.input.Type("a!@#b");
      this.nameEntry.Update();

      Assert.Equal("ab", this.nameEntry.Text);
    }

    /// <summary>
    /// Input beyond twelve characters is discarded.
    /// </summary>
    [Fact]
    public void Update_CapsLengthAtTwelveCharacters()
    {
      this.input.Type("abcdefghijklmnop");
      this.nameEntry.Update();

      Assert.Equal("abcdefghijkl", this.nameEntry.Text);
    }

    /// <summary>
    /// Backspace removes the last character and echoes the deletion.
    /// </summary>
    [Fact]
    public void Update_BackspaceDeletesLastCharacterWithEcho()
    {
      this.input.Type("ab");
      this.nameEntry.Update();

      this.input.Press(Input.Backspace);
      this.nameEntry.Update();

      Assert.Equal("a", this.nameEntry.Text);
      Assert.Equal("b deleted", this.tts.LastSpokenText);
    }

    /// <summary>
    /// Backspace on an empty name changes nothing.
    /// </summary>
    [Fact]
    public void Update_BackspaceOnEmptyTextIsIgnored()
    {
      this.input.Press(Input.Backspace);

      Assert.Equal(NameEntryStatus.Editing, this.nameEntry.Update());
      Assert.Equal(string.Empty, this.nameEntry.Text);
    }

    /// <summary>
    /// Enter with a non-empty name confirms with the trimmed name.
    /// </summary>
    [Fact]
    public void Update_EnterConfirmsTrimmedName()
    {
      this.input.Type(" Zoe ");
      this.nameEntry.Update();

      this.input.Press(Input.Enter);

      Assert.Equal(NameEntryStatus.Confirmed, this.nameEntry.Update());
      Assert.Equal("Zoe", this.nameEntry.Name);
    }

    /// <summary>
    /// Enter with an empty or all-spaces name cancels instead of confirming.
    /// </summary>
    [Fact]
    public void Update_EnterWithEmptyNameCancels()
    {
      this.input.Type("   ");
      this.nameEntry.Update();

      this.input.Press(Input.Enter);

      Assert.Equal(NameEntryStatus.Cancelled, this.nameEntry.Update());
    }

    /// <summary>
    /// Escape cancels name entry.
    /// </summary>
    [Fact]
    public void Update_EscapeCancels()
    {
      this.input.Type("Zoe");
      this.nameEntry.Update();

      this.input.Press(Input.Back);

      Assert.Equal(NameEntryStatus.Cancelled, this.nameEntry.Update());
    }

    /// <summary>
    /// Opening announces the prompt and resets previous text.
    /// </summary>
    [Fact]
    public void Open_AnnouncesPromptAndResetsText()
    {
      this.input.Type("old");
      this.nameEntry.Update();

      this.nameEntry.Open();

      Assert.Equal(string.Empty, this.nameEntry.Text);
      Assert.Equal("Enter your name", this.tts.LastSpokenText);
    }
  }
}
