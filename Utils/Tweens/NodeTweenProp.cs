using Godot;
using static Godot.Tween;

namespace GodotUtils;

/// <summary>
/// Provides an API for animating a property of a <see cref="Node"/> using Godot’s Tween system.
/// </summary>
public sealed class NodeTweenProp(Node node, string property) : BaseTween<NodeTweenProp>(node)
{
    /// <summary>
    /// Animates the specified property of the bound <see cref="Node"/>.
    /// The tween uses a <see cref="TransitionType.Sine"/> transition by default.
    /// 
    /// <code>
    /// var tween = new <see cref="NodeTweenProp"/>(this, ColorRect.PropertyName.Color);
    /// tween.Animate(Colors.Red, 0.5);
    /// tween.Animate(Colors.Green, 0.5);
    /// tween.Animate(Colors.Blue, 0.5);
    /// </code>
    /// 
    /// </summary>
    /// 
    /// <param name="finalValue">The value the property should reach.</param>
    /// 
    /// <param name="duration">How long, in seconds, the animation should take.</param>
    /// 
    /// <returns>The current <see cref="NodeTweenProp"/> instance for chaining.</returns>
    public NodeTweenProp Animate(Variant finalValue, double duration)
    {
        _tweener = _tween
            .TweenProperty(_node, property, finalValue, duration)
            .SetTrans(TransitionType.Sine);

        return this;
    }
}
