using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GodotUtils;

/// <summary>
/// Creates a pool of nodes to eliminate expensive queue free calls.
/// <para>If <typeparamref name="TNode"/> implements <see cref="IPoolAssignable{TNode}"/>, <c>Get()</c> will automatically call <c>AssignPool()</c>.</para>
/// <para>If <typeparamref name="TNode"/> implements <see cref="IActivatable"/>, <c>Get()</c> and <c>Release()</c> will automatically call <c>SetActive</c>.</para>
/// </summary>
/// <typeparam name="TNode">The nodes managed in the pool.</typeparam>
public class GodotPool<TNode> where TNode : CanvasItem
{
    /// <summary>
    /// Nodes currently in use by the pool.
    /// </summary>
    public IEnumerable<TNode> ActiveNodes => _activeNodes;

    // Only evaluate reflection checks once per type, not per pool instance
    private static readonly bool _TNodeIsActivatable = typeof(IActivatable).IsAssignableFrom(typeof(TNode));
    private static readonly bool _TNodeIsPoolAssignable = typeof(IPoolAssignable<TNode>).IsAssignableFrom(typeof(TNode));

    private readonly Func<TNode> _createNode;
    private readonly Node _parent;
    private readonly Stack<TNode> _inactiveNodes; // The nodes NOT in use
    private readonly HashSet<TNode> _activeNodes; // The nodes in use
    private readonly Action<TNode, bool> _setActive;
    private readonly Action<TNode> _assignPool;

    /// <summary>
    /// Creates a pool of nodes using <paramref name="createNode"/> and attaches them as children of <paramref name="parent"/> to avoid expensive <c>QueueFree()</c> calls.
    /// <para>If <typeparamref name="TNode"/> implements <see cref="IPoolAssignable{TNode}"/>, <c>Get()</c> will automatically call <c>AssignPool()</c>.</para>
    /// <para>If <typeparamref name="TNode"/> implements <see cref="IActivatable"/>, <c>Get()</c> and <c>Release()</c> will automatically call <c>SetActive</c>.</para>
    /// </summary>
    public GodotPool(Node parent, Func<TNode> createNode)
    {
        _createNode = createNode ?? throw new ArgumentNullException(nameof(createNode));
        _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        _inactiveNodes = [];
        _activeNodes = [];

        // TNode implements IActivatable?
        if (_TNodeIsActivatable)
        {
            _setActive = (node, active) => ((IActivatable)node).SetActive(active);
        }

        // TNode implements IPoolAssignable?
        if (_TNodeIsPoolAssignable)
        {
            _assignPool = node => ((IPoolAssignable<TNode>)node).AssignPool(this);
        }
    }

    /// <summary>
    /// Returns an available <typeparamref name="TNode"/> or creates a new one if all are in use.
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
            _assignPool?.Invoke(node);
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
        _setActive?.Invoke(node, true);

        return node;
    }

    /// <summary>
    /// Releases the <paramref name="node"/> from the pool.
    /// </summary>
    public void Release(TNode node)
    {
        // Mark the active node as inactive
        _activeNodes.Remove(node);
        _inactiveNodes.Push(node);

        // Deactivate the node
        _setActive?.Invoke(node, false);

        node.Hide();
    }

    /// <summary>
    /// Queue frees all inactive and active nodes in the pool.
    /// </summary>
    public void QueueFreeAll()
    {
        // Queue free inactive nodes
        while (_inactiveNodes.Count > 0)
        {
            TNode node = _inactiveNodes.Pop();
            node.QueueFree();
        }

        // Queue free active nodes
        while (_activeNodes.Count > 0)
        {
            TNode node = _activeNodes.First();
            _activeNodes.Remove(node);
            node.QueueFree();
        }
    }
}
