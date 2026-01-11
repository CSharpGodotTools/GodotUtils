using Godot;

namespace GodotUtils;

public interface IComponentPoolable<TNode> where TNode : CanvasItem, IComponentPoolable<TNode>
{
    ComponentHost Components { get; }

    /// <summary>
    /// Invoked when a new <typeparamref name="TNode"/> is created.
    /// </summary>
    void OnCreate(ComponentPool<TNode> pool);
}
