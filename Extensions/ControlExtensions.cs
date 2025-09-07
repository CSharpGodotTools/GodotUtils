using Godot;
using Layout = Godot.Control.LayoutPreset;

namespace GodotUtils;

public static class ControlExtensions
{
    /// <summary>
    /// Sets the layout for the specified control.
    /// </summary>
    /// <remarks>
    /// Applies the layout immediately if the control is inside the tree, otherwise applies it when the control becomes ready.
    /// </remarks>
    public static Control SetLayout(this Control control, Layout layout)
    {
        if (control.IsInsideTree())
        {
            control.SetAnchorsAndOffsetsPreset(layout);
        }
        else
        {
            control.Ready += () => control.SetAnchorsAndOffsetsPreset(layout);
        }

        return control;
    }

    public static Control SetFontSize(this Control control, int fontSize)
    {
        control.AddThemeFontSizeOverride("font_size", fontSize);
        return control;
    }

    public static Control SetMarginLeft(this Control control, int padding)
    {
        control.AddThemeConstantOverride("margin_left", padding);
        return control;
    }

    public static Control SetMarginRight(this Control control, int padding)
    {
        control.AddThemeConstantOverride("margin_right", padding);
        return control;
    }

    public static Control SetMarginTop(this Control control, int padding)
    {
        control.AddThemeConstantOverride("margin_top", padding);
        return control;
    }

    public static Control SetMarginBottom(this Control control, int padding)
    {
        control.AddThemeConstantOverride("margin_bottom", padding);
        return control;
    }
}
