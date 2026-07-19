namespace SoundArcade.Tests
{
  using System.Collections.Generic;
  using SoundArcade.Abstractions;
  using SoundArcade.Tests.Mock;
  using Xunit;

  /// <summary>
  /// Tests for the <see cref="MockInput"/> text-input capability.
  /// </summary>
  public sealed class MockInputTests
  {
    /// <summary>
    /// Typed characters are returned in order and drained by a single call.
    /// </summary>
    [Fact]
    public void ReadTypedCharacters_DrainsQueuedCharactersOnce()
    {
      MockInput input = new MockInput();
      input.Type("ab");

      Assert.Equal(new List<char> { 'a', 'b' }, input.ReadTypedCharacters());
      Assert.Empty(input.ReadTypedCharacters());
    }

    /// <summary>
    /// Backspace is pollable like any other logical input.
    /// </summary>
    [Fact]
    public void InputPressed_BackspaceConsumesQueuedPress()
    {
      MockInput input = new MockInput();
      input.Press(Input.Backspace);

      Assert.True(input.InputPressed(Input.Backspace));
      Assert.False(input.InputPressed(Input.Backspace));
    }
  }
}
