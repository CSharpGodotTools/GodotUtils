#if DEBUG
using Godot;
using System;
using System.Collections.Generic;

namespace GodotUtils.Debugging;

public class VisualizeAutoload : IDisposable
{
    public Dictionary<Node, VBoxContainer> VisualNodes { get; set; }

    public static VisualizeAutoload Instance { get; private set; }
    public Dictionary<Node, VBoxContainer> VisualNodesWithoutVisualAttribute { get; private set; } = [];

    private Visualize _visualize;

    public VisualizeAutoload()
    {
        if (Instance != null)
            throw new InvalidOperationException($"{nameof(VisualizeAutoload)} was initialized already");

        _visualize = new Visualize();
        Instance = this;
    }

    public void Update()
    {
        _visualize.Update();
    }

    public void Dispose()
    {
        Instance = null;
    }
}

public class VisualNodeInfo(List<Action> actions, Control visualControl, Node node, Vector2 offset)
{
    public List<Action> Actions { get; } = actions ?? throw new ArgumentNullException(nameof(actions));
    public Control VisualControl { get; } = visualControl ?? throw new ArgumentNullException(nameof(visualControl));
    public Vector2 Offset { get; } = offset;
    public Node Node { get; } = node ?? throw new ArgumentNullException(nameof(node));
}
#endif
