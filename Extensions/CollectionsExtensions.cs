using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;

namespace GodotUtils;

/// <summary>
/// Extension helpers for collections.
/// </summary>
public static class ExtensionsCollection
{
    /// <summary>
    /// Iterates over the sequence and invokes <paramref name="action"/> for each element.
    /// </summary>
    public static void ForEach<T>(this IEnumerable<T> value, Action<T> action)
    {
        foreach (T element in value)
        {
            action(element);
        }
    }

    /// <summary>
    /// Returns true when the dictionary already contains the key.
    /// </summary>
    public static bool Duplicate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key,
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string caller = null,
        [CallerFilePath] string path = null)
    {
        if (!dict.ContainsKey(key))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Returns true when the dictionary does not contain the key.
    /// </summary>
    public static bool DoesNotHave<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key,
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string caller = null,
        [CallerFilePath] string path = null)
    {
        if (dict.ContainsKey(key))
        {
            return false;
        }

        return true;
    }
}
