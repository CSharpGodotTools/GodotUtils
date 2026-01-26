using GodotUtils.Debugging;
using GodotUtils.Framework;
using GodotUtils.Framework.Debugging;
using GodotUtils.Framework.UI;
using GodotUtils.Framework.UI.Console;

namespace GodotUtils;

public static class GameFramework
{
    public static GameConsoleFramework Console => AutoloadsFramework.Instance.GameConsole;
    public static OptionsManager Options => AutoloadsFramework.Instance.OptionsManager;
    public static AudioManager Audio => AutoloadsFramework.Instance.AudioManager;
    public static Services Services => AutoloadsFramework.Instance.Services;
    public static Logger Logger => AutoloadsFramework.Instance.Logger;
    
    public static class Debug
    {
        public static MetricsOverlay Metrics => AutoloadsFramework.Instance.MetricsOverlay;
        public static Profiler Profiler => AutoloadsFramework.Instance.Profiler;
    }
}
