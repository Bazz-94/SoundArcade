namespace SoundArcade.Application.Scenes
{
  using System;
  using System.Collections.Generic;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Colors;
  using SoundArcade.Domain.Models;

  /// <summary>
  /// Menu of cycling control items; pressing a control advances its value to the next level.
  /// </summary>
  public sealed class SettingsMenu : Menu
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsMenu"/> class.
    /// </summary>
    /// <param name="input">Input abstraction.</param>
    /// <param name="tts">Text-to-speech abstraction.</param>
    /// <param name="renderer">Renderer abstraction.</param>
    /// <param name="id">Stable menu identifier.</param>
    /// <param name="controls">Cycling controls to present.</param>
    /// <param name="backItem">Back menu item.</param>
    /// <param name="theme">Theme for colors.</param>
    /// <param name="menuTitle">Title of the menu.</param>
    public SettingsMenu(
      IInput input,
      ITts tts,
      IRenderer renderer,
      int id,
      IEnumerable<ControlItem> controls,
      MenuItem backItem,
      Theme theme,
      string menuTitle)
      : base(input, tts, renderer, id, [.. controls, backItem], theme, menuTitle)
    {
    }

    /// <summary>
    /// Handles navigation, presses, and left/right cycling of the selected control.
    /// </summary>
    public override void Update()
    {
      base.Update();

      if (this.SelectedItem is ControlItem control)
      {
        if (this.Input.InputPressed(Abstractions.Input.Left))
        {
          Cycle(control, -1);
        }

        if (this.Input.InputPressed(Abstractions.Input.Right))
        {
          Cycle(control, 1);
        }
      }
    }

    /// <summary>
    /// Cycles control items to their next value; other items press normally.
    /// </summary>
    /// <param name="item">Item that was pressed.</param>
    protected override void OnItemPressed(MenuItem item)
    {
      if (item is ControlItem control)
      {
        Cycle(control, 1);
      }
      else
      {
        base.OnItemPressed(item);
      }
    }

    private static void Cycle(ControlItem control, int direction)
    {
      int nearestIndex = 0;

      for (int i = 1; i < control.Values.Count; i++)
      {
        if (Math.Abs(control.Values[i] - control.Value) < Math.Abs(control.Values[nearestIndex] - control.Value))
        {
          nearestIndex = i;
        }
      }

      control.SetValue(control.Values[WrapIndex(nearestIndex + direction, control.Values.Count)]);
    }
  }
}
