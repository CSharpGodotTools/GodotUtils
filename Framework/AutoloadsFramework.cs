using Godot;
using GodotUtils.Debugging;
using GodotUtils.Framework.UI;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GodotUtils.Framework;

// This must be sealed because if an autoload script in the Godot assembly inherits from a node type in a
// separate assembly then Godot will fail to load its assemblies everytime the external Godot assembly is built
// and will require a Godot engine restart everytime. By making this class sealed we entirely prevent this issue
// and force a component design approach.
public sealed class AutoloadsFramework
{
    public event Func<Task> PreQuit;

    public static AutoloadsFramework Instance { get; private set; }

    public MetricsOverlay MetricsOverlay { get; }
    public OptionsManager OptionsManager { get; }

    private readonly Node _autoloadsNode;

    public AutoloadsFramework(Node autoloadsNode, AutoloadsFrameworkConfig config, IHotkeysFactory hotkeysFactory)
    {
        _autoloadsNode = autoloadsNode;

        Instance = this;
        MetricsOverlay = new MetricsOverlay(config.MetricsToggleKeyAction);
        OptionsManager = new OptionsManager(this, config.FullscreenToggleKeyAction, hotkeysFactory);
    }

    public void Update()
    {
        MetricsOverlay.Update();
    }

    public async Task ExitGame()
    {
        _autoloadsNode.GetTree().AutoAcceptQuit = false;

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

        _autoloadsNode.GetTree().Quit();
    }
}
