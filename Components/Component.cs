using Godot;
using System;

namespace GodotUtils;

public class Component : IDisposable
{
    protected Node Owner;
    private ComponentManager _componentManager;

    public Component(Node owner)
    {
        Owner = owner;
        Owner.Ready += RunCodeOnReady;
        Owner.TreeExited += UnsubscribeOnTreeExit;
    }

    public virtual void Ready() { }
    public virtual void Process(double delta) { }
    public virtual void PhysicsProcess(double delta) { }
    public virtual void ProcessInput(InputEvent @event) { }
    public virtual void UnhandledInput(InputEvent @event) { }
    public virtual void Dispose() { }

    protected void SetProcess(bool enabled)
    {
        if (enabled)
            _componentManager.RegisterProcess(this);
        else
            _componentManager.UnregisterProcess(this);
    }

    protected void SetPhysicsProcess(bool enabled)
    {
        if (enabled)
            _componentManager.RegisterPhysicsProcess(this);
        else
            _componentManager.UnregisterPhysicsProcess(this);
    }

    protected void SetInput(bool enabled)
    {
        if (enabled)
            _componentManager.RegisterInput(this);
        else
            _componentManager.UnregisterInput(this);
    }

    protected void SetUnhandledInput(bool enabled)
    {
        if (enabled)
            _componentManager.RegisterUnhandledInput(this);
        else
            _componentManager.UnregisterUnhandledInput(this);
    }

    private void RunCodeOnReady()
    {
        _componentManager = Owner.GetAutoload<Autoloads>(nameof(Autoloads)).ComponentManager;
        Ready();
    }

    private void UnsubscribeOnTreeExit()
    {
        Dispose();
        _componentManager.UnregisterAll(this);
        Owner.Ready -= RunCodeOnReady;
        Owner.TreeExited -= UnsubscribeOnTreeExit;
    }
}
