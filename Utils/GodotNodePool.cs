using Godot;
using System;
using System.Collections.Generic;

namespace GodotUtils;

/// <summary>
/// Reuses nodes to reduce allocations and improve performance.
/// Nodes are automatically added to the specified parent when created.
/// </summary>
/// <typeparam name="TValue">The type of node managed by the pool.</typeparam>
public class GodotNodePool<TValue>(Node parent, Func<TValue> createNode) where TValue : Node
{
    /// <summary>
    /// Nodes currently in use by the pool.
    /// </summary>
    public IEnumerable<TValue> ActiveNodes => _nodesInUse;

    private readonly List<TValue> _nodes = []; // The nodes NOT in use
    private readonly HashSet<TValue> _nodesInUse = []; // The nodes in use
    private readonly Func<TValue> _createNode = createNode;
    private readonly Node _parent = parent;

    /// <summary>
    /// Returns an available node or creates a new one if all are in use.
    /// </summary>
    public TValue Get()
    {
        foreach (TValue node in _nodes)
        {
            if (_nodesInUse.Add(node))
                return node;
        }

        TValue newObj = _createNode();
        _parent.AddChild(newObj);
        _nodes.Add(newObj);
        _nodesInUse.Add(newObj);
        return newObj;
    }

    /// <summary>
    /// Releases a node from the pool.
    /// </summary>
    public void Release(TValue obj)
    {
        _nodesInUse.Remove(obj);
    }

    /// <summary>
    /// Removes all nodes from the pool and queue frees them.
    /// </summary>
    public void Clear()
    {
        foreach (Node node in _nodes)
            node.QueueFree();

        _nodes.Clear();
        _nodesInUse.Clear();
    }
}
