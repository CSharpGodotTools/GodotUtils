using Godot;
using System;
using System.Threading.Tasks;
using static Godot.Tween;

namespace GodotUtils;

/// <summary>
/// Base class that implements the API shared by all Godot tween builders.
/// </summary>
/// <typeparam name="TSelf">
/// The concrete subclass type (e.g. <see cref="NodeTween"/> or <see cref="NodeTweenProp"/>).
/// </typeparam>
public abstract class BaseTween<TSelf> where TSelf : BaseTween<TSelf>
{
    /// <summary>The <see cref="Node"/> that the tween will operate on.</summary>
    protected readonly Node _node;

    /// <summary>The underlying <see cref="Tween"/> instance used for all tween operations.</summary>
    protected Tween _tween;

    /// <summary>
    /// The currently active <see cref="PropertyTweener"/> that allows configuration of
    /// transition and ease types. It is created when <c>Animate(...)</c> is called.
    /// </summary>
    protected PropertyTweener _tweener;

    /// <summary>
    /// Creates a new tween builder bound to the specified <paramref name="node"/>.
    /// The internal <see cref="Tween"/> is freshly instantiated and set to
    /// <see cref="TweenProcessMode.Physics"/> to keep physics‑driven objects (e.g. cameras) in sync.
    /// </summary>
    /// <param name="node">The node that will be animated.</param>
    public BaseTween(Node node)
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
    public static NodeTween Delay(Node node, double seconds, Action callback)
    {
        NodeTween tween = new(node);

        tween.Delay(seconds)
            .Callback(callback);

        return tween;
    }

    /// <summary>Sets the <see cref="Tween"/> processing mode to the specified <paramref name="mode"/>.</summary>
    public TSelf SetProcessMode(TweenProcessMode mode)
    {
        _tween = _tween.SetProcessMode(mode);
        return Self;
    }

    /// <summary>
    /// Sets the animation to repeat
    /// </summary>
    public TSelf Loop(int loops = 0)
    {
        _tween = _tween.SetLoops(loops);
        return Self;
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
    public TSelf Parallel()
    {
        _tween = _tween.Parallel();
        return Self;
    }

    /// <summary>
    /// <para>If <paramref name="parallel"/> is <see langword="true"/>, the <see cref="Tweener"/>s appended after this method will by default run simultaneously, as opposed to sequentially.</para>
    /// <para><code>
    /// tween.SetParallel()
    /// tween.Animate(...)
    /// tween.Animate(...)
    /// </code></para>
    /// </summary>
    public TSelf SetParallel(bool parallel = true)
    {
        _tween = _tween.SetParallel(parallel);
        return Self;
    }

    /// <summary>Registers a callback to be invoked when the tween reaches this point in the chain.</summary>
    public TSelf Callback(Action callback)
    {
        _tween.TweenCallback(Callable.From(callback));
        return Self;
    }

    /// <summary>Inserts a delay in <paramref name="seconds"/> before the next tween step.</summary>
    public TSelf Delay(double seconds)
    {
        _tween.TweenCallback(Callable.From(() => { /* Empty Action */ })).SetDelay(seconds);
        return Self;
    }

    /// <summary>
    /// A <paramref name="callback"/> is executed when the tween has finished
    /// </summary>
    public TSelf Finished(Action callback)
    {
        _tween.Finished += callback;
        return Self;
    }

    /// <summary>
    /// If the tween is looping, this can be used to stop it
    /// </summary>
    public TSelf Stop()
    {
        _tween.Stop();
        return Self;
    }

    /// <summary>
    /// Pause the tween
    /// </summary>
    public TSelf Pause()
    {
        _tween.Pause();
        return Self;
    }

    /// <summary>
    /// If the tween was paused with Pause(), resume it with Resume()
    /// </summary>
    public TSelf Resume()
    {
        _tween.Play();
        return Self;
    }

    /// <summary>
    /// Sets the transition type of the current <see cref="PropertyTweener"/> to the specified <paramref name="transType"/>.
    /// </summary>
    public TSelf SetTrans(TransitionType transType)
    {
        return UpdateTweener(nameof(SetTrans), () => _tweener.SetTrans(transType));
    }

    /// <summary>
    /// Sets the ease type of the current <see cref="PropertyTweener"/> to the specified <paramref name="easeType"/>.
    /// </summary>
    public TSelf SetEase(EaseType easeType)
    {
        return UpdateTweener(nameof(SetEase), () => _tweener.SetEase(easeType));
    }

    /// <summary>Sets the transition type to <see cref="TransitionType.Linear"/>.</summary>
    public TSelf TransLinear() => SetTrans(TransitionType.Linear);

    /// <summary>Sets the transition type to <see cref="TransitionType.Back"/>.</summary>
    public TSelf TransBack() => SetTrans(TransitionType.Back);

    /// <summary>Sets the transition type to <see cref="TransitionType.Sine"/>.</summary>
    public TSelf TransSine() => SetTrans(TransitionType.Sine);

    /// <summary>Sets the transition type to <see cref="TransitionType.Bounce"/>.</summary>
    public TSelf TransBounce() => SetTrans(TransitionType.Bounce);

    /// <summary>Sets the transition type to <see cref="TransitionType.Circ"/>.</summary>
    public TSelf TransCirc() => SetTrans(TransitionType.Circ);

    /// <summary>Sets the transition type to <see cref="TransitionType.Cubic"/>.</summary>
    public TSelf TransCubic() => SetTrans(TransitionType.Cubic);

    /// <summary>Sets the transition type to <see cref="TransitionType.Elastic"/>.</summary>
    public TSelf TransElastic() => SetTrans(TransitionType.Elastic);

    /// <summary>Sets the transition type to <see cref="TransitionType.Expo"/>.</summary>
    public TSelf TransExpo() => SetTrans(TransitionType.Expo);

    /// <summary>Sets the transition type to <see cref="TransitionType.Quad"/>.</summary>
    public TSelf TransQuad() => SetTrans(TransitionType.Quad);

    /// <summary>Sets the transition type to <see cref="TransitionType.Quart"/>.</summary>
    public TSelf TransQuart() => SetTrans(TransitionType.Quart);

    /// <summary>Sets the transition type to <see cref="TransitionType.Quint"/>.</summary>
    public TSelf TransQuint() => SetTrans(TransitionType.Quint);

    /// <summary>Sets the transition type to <see cref="TransitionType.Spring"/>.</summary>
    public TSelf TransSpring() => SetTrans(TransitionType.Spring);

    /// <summary>Sets the ease type to <see cref="EaseType.In"/>.</summary>
    public TSelf EaseIn() => SetEase(EaseType.In);

    /// <summary>Sets the ease type to <see cref="EaseType.Out"/>.</summary>
    public TSelf EaseOut() => SetEase(EaseType.Out);

    /// <summary>Sets the ease type to <see cref="EaseType.InOut"/>.</summary>
    public TSelf EaseInOut() => SetEase(EaseType.InOut);

    /// <summary>Sets the ease type to <see cref="EaseType.OutIn"/>.</summary>
    public TSelf EaseOutIn() => SetEase(EaseType.OutIn);

    /// <summary>
    /// Checks if the tween is still playing
    /// </summary>
    public bool IsRunning()
    {
        return _tween.IsRunning();
    }

    /// <summary>
    /// Kill the tween
    /// </summary>
    public TSelf Kill()
    {
        _tween?.Kill();
        return Self;
    }

    /// <summary>
    /// Executes the supplied <paramref name="action"/> on the current <see cref="PropertyTweener"/>.
    /// Throws an exception if no tweener has been created (i.e., <c>Animate(...)</c> has not been called yet).
    /// </summary>
    private TSelf UpdateTweener(string methodName, Action action)
    {
        if (_tweener == null)
        {
            throw new Exception($"Cannot call {methodName}() because no tweener has been set with tween.Animate(...)");
        }

        action();
        return Self;
    }

    /// <summary>Provides a strongly‑typed reference to the concrete subclass (used for chaining).</summary>
    private TSelf Self => (TSelf)this;
}
