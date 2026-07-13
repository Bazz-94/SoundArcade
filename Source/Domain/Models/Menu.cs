namespace SoundArcade.Domain.Models
{
  using System;
  using System.Collections.Generic;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Colors;

  /// <summary>
  /// Menu component with selectable menu items.
  /// </summary>
  public class Menu : UIComponent
  {
    private const int StartY = 350;
    private const int TitleOffsetY = 80;
    private const int ItemSpacing = 60;
    private const int ItemWidth = 280;
    private const int ItemHeight = 44;
    private const int TitleFontSizeOffset = 4;

    private string MenuTitle { get; }

    /// <summary>
    /// Gets the menu items in display order.
    /// </summary>
    public IReadOnlyList<MenuItem> Items { get; }

    private int SelectedIndex { get; set; }

    /// <summary>
    /// Gets the currently selected item.
    /// </summary>
    protected MenuItem SelectedItem => this.Items[this.SelectedIndex];

    private Theme Theme { get; }

    /// <summary>
    /// Gets the input abstraction.
    /// </summary>
    protected IInput Input { get; }

    /// <summary>
    /// Gets the text-to-speech abstraction.
    /// </summary>
    protected ITts Tts { get; }

    /// <summary>
    /// Gets the renderer abstraction.
    /// </summary>
    protected IRenderer Renderer { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Menu"/> class.
    /// </summary>
    /// <param name="input">Input abstraction.</param>
    /// <param name="tts">Text-to-speech abstraction.</param>
    /// <param name="renderer">Renderer abstraction.</param>
    /// <param name="id">Stable menu identifier.</param>
    /// <param name="items">Menu items.</param>
    /// <param name="theme">Theme for colors.</param>
    /// <param name="menuTitle">Title of the menu.</param>
    public Menu(IInput input, ITts tts, IRenderer renderer, int id, IEnumerable<MenuItem> items, Theme theme, string menuTitle)
      : base(theme.ColorPalette.Tertiary, id, menuTitle)
    {
      ArgumentNullException.ThrowIfNull(input);
      ArgumentNullException.ThrowIfNull(tts);
      ArgumentNullException.ThrowIfNull(renderer);
      ArgumentNullException.ThrowIfNull(items);

      List<MenuItem> itemList = [.. items];

      if (itemList.Count == 0)
      {
        throw new ArgumentException("Menu must contain at least one item.", nameof(items));
      }

      this.Items = itemList;
      this.Theme = theme;
      this.Input = input;
      this.Tts = tts;
      this.Renderer = renderer;
      this.MenuTitle = menuTitle;
    }

    /// <summary>
    /// Resets the selection to the first item and announces the menu title and item.
    /// </summary>
    public void SelectFirstItem()
    {
      this.SelectedIndex = 0;
      this.Tts.Stop();
      this.Tts.SpeakAsync(this.DisplayText);
      this.Tts.SpeakAsync(this.SelectedItem.DisplayText);
    }

    /// <summary>
    /// Selects the previous item.
    /// </summary>
    public void MovePrevious()
    {
      this.SelectedIndex = WrapIndex(this.SelectedIndex - 1, this.Items.Count);
    }

    /// <summary>
    /// Selects the next item.
    /// </summary>
    public void MoveNext()
    {
      this.SelectedIndex = WrapIndex(this.SelectedIndex + 1, this.Items.Count);
    }

    /// <summary>
    /// Wraps an index into the range [0, length). Length must be positive.
    /// </summary>
    /// <param name="index">Index to wrap.</param>
    /// <param name="length">Collection length.</param>
    /// <returns>Wrapped index.</returns>
    protected static int WrapIndex(int index, int length)
    {
      int wrapped = index % length;

      if (wrapped < 0)
      {
        wrapped += length;
      }

      return wrapped;
    }

    /// <summary>
    /// Renders the menu title and items.
    /// </summary>
    public virtual void Render()
    {
      int centerX = this.Renderer.GetScreenWidth() / 2;

      this.Renderer.DrawScreenTextCentered(centerX, StartY - TitleOffsetY, this.MenuTitle, this.Theme.FontSize + TitleFontSizeOffset, this.Color);

      int itemIndex = 0;
      foreach (MenuItem item in this.Items)
      {
        item.SetLayout(centerX, StartY + (itemIndex * ItemSpacing), ItemWidth, ItemHeight);
        item.Render(this.Renderer, itemIndex == this.SelectedIndex);

        itemIndex++;
      }
    }

    /// <summary>
    /// Handles navigation and item presses for the current frame.
    /// </summary>
    public virtual void Update()
    {
      bool selectionChanged = false;

      if (this.Input.InputPressed(Abstractions.Input.Up))
      {
        this.MovePrevious();
        selectionChanged = true;
      }

      if (this.Input.InputPressed(Abstractions.Input.Down))
      {
        this.MoveNext();
        selectionChanged = true;
      }

      if (selectionChanged)
      {
        this.Tts.Stop();
        this.Tts.SpeakAsync(this.SelectedItem.DisplayText);
      }

      if (this.Input.InputPressed(Abstractions.Input.Enter))
      {
        this.OnItemPressed(this.SelectedItem);
      }
    }

    /// <summary>
    /// Handles a press on the given item; default invokes the item's pressed action.
    /// </summary>
    /// <param name="item">Item that was pressed.</param>
    protected virtual void OnItemPressed(MenuItem item)
    {
      item.OnPressed();
    }
  }
}
