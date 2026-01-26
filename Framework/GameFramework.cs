using GodotUtils.Debugging;
using GodotUtils.Framework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotUtils.Framework;

public class GameFramework
{
    public static MetricsOverlay Metrics => AutoloadsFramework.Instance.MetricsOverlay;
    public static OptionsManager Options => AutoloadsFramework.Instance.OptionsManager;
}
