using Godot;

namespace GodotUtils;

public interface IPoolable<TNode> where TNode : CanvasItem, IPoolable<TNode>
{
    /// <summary>
    /// Invoked when a new <typeparamref name="TNode"/> is created.
    /// </summary>
    void OnCreate(LifecyclePool<TNode> pool);

    /// <summary>
    /// Invoked when a <typeparamref name="TNode"/> is aquired with <c><see cref="PoolCore{TNode}"/>.Get()</c>
    /// </summary>
    void OnAcquire();

    /// <summary>
    /// Invoked when a <typeparamref name="TNode"/> is released from the pool.
    /// </summary>
    void OnRelease();
}
