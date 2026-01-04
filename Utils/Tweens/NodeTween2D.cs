using Godot;

namespace GodotUtils;

/// <summary>
/// Provides tweening functionality for Node2D properties.
/// </summary>
public class NodeTween2D(Node2D node) : NodeTween(node)
{
    // Position
    public NodeTween2D AnimatePosition(Vector2 position, double duration) => (NodeTween2D)Animate(Node2D.PropertyName.Position, position, duration);
    public NodeTween2D AnimatePositionX(double x, double duration) => (NodeTween2D)Animate("position:x", x, duration);
    public NodeTween2D AnimatePositionY(double y, double duration) => (NodeTween2D)Animate("position:y", y, duration);
    public NodeTween2D AnimatePositionZ(double z, double duration) => (NodeTween2D)Animate("position:z", z, duration);
    public NodeTween2D AnimateGlobalPosition(Vector2 globalPosition, double duration) => (NodeTween2D)Animate(Node2D.PropertyName.GlobalPosition, globalPosition, duration);
    public NodeTween2D AnimateGlobalPositionX(double x, double duration) => (NodeTween2D)Animate("global_position:x", x, duration);
    public NodeTween2D AnimateGlobalPositionY(double y, double duration) => (NodeTween2D)Animate("global_position:y", y, duration);
    public NodeTween2D AnimateGlobalPositionZ(double z, double duration) => (NodeTween2D)Animate("global_position:z", z, duration);

    // Rotation
    public NodeTween2D AnimateRotation(double rotation, double duration) => (NodeTween2D)Animate(Node2D.PropertyName.Rotation, rotation, duration);
    public NodeTween2D AnimateRotationX(double x, double duration) => (NodeTween2D)Animate("rotation:x", x, duration);
    public NodeTween2D AnimateRotationY(double y, double duration) => (NodeTween2D)Animate("rotation:y", y, duration);
    public NodeTween2D AnimateRotationZ(double z, double duration) => (NodeTween2D)Animate("rotation:z", z, duration);
    public NodeTween2D AnimateGlobalRotation(double rotation, double duration) => (NodeTween2D)Animate(Node2D.PropertyName.GlobalRotation, rotation, duration);
    public NodeTween2D AnimateGlobalRotationX(double x, double duration) => (NodeTween2D)Animate("global_rotation:x", x, duration);
    public NodeTween2D AnimateGlobalRotationY(double y, double duration) => (NodeTween2D)Animate("global_rotation:y", y, duration);
    public NodeTween2D AnimateGlobalRotationZ(double z, double duration) => (NodeTween2D)Animate("global_rotation:z", z, duration);

    // Scale
    public NodeTween2D AnimateScale(Vector2 scale, double duration) => (NodeTween2D)Animate(Node2D.PropertyName.Scale, scale, duration);
    public NodeTween2D AnimateScaleX(double x, double duration) => (NodeTween2D)Animate("scale:x", x, duration);
    public NodeTween2D AnimateScaleY(double y, double duration) => (NodeTween2D)Animate("scale:y", y, duration);
    public NodeTween2D AnimateScaleZ(double z, double duration) => (NodeTween2D)Animate("scale:z", z, duration);
    public NodeTween2D AnimateGlobalScale(Vector2 globalScale, double duration) => (NodeTween2D)Animate(Node2D.PropertyName.GlobalScale, globalScale, duration);
    public NodeTween2D AnimateGlobalScaleX(double x, double duration) => (NodeTween2D)Animate("global_scale:x", x, duration);
    public NodeTween2D AnimateGlobalScaleY(double y, double duration) => (NodeTween2D)Animate("global_scale:y", y, duration);
    public NodeTween2D AnimateGlobalScaleZ(double z, double duration) => (NodeTween2D)Animate("global_scale:z", z, duration);

    // Color
    public NodeTween2D AnimateColor(Color color, double duration) => (NodeTween2D)Animate(CanvasItem.PropertyName.SelfModulate, color, duration);
    public NodeTween2D AnimateColorRecursive(Color color, double duration) => (NodeTween2D)Animate(CanvasItem.PropertyName.Modulate, color, duration);

    // Skew
    public NodeTween2D AnimateSkew(float skew, double duration) => (NodeTween2D)Animate(Node2D.PropertyName.Skew, skew, duration);
    public NodeTween2D AnimateGlobalSkew(float globalSkew, double duration) => (NodeTween2D)Animate(Node2D.PropertyName.GlobalSkew, globalSkew, duration);
}
