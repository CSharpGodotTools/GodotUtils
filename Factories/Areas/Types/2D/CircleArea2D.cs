using Godot;

namespace GodotUtils;

public class CircleArea2D : ShapeArea2D<CircleShape2D>
{
    public float Radius
    {
        get => Shape.Radius;
        set => Shape.Radius = value;
    }

    internal CircleArea2D(float radius) : base(new CircleShape2D { Radius = radius }) 
    {
    }
}
