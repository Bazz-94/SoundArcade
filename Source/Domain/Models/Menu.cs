namespace SoundArcade.Domain.Models
{
  using System;
  using System.Collections.Generic;
  using System.Numerics;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Colors;

  /// <summary>
  /// Menu component with selectable menu items.
  /// </summary>
  public class Menu : UIComponent
  {
    public float MenuZ { get; set; } = 2.5f;
    public float DefaultMenuStartY { get; set; } = 8.0f;
    public float DefaultMenuItemSpacing { get; set; } = 0.9f;
    public int MenuItemFontSize { get; set; } = 22;
    public float MenuItemTextZOffset { get; set; } = -0.1f;
    public Vector3 MenuItemSize { get; set; } = new Vector3(4f, 0.28f, 1f);

    /// <summary>
    /// Initializes a new instance of the <see cref="Menu"/> class.
    /// </summary>
    /// <param name="id">Stable menu identifier.</param>
    /// <param name="items">Menu items.</param>
    /// <param name="menuTitle">Title of the menu.</param>
    /// <param name="input">Input abstraction.</param>
    /// <param name="tts">Text-to-speech abstraction.</param>
    public Menu(IInput input, ITts tts, IRenderer renderer, int id, IEnumerable<MenuItem> items, MenuColors menuColors, string menuTitle)
      : base(id, menuTitle)
    {
      this.Items = [.. items];

      if (this.Items.Count == 0)
      {
        throw new ArgumentException("Menu must contain at least one item.", nameof(items));
      }

      this.Input = input;
      this.Tts = tts;
      this.Renderer = renderer;
      this.SelectedColor = menuColors.Text;
      this.UnselectedColor = menuColors.MenuItem;
      this.MenuTitle = menuTitle;
    }

    private Color SelectedColor { get; }
    private Color UnselectedColor { get; }
    private string MenuTitle { get; }
    private List<MenuItem> Items { get; }
    private int SelectedIndex { get; set; }
    private MenuItem SelectedItem => this.Items[this.SelectedIndex];

    public IInput Input { get; }
    public ITts Tts { get; }
    public IRenderer Renderer { get; }

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

    private static int WrapIndex(int index, int length)
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

    public void Render()
    {
      int itemIndex = 0;

      this.Renderer.DrawText(new Vector3(0.0f, this.DefaultMenuStartY + 2, this.MenuZ), this.MenuTitle, this.MenuItemFontSize + 4, this.SelectedColor);

      foreach (MenuItem item in this.Items)
      {
        float y = this.DefaultMenuStartY - (itemIndex * this.DefaultMenuItemSpacing);
        bool isSelected = itemIndex == this.SelectedIndex;
        Color itemColor = isSelected ? this.SelectedColor : this.UnselectedColor;
        Color textColor = isSelected ? this.UnselectedColor : this.SelectedColor;
        this.Renderer.DrawBox(new Vector3(0.0f, y, this.MenuZ), this.MenuItemSize, itemColor);
        this.Renderer.DrawText(new Vector3(0.0f, y, this.MenuZ + this.MenuItemTextZOffset), item.DisplayText, this.MenuItemFontSize, textColor);
        itemIndex++;
      }
    }

    public void Update()
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
        this.SelectedItem.OnPressed();
      }
    }
  }
}