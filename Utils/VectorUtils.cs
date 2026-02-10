using Godot;

namespace GodotUtils;

public static class VectorUtils
{
    /// <summary>
    /// Returns a random normalized 2D direction vector.
    /// </summary>
    public static Vector2 Random()
    {
        Vector2 vector = new Vector2(MathUtils.RandRange(-1.0, 1.0), MathUtils.RandRange(-1.0, 1.0));
        return vector.Normalized();
    }
}
