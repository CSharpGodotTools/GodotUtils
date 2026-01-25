using Godot;

namespace GodotUtils;

public static class AreaFactory2D
{
    public static WorldBoundaryArea2D CreateWorldBoundary(Vector2 normal) => new(normal);
    public static CapsuleArea2D CreateCapsule(float radius, float height) => new(radius, height);
    public static CircleArea2D CreateCircle(float radius) => new(radius);
    public static RectArea2D CreateRect(Vector2 size) => new(size);
}
