using Godot;
using System.Collections.Generic;

namespace GodotUtils;

/// <summary>
/// Using any kind of Godot functions from C# is expensive, so we try to minimize this with centralized logic.
/// See <see href="https://www.reddit.com/r/godot/comments/1me7669/a_follow_up_to_my_first_c_stress_test/">stress test results</see>.
/// </summary>
public class ComponentManager
{
    public static ComponentManager Instance { get; private set; }

    private readonly List<Component> _process = [];
    private readonly List<Component> _physicsProcess = [];
    private readonly List<Component> _unhandledInput = [];
    private readonly List<Component> _input = [];

    private readonly Node _managerNode;

    public ComponentManager(Node managerNode)
    {
        _managerNode = managerNode;
    }

    // Disable overrides on startup for performance
    public void EnterTree()
    {
        //_managerNode.SetProcess(false); // Assume there will always be at least one process
        //_managerNode.SetPhysicsProcess(false); // Assume there will always be at least one physics process
        _managerNode.SetProcessInput(false);
        _managerNode.SetProcessUnhandledInput(false);
    }

    public void Ready()
    {
        Instance = this;
    }

    // Handle Godot overrides
    public void Process(double delta)
    {
        for (int i = _process.Count - 1; i >= 0; i--)
        {
            _process[i].Process(delta);
        }
    }

    public void PhysicsProcess(double delta)
    {
        for (int i = _physicsProcess.Count - 1; i >= 0; i--)
        {
            _physicsProcess[i].PhysicsProcess(delta);
        }
    }

    public void Input(InputEvent @event)
    {
        for (int i = _input.Count - 1; i >= 0; i--)
        {
            _input[i].ProcessInput(@event);
        }
    }

    public void UnhandledInput(InputEvent @event)
    {
        for (int i = _unhandledInput.Count - 1; i >= 0; i--)
        {
            _unhandledInput[i].UnhandledInput(@event);
        }
    }

    // Exposed register functions
    public void RegisterProcess(Component component)
    {
        if (_process.Contains(component))
            return;

        _process.Add(component);

        // Assume there will always be at least one process
        //if (_process.Count == 1)
        //    SetProcess(true);
    }

    public void UnregisterProcess(Component component)
    {
        _process.Remove(component);

        // Assume there will always be at least one process
        //if (_process.Count == 0)
        //    SetProcess(false);
    }

    public void RegisterPhysicsProcess(Component component)
    {
        if (_physicsProcess.Contains(component))
            return;

        _physicsProcess.Add(component);

        // Assume there will always be at least one physics process
        //if (_physicsProcess.Count == 1)
        //    SetPhysicsProcess(true);
    }

    public void UnregisterPhysicsProcess(Component component)
    {
        _physicsProcess.Remove(component);

        // Assume there will always be at least one physics process
        //if (_physicsProcess.Count == 0)
        //    SetPhysicsProcess(false);
    }

    public void RegisterInput(Component component)
    {
        if (_input.Contains(component))
            return;

        _input.Add(component);

        if (_input.Count == 1)
            _managerNode.SetProcessInput(true);
    }

    public void UnregisterInput(Component component)
    {
        _input.Remove(component);

        if (_input.Count == 0)
            _managerNode.SetProcessInput(false);
    }

    public void RegisterUnhandledInput(Component component)
    {
        if (_unhandledInput.Contains(component))
            return;

        _unhandledInput.Add(component);

        if (_unhandledInput.Count == 1)
            _managerNode.SetProcessUnhandledInput(true);
    }

    public void UnregisterUnhandledInput(Component component)
    {
        _unhandledInput.Remove(component);

        if (_unhandledInput.Count == 0)
            _managerNode.SetProcessUnhandledInput(false);
    }

    public void UnregisterAll(Component component)
    {
        UnregisterProcess(component);
        UnregisterPhysicsProcess(component);
        UnregisterInput(component);
        UnregisterUnhandledInput(component);
    }
}
