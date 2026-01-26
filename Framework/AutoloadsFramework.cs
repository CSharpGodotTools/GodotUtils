using Godot;
using GodotUtils.Debugging;

namespace GodotUtils.Framework;

public partial class AutoloadsFramework : Node
{
    public MetricsOverlay MetricsOverlay { get; }

    public AutoloadsFramework(AutoloadsFrameworkConfig config)
    {
        MetricsOverlay = new MetricsOverlay(config.MetricsToggleKeyAction);
    }

    public override void _Process(double delta)
    {
        MetricsOverlay.Update();
    }
}
