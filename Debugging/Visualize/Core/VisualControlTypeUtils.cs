#if DEBUG
using Godot;
using System;

namespace GodotUtils.Debugging;

/// <summary>
/// More utility methods
/// </summary>
public static partial class VisualControlTypes
{
    private static void SetControlValue(Control control, object value)
    {
        switch (control)
        {
            case ColorPickerButton colorPickerButton:
                colorPickerButton.Color = (Color)value;
                break;
            case LineEdit lineEdit:
                lineEdit.Text = (string)value;
                break;
            case SpinBox spinBox:
                spinBox.Value = Convert.ToDouble(value);
                break;
            case CheckBox checkBox:
                checkBox.ButtonPressed = (bool)value;
                break;
            case OptionButton optionButton:
                optionButton.Select((int)value);
                break;
            // Add more control types here as needed
        }
    }

    // Helper method to remove an element from an array
    private static Array RemoveAt(this Array source, int index)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (index < 0 || index >= source.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index), index, $"[Visualize] Index was out of range");
        }

        Array dest = Array.CreateInstance(source.GetType().GetElementType(), source.Length - 1);
        Array.Copy(source, 0, dest, 0, index);
        Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

        return dest;
    }

    private static SpinBox CreateSpinBox(Type type)
    {
        SpinBox spinBox = new()
        {
            UpdateOnTextChanged = true,
            AllowLesser = false,
            AllowGreater = false,
            MinValue = int.MinValue,
            MaxValue = int.MaxValue,
            Alignment = HorizontalAlignment.Center
        };

        spinBox.Step = type switch
        {
            _ when type == typeof(float) => 0.1,
            _ when type == typeof(double) => 0.1,
            _ when type == typeof(decimal) => 0.01,
            _ when type == typeof(int) => 1,
            _ => 1
        };

        return spinBox;
    }
}
#endif
