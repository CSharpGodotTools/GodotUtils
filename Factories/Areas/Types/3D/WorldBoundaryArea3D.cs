using Godot;

namespace GodotUtils;

public class WorldBoundaryArea3D : ShapeArea3D<WorldBoundaryShape3D>
{
    public Plane Plane
    {
        get => Shape.Plane;
        set => Shape.Plane = value;
    }

    internal WorldBoundaryArea3D(Plane plane) : base(new WorldBoundaryShape3D { Plane = plane })
    {
    }
}
