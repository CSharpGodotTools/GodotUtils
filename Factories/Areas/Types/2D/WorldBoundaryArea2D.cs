using Godot;

namespace GodotUtils;

public class WorldBoundaryArea2D : ShapeArea2D<WorldBoundaryShape2D>
{
    public Vector2 Normal
    {
        get => Shape.Normal;
        set => Shape.Normal = value;
    }

    internal WorldBoundaryArea2D(Vector2 normal) : base(new WorldBoundaryShape2D { Normal = normal })
    {
    }
}
