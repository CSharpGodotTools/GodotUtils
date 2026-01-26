using Godot;
using GodotUtils.Debugging;
using GodotUtils.Framework.UI;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GodotUtils.Framework;

public partial class AutoloadsFramework : Node
{
    public event Func<Task> PreQuit;

    public static AutoloadsFramework Instance { get; private set; }

    public MetricsOverlay MetricsOverlay { get; }
    public OptionsManager OptionsManager { get; }

    public AutoloadsFramework(AutoloadsFrameworkConfig config, IHotkeysFactory hotkeysFactory)
    {
        Instance = this;
        MetricsOverlay = new MetricsOverlay(config.MetricsToggleKeyAction);
        OptionsManager = new OptionsManager(this, config.FullscreenToggleKeyAction, hotkeysFactory);
    }

    public override void _Process(double delta)
    {
        MetricsOverlay.Update();
    }

    public async Task ExitGame()
    {
        GetTree().AutoAcceptQuit = false;

        // Wait for cleanup
        if (PreQuit != null)
        {
            // Since the PreQuit event contains a Task only the first subscriber will be invoked
            // with await PreQuit?.Invoke(); so need to ensure all subs are invoked.
            foreach (Func<Task> subscriber in PreQuit.GetInvocationList().Cast<Func<Task>>())
            {
                try
                {
                    await subscriber();
                }
                catch (Exception ex)
                {
                    GD.PrintErr($"PreQuit subscriber failed: {ex}");
                }
            }
        }

        GetTree().Quit();
    }
}
