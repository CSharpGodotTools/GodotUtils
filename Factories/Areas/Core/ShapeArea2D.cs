using Godot;

namespace GodotUtils;

public abstract class ShapeArea2D<TShape> : BaseShapeArea<Area2D> where TShape : Shape2D
{
    protected readonly TShape Shape;
    private readonly CollisionShape2D _collision;

    protected ShapeArea2D(TShape shape) : base(new Area2D())
    {
        Shape = shape;
        _collision = new CollisionShape2D { Shape = Shape };
        Area.AddChild(_collision);
    }

    public override void SetColor(Color color, bool transparent = false)
    {
        color.A = transparent ? 0 : 255;
        _collision.DebugColor = color;
    }

    public override Color GetColor() => _collision.DebugColor;
}
