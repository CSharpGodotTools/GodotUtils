using Godot;

namespace GodotUtils;

// Concrete type for basic Node type.
public class NodeTween : BaseTween<NodeTween>
{
    protected override NodeTween Self => this;

    internal NodeTween(Node node) : base(node)
    {
    }
}
