using Godot;
using System;
using System.Collections.Generic;

namespace GodotUtils;

public interface IPoolable<T> where T : CanvasItem, IPoolable<T>
{
    void AssignPool(GodotPool<T> pool);
    void SetActive(bool active);
}

/// <summary>
/// Reuses nodes to reduce allocations and improve performance.
/// <para>Nodes are automatically added to the specified parent when created.</para>
/// <para>Nodes in the pool should never be queue freed unless <c>Clear()</c> is called.</para>
/// </summary>
/// <typeparam name="TNode">The type of node managed by the pool.</typeparam>
public class GodotPool<TNode>(Node parent, Func<TNode> createNode) where TNode : CanvasItem, IPoolable<TNode>
{
    /// <summary>
    /// Nodes currently in use by the pool.
    /// </summary>
    public IEnumerable<TNode> ActiveNodes => _activeNodes;

    private readonly Stack<TNode> _inactiveNodes = []; // The nodes NOT in use
    private readonly HashSet<TNode> _activeNodes = []; // The nodes in use
    private readonly Func<TNode> _createNode = createNode;
    private readonly Node _parent = parent;

    /// <summary>
    /// Returns an available node or creates a new one if all are in use.
    /// </summary>
    public TNode Get()
    {
        TNode node;

        // Is there an inactive node that can be activated?
        if (_inactiveNodes.Count > 0)
        {
            // O(1) lookup time
            node = _inactiveNodes.Pop();
        }
        else
        {
            // No inactive nodes found, need to create a new node
            node = _createNode();
            node.AssignPool(this);
            _parent.AddChild(node);

#if DEBUG
            // For performance, this is only done in debug mode
            node.Name = $"{typeof(TNode).Name}_{node.GetInstanceId()}";
#endif
        }

        // Keep track of this active node
        _activeNodes.Add(node);

        // Activate the node
        node.SetActive(true);
        node.Show();

        return node;
    }

    /// <summary>
    /// Releases a node from the pool.
    /// </summary>
    public void Release(TNode node)
    {
        // Mark the active node as inactive
        _activeNodes.Remove(node);
        _inactiveNodes.Push(node);

        // Deactivate the node
        node.SetActive(false);
        node.Hide();
    }

    /// <summary>
    /// Removes all nodes from the pool and queue frees them.
    /// </summary>
    public void Clear()
    {
        foreach (Node node in _inactiveNodes)
            node.QueueFree();

        _inactiveNodes.Clear();
        _activeNodes.Clear();
    }
}
