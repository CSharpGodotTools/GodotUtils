using Godot;

namespace GodotUtils;

public static class ExtensionsArea2D
{
    public static void SetMonitoringDeferred(this Area2D area, bool enabled)
    {
        area.SetDeferred(Area2D.PropertyName.Monitoring, enabled);
    }

    public static void SetMonitorableDeferred(this Area2D area, bool enabled)
    {
        area.SetDeferred(Area2D.PropertyName.Monitorable, enabled);
    }
}
