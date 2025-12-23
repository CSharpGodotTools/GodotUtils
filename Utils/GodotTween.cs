using Godot;
using static Godot.Tween;
using System;
using System.Threading.Tasks;

namespace GodotUtils;

public class GodotTween
{
    protected PropertyTweener _tweener;
    protected Tween _tween;
    private readonly Node _node;
    private string _animatingProperty;

    public GodotTween(Node node)
    {
        _node = node;

        // Ensure the Tween is fresh when re-creating it
        Kill();
        _tween = node.CreateTween();

        // This helps to prevent the camera from lagging behind the players movement
        _tween.SetProcessMode(TweenProcessMode.Physics);
    }

    /// <summary>
    /// A delay in <paramref name="seconds"/> followed by a <paramref name="callback"/>
    /// </summary>
    public static GodotTween Delay(Node node, double seconds, Action callback)
    {
        GodotTween tween = new(node);

        tween.Delay(seconds)
            .Callback(callback);

        return tween;
    }

    /// <summary>
    /// Animates the property that was set with SetAnimatingProp(string prop)
    /// 
    /// <code>
    /// tween.SetAnimatingProp(ColorRect.PropertyName.Color);
    /// tween.AnimateProp(Colors.Transparent, 0.5);
    /// </code>
    /// </summary>
    public GodotTween AnimateProp(Variant finalValue, double duration)
    {
        if (string.IsNullOrWhiteSpace(_animatingProperty))
        {
            throw new Exception("No animation property has been set with tween.SetAnimatingProp(...)");
        }

        return Animate(_animatingProperty, finalValue, duration);
    }

    /// <summary>
    /// Animates a specified <paramref name="property"/>. All tweens use the 
    /// Sine transition by default.
    /// 
    /// <code>
    /// tween.Animate(ColorRect.PropertyName.Color, Colors.Transparent, 0.5);
    /// </code>
    /// </summary>
    public GodotTween Animate(string property, Variant finalValue, double duration)
    {
        _tweener = _tween
            .TweenProperty(_node, property, finalValue, duration)
            .SetTrans(TransitionType.Sine);

        return this;
    }

    /// <summary>
    /// Sets the <paramref name="property"/> to be animated on
    /// 
    /// <code>
    /// tween.SetAnimatingProp(ColorRect.PropertyName.Color);
    /// tween.AnimateProp(Colors.Transparent, 0.5);
    /// </code>
    /// </summary>
    public GodotTween SetAnimatingProp(string property)
    {
        _animatingProperty = property;
        return this;
    }

    public GodotTween SetProcessMode(TweenProcessMode mode)
    {
        _tween = _tween.SetProcessMode(mode);
        return this;
    }

    /// <summary>
    /// Sets the animation to repeat
    /// </summary>
    public GodotTween Loop(int loops = 0)
    {
        _tween = _tween.SetLoops(loops);
        return this;
    }

    public async Task FinishedAsync()
    {
        await _node.ToSignal(_tween, Tween.SignalName.Finished);
    }

    /// <summary>
    /// <para>Makes the next <see cref="Tweener"/> run parallelly to the previous one.</para>
    /// <para><b>Example:</b></para>
    /// <para><code>
    /// GTween tween = new(...);
    /// tween.Animate(...);
    /// tween.Parallel().Animate(...);
    /// tween.Parallel().Animate(...);
    /// </code></para>
    /// <para>All <see cref="Tweener"/>s in the example will run at the same time.</para>
    /// <para>You can make the <see cref="Tween"/> parallel by default by using <see cref="Tween.SetParallel(bool)"/>.</para>
    /// </summary>
    public GodotTween Parallel()
    {
        _tween = _tween.Parallel();
        return this;
    }

    /// <summary>
    /// <para>If <paramref name="parallel"/> is <see langword="true"/>, the <see cref="Tweener"/>s appended after this method will by default run simultaneously, as opposed to sequentially.</para>
    /// <para><code>
    /// tween.SetParallel()
    /// tween.Animate(...)
    /// tween.Animate(...)
    /// </code></para>
    /// </summary>
    public GodotTween SetParallel(bool parallel = true)
    {
        _tween = _tween.SetParallel(parallel);
        return this;
    }

    public GodotTween Callback(Action callback)
    {
        _tween.TweenCallback(Callable.From(callback));
        return this;
    }

    public GodotTween Delay(double seconds)
    {
        _tween.TweenCallback(Callable.From(() => { /* Empty Action */ })).SetDelay(seconds);
        return this;
    }

    /// <summary>
    /// A <paramref name="callback"/> is executed when the tween has finished
    /// </summary>
    public GodotTween Finished(Action callback)
    {
        _tween.Finished += callback;
        return this;
    }

    /// <summary>
    /// If the tween is looping, this can be used to stop it
    /// </summary>
    public GodotTween Stop()
    {
        _tween.Stop();
        return this;
    }

    /// <summary>
    /// Pause the tween
    /// </summary>
    public GodotTween Pause()
    {
        _tween.Pause();
        return this;
    }

    /// <summary>
    /// If the tween was paused with Pause(), resume it with Resume()
    /// </summary>
    public GodotTween Resume()
    {
        _tween.Play();
        return this;
    }

    /// <summary>
    /// Kill the tween
    /// </summary>
    public GodotTween Kill()
    {
        _tween?.Kill();
        return this;
    }

    public GodotTween SetTrans(TransitionType transType)
    {
        return UpdateTweener(nameof(SetTrans), () => _tweener.SetTrans(transType));
    }

    public GodotTween SetEase(EaseType easeType)
    {
        return UpdateTweener(nameof(SetEase), () => _tweener.SetEase(easeType));
    }

    public GodotTween TransLinear() => SetTrans(TransitionType.Linear);
    public GodotTween TransBack() => SetTrans(TransitionType.Back);
    public GodotTween TransSine() => SetTrans(TransitionType.Sine);
    public GodotTween TransBounce() => SetTrans(TransitionType.Bounce);
    public GodotTween TransCirc() => SetTrans(TransitionType.Circ);
    public GodotTween TransCubic() => SetTrans(TransitionType.Cubic);
    public GodotTween TransElastic() => SetTrans(TransitionType.Elastic);
    public GodotTween TransExpo() => SetTrans(TransitionType.Expo);
    public GodotTween TransQuad() => SetTrans(TransitionType.Quad);
    public GodotTween TransQuart() => SetTrans(TransitionType.Quart);
    public GodotTween TransQuint() => SetTrans(TransitionType.Quint);
    public GodotTween TransSpring() => SetTrans(TransitionType.Spring);

    public GodotTween EaseIn() => SetEase(EaseType.In);
    public GodotTween EaseOut() => SetEase(EaseType.Out);
    public GodotTween EaseInOut() => SetEase(EaseType.InOut);
    public GodotTween EaseOutIn() => SetEase(EaseType.OutIn);

    /// <summary>
    /// Checks if the tween is still playing
    /// </summary>
    public bool IsRunning()
    {
        return _tween.IsRunning();
    }

    private GodotTween UpdateTweener(string methodName, Action action)
    {
        if (_tweener == null)
        {
            throw new Exception($"Cannot call {methodName}() because no tweener has been set with tween.Animate(...)");
        }

        action();
        return this;
    }
}
