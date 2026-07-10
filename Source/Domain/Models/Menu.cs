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
    public int MenuStartY { get; set; } = 350;
    public int MenuTitleOffsetY { get; set; } = 80;
    public int MenuItemSpacing { get; set; } = 60;
    public int MenuItemWidth { get; set; } = 280;
    public int MenuItemHeight { get; set; } = 44;
    public int MenuItemFontSize { get; set; } = 22;
    private string MenuTitle { get; }
    public List<MenuItem> Items { get; }
    private int SelectedIndex { get; set; }
    /// <summary>
    /// Gets the currently selected item.
    /// </summary>
    protected MenuItem SelectedItem => this.Items[this.SelectedIndex];
    private Theme Theme { get; }
    public IInput Input { get; }
    public ITts Tts { get; }
    public IRenderer Renderer { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Menu"/> class.
    /// </summary>
    /// <param name="id">Stable menu identifier.</param>
    /// <param name="items">Menu items.</param>
    /// <param name="menuTitle">Title of the menu.</param>
    /// <param name="input">Input abstraction.</param>
    /// <param name="tts">Text-to-speech abstraction.</param>
    public Menu(IInput input, ITts tts, IRenderer renderer, int id, IEnumerable<MenuItem> items, Theme theme, string menuTitle)
      : base(theme.ColorPalette.Tertiary, id, menuTitle)
    {
      this.Theme = theme;
      this.Items = [.. items];

      if (this.Items.Count == 0)
      {
        throw new ArgumentException("Menu must contain at least one item.", nameof(items));
      }

      this.Input = input;
      this.Tts = tts;
      this.Renderer = renderer;
      this.MenuTitle = menuTitle;
    }

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
    /// Wraps an index into the range [0, length).
    /// </summary>
    /// <param name="index">Index to wrap.</param>
    /// <param name="length">Collection length.</param>
    /// <returns>Wrapped index.</returns>
    protected static int WrapIndex(int index, int length)
    {
      if (length <= 0)
      {
        return 0;
      }

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

      this.Renderer.DrawScreenTextCentered(centerX, this.MenuStartY - this.MenuTitleOffsetY, this.MenuTitle, this.MenuItemFontSize + 4, this.Color);

      int itemIndex = 0;
      foreach (MenuItem item in this.Items)
      {
        item.X = centerX;
        item.Y = this.MenuStartY + (itemIndex * this.MenuItemSpacing);
        item.Color = this.Theme.ColorPalette.Primary;
        item.TextColor = this.Theme.ColorPalette.Accent;
        item.Width = this.MenuItemWidth;
        item.Height = this.MenuItemHeight;
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