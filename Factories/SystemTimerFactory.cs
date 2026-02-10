using Object = System.Object;
using System.Timers;
using System;
using Timer = System.Timers.Timer;

namespace GodotUtils;

/// <summary>
/// Factory helpers for system timers.
/// </summary>
public static class SystemTimerFactory
{
    /// <summary>
    /// Creates and configures a system timer.
    /// </summary>
    public static Timer Create(double delayMs, Action action, bool enabled = true, bool autoreset = true)
    {
        Timer timer = new(delayMs)
        {
            Enabled = enabled,
            AutoReset = autoreset
        };
        
        timer.Elapsed += Callback;

        return timer;

        void Callback(Object source, ElapsedEventArgs e) => action();
    }
}
