using GodotUtils.Debugging;
using GodotUtils.Framework;
using GodotUtils.Framework.UI;

namespace GodotUtils;

public static class GameFramework
{
    public static MetricsOverlay Metrics => AutoloadsFramework.Instance.MetricsOverlay;
    public static OptionsManager Options => AutoloadsFramework.Instance.OptionsManager;
}
