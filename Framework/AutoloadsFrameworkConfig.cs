using Godot;

namespace GodotUtils.Framework;

public sealed class AutoloadsFrameworkConfig
{
    public required StringName MetricsToggleKeyAction { get; init; }
    public required StringName FullscreenToggleKeyAction { get; init; }
}
