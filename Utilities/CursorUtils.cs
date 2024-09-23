﻿using Godot;
using System.Collections.Generic;

namespace GodotUtils;

public static class CursorUtils
{
    public static Node GetAreaUnderCursor(Node2D node)
    {
        return GetPhysicsNodeAtPosition(node, node.GetGlobalMousePosition(), true, false);
    }

    public static Node GetBodyUnderCursor(Node2D node)
    {
        return GetPhysicsNodeAtPosition(node, node.GetGlobalMousePosition(), false, true);
    }

    public static Node GetAreaUnder(Node2D node)
    {
        return GetPhysicsNodeAtPosition(node, node.GlobalPosition, true, false, true);
    }

    public static Node GetBodyUnder(Node2D node)
    {
        return GetPhysicsNodeAtPosition(node, node.GlobalPosition, false, true, true);
    }

    private static Node GetPhysicsNodeAtPosition(Node2D node, Vector2 position, bool collideWithAreas, bool collideWithBodies, bool excludeSelf = false)
    {
        // Create a shape query parameters object
        PhysicsShapeQueryParameters2D queryParams = new();
        queryParams.Transform = new Transform2D(0, position);
        queryParams.CollideWithAreas = collideWithAreas;
        queryParams.CollideWithBodies = collideWithBodies;

        if (excludeSelf)
        {
            List<Rid> rids = [];

            foreach (Node child in node.GetChildren<Node>())
            {
                if (child is CollisionObject2D collision)
                {
                    rids.Add(collision.GetRid());
                }
            }

            queryParams.Exclude = new Godot.Collections.Array<Rid>(rids);
        }

        // Use a small circle shape to simulate a point intersection
        CircleShape2D circleShape = new();
        circleShape.Radius = 1.0f;
        queryParams.Shape = circleShape;

        // Perform the query
        PhysicsDirectSpaceState2D spaceState =
            PhysicsServer2D.SpaceGetDirectState(node.GetWorld2D().GetSpace());

        Godot.Collections.Array<Godot.Collections.Dictionary> results =
            spaceState.IntersectShape(queryParams, 1);

        foreach (Godot.Collections.Dictionary result in results)
        {
            if (result != null && result.ContainsKey("collider"))
            {
                return result["collider"].As<Node>();
            }
        }

        return null;
    }
}
