using Godot;

namespace GodotUtils;

public interface IPoolable<TNode> where TNode : CanvasItem, IPoolable<TNode>
{
    /// <summary>
    /// Enable or disable the <typeparamref name="TNode"/>. This usually involves enabling or disabling all scripts from running.
    /// </summary>
    void SetActive(bool active);

    /// <summary>
    /// The pool is assigned before <c>_Ready()</c> is called.
    /// </summary>
    void AssignPool(GodotPool<TNode> pool);
}
