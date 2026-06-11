namespace SoundArcade.Domain.Models
{
  using System;
  using System.Collections.Generic;
  using System.Numerics;
  using SoundArcade.Abstractions;

  /// <summary>
  /// Base scene for keyboard-driven menus with focus announcements and command dispatch.
  /// </summary>
  public abstract class MenuScene : IScene
  {
    private const float DefaultMenuStartY = 3.0f;
    private const float DefaultMenuItemSpacing = 0.8f;
    private const int MenuItemFontSize = 22;
    private const float MenuItemTextZOffset = 0.16f;
    private static readonly Vector3 MenuItemSize = new Vector3(2.4f, 0.28f, 0.28f);

    protected IInput Input { get; set; }
    protected ITts Tts { get; set; }
    protected IRenderer Renderer { get; set; }
    private Color SelectedColor { get; set; }
    private Color UnselectedColor { get; set; }
    private float MenuZ { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuScene"/> class.
    /// </summary>
    /// <param name="menu">Menu model for this scene.</param>
    /// <param name="input">Input abstraction.</param>
    /// <param name="tts">Text-to-speech abstraction.</param>
    /// <param name="renderer">Renderer abstraction.</param>
    /// <param name="selectedColor">Selected item color.</param>
    /// <param name="unselectedColor">Unselected item color.</param>
    /// <param name="menuZ">Menu z position for rendering.</param>
    /// <param name="backAction">Optional back action.</param>
    /// <param name="afterRender">Optional extra render callback.</param>
    public MenuScene(
      Menu menu,
      IInput input,
      ITts tts,
      IRenderer renderer,
      Color selectedColor,
      Color unselectedColor,
      float menuZ)
    {
      this.menu = menu;
      this.Input = input;
      this.Tts = tts;
      this.Renderer = renderer;
      this.SelectedColor = selectedColor;
      this.UnselectedColor = unselectedColor;
      this.MenuZ = menuZ;
    }

    private readonly Menu menu;

    protected abstract IReadOnlyDictionary<int, Action<MenuItem>> GetItemActions();

    protected virtual void OnBackSelected()
    {
    }

    protected virtual void AfterRender()
    {
    }

    /// <inheritdoc />
    public void OnEnter()
    {
      menu.SelectFirst();
      this.Tts.SpeakAsync(menu.DisplayText);
      this.Tts.SpeakAsync(menu.SelectedItem.DisplayText);
    }

    /// <inheritdoc />
    public void OnExit()
    {
    }

    /// <inheritdoc />
    public void Update(float deltaTime)
    {
      bool selectionChanged = false;

      if (this.Input.InputPressed(Abstractions.Input.Up))
      {
        menu.MovePrevious();
        selectionChanged = true;
      }

      if (this.Input.InputPressed(Abstractions.Input.Down))
      {
        menu.MoveNext();
        selectionChanged = true;
      }

      if (selectionChanged)
      {
        this.Tts.SpeakAsync(menu.SelectedItem.DisplayText);
      }

      if (this.Input.InputPressed(Abstractions.Input.Back))
      {
        this.OnBackSelected();
        return;
      }

      if (this.Input.InputPressed(Abstractions.Input.Enter))
      {
        this.OnItemSelected(menu.SelectedItem);
      }
    }

    /// <inheritdoc />
    public void Render()
    {
      int itemIndex = 0;

      foreach (MenuItem item in menu.Items)
      {
        float y = DefaultMenuStartY - (itemIndex * DefaultMenuItemSpacing);
        bool isSelected = itemIndex == menu.SelectedIndex;
        Color itemColor = isSelected ? this.SelectedColor : this.UnselectedColor;
        Color textColor = isSelected ? this.UnselectedColor : this.SelectedColor;
        this.Renderer.DrawBox(new Vector3(0.0f, y, this.MenuZ), MenuItemSize, itemColor);
        this.Renderer.DrawText(new Vector3(0.0f, y, this.MenuZ + MenuItemTextZOffset), item.DisplayText, MenuItemFontSize, textColor);
        itemIndex++;
      }

      this.AfterRender();
    }

    private void OnItemSelected(MenuItem item)
    {
      IReadOnlyDictionary<int, Action<MenuItem>> itemActions = this.GetItemActions();

      if (itemActions.TryGetValue(item.Id, out Action<MenuItem>? action))
      {
        action(item);
        return;
      }

      throw new InvalidOperationException($"No action configured for menu item {item.Id}.");
    }
  }
}
