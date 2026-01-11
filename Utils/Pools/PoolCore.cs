#nullable enable
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GodotUtils;

internal sealed class PoolCore<TNode> where TNode : CanvasItem
{
    /// <summary>
    /// Nodes currently in use by the pool.
    /// </summary>
    public IEnumerable<TNode> ActiveNodes => _activeNodes;

    private readonly Func<TNode> _createNodeFunc;
    private readonly Node _parent;
    private readonly Stack<TNode> _inactiveNodes = []; // The nodes NOT in use
    private readonly HashSet<TNode> _activeNodes = []; // The nodes in use

    /// <summary>
    /// Creates a pool of nodes using <paramref name="createNodeFunc"/> and attaches them as children of <paramref name="parent"/> to avoid expensive <c>QueueFree()</c> calls.
    /// </summary>
    public PoolCore(Node parent, Func<TNode> createNodeFunc)
    {
        _createNodeFunc = createNodeFunc ?? throw new ArgumentNullException(nameof(createNodeFunc));
        _parent = parent ?? throw new ArgumentNullException(nameof(parent));
    }

    /// <summary>
    /// Returns an available <typeparamref name="TNode"/> or creates a new one if all are in use.
    /// </summary>
    public TNode Acquire(Action<TNode>? onCreate, Action<TNode>? onAcquire)
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
            node = _createNodeFunc();
            onCreate?.Invoke(node);
            _parent.AddChild(node);

#if DEBUG
            // For performance, this is only done in debug mode
            node.Name = $"{typeof(TNode).Name}_{node.GetInstanceId()}";
#endif
        }

        // Keep track of this active node
        _activeNodes.Add(node);

        // Activate the node
        node.Show();
        onAcquire?.Invoke(node);

        return node;
    }

    /// <summary>
    /// Releases the <paramref name="node"/> from the pool.
    /// </summary>
    public void Release(TNode node, Action<TNode>? onRelease)
    {
        // Mark the active node as inactive
        _activeNodes.Remove(node);
        _inactiveNodes.Push(node);

        // Deactivate the node
        node.Hide();
        onRelease?.Invoke(node);
    }

    /// <summary>
    /// Queue frees all inactive and active nodes in the pool.
    /// </summary>
    public void QueueFreeAll()
    {
        // Queue free inactive nodes
        while (_inactiveNodes.Count > 0)
            _inactiveNodes.Pop().QueueFree();

        // Queue free active nodes
        while (_activeNodes.Count > 0)
        {
            TNode node = _activeNodes.First();
            _activeNodes.Remove(node);
            node.QueueFree();
        }
    }
}
#nullable disable
