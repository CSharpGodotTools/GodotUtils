using Godot;

namespace GodotUtils;

public static class AreaFactory
{
    public static CircleArea CreateCircle(float radius) => new(radius);
    public static RectArea CreateRect(Vector2 size) => new(size);
}

public class RectArea : BaseArea
{
    public Vector2 Size
    {
        get => _shape.Size;
        set => _shape.Size = value;
    }

    private readonly RectangleShape2D _shape;

    internal RectArea(Vector2 size) : base(new RectangleShape2D { Size = size})
    {
        _shape = (RectangleShape2D)Shape;
    }
}

public class CircleArea : BaseArea
{
    public float Radius
    {
        get => _shape.Radius;
        set => _shape.Radius = value;
    }

    private readonly CircleShape2D _shape;

    internal CircleArea(float radius) : base(new CircleShape2D { Radius = radius })
    {
        _shape = (CircleShape2D)Shape;
    }
}

public abstract class BaseArea
{
    protected Shape2D Shape => _shape;

    private readonly Area2D _area;
    private readonly CollisionShape2D _cShape;
    private readonly Shape2D _shape;

    protected BaseArea(Shape2D shape)
    {
        _area = new Area2D();
        _shape = shape;
        _cShape = new CollisionShape2D
        {
            Shape = _shape
        };

        _area.AddChild(_cShape);
    }

    public void SetColor(Color color, bool transparent = false)
    {
        color.A = transparent ? 0 : 255;
        _cShape.DebugColor = color;
    }

    public Color GetColor() => _cShape.DebugColor;

    public static implicit operator Area2D(BaseArea area) => area._area;
}
