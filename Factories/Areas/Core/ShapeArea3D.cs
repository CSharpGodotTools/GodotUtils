using Godot;

namespace GodotUtils;

public abstract class ShapeArea3D<TShape> : BaseShapeArea<Area3D> where TShape : Shape3D
{
    protected readonly TShape Shape;
    private readonly CollisionShape3D _collision;

    protected ShapeArea3D(TShape shape) : base(new Area3D())
    {
        Shape = shape;
        _collision = new CollisionShape3D { Shape = Shape };
        Area.AddChild(_collision);
    }

    public override void SetColor(Color color, bool transparent = false)
    {
        color.A = transparent ? 0 : 255;
        _collision.DebugColor = color;
    }

    public override Color GetColor() => _collision.DebugColor;
}
