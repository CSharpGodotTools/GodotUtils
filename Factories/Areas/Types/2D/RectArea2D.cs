using Godot;

namespace GodotUtils;

public class RectArea2D : ShapeArea2D<RectangleShape2D>
{
    public Vector2 Size
    {
        get => Shape.Size;
        set => Shape.Size = value;
    }

    internal RectArea2D(Vector2 size) : base(new RectangleShape2D { Size = size }) 
    { 
    }
}
