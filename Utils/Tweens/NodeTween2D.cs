using Godot;

namespace GodotUtils;

/// <summary>
/// Provides tweening functionality for Node2D properties.
/// </summary>
public class NodeTween2D(Node2D node) : NodeTween(node)
{
    // Position
    public NodeTween AnimatePosition(Vector2 position, double duration) => Animate(Node2D.PropertyName.Position, position, duration);
    public NodeTween AnimatePositionX(double x, double duration) => Animate("position:x", x, duration);
    public NodeTween AnimatePositionY(double y, double duration) => Animate("position:y", y, duration);
    public NodeTween AnimatePositionZ(double z, double duration) => Animate("position:z", z, duration);
    public NodeTween AnimateGlobalPosition(Vector2 globalPosition, double duration) => Animate(Node2D.PropertyName.GlobalPosition, globalPosition, duration);
    public NodeTween AnimateGlobalPositionX(double x, double duration) => Animate("global_position:x", x, duration);
    public NodeTween AnimateGlobalPositionY(double y, double duration) => Animate("global_position:y", y, duration);
    public NodeTween AnimateGlobalPositionZ(double z, double duration) => Animate("global_position:z", z, duration);

    // Rotation
    public NodeTween AnimateRotation(double rotation, double duration) => Animate(Node2D.PropertyName.Rotation, rotation, duration);
    public NodeTween AnimateRotationX(double x, double duration) => Animate("rotation:x", x, duration);
    public NodeTween AnimateRotationY(double y, double duration) => Animate("rotation:y", y, duration);
    public NodeTween AnimateRotationZ(double z, double duration) => Animate("rotation:z", z, duration);
    public NodeTween AnimateGlobalRotation(double rotation, double duration) => Animate(Node2D.PropertyName.GlobalRotation, rotation, duration);
    public NodeTween AnimateGlobalRotationX(double x, double duration) => Animate("global_rotation:x", x, duration);
    public NodeTween AnimateGlobalRotationY(double y, double duration) => Animate("global_rotation:y", y, duration);
    public NodeTween AnimateGlobalRotationZ(double z, double duration) => Animate("global_rotation:z", z, duration);

    // Scale
    public NodeTween AnimateScale(Vector2 scale, double duration) => Animate(Node2D.PropertyName.Scale, scale, duration);
    public NodeTween AnimateScaleX(double x, double duration) => Animate("scale:x", x, duration);
    public NodeTween AnimateScaleY(double y, double duration) => Animate("scale:y", y, duration);
    public NodeTween AnimateScaleZ(double z, double duration) => Animate("scale:z", z, duration);
    public NodeTween AnimateGlobalScale(Vector2 globalScale, double duration) => Animate(Node2D.PropertyName.GlobalScale, globalScale, duration);
    public NodeTween AnimateGlobalScaleX(double x, double duration) => Animate("global_scale:x", x, duration);
    public NodeTween AnimateGlobalScaleY(double y, double duration) => Animate("global_scale:y", y, duration);
    public NodeTween AnimateGlobalScaleZ(double z, double duration) => Animate("global_scale:z", z, duration);

    // Skew
    public NodeTween AnimateSkew(float skew, double duration) => Animate(Node2D.PropertyName.Skew, skew, duration);
    public NodeTween AnimateGlobalSkew(float globalSkew, double duration) => Animate(Node2D.PropertyName.GlobalSkew, globalSkew, duration);
}
