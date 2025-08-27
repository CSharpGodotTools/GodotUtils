using Godot;
using System.Collections.Generic;
using System.Linq;

namespace GodotUtils;

/// <summary>
/// Utility class for handling cursor-related operations in a 2D Godot scene.
/// </summary>
public static class CursorUtils
{
    public static List<Area2D> GetAreasUnderCursor(Node2D node, int maxResults = 1)
    {
        return [.. GetPhysicsNodesAtPosition(node, node.GetGlobalMousePosition(), true, false, false, maxResults).Select(x => x as Area2D)];
    }
    
    public static List<PhysicsBody2D> GetBodiesUnderCursor(Node2D node, int maxResults = 1)
    {
        return [.. GetPhysicsNodesAtPosition(node, node.GetGlobalMousePosition(), false, true, false, maxResults).Select(x => x as PhysicsBody2D)];
    }
    
    public static List<Area2D> GetAreasUnder(Node2D node, int maxResults = 1)
    {
        return [.. GetPhysicsNodesAtPosition(node, node.GlobalPosition, true, false, true, maxResults).Select(x => x as Area2D)];
    }
    
    public static List<PhysicsBody2D> GetBodiesUnder(Node2D node, int maxResults = 1)
    {
        return [.. GetPhysicsNodesAtPosition(node, node.GlobalPosition, false, true, true, maxResults).Select(x => x as PhysicsBody2D)];
    }
    
    private static List<Node> GetPhysicsNodesAtPosition(Node2D node, Vector2 position, bool collideWithAreas, bool collideWithBodies, bool excludeSelf = false, int maxResults = 1)
    {
        // Create a shape query parameters object
        PhysicsPointQueryParameters2D queryParams = new();
        queryParams.Position = position;
        queryParams.CollideWithAreas = collideWithAreas;
        queryParams.CollideWithBodies = collideWithBodies;

        // Exclude the node itself and its children from the query
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

        // Perform the query
        PhysicsDirectSpaceState2D spaceState = PhysicsServer2D.SpaceGetDirectState(node.GetWorld2D().GetSpace());
        
        Godot.Collections.Array<Godot.Collections.Dictionary> results = spaceState.IntersectPoint(queryParams, maxResults);

        List<Node> nodes = [];

        foreach (Godot.Collections.Dictionary result in results)
        {
            if (result != null && result.ContainsKey("collider"))
            {
                nodes.Add(result["collider"].As<Node>());
            }
        }

        return nodes;
    }
}
