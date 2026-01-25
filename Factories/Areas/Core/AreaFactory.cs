using Godot;

namespace GodotUtils;

public static class AreaFactory
{
    // 2D
    public static WorldBoundaryArea2D CreateWorldBoundary2D(Vector2 normal) => new(normal);
    public static CapsuleArea2D CreateCapsule2D(float radius, float height) => new(radius, height);
    public static CircleArea2D CreateCircle2D(float radius) => new(radius);
    public static RectArea2D CreateRect2D(Vector2 size) => new(size);

    // 3D
    public static WorldBoundaryArea3D CreateWorldBoundary3D(Plane plane) => new(plane);
    public static CylinderArea3D CreateCylinder3D(float radius, float height) => new(radius, height);
    public static CapsuleArea3D CreateCapsule3D(float radius, float height) => new(radius, height);
    public static SphereArea3D CreateSphere3D(float radius) => new(radius);
    public static BoxArea3D CreateBox3D(Vector3 size) => new(size);
}
