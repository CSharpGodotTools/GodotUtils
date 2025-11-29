#if DEBUG
using System;

namespace GodotUtils.Debugging;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method)]
public class VisualizeAttribute : Attribute
{
}
#endif
