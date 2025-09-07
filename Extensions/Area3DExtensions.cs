using Godot;

namespace GodotUtils;

public static class Area3DExtensions
{
    public static void SetMonitoringDeferred(this Area3D area, bool enabled)
    {
        area.SetDeferred(Area3D.PropertyName.Monitoring, enabled);
    }

    public static void SetMonitorableDeferred(this Area3D area, bool enabled)
    {
        area.SetDeferred(Area3D.PropertyName.Monitorable, enabled);
    }
}
