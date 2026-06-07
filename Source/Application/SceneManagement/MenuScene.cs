namespace SoundArcade.Application.SceneManagement
{
  using System;
  using System.Collections.Generic;
  using System.Numerics;
  using SoundArcade.Abstractions;

  /// <summary>
  /// Base scene for keyboard-driven menus with focus announcements and command dispatch.
  /// </summary>
  public class MenuScene : IScene
  {
    private const float DefaultMenuStartY = 3.0f;
    private const float DefaultMenuItemSpacing = 0.8f;
    private const int MenuItemFontSize = 22;
    private const float MenuItemTextZOffset = 0.16f;
    private static readonly Vector3 MenuItemSize = new Vector3(2.4f, 0.28f, 0.28f);

    private readonly IInput input;
    private readonly ITts tts;
    private readonly IRenderer renderer;
    private readonly Color selectedColor;
    private readonly Color unselectedColor;
    private readonly float menuZ;
    private readonly IReadOnlyDictionary<int, Action<MenuItem>> itemActions;
    private readonly Action? backAction;
    private readonly Action? afterRender;

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
    /// <param name="itemActions">Menu item action map.</param>
    /// <param name="backAction">Optional back action.</param>
    /// <param name="afterRender">Optional extra render callback.</param>
    public MenuScene(
      Menu menu,
      IInput input,
      ITts tts,
      IRenderer renderer,
      Color selectedColor,
      Color unselectedColor,
      float menuZ,
      IReadOnlyDictionary<int, Action<MenuItem>> itemActions,
      Action? backAction = null,
      Action? afterRender = null)
    {
      this.menu = menu;
      this.input = input;
      this.tts = tts;
      this.renderer = renderer;
      this.selectedColor = selectedColor;
      this.unselectedColor = unselectedColor;
      this.menuZ = menuZ;
      this.itemActions = itemActions;
      this.backAction = backAction;
      this.afterRender = afterRender;
    }

    private readonly Menu menu;

    /// <inheritdoc />
    public void OnEnter()
    {
      menu.SelectFirst();
      tts.SpeakAsync(menu.DisplayText);
      tts.SpeakAsync(menu.SelectedItem.DisplayText);
    }

    /// <inheritdoc />
    public void OnExit()
    {
    }

    /// <inheritdoc />
    public void Update(float deltaTime)
    {
      bool selectionChanged = false;

      if (input.InputPressed(Input.Up))
      {
        menu.MovePrevious();
        selectionChanged = true;
      }

      if (input.InputPressed(Input.Down))
      {
        menu.MoveNext();
        selectionChanged = true;
      }

      if (selectionChanged)
      {
        tts.SpeakAsync(menu.SelectedItem.DisplayText);
      }

      if (input.InputPressed(Input.Back))
      {
        this.OnBackSelected();
        return;
      }

      if (input.InputPressed(Input.Enter))
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
        Color itemColor = isSelected ? selectedColor : unselectedColor;
        Color textColor = isSelected ? unselectedColor : selectedColor;
        renderer.DrawBox(new Vector3(0.0f, y, menuZ), MenuItemSize, itemColor);
        renderer.DrawText(new Vector3(0.0f, y, menuZ + MenuItemTextZOffset), item.DisplayText, MenuItemFontSize, textColor);
        itemIndex++;
      }

      if (afterRender is not null)
      {
        afterRender();
      }
    }

    private void OnItemSelected(MenuItem item)
    {
      if (itemActions.TryGetValue(item.Id, out Action<MenuItem>? action))
      {
        action(item);
        return;
      }

      throw new InvalidOperationException($"No action configured for menu item {item.Id}.");
    }

    private void OnBackSelected()
    {
      if (backAction is null)
      {
        return;
      }

      backAction();
    }
  }
}
