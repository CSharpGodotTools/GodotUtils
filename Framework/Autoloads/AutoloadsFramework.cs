using Godot;
using GodotUtils.Debugging;
using GodotUtils.Framework.Debugging;
using GodotUtils.Framework.UI;
using GodotUtils.Framework.UI.Console;
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

    public AudioManager AudioManager { get; private set; }
    public MetricsOverlay MetricsOverlay { get; private set; }
    public OptionsManager OptionsManager { get; private set; }
    public GameConsoleFramework GameConsole { get; private set; }
    public Services Services { get; private set; }
#if NETCODE_ENABLED
    public Logger Logger { get; private set; }
#endif
    public Profiler Profiler { get; }

    private readonly Node _autoloadsNode;
    private readonly AutoloadsFrameworkConfig _config;
    private readonly IHotkeysFactory _hotkeysFactory;
    private ISceneManager _sceneManager;

    public AutoloadsFramework(Node autoloadsNode, AutoloadsFrameworkConfig config, IHotkeysFactory hotkeysFactory)
    {
        Instance = this;
        _autoloadsNode = autoloadsNode;
        _config = config;
        _hotkeysFactory = hotkeysFactory;
        Profiler = new Profiler();
    }

    public void EnterTree(Node gameConsole, ISceneManager sceneManager)
    {
        _sceneManager = sceneManager;
        GameConsole = new GameConsoleFramework(gameConsole, _config.ConsoleToggleKeyAction, _config.UpKeyAction, _config.DownKeyAction);
        MetricsOverlay = new MetricsOverlay(_config.MetricsToggleKeyAction);
        OptionsManager = new OptionsManager(this, _config.FullscreenToggleKeyAction, _hotkeysFactory);
        AudioManager = new AudioManager(_autoloadsNode);
        Services = new Services(_sceneManager);
    }
    
    public void Ready()
    {
        Logger = new Logger(GameConsole);
        GameConsole.Ready();
    }

    public void Update()
    {
        MetricsOverlay.Update();
#if NETCODE_ENABLED
        Logger.Update();
#endif
        GameConsole.Process();
    }

    public void Notification(int what)
    {
        if (what == Node.NotificationWMCloseRequest)
        {
            ExitGame().FireAndForget();
        }
    }

    public void ExitTree()
    {
        AudioManager.Dispose();
        OptionsManager.Dispose();
#if NETCODE_ENABLED
        Logger.Dispose();
#endif
        GameConsole.ExitTree();
        Instance = null;
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
