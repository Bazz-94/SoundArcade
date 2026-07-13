namespace SoundArcade.Tests
{
  using System;
  using SoundArcade.Domain.Colors;
  using SoundArcade.Domain.Models;
  using SoundArcade.Tests.Mock;
  using Xunit;

  /// <summary>
  /// Tests for the menu component.
  /// </summary>
  public sealed class MenuTests
  {
    private const string MenuTitle = "Test Menu";

    private static Menu CreateMenu(MockInput input, MockTts tts, out bool[] pressed)
    {
      Theme theme = new Theme();
      bool[] pressedFlags = new bool[3];
      pressed = pressedFlags;

      return new Menu(
        input,
        tts,
        new MockRenderer(),
        id: 0,
        items:
        [
          new MenuItem(theme, 0, "First", () => pressedFlags[0] = true),
          new MenuItem(theme, 1, "Second", () => pressedFlags[1] = true),
          new MenuItem(theme, 2, "Third", () => pressedFlags[2] = true)
        ],
        theme: theme,
        menuTitle: MenuTitle);
    }

    /// <summary>
    /// Verifies the menu rejects an empty item list.
    /// </summary>
    [Fact]
    public void Constructor_rejects_empty_items()
    {
      Theme theme = new Theme();

      Assert.Throws<ArgumentException>(() => new Menu(
        new MockInput(),
        new MockTts(),
        new MockRenderer(),
        id: 0,
        items: [],
        theme: theme,
        menuTitle: MenuTitle));
    }

    /// <summary>
    /// Verifies selecting the first item announces the menu title and item text.
    /// </summary>
    [Fact]
    public void SelectFirstItem_announces_title_and_first_item()
    {
      MockTts tts = new MockTts();
      Menu menu = CreateMenu(new MockInput(), tts, out bool[] _);

      menu.SelectFirstItem();

      Assert.Equal("First", tts.LastSpokenText);
    }

    /// <summary>
    /// Verifies down navigation moves the selection and announces the new item.
    /// </summary>
    [Fact]
    public void Down_moves_selection_and_announces_item()
    {
      MockInput input = new MockInput();
      MockTts tts = new MockTts();
      Menu menu = CreateMenu(input, tts, out bool[] pressed);

      input.Press(Abstractions.Input.Down);
      menu.Update();

      Assert.Equal("Second", tts.LastSpokenText);

      input.Press(Abstractions.Input.Enter);
      menu.Update();

      Assert.True(pressed[1]);
    }

    /// <summary>
    /// Verifies up navigation from the first item wraps to the last item.
    /// </summary>
    [Fact]
    public void Up_from_first_item_wraps_to_last()
    {
      MockInput input = new MockInput();
      MockTts tts = new MockTts();
      Menu menu = CreateMenu(input, tts, out bool[] _);

      input.Press(Abstractions.Input.Up);
      menu.Update();

      Assert.Equal("Third", tts.LastSpokenText);
    }

    /// <summary>
    /// Verifies down navigation from the last item wraps to the first item.
    /// </summary>
    [Fact]
    public void Down_from_last_item_wraps_to_first()
    {
      MockInput input = new MockInput();
      MockTts tts = new MockTts();
      Menu menu = CreateMenu(input, tts, out bool[] _);

      foreach (int _ in new int[3])
      {
        input.Press(Abstractions.Input.Down);
        menu.Update();
      }

      Assert.Equal("First", tts.LastSpokenText);
    }

    /// <summary>
    /// Verifies pressing enter invokes the selected item's action.
    /// </summary>
    [Fact]
    public void Enter_invokes_selected_item_action()
    {
      MockInput input = new MockInput();
      Menu menu = CreateMenu(input, new MockTts(), out bool[] pressed);

      input.Press(Abstractions.Input.Enter);
      menu.Update();

      Assert.True(pressed[0]);
      Assert.False(pressed[1]);
    }
  }
}
