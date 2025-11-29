#if DEBUG
using Godot;

namespace GodotUtils.Debugging;

public static partial class VisualControlTypes
{
    private static VisualControlInfo VisualObject(VisualControlContext context)
    {
        LineEdit lineEdit = new() { Text = context.InitialValue?.ToString() ?? string.Empty };
        lineEdit.TextChanged += text => context.ValueChanged(text);

        return new VisualControlInfo(new LineEditControl(lineEdit));
    }
}
#endif
