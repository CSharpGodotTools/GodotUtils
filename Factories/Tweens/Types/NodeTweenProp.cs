using Godot;
using static Godot.Tween;

namespace GodotUtils;

/// <summary>
/// Provides an API for animating a property of a <see cref="Node"/> using Godot’s Tween system.
/// </summary>
public sealed class NodeTweenProp : BaseTween<NodeTweenProp>
{
    protected override NodeTweenProp Self => this;

    private readonly string _property;

    internal NodeTweenProp(Node node, string property) : base(node)
    {
        _property = property;
    }

    /// <summary>
    /// Animates the specified property of the bound <see cref="Node"/>.
    /// The tween uses a <see cref="TransitionType.Sine"/> transition by default.
    /// <code>
    /// Tweens.Animate(this, ColorRect.PropertyName.Color);
    /// tween.PropertyTo(Colors.Red, 0.5);
    /// tween.PropertyTo(Colors.Green, 0.5);
    /// tween.PropertyTo(Colors.Blue, 0.5);
    /// </code>
    /// </summary>
    /// <param name="finalValue">The value the property should reach.</param>
    /// <param name="duration">How long, in seconds, the animation should take.</param>
    /// <returns>The current <see cref="NodeTweenProp"/> instance for chaining.</returns>
    public NodeTweenProp PropertyTo(Variant finalValue, double duration)
    {
        _tweener = _tween
            .TweenProperty(_node, _property, finalValue, duration)
            .SetTrans(TransitionType.Sine);

        return this;
    }
}
