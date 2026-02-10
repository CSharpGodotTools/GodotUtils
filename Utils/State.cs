using System;

namespace GodotUtils;

public class State(string name = "")
{
    private static readonly Action Noop = static () => { };
    private static readonly Action<double> NoopUpdate = static _ => { };

    public Action Enter { get; set; } = Noop;
    public Action<double> Update { get; set; } = NoopUpdate;
    public Action Exit { get; set; } = Noop;

    private readonly string _name = name;
    
    /// <summary>
    /// Returns the state name in lowercase.
    /// </summary>
    public override string ToString() => _name.ToLower();
}

public interface IStateMachine
{
    /// <summary>
    /// Switches to the provided state.
    /// </summary>
    void SwitchState(State newState);
}
