using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GodotUtils;

/// <summary>
/// Creates a pool of nodes to eliminate expensive queue free calls.
/// <code>
/// // Create the pool
/// GodotPool pool = new(parentNode, () => projectilePackedScene.Instantiate());
/// 
/// // Get a projectile from the pool
/// Projectile projectile = pool.Get();
/// 
/// // Projectile goes off screen or dies
/// projectile.Release(); // Never use QueueFree()
/// </code>
/// </summary>
/// <typeparam name="TNode">The nodes managed in the pool.</typeparam>
public class GodotPool<TNode> where TNode : CanvasItem, IPoolable<TNode>
{
    /// <summary>
    /// Nodes currently in use by the pool.
    /// </summary>
    public IEnumerable<TNode> ActiveNodes => _activeNodes;

    private readonly Func<TNode> _createNodeFunc;
    private readonly Node _parent;
    private readonly Stack<TNode> _inactiveNodes; // The nodes NOT in use
    private readonly HashSet<TNode> _activeNodes; // The nodes in use

    /// <summary>
    /// Creates a pool of nodes using <paramref name="createNodeFunc"/> and attaches them as children of <paramref name="parent"/> to avoid expensive <c>QueueFree()</c> calls.
    /// </summary>
    public GodotPool(Node parent, Func<TNode> createNodeFunc)
    {
        _createNodeFunc = createNodeFunc ?? throw new ArgumentNullException(nameof(createNodeFunc));
        _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        _inactiveNodes = [];
        _activeNodes = [];
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
            node = _createNodeFunc();
            node.OnCreate(this);
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
        node.OnAcquire();

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
        node.Hide();
        node.OnRelease();
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
