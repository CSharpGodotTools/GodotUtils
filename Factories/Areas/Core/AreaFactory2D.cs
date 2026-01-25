using Godot;

namespace GodotUtils;

public static class AreaFactory2D
{
    public static WorldBoundaryArea2D WorldBoundary(Vector2 normal) => new(normal);
    public static CapsuleArea2D Capsule(float radius, float height) => new(radius, height);
    public static CircleArea2D Circle(float radius) => new(radius);
    public static RectArea2D Rect(Vector2 size) => new(size);
}
