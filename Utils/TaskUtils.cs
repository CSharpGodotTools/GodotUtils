using Godot;
using System;
using System.Threading.Tasks;

namespace GodotUtils;

public static class TaskUtils
{
    public static void FireAndForget(this Task task)
    {
        _ = task.ContinueWith(t =>
        {
            foreach (Exception ex in t.Exception.Flatten().InnerExceptions)
            {
                GD.PrintErr($"FireAndForget task exception: {ex}");
            }
        }, TaskContinuationOptions.OnlyOnFaulted);
    }

    // Return type of void was used here intentionally
    public static async void TryRun(this Func<Task> task)
    {
        if (task != null)
        {
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
}
