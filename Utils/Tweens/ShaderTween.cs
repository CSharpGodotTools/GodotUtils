using Godot;
using static Godot.Tween;
using System;

namespace GodotUtils;

/// <summary>
/// Provides an API for animating shader parameters of a <see cref="ShaderMaterial"/>
/// attached to a <see cref="Node2D"/> using Godot’s Tween system.
/// </summary>
public class ShaderTween : BaseTween<ShaderTween>
{
    private readonly ShaderMaterial _shaderMaterial;

    /// <summary>
    /// Creates a new <see cref="ShaderTween"/> bound to the supplied <paramref name="node"/>.
    /// The node must have a <see cref="ShaderMaterial"/> assigned to its <c>Material</c> property.
    /// </summary>
    /// <param name="node">The <see cref="Node2D"/> whose shader material will be animated.</param>
    public ShaderTween(Node node) : base(node)
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
    }

    /// <summary>
    /// Animates the shader parameter identified by <paramref name="shaderParam"/>.
    /// The tween uses a <see cref="TransitionType.Sine"/> transition by default.
    /// 
    /// <code>
    /// var tween = new <see cref="ShaderTween"/>(this);
    /// tween.Animate("blend_intensity", 1.0f, 2.0);
    /// </code>
    /// 
    /// </summary>
    /// 
    /// <param name="shaderParam">
    /// The name of the shader uniform to animate (e.g. <c>"blend_intensity"</c>).
    /// </param>
    /// 
    /// <param name="finalValue">The value the shader parameter should reach.</param>
    /// 
    /// <param name="duration">How long, in seconds, the animation should take.</param>
    /// 
    /// <returns>The current <see cref="ShaderTween"/> instance for chaining.</returns>
    public ShaderTween Animate(string shaderParam, Variant finalValue, double duration)
    {
        _tweener = _tween
            .TweenProperty(_shaderMaterial, $"shader_parameter/{shaderParam}", finalValue, duration)
            .SetTrans(TransitionType.Sine);

        return this;
    }
}
