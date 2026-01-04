using Godot;
using System;
using static Godot.Tween;

namespace GodotUtils;

/// <summary>
/// Fluent API for animating a *single* shader‑parameter of a <see cref="ShaderMaterial"/>
/// attached to a <see cref="Node2D"/>.  
/// The shader‑parameter name is supplied to the constructor, so the <c>Animate</c> method
/// only needs the target value and the duration.
/// </summary>
public sealed class ShaderTweenProp : BaseTween<ShaderTweenProp>
{
    protected override ShaderTweenProp Self => this;

    private readonly ShaderMaterial _shaderMaterial;
    private readonly string _shaderParam;

    /// <summary>
    /// Creates a new <see cref="ShaderTweenProp"/> bound to <paramref name="node"/>
    /// and pre‑selects the shader‑parameter <paramref name="shaderParam"/> that will be animated.
    /// </summary>
    /// <param name="node">
    /// The <see cref="Node2D"/> whose <c>Material</c> must be a <see cref="ShaderMaterial"/>.
    /// </param>
    /// <param name="shaderParam">
    /// The name of the shader uniform to animate (e.g. <c>"blend_intensity"</c>).
    /// </param>
    /// <exception cref="Exception">
    /// Thrown when the supplied <paramref name="node"/> does not have a <see cref="ShaderMaterial"/>
    /// assigned to its <c>Material</c> property.
    /// </exception>
    internal ShaderTweenProp(Node node, string shaderParam) : base(node)
    {
        if (node is not CanvasItem canvasItem)
        {
            throw new Exception("ShaderTweenProp only supports CanvasItem-derived nodes (Node2D, Control).");
        }

        if (canvasItem.Material is not ShaderMaterial shaderMaterial)
        {
            throw new Exception("Animating shader material has not been set. Ensure the node has a ShaderMaterial assigned.");
        }

        _shaderMaterial = shaderMaterial;
        _shaderParam = shaderParam;
    }

    /// <summary>
    /// Animates the shader‑parameter supplied to the constructor.
    /// The tween uses a <see cref="TransitionType.Sine"/> transition by default.
    /// 
    /// <code>
    /// var tween = new GodotShaderTweenProperty(this, "blend_intensity");
    /// tween.Animate(1.0f, 2.0);
    /// </code>
    /// 
    /// </summary>
    /// <param name="finalValue">The value the shader parameter should reach.</param>
    /// <param name="duration">
    /// How long, in seconds, the animation should take.
    /// </param>
    /// <returns>The current <see cref="ShaderTweenProp"/> instance for chaining.</returns>
    public ShaderTweenProp PropertyTo(Variant finalValue, double duration)
    {
        _tweener = _tween
            .TweenProperty(
                _shaderMaterial,
                $"shader_parameter/{_shaderParam}",
                finalValue,
                duration)
            .SetTrans(TransitionType.Sine);

        return this;
    }
}
