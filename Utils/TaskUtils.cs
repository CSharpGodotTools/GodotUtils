using Godot;
using System;
using System.Threading.Tasks;

namespace GodotUtils;

public static class TaskUtils
{
    /// <summary>
    /// Logs exceptions from a task without awaiting it.
    /// </summary>
    public static void FireAndForget(this Task task)
    {
        _ = task.ContinueWith(static (Task t) =>
        {
            foreach (Exception ex in t.Exception.Flatten().InnerExceptions)
            {
                GD.PrintErr($"FireAndForget task exception: {ex}");
            }
        }, TaskContinuationOptions.OnlyOnFaulted);
    }

    /// <summary>
    /// Runs a task and logs any errors, intended for fire-and-forget usage.
    /// </summary>
    public static async void TryRun(this Func<Task> task)
    {
        if (task == null)
            return;

        try
        {
            await task();
        }
        catch (Exception e)
        {
            GD.PrintErr($"Error: {e}");
        }
    }
}
