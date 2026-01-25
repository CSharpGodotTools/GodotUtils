using Godot;

namespace GodotUtils;

public class CylinderArea3D : ShapeArea3D<CylinderShape3D>
{
    public float Radius
    {
        get => Shape.Radius;
        set => Shape.Radius = value;
    }

    public float Height
    {
        get => Shape.Height;
        set => Shape.Height = value;
    }

    internal CylinderArea3D(float radius, float height) : base(new CylinderShape3D { Radius = radius, Height = height })
    {
    }
}
