using Godot;
using System;
using System.Threading.Tasks;

namespace GodotUtils;

public class Component
{
    protected Node Owner;
    private ComponentManager _componentManager;

    public Component(Node owner)
    {
        Owner = owner;
        Owner.Ready += InitializeComponent;
        Owner.TreeExited += CleanupOnTreeExit;
    }

    protected internal virtual void Ready() { }
    protected internal virtual void Deferred() { }
    protected internal virtual void Process(double delta) { }
    protected internal virtual void PhysicsProcess(double delta) { }
    protected internal virtual void ProcessInput(InputEvent @event) { }
    protected internal virtual void UnhandledInput(InputEvent @event) { }
    protected internal virtual void Dispose() { }

    public void SetActive(bool active)
    {
        SetProcess(active);
        SetPhysicsProcess(active);
        SetInput(active);
        SetUnhandledInput(active);
    }

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

    private async Task CallNextFrame(Action action)
    {
        await Owner.WaitOneFrame();
        action();
    }

    private void InitializeComponent()
    {
        _componentManager = ComponentManager.Instance;
        Ready();
        CallNextFrame(Deferred).FireAndForget();
    }

    private void CleanupOnTreeExit()
    {
        Dispose();
        _componentManager.UnregisterAll(this);
        Owner.Ready -= InitializeComponent;
        Owner.TreeExited -= CleanupOnTreeExit;
    }
}
