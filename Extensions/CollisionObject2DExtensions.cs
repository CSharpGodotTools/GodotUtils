using Godot;

namespace GodotUtils;

public static class CollisionObject2DExtensions
{
    public static void SetCollisionLayers(this CollisionObject2D collisionObject, params int[] layers)
    {
        collisionObject.CollisionLayer = (uint)MathUtils.GetLayerValues(layers);
    }

    public static void SetCollisionMasks(this CollisionObject2D collisionObject, params int[] layers)
    {
        collisionObject.CollisionMask = (uint)MathUtils.GetLayerValues(layers);
    }

    public static void ClearCollisionLayers(this CollisionObject2D collisionObject2D)
    {
        collisionObject2D.CollisionLayer = 0;
    }

    public static void ClearCollisionMasks(this CollisionObject2D collisionObject2D)
    {
        collisionObject2D.CollisionMask = 0;
    }
}
