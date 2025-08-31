using Godot;
using System;

namespace GodotUtils;

public class GodotTimerFactory
{
    /// <summary>
    /// Creates a oneshot timer not bound to any node that removes itself on <paramref name="timeout"/> after <paramref name="seconds"/>.
    /// </summary>
    public static void OneShot(SceneTree tree, double seconds, Action timeout)
    {
        SceneTreeTimer timer = tree.CreateTimer(seconds);
        timer.Timeout += timeout;
    }

    /// <summary>
    /// Creates a oneshot timer bound to <paramref name="node"/> that removes itself on <paramref name="timeout"/> after <paramref name="seconds"/>. Since the timer is bound to the node, it will get removed if the node is removed.
    /// </summary>
    public static Timer OneShot(Node node, double seconds, Action timeout)
    {
        return Create(node, seconds, true, timeout);
    }

    /// <summary>
    /// Creates a looping timer bound to <paramref name="node"/> that invokes <paramref name="timeout"/> after <paramref name="seconds"/>. Since the timer is bound to the node, it will get removed if the node is removed.
    /// </summary>
    public static Timer Looping(Node node, double seconds, Action timeout)
    {
        return Create(node, seconds, false, timeout);
    }

    /// <summary>
    /// Creates a timer bound to <paramref name="node"/> that invokes <paramref name="timeout"/> after <paramref name="seconds"/> and removes itself if <paramref name="oneShot"/> is true. Since the timer is bound to the node, it will get removed if the node is removed.
    /// </summary>
    private static Timer Create(Node node, double seconds, bool oneShot, Action timeout)
    {
        Timer timer = new()
        {
            WaitTime = seconds,
            Autostart = true,
            OneShot = oneShot,
        };

        if (oneShot)
        {
            timer.Timeout += () =>
            {
                timeout();
                timer.QueueFree();
            };
        }
        else
        {
            timer.Timeout += timeout;
        }

        node.AddChild(timer);

        return timer;
    }
}
