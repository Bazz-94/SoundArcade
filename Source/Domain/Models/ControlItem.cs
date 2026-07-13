namespace SoundArcade.Domain.Models
{
  using System;
  using System.Collections.Generic;
  using SoundArcade.Abstractions;
  using SoundArcade.Domain.Colors;

  /// <summary>
  /// Menu item holding one value from a fixed set of levels; the owning menu drives cycling
  /// via <see cref="SetValue"/>. Renders the value beside the label.
  /// </summary>
  public sealed class ControlItem : MenuItem
  {
    private const int ValueTextOffsetX = 220;
    private const int ValueFontSize = 24;
    private const float PercentScale = 100.0f;

    /// <summary>
    /// Gets the values this control cycles through.
    /// </summary>
    public IReadOnlyList<float> Values { get; }

    /// <summary>
    /// Gets the current value.
    /// </summary>
    public float Value { get; private set; }

    private Action<float> OnValueChanged { get; }
    private Color ValueColor { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ControlItem"/> class.
    /// </summary>
    /// <param name="theme">Theme for colors.</param>
    /// <param name="id">Stable item identifier.</param>
    /// <param name="displayText">Display text announced to users.</param>
    /// <param name="values">Values to cycle through.</param>
    /// <param name="initialValue">Starting value; snapped to the nearest list value.</param>
    /// <param name="onValueChanged">Callback invoked with the new value after each cycle.</param>
    public ControlItem(
      Theme theme,
      int id,
      string displayText,
      IReadOnlyList<float> values,
      float initialValue,
      Action<float> onValueChanged)
      : base(theme, id, displayText, () => { })
    {
      if (values.Count == 0)
      {
        throw new ArgumentException("ControlItem must have at least one value.", nameof(values));
      }

      this.Values = values;
      this.Value = NearestValue(values, initialValue);
      this.OnValueChanged = onValueChanged;
      this.ValueColor = theme.ColorPalette.Accent;
    }

    /// <summary>
    /// Sets the current value and notifies the value-changed callback.
    /// </summary>
    /// <param name="value">New value.</param>
    public void SetValue(float value)
    {
      if (!Contains(this.Values, value))
      {
        throw new ArgumentException("ControlItem value must be one of its values.", nameof(value));
      }

      this.Value = value;
      this.OnValueChanged(value);
    }

    /// <summary>
    /// Returns true when the value is present in the list.
    /// </summary>
    /// <param name="values">Values to search.</param>
    /// <param name="value">Value to find.</param>
    private static bool Contains(IReadOnlyList<float> values, float value)
    {
      foreach (float candidate in values)
      {
        if (candidate == value)
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Returns the list value closest to the requested value.
    /// </summary>
    /// <param name="values">Values to search.</param>
    /// <param name="value">Requested value.</param>
    private static float NearestValue(IReadOnlyList<float> values, float value)
    {
      float nearest = values[0];

      foreach (float candidate in values)
      {
        if (MathF.Abs(candidate - value) < MathF.Abs(nearest - value))
        {
          nearest = candidate;
        }
      }

      return nearest;
    }

    /// <summary>
    /// Renders the item and its current value as a percentage beside the label.
    /// </summary>
    /// <param name="renderer">Renderer abstraction.</param>
    /// <param name="isSelected">Whether the item is currently selected.</param>
    public override void Render(IRenderer renderer, bool isSelected)
    {
      base.Render(renderer, isSelected);
      renderer.DrawScreenTextCentered(
        this.X + ValueTextOffsetX,
        this.Y,
        ((int)MathF.Round(this.Value * PercentScale)).ToString(),
        ValueFontSize,
        this.ValueColor);
    }
  }
}
