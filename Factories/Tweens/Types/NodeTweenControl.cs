using Godot;

namespace GodotUtils;

/// <summary>
/// Provides tweening functionality for Control properties.
/// </summary>
public class NodeTweenControl : BaseTween<NodeTweenControl>
{
    protected override NodeTweenControl Self => this;

    internal NodeTweenControl(Control control) : base(control)
    {
    }

    // Position
    public NodeTweenControl Position(Vector2 position, double duration) => (NodeTweenControl)Property(Control.PropertyName.Position, position, duration);
    public NodeTweenControl PositionX(double x, double duration) => (NodeTweenControl)Property("position:x", x, duration);
    public NodeTweenControl PositionY(double y, double duration) => (NodeTweenControl)Property("position:y", y, duration);
    public NodeTweenControl PositionZ(double z, double duration) => (NodeTweenControl)Property("position:z", z, duration);
    public NodeTweenControl GlobalPosition(Vector2 globalPosition, double duration) => (NodeTweenControl)Property(Control.PropertyName.GlobalPosition, globalPosition, duration);
    public NodeTweenControl GlobalPositionX(double x, double duration) => (NodeTweenControl)Property("global_position:x", x, duration);
    public NodeTweenControl GlobalPositionY(double y, double duration) => (NodeTweenControl)Property("global_position:y", y, duration);
    public NodeTweenControl GlobalPositionZ(double z, double duration) => (NodeTweenControl)Property("global_position:z", z, duration);

    // Rotation
    public NodeTweenControl Rotation(double rotation, double duration) => (NodeTweenControl)Property(Control.PropertyName.Rotation, rotation, duration);
    public NodeTweenControl RotationX(double x, double duration) => (NodeTweenControl)Property("rotation:x", x, duration);
    public NodeTweenControl RotationY(double y, double duration) => (NodeTweenControl)Property("rotation:y", y, duration);
    public NodeTweenControl RotationZ(double z, double duration) => (NodeTweenControl)Property("rotation:z", z, duration);

    // Scale
    public NodeTweenControl Scale(Vector2 scale, double duration) => (NodeTweenControl)Property(Control.PropertyName.Scale, scale, duration);
    public NodeTweenControl ScaleX(double x, double duration) => (NodeTweenControl)Property("scale:x", x, duration);
    public NodeTweenControl ScaleY(double y, double duration) => (NodeTweenControl)Property("scale:y", y, duration);
    public NodeTweenControl ScaleZ(double z, double duration) => (NodeTweenControl)Property("scale:z", z, duration);

    // Size
    public NodeTweenControl Size(Vector2 size, double duration) => (NodeTweenControl)Property(Control.PropertyName.Size, size, duration);
    public NodeTweenControl CustomMinimumSize(Vector2 customMinimumSize, double duration) => (NodeTweenControl)Property(Control.PropertyName.CustomMinimumSize, customMinimumSize, duration);
    public NodeTweenControl PivotOffset(Vector2 pivotOffset, double duration) => (NodeTweenControl)Property(Control.PropertyName.PivotOffset, pivotOffset, duration);
    public NodeTweenControl SizeFlagsStretchRatio(float stretchRatio, double duration) => (NodeTweenControl)Property(Control.PropertyName.SizeFlagsStretchRatio, stretchRatio, duration);

    // Color
    public NodeTweenControl Color(Color color, double duration) => (NodeTweenControl)Property(CanvasItem.PropertyName.SelfModulate, color, duration);
    public NodeTweenControl ColorRecursive(Color color, double duration) => (NodeTweenControl)Property(CanvasItem.PropertyName.Modulate, color, duration);
}
