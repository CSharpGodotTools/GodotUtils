#if DEBUG
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GodotUtils.Debugging.Visualize;

/// <summary>
/// This logger shows all messages in game making it easier to debug
/// </summary> 
public class Visualize : IDisposable
{
    private const int MaxLabelsVisible = 5;
    private static readonly Vector2 DefaultOffset = new(100, 100);

    private Dictionary<ulong, VisualNodeInfo> _nodeTrackers = [];
    private static Visualize _instance;

    public Visualize()
    {
        _instance = this;
    }

    public void Update()
    {
        foreach (KeyValuePair<ulong, VisualNodeInfo> kvp in _nodeTrackers)
        {
            VisualNodeInfo info = kvp.Value;
            Node node = info.Node;
            Control visualControl = info.VisualControl;

            // Update position based on node type
            if (node != null) // Checking null here every frame is costly. No need to update the position if the position never changes!
            {
                if (node is Node2D node2D)
                {
                    visualControl.GlobalPosition = node2D.GlobalPosition + info.Offset;
                }
                else if (node is Control control)
                {
                    visualControl.GlobalPosition = control.GlobalPosition + info.Offset;
                }
            }

            // Execute actions
            foreach (Action action in info.Actions)
            {
                action();
            }
        }
    }

    public void Dispose()
    {
        _instance = null;
    }

    public static void Register(Node node, params string[] readonlyMembers)
    {
        VisualData visualData = VisualizeAttributeHandler.RetrieveData(node);

        if (visualData != null)
        {
            (Control visualPanel, List<Action> actions) = VisualUI.CreateVisualPanel(visualData, readonlyMembers);

            ulong instanceId = node.GetInstanceId();

            Node positionalNode = GetClosestParentOfType(node, typeof(Node2D), typeof(Control));

            if (positionalNode == null)
            {
                PrintUtils.Warning($"[Visualize] No positional parent node could be found for {node.Name} so its visual panel will be created at position {DefaultOffset}");
            }

            if (positionalNode != null)
            {
                // Immediately set the visual panels position to the positional nodes position
                if (positionalNode is Node2D node2D)
                {
                    visualPanel.GlobalPosition = node2D.GlobalPosition;
                }
                else if (positionalNode is Control control)
                {
                    visualPanel.GlobalPosition = control.GlobalPosition;
                }
            }
            else
            {
                visualPanel.GlobalPosition = DefaultOffset;
            }

            // Ensure the added visual panel is not overlapping with any other visual panels
            IEnumerable<Control> controls = _instance._nodeTrackers.Select(x => x.Value.VisualControl);

            Vector2 offset = Vector2.Zero;

            foreach (Control existingControl in controls)
            {
                if (existingControl == visualPanel)
                    continue; // Skip checking against itself

                if (ControlsOverlapping(visualPanel, existingControl))
                {
                    // Move vbox down by the existing controls height
                    offset += new Vector2(0, existingControl.GetRect().Size.Y);
                }
            }

            _instance._nodeTrackers.Add(instanceId, new VisualNodeInfo(actions, visualPanel, positionalNode ?? node, offset));
        }

        node.TreeExited += () =>
        {
            RemoveVisualNode(node);
        };
    }

    public static void Log(object message, Node node, double fadeTime = 5)
    {
        VBoxContainer vbox = GetOrCreateVBoxContainer(node);

        if (vbox != null)
        {
            AddLabel(vbox, message, fadeTime);
        }
    }

    private static bool ControlsOverlapping(Control control1, Control control2)
    {
        // Get the bounding rectangles of the control nodes
        Rect2 rect1 = control1.GetRect();
        Rect2 rect2 = control2.GetRect();

        // Check if the rectangles intersect
        return rect1.Intersects(rect2);
    }

    private static void RemoveVisualNode(Node node)
    {
        ulong instanceId = node.GetInstanceId();

        if (_instance._nodeTrackers.TryGetValue(instanceId, out VisualNodeInfo info))
        {
            // GetParent to queue free the CanvasLayer this VisualControl is a child of
            info.VisualControl.GetParent().QueueFree();
            _instance._nodeTrackers.Remove(instanceId);
        }
    }

    private static Node GetClosestParentOfType(Node node, params Type[] typesToCheck)
    {
        // Check if the current node is of one of the specified types
        if (IsNodeOfType(node, typesToCheck))
            return node;

        // Recursively get the parent and check its type
        Node parent = node.GetParent();

        while (parent != null)
        {
            if (IsNodeOfType(parent, typesToCheck))
                return parent;

            parent = parent.GetParent();
        }

        // If no suitable parent is found, return null
        return null;
    }

    private static bool IsNodeOfType(Node node, Type[] typesToCheck)
    {
        foreach (Type type in typesToCheck)
        {
            if (type.IsInstanceOfType(node))
                return true;
        }

        return false;
    }

    private static VBoxContainer GetOrCreateVBoxContainer(Node node)
    {
        if (VisualizeAutoload.Instance.VisualNodes != null && VisualizeAutoload.Instance.VisualNodes.TryGetValue(node, out VBoxContainer vbox))
            return vbox;

        if (node is not Control and not Node2D)
            return null;

        if (!VisualizeAutoload.Instance.VisualNodesWithoutVisualAttribute.TryGetValue(node, out vbox))
        {
            vbox = new VBoxContainer { Scale = Vector2.One * VisualUI.VisualUiScaleFactor };
            node.AddChild(vbox);
            VisualizeAutoload.Instance.VisualNodesWithoutVisualAttribute[node] = vbox;
        }

        return vbox;
    }

    private static void AddLabel(VBoxContainer vbox, object message, double fadeTime)
    {
        Label label = new() { Text = message?.ToString() };

        vbox.AddChild(label);
        vbox.MoveChild(label, 0);

        if (vbox.GetChildCount() > MaxLabelsVisible)
        {
            vbox.RemoveChild(vbox.GetChild(vbox.GetChildCount() - 1));
        }

        _ = new GodotTween(label)
            .SetAnimatingProp(CanvasItem.PropertyName.Modulate)
            .AnimateProp(Colors.Transparent, fadeTime)
            .Callback(label.QueueFree);
    }
}
#endif
