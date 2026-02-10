using Godot;

namespace GodotUtils;

/// <summary>
/// Factory helpers for color picker buttons.
/// </summary>
public static class ColorPickerButtonFactory
{
    /// <summary>
    /// Creates a color picker button with a preset initial color.
    /// </summary>
    public static ColorPickerButton Create(Color initialColor)
    {
        ColorPickerButton button = new ColorPickerButton()
        {
            CustomMinimumSize = Vector2.One * 30
        };

        button.PickerCreated += () =>
        {
            ColorPicker picker = button.GetPicker();

            picker.Color = initialColor;

            PopupPanel popupPanel = picker.GetParent<PopupPanel>();

            popupPanel.InitialPosition = Window.WindowInitialPosition.Absolute;

            popupPanel.AboutToPopup += () =>
            {
                Vector2 viewportSize = popupPanel.GetTree().Root.GetViewport().GetVisibleRect().Size;

                // Position the ColorPicker to be at the top right of the screen
                popupPanel.Position = new Vector2I((int)(viewportSize.X - popupPanel.Size.X), 0);
            };
        };

        button.PopupClosed += button.ReleaseFocus;

        return button;
    }
}
