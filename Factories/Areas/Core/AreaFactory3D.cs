using Godot;

namespace GodotUtils;

public static class AreaFactory3D
{
    public static WorldBoundaryArea3D WorldBoundary(Plane plane) => new(plane);
    public static CylinderArea3D Cylinder(float radius, float height) => new(radius, height);
    public static CapsuleArea3D Capsule(float radius, float height) => new(radius, height);
    public static SphereArea3D Sphere(float radius) => new(radius);
    public static BoxArea3D Box(Vector3 size) => new(size);
}
