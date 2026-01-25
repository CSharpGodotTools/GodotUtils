using Godot;

namespace GodotUtils;

public class SphereArea3D : ShapeArea3D<SphereShape3D>
{
    public float Radius
    {
        get => Shape.Radius;
        set => Shape.Radius = value;
    }

    internal SphereArea3D(float radius) : base(new SphereShape3D { Radius = radius })
    {
    }
}
