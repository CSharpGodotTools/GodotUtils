using Godot;

namespace GodotUtils;

public interface IPoolable<TNode> where TNode : CanvasItem, IPoolable<TNode>
{
    void OnCreate(GodotPool<TNode> pool);
    void OnAquire();
    void OnRelease();
}
