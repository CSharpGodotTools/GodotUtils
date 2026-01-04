using Godot;

namespace GodotUtils;

/// <summary>
/// Provides tweening functionality for Control properties.
/// </summary>
public class NodeTweenControl(Control control) : NodeTween(control)
{
    // Position
    public NodeTweenControl AnimatePosition(Vector2 position, double duration) => (NodeTweenControl)Animate(Control.PropertyName.Position, position, duration);
    public NodeTweenControl AnimatePositionX(double x, double duration) => (NodeTweenControl)Animate("position:x", x, duration);
    public NodeTweenControl AnimatePositionY(double y, double duration) => (NodeTweenControl)Animate("position:y", y, duration);
    public NodeTweenControl AnimatePositionZ(double z, double duration) => (NodeTweenControl)Animate("position:z", z, duration);
    public NodeTweenControl AnimateGlobalPosition(Vector2 globalPosition, double duration) => (NodeTweenControl)Animate(Control.PropertyName.GlobalPosition, globalPosition, duration);
    public NodeTweenControl AnimateGlobalPositionX(double x, double duration) => (NodeTweenControl)Animate("global_position:x", x, duration);
    public NodeTweenControl AnimateGlobalPositionY(double y, double duration) => (NodeTweenControl)Animate("global_position:y", y, duration);
    public NodeTweenControl AnimateGlobalPositionZ(double z, double duration) => (NodeTweenControl)Animate("global_position:z", z, duration);

    // Rotation
    public NodeTweenControl AnimateRotation(double rotation, double duration) => (NodeTweenControl)Animate(Control.PropertyName.Rotation, rotation, duration);
    public NodeTweenControl AnimateRotationX(double x, double duration) => (NodeTweenControl)Animate("rotation:x", x, duration);
    public NodeTweenControl AnimateRotationY(double y, double duration) => (NodeTweenControl)Animate("rotation:y", y, duration);
    public NodeTweenControl AnimateRotationZ(double z, double duration) => (NodeTweenControl)Animate("rotation:z", z, duration);

    // Scale
    public NodeTweenControl AnimateScale(Vector2 scale, double duration) => (NodeTweenControl)Animate(Control.PropertyName.Scale, scale, duration);
    public NodeTweenControl AnimateScaleX(double x, double duration) => (NodeTweenControl)Animate("scale:x", x, duration);
    public NodeTweenControl AnimateScaleY(double y, double duration) => (NodeTweenControl)Animate("scale:y", y, duration);
    public NodeTweenControl AnimateScaleZ(double z, double duration) => (NodeTweenControl)Animate("scale:z", z, duration);

    // Size
    public NodeTweenControl AnimateSize(Vector2 size, double duration) => (NodeTweenControl)Animate(Control.PropertyName.Size, size, duration);
    public NodeTweenControl AnimateCustomMinimumSize(Vector2 customMinimumSize, double duration) => (NodeTweenControl)Animate(Control.PropertyName.CustomMinimumSize, customMinimumSize, duration);
    public NodeTweenControl AnimatePivotOffset(Vector2 pivotOffset, double duration) => (NodeTweenControl)Animate(Control.PropertyName.PivotOffset, pivotOffset, duration);
    public NodeTweenControl AnimateSizeFlagsStretchRatio(float stretchRatio, double duration) => (NodeTweenControl)Animate(Control.PropertyName.SizeFlagsStretchRatio, stretchRatio, duration);

    // Color
    public NodeTweenControl AnimateColor(Color color, double duration) => (NodeTweenControl)Animate(CanvasItem.PropertyName.SelfModulate, color, duration);
    public NodeTweenControl AnimateColorRecursive(Color color, double duration) => (NodeTweenControl)Animate(CanvasItem.PropertyName.Modulate, color, duration);
}
