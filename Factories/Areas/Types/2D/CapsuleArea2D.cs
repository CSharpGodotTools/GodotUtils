using Godot;

namespace GodotUtils;

public class CapsuleArea2D : ShapeArea2D<CapsuleShape2D>
{
    public float Radius
    {
        get => Shape.Radius;
        set => Shape.Radius = value;
    }

    public float Height
    {
        get => Shape.Height;
        set => Shape.Height = value;
    }

    internal CapsuleArea2D(float radius, float height) : base(new CapsuleShape2D { Radius = radius, Height = height })
    {
    }
}
