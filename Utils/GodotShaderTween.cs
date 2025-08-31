using Godot;
using static Godot.Tween;
using System;

namespace GodotUtils;

public class GodotShaderTween : GodotTween
{
    private ShaderMaterial _animatingShaderMaterial;

    public GodotShaderTween(Node2D node) : base(node)
    {
        _animatingShaderMaterial = node.Material as ShaderMaterial;

        if (_animatingShaderMaterial == null)
        {
            throw new Exception("Animating shader material has not been set. Make sure the node has a shader material");
        }
    }

    /// <summary>
    /// Animates a specified <paramref name="shaderParam"/>. All tweens use the Sine transition by default.
    /// 
    /// <code>
    /// tween.AnimateShader("blend_intensity", 1.0f, 2.0);
    /// </code>
    /// </summary>
    public GodotShaderTween AnimateShader(string shaderParam, Variant finalValue, double duration)
    {
        _tweener = _tween
            .TweenProperty(_animatingShaderMaterial, $"shader_parameter/{shaderParam}", finalValue, duration)
            .SetTrans(TransitionType.Sine);

        return this;
    }
}
