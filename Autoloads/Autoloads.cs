using __TEMPLATE__.UI;
using Godot;
using GodotUtils.UI;
using GodotUtils.UI.Console;
using GodotUtils.Debugging;
#if DEBUG
using GodotUtils.Debugging.Visualize;
#endif
using System;
using System.Threading.Tasks;

namespace GodotUtils;

// Autoload
// Access this with GetNode<Autoloads>("/root/Autoloads")
public partial class Autoloads : Node
{
    [Export] private Scenes _scenes;

    public event Func<Task> PreQuit;

    public static Autoloads Instance { get; private set; }

    // Game developers should be able to access each individual manager
    public ComponentManager ComponentManager { get; private set; }
    public AudioManager     AudioManager     { get; private set; }
    public OptionsManager   OptionsManager   { get; private set; }
    public Services         Services         { get; private set; }
    public MetricsOverlay   MetricsOverlay   { get; private set; }
    public SceneManager     SceneManager     { get; private set; }
    public GameConsole      GameConsole      { get; private set; }

    public override void _EnterTree()
    {
        if (Instance != null)
            throw new InvalidOperationException("Global has been initialized already");

        Instance = this;
        ComponentManager = GetNode<ComponentManager>("ComponentManager");
        GameConsole = GetNode<GameConsole>("%Console");
        SceneManager = new SceneManager(this, _scenes);
        Services = new Services(this);

#if NETCODE_ENABLED
        _ = new Logger(GameConsole);
#endif
    }

    public override void _Ready()
    {
        CommandLineArgs.Init();
        Commands.RegisterAll();

        OptionsManager = new OptionsManager(this);
        AudioManager = new AudioManager(this);
        MetricsOverlay = new MetricsOverlay(this);

#if DEBUG
        _ = new VisualizeAutoload(this);
#endif
    }

    public override async void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            await QuitAndCleanup();
        }
    }

    public override void _ExitTree()
    {
        SceneManager.Dispose();

        Profiler.Dispose();

        Instance = null;
        PreQuit = null;
    }

    // Using deferred is always complicated...
    public void DeferredSwitchSceneProxy(string rawName, Variant transTypeVariant)
    {
        if (SceneManager.Instance == null)
            return;

        SceneManager.Instance.DeferredSwitchScene(rawName, transTypeVariant);
    }

    public async Task QuitAndCleanup()
    {
        GetTree().AutoAcceptQuit = false;

        // Wait for cleanup
        if (PreQuit != null)
            await PreQuit?.Invoke();

        GetTree().Quit();
    }
}
