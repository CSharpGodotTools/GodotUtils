using Godot;

namespace GodotUtils;

/// <summary>
/// Provides tweening functionality for Node2D properties.
/// </summary>
public class NodeTween2D : BaseTween<NodeTween2D>
{
    protected override NodeTween2D Self => this;

    internal NodeTween2D(Node2D node) : base(node)
    { 
    }

    // Position
    public NodeTween2D Position(Vector2 position, double duration) => (NodeTween2D)Property(Node2D.PropertyName.Position, position, duration);
    public NodeTween2D PositionX(double x, double duration) => (NodeTween2D)Property("position:x", x, duration);
    public NodeTween2D PositionY(double y, double duration) => (NodeTween2D)Property("position:y", y, duration);
    public NodeTween2D PositionZ(double z, double duration) => (NodeTween2D)Property("position:z", z, duration);
    public NodeTween2D GlobalPosition(Vector2 globalPosition, double duration) => (NodeTween2D)Property(Node2D.PropertyName.GlobalPosition, globalPosition, duration);
    public NodeTween2D GlobalPositionX(double x, double duration) => (NodeTween2D)Property("global_position:x", x, duration);
    public NodeTween2D GlobalPositionY(double y, double duration) => (NodeTween2D)Property("global_position:y", y, duration);
    public NodeTween2D GlobalPositionZ(double z, double duration) => (NodeTween2D)Property("global_position:z", z, duration);

    // Rotation
    public NodeTween2D Rotation(double rotation, double duration) => (NodeTween2D)Property(Node2D.PropertyName.Rotation, rotation, duration);
    public NodeTween2D RotationX(double x, double duration) => (NodeTween2D)Property("rotation:x", x, duration);
    public NodeTween2D RotationY(double y, double duration) => (NodeTween2D)Property("rotation:y", y, duration);
    public NodeTween2D RotationZ(double z, double duration) => (NodeTween2D)Property("rotation:z", z, duration);
    public NodeTween2D GlobalRotation(double rotation, double duration) => (NodeTween2D)Property(Node2D.PropertyName.GlobalRotation, rotation, duration);
    public NodeTween2D GlobalRotationX(double x, double duration) => (NodeTween2D)Property("global_rotation:x", x, duration);
    public NodeTween2D GlobalRotationY(double y, double duration) => (NodeTween2D)Property("global_rotation:y", y, duration);
    public NodeTween2D GlobalRotationZ(double z, double duration) => (NodeTween2D)Property("global_rotation:z", z, duration);

    // Scale
    public NodeTween2D Scale(Vector2 scale, double duration) => (NodeTween2D)Property(Node2D.PropertyName.Scale, scale, duration);
    public NodeTween2D ScaleX(double x, double duration) => (NodeTween2D)Property("scale:x", x, duration);
    public NodeTween2D ScaleY(double y, double duration) => (NodeTween2D)Property("scale:y", y, duration);
    public NodeTween2D ScaleZ(double z, double duration) => (NodeTween2D)Property("scale:z", z, duration);
    public NodeTween2D GlobalScale(Vector2 globalScale, double duration) => (NodeTween2D)Property(Node2D.PropertyName.GlobalScale, globalScale, duration);
    public NodeTween2D GlobalScaleX(double x, double duration) => (NodeTween2D)Property("global_scale:x", x, duration);
    public NodeTween2D GlobalScaleY(double y, double duration) => (NodeTween2D)Property("global_scale:y", y, duration);
    public NodeTween2D GlobalScaleZ(double z, double duration) => (NodeTween2D)Property("global_scale:z", z, duration);

    // Color
    public NodeTween2D Color(Color color, double duration) => (NodeTween2D)Property(CanvasItem.PropertyName.SelfModulate, color, duration);
    public NodeTween2D ColorRecursive(Color color, double duration) => (NodeTween2D)Property(CanvasItem.PropertyName.Modulate, color, duration);

    // Skew
    public NodeTween2D Skew(float skew, double duration) => (NodeTween2D)Property(Node2D.PropertyName.Skew, skew, duration);
    public NodeTween2D GlobalSkew(float globalSkew, double duration) => (NodeTween2D)Property(Node2D.PropertyName.GlobalSkew, globalSkew, duration);
}
