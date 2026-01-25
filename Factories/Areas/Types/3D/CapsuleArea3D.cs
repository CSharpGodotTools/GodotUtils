using Godot;

namespace GodotUtils;

public class CapsuleArea3D : ShapeArea3D<CapsuleShape3D>
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

    internal CapsuleArea3D(float radius, float height) : base(new CapsuleShape3D { Radius = radius, Height = height })
    {
    }
}
