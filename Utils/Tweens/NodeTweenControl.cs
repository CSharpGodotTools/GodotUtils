using Godot;

namespace GodotUtils;

/// <summary>
/// Provides tweening functionality for Control properties.
/// </summary>
public class NodeTweenControl(Control control) : NodeTween(control)
{
    // Position
    public NodeTween AnimatePosition(Vector2 position, double duration) => Animate(Control.PropertyName.Position, position, duration);
    public NodeTween AnimatePositionX(double x, double duration) => Animate("position:x", x, duration);
    public NodeTween AnimatePositionY(double y, double duration) => Animate("position:y", y, duration);
    public NodeTween AnimatePositionZ(double z, double duration) => Animate("position:z", z, duration);
    public NodeTween AnimateGlobalPosition(Vector2 globalPosition, double duration) => Animate(Control.PropertyName.GlobalPosition, globalPosition, duration);
    public NodeTween AnimateGlobalPositionX(double x, double duration) => Animate("global_position:x", x, duration);
    public NodeTween AnimateGlobalPositionY(double y, double duration) => Animate("global_position:y", y, duration);
    public NodeTween AnimateGlobalPositionZ(double z, double duration) => Animate("global_position:z", z, duration);

    // Rotation
    public NodeTween AnimateRotation(double rotation, double duration) => Animate(Control.PropertyName.Rotation, rotation, duration);
    public NodeTween AnimateRotationX(double x, double duration) => Animate("rotation:x", x, duration);
    public NodeTween AnimateRotationY(double y, double duration) => Animate("rotation:y", y, duration);
    public NodeTween AnimateRotationZ(double z, double duration) => Animate("rotation:z", z, duration);

    // Scale
    public NodeTween AnimateScale(Vector2 scale, double duration) => Animate(Control.PropertyName.Scale, scale, duration);
    public NodeTween AnimateScaleX(double x, double duration) => Animate("scale:x", x, duration);
    public NodeTween AnimateScaleY(double y, double duration) => Animate("scale:y", y, duration);
    public NodeTween AnimateScaleZ(double z, double duration) => Animate("scale:z", z, duration);

    // Size
    public NodeTween AnimateSize(Vector2 size, double duration) => Animate(Control.PropertyName.Size, size, duration);
    public NodeTween AnimateCustomMinimumSize(Vector2 customMinimumSize, double duration) => Animate(Control.PropertyName.CustomMinimumSize, customMinimumSize, duration);
    public NodeTween AnimatePivotOffset(Vector2 pivotOffset, double duration) => Animate(Control.PropertyName.PivotOffset, pivotOffset, duration);
    public NodeTween AnimateSizeFlagsStretchRatio(float stretchRatio, double duration) => Animate(Control.PropertyName.SizeFlagsStretchRatio, stretchRatio, duration);
}
