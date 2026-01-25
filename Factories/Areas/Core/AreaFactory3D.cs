using Godot;

namespace GodotUtils;

public static class AreaFactory3D
{
    public static WorldBoundaryArea3D CreateWorldBoundary(Plane plane) => new(plane);
    public static CylinderArea3D CreateCylinder(float radius, float height) => new(radius, height);
    public static CapsuleArea3D CreateCapsule(float radius, float height) => new(radius, height);
    public static SphereArea3D CreateSphere(float radius) => new(radius);
    public static BoxArea3D CreateBox(Vector3 size) => new(size);
}
