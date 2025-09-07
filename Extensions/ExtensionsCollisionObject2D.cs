using Godot;

namespace GodotUtils;

public static class ExtensionsCollisionObject2D
{
    /// <summary>
    /// Set the <paramref name="layers"/> for CollisionLayer and CollisionMask
    /// </summary>
    public static void SetCollisionLayers(this CollisionObject2D collisionObject, params int[] layers)
    {
        collisionObject.CollisionLayer = (uint)MathUtils.GetLayerValues(layers);
    }

    public static void SetCollisionMasks(this CollisionObject2D collisionObject, params int[] layers)
    {
        collisionObject.CollisionMask = (uint)MathUtils.GetLayerValues(layers);
    }
}
