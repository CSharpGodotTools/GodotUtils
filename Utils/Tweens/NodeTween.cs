using Godot;
using static Godot.Tween;

namespace GodotUtils;

/// <summary>
/// Provides an API for animating a property of a <see cref="Node"/> using Godot’s Tween system.
/// </summary>
public sealed class NodeTween(Node node) : BaseTween<NodeTween>(node)
{
    /// <summary>
    /// Animates the specified <paramref name="property"/> of the bound <see cref="Node"/>.
    /// The tween uses a <see cref="TransitionType.Sine"/> transition by default.
    /// 
    /// <code>
    /// var tween = new <see cref="NodeTween"/>(this);
    /// tween.Animate(ColorRect.PropertyName.Color, Colors.Transparent, 0.5);
    /// </code>
    /// 
    /// </summary>
    /// 
    /// <param name="property">
    /// The name of the property to animate (e.g. <c>ColorRect.PropertyName.Color</c>).
    /// </param>
    /// 
    /// <param name="finalValue">The value the property should reach.</param>
    /// 
    /// <param name="duration">How long, in seconds, the animation should take.</param>
    /// 
    /// <returns>The current <see cref="NodeTween"/> instance for chaining.</returns>
    public NodeTween Animate(string property, Variant finalValue, double duration)
    {
        _tweener = _tween
            .TweenProperty(_node, property, finalValue, duration)
            .SetTrans(TransitionType.Sine);

        return this;
    }
}
