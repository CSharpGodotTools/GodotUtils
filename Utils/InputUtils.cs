using Godot;

namespace GodotUtils;

public static class InputUtils
{
    /// <summary>
    /// Returns true when moving left based on arrow or WASD input.
    /// </summary>
    public static bool IsMovingLeft()
    {
        return IsPressed(Key.Left, Key.A);
    }

    /// <summary>
    /// Returns true when moving right based on arrow or WASD input.
    /// </summary>
    public static bool IsMovingRight()
    {
        return IsPressed(Key.Right, Key.D);
    }

    /// <summary>
    /// Returns true when moving up based on arrow or WASD input.
    /// </summary>
    public static bool IsMovingUp()
    {
        return IsPressed(Key.Up, Key.W);
    }

    /// <summary>
    /// Returns true when moving down based on arrow or WASD input.
    /// </summary>
    public static bool IsMovingDown()
    {
        return IsPressed(Key.Down, Key.S);
    }

    private static bool IsPressed(Key primary, Key alternate)
    {
        return Input.IsKeyPressed(primary) || Input.IsKeyPressed(alternate);
    }
}
