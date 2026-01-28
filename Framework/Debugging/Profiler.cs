using Godot;
using GodotUtils;
using System;
using System.Collections.Generic;

namespace GodotUtils.Framework.Debugging;

public class Profiler
{
    // Variables
    private Dictionary<string, ProfilerEntry> _entries = [];
    private const int DefaultAccuracy = 2;

    // API
    public void Start(string key)
    {
        if (!_entries.TryGetValue(key, out ProfilerEntry entry))
        {
            entry = new ProfilerEntry();
            _entries[key] = entry;
        }

        entry.Start();
    }

    public void Stop(string key, int accuracy = DefaultAccuracy)
    {
        ProfilerEntry entry = _entries[key];

        ulong elapsedUsec = Time.GetTicksUsec() - entry.StartTimeUsec;
        ulong elapsedMs = elapsedUsec / 1000UL;

        GD.Print($"{key} {elapsedMs.ToString($"F{accuracy}")} ms");
        entry.Reset();
    }

    public void Start(string key, int accuracy = DefaultAccuracy)
    {
        StartMonitor(key, accuracy, GameFramework.Debug.Metrics.StartMonitoring);
    }

    public void Stop(string key)
    {
        _entries[key].Stop();
    }

    // Private Methods
    private void StartMonitor(string key, int accuracy, Action<string, Func<object>> registerAction)
    {
        if (!_entries.TryGetValue(key, out ProfilerEntry entry))
        {
            entry = new ProfilerEntry();
            _entries[key] = entry;

            // Register the metric with the appropriate overlay
            registerAction(key, () => _entries[key].GetAverageMs(accuracy) + " ms");
        }

        entry.Start();
    }

    // Dispose
    public void Dispose()
    {
        _entries = null;
    }
}
