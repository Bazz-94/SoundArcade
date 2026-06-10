namespace SoundArcade.Domain.Models
{
  using System;
  using System.Collections.Generic;

  /// <summary>
  /// Menu component with selectable menu items.
  /// </summary>
  public class Menu : UIComponent
  {
    private readonly List<MenuItem> items = new List<MenuItem>();

    /// <summary>
    /// Initializes a new instance of the <see cref="Menu"/> class.
    /// </summary>
    /// <param name="id">Stable menu identifier.</param>
    /// <param name="displayText">Display text announced to users.</param>
    /// <param name="items">Menu items.</param>
    public Menu(int id, string displayText, IEnumerable<MenuItem> items)
      : base(id, displayText)
    {
      foreach (MenuItem item in items)
      {
        this.items.Add(item);
      }

      if (this.items.Count == 0)
      {
        throw new ArgumentException("Menu must contain at least one item.", nameof(items));
      }
    }

    /// <summary>
    /// Gets the menu items.
    /// </summary>
    public IReadOnlyList<MenuItem> Items => items;

    /// <summary>
    /// Gets the selected index.
    /// </summary>
    public int SelectedIndex { get; private set; }

    /// <summary>
    /// Gets the currently selected item.
    /// </summary>
    public MenuItem SelectedItem => items[this.SelectedIndex];

    /// <summary>
    /// Selects the previous item.
    /// </summary>
    public void MovePrevious()
    {
      this.SelectedIndex = WrapIndex(this.SelectedIndex - 1, items.Count);
    }

    /// <summary>
    /// Selects the next item.
    /// </summary>
    public void MoveNext()
    {
      this.SelectedIndex = WrapIndex(this.SelectedIndex + 1, items.Count);
    }

    /// <summary>
    /// Selects the first item.
    /// </summary>
    public void SelectFirst()
    {
      this.SelectedIndex = 0;
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
  }
}