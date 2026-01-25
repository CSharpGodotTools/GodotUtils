using Godot;

namespace GodotUtils;

public class BoxArea3D : ShapeArea3D<BoxShape3D>
{
    public Vector3 Size
    {
        get => Shape.Size;
        set => Shape.Size = value;
    }

    internal BoxArea3D(Vector3 size) : base(new BoxShape3D { Size = size }) { }
}
