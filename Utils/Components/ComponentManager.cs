using Godot;
using System.Collections.Generic;

namespace GodotUtils;

/// <summary>
/// Centralized component dispatch to reduce per-node callbacks.
/// </summary>
public class ComponentManager
{
    /// <summary>
    /// Gets the active component manager instance.
    /// </summary>
    public static ComponentManager Instance { get; private set; }

    private readonly List<Component> _process = [];
    private readonly List<Component> _physicsProcess = [];
    private readonly List<Component> _unhandledInput = [];
    private readonly List<Component> _input = [];

    private readonly Node _managerNode;

    /// <summary>
    /// Creates a manager tied to the provided node.
    /// </summary>
    public ComponentManager(Node managerNode)
    {
        _managerNode = managerNode;
    }

    // Disable overrides on startup for performance
    /// <summary>
    /// Disables processing callbacks until components register.
    /// </summary>
    public void EnterTree()
    {
        //_managerNode.SetProcess(false); // Assume there will always be at least one process
        //_managerNode.SetPhysicsProcess(false); // Assume there will always be at least one physics process
        _managerNode.SetProcessInput(false);
        _managerNode.SetProcessUnhandledInput(false);
    }

    /// <summary>
    /// Marks this instance as the active manager.
    /// </summary>
    public void Ready()
    {
        Instance = this;
    }

    // Handle Godot overrides
    /// <summary>
    /// Dispatches per-frame processing to registered components.
    /// </summary>
    public void Process(double delta)
    {
        for (int i = _process.Count - 1; i >= 0; i--)
        {
            _process[i].Process(delta);
        }
    }

    /// <summary>
    /// Dispatches physics processing to registered components.
    /// </summary>
    public void PhysicsProcess(double delta)
    {
        for (int i = _physicsProcess.Count - 1; i >= 0; i--)
        {
            _physicsProcess[i].PhysicsProcess(delta);
        }
    }

    /// <summary>
    /// Dispatches input events to registered components.
    /// </summary>
    public void Input(InputEvent @event)
    {
        for (int i = _input.Count - 1; i >= 0; i--)
        {
            _input[i].ProcessInput(@event);
        }
    }

    /// <summary>
    /// Dispatches unhandled input events to registered components.
    /// </summary>
    public void UnhandledInput(InputEvent @event)
    {
        for (int i = _unhandledInput.Count - 1; i >= 0; i--)
        {
            _unhandledInput[i].UnhandledInput(@event);
        }
    }

    // Exposed register functions
    /// <summary>
    /// Registers a component for per-frame processing.
    /// </summary>
    public void RegisterProcess(Component component)
    {
        if (_process.Contains(component))
            return;

        _process.Add(component);

        // Assume there will always be at least one process
        //if (_process.Count == 1)
        //    SetProcess(true);
    }

    /// <summary>
    /// Unregisters a component from per-frame processing.
    /// </summary>
    public void UnregisterProcess(Component component)
    {
        _process.Remove(component);

        // Assume there will always be at least one process
        //if (_process.Count == 0)
        //    SetProcess(false);
    }

    /// <summary>
    /// Registers a component for physics processing.
    /// </summary>
    public void RegisterPhysicsProcess(Component component)
    {
        if (_physicsProcess.Contains(component))
            return;

        _physicsProcess.Add(component);

        // Assume there will always be at least one physics process
        //if (_physicsProcess.Count == 1)
        //    SetPhysicsProcess(true);
    }

    /// <summary>
    /// Unregisters a component from physics processing.
    /// </summary>
    public void UnregisterPhysicsProcess(Component component)
    {
        _physicsProcess.Remove(component);

        // Assume there will always be at least one physics process
        //if (_physicsProcess.Count == 0)
        //    SetPhysicsProcess(false);
    }

    /// <summary>
    /// Registers a component for input processing.
    /// </summary>
    public void RegisterInput(Component component)
    {
        if (_input.Contains(component))
            return;

        _input.Add(component);

        if (_input.Count == 1)
            _managerNode.SetProcessInput(true);
    }

    /// <summary>
    /// Unregisters a component from input processing.
    /// </summary>
    public void UnregisterInput(Component component)
    {
        _input.Remove(component);

        if (_input.Count == 0)
            _managerNode.SetProcessInput(false);
    }

    /// <summary>
    /// Registers a component for unhandled input processing.
    /// </summary>
    public void RegisterUnhandledInput(Component component)
    {
        if (_unhandledInput.Contains(component))
            return;

        _unhandledInput.Add(component);

        if (_unhandledInput.Count == 1)
            _managerNode.SetProcessUnhandledInput(true);
    }

    /// <summary>
    /// Unregisters a component from unhandled input processing.
    /// </summary>
    public void UnregisterUnhandledInput(Component component)
    {
        _unhandledInput.Remove(component);

        if (_unhandledInput.Count == 0)
            _managerNode.SetProcessUnhandledInput(false);
    }

    /// <summary>
    /// Unregisters a component from all processing types.
    /// </summary>
    public void UnregisterAll(Component component)
    {
        UnregisterProcess(component);
        UnregisterPhysicsProcess(component);
        UnregisterInput(component);
        UnregisterUnhandledInput(component);
    }
}
