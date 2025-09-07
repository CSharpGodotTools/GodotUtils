using Godot;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace GodotUtils.Debugging;

public static class ParamValidator
{
    public static void ThrowIfNull(Node node, object obj, [CallerArgumentExpression(nameof(obj))] string paramName = "")
    {
        ArgumentNullException.ThrowIfNull(node, nameof(node));

        if (obj == null)
        {
            string scriptName = Path.GetFileName(node?.GetScript().As<Script>().ResourcePath);

            node.ProcessMode = Node.ProcessModeEnum.Disabled;

            throw new Exception($"Value cannot be null. (Parameter '{paramName}' in {scriptName})");
        }
    }
}
