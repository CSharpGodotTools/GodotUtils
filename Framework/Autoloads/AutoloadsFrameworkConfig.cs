using Godot;

namespace GodotUtils.Framework;

public sealed class AutoloadsFrameworkConfig
{
    public required StringName MetricsToggleKeyAction { get; init; }
    public required StringName FullscreenToggleKeyAction { get; init; }
    public required StringName ConsoleToggleKeyAction { get; init; }
    public required StringName UpKeyAction { get; init; }
    public required StringName DownKeyAction { get; init; }
}
