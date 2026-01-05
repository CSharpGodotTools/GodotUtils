using Godot;

namespace GodotUtils;

public interface IPoolAssignable<TNode> where TNode : CanvasItem
{
    void AssignPool(GodotPool<TNode> pool);
}
