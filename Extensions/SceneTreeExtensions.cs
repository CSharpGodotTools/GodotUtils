using Godot;

namespace GodotUtils;

public static class SceneTreeExtensions
{
    /// <summary>
    /// Returns the current scene as the specified type.
    /// <code>MyScene scene = tree.GetCurrentScene&lt;MyScene&gt;();</code>
    /// </summary>
    /// <typeparam name="T">The type of the current scene, must inherit from Node.</typeparam>
    /// <param name="tree">The SceneTree instance to call this extension on.</param>
    /// <returns>The current scene as type T, or null if the cast fails.</returns>
    public static T GetCurrentScene<T>(this SceneTree tree) where T : Node
    {
        return tree.CurrentScene as T;
    }

    /// <summary>
    /// Retrieves an autoload from the scene tree using the given name.
    /// <code>Global global = someTree.GetAutoload&lt;Global&gt;("Global");</code>
    /// </summary>
    /// <typeparam name="T">Type of the autoload node, must inherit from Node.</typeparam>
    /// <param name="tree">The SceneTree instance to call this extension on.</param>
    /// <param name="autoload">The name of the autoload as registered in Project Settings -> AutoLoad.</param>
    /// <returns>The autoload instance of type T.</returns>
    public static T GetAutoload<T>(this SceneTree tree, string autoload) where T : Node
    {
        return tree.Root.GetNode<T>($"/root/{autoload}");
    }

    /// <summary>
    /// Removes focus from the currently focused UI control, if any.
    /// </summary>
    /// <param name="tree">The SceneTree instance to call this extension on.</param>
    public static void UnfocusCurrentControl(this SceneTree tree)
    {
        // Get the currently focused control
        Control focusedControl = tree.Root.GuiGetFocusOwner();

        if (focusedControl != null)
        {
            // Set the focus mode to None to unfocus it
            focusedControl.FocusMode = Control.FocusModeEnum.None;
        }
    }
}
