using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace GodotUtils;

public static class NodeExtensions
{
    /// <summary>
    /// Retrieves an autoload from the scene tree using the given name.
    /// <code>Global global = someNode.GetAutoload&lt;Global&gt;("Global");</code>
    /// </summary>
    /// <typeparam name="T">Type of the autoload node, must inherit from Node.</typeparam>
    /// <param name="node">Any node in the scene tree to call this extension on.</param>
    /// <param name="autoload">The name of the autoload as registered in Project Settings -> AutoLoad.</param>
    /// <returns>The autoload instance of type T.</returns>
    public static T GetAutoload<T>(this Node node, string autoload) where T : Node
    {
        return node.GetNode<T>($"/root/{autoload}");
    }

    public static void AddToCurrentSceneDeferred(this Node node, Node child)
    {
        GetCurrentScene(node).CallDeferred(Node.MethodName.AddChild, child);
    }

    public static void AddToCurrentScene(this Node node, Node child)
    {
        GetCurrentScene(node).AddChild(child);
    }

    public static Node GetNodeInCurrentScene(this Node node, string path)
    {
        return GetCurrentScene(node).GetNode(path);
    }

    public static Node GetCurrentScene(this Node node)
    {
        return node.GetTree().CurrentScene;
    }

    /// <summary>
    /// Retrieves a node of type <typeparamref name="T"/> from the current scene at the specified path.
    /// </summary>
    /// <typeparam name="T">The type of the node to retrieve.</typeparam>
    /// <param name="node">The node from which to start the operation.</param>
    /// <param name="path">The path to the node in the scene tree.</param>
    /// <returns>The node at the specified path, cast to type <typeparamref name="T"/>.</returns>
    public static T GetSceneNode<T>(this Node node, string path) where T : Node
    {
        return node.GetTree().CurrentScene.GetNode<T>(path);
    }

    /// <summary>
    /// Retrieves a node of type <typeparamref name="T"/> from the current scene.
    /// </summary>
    /// <typeparam name="T">The type of the node to retrieve.</typeparam>
    /// <param name="node">The node from which to start the operation.</param>
    /// <returns>The node of type <typeparamref name="T"/>, if found.</returns>
    public static T GetSceneNode<T>(this Node node) where T : Node
    {
        return node.GetTree().CurrentScene.GetNode<T>(recursive: false);
    }

    /// <summary>
    /// Recursively searches for all nodes of <paramref name="type"/>
    /// </summary>
    public static List<Node> GetNodes(this Node node, Type type)
    {
        List<Node> nodes = [];
        RecursiveTypeMatchSearch(node, type, nodes);
        return nodes;
    }

    private static void RecursiveTypeMatchSearch(Node node, Type type, List<Node> nodes)
    {
        if (node.GetType() == type)
        {
            nodes.Add(node);
        }

        foreach (Node child in node.GetChildren())
        {
            RecursiveTypeMatchSearch(child, type, nodes);
        }
    }

    /// <summary>
    /// Attempt to find a child node of type T
    /// </summary>
    public static bool TryGetNode<T>(this Node node, out T foundNode, bool recursive = true) where T : Node
    {
        foundNode = FindNode<T>(node.GetChildren(), recursive);
        return foundNode != null;
    }

    /// <summary>
    /// Check if a child node of type T exists
    /// </summary>
    public static bool HasNode<T>(this Node node, bool recursive = true) where T : Node
    {
        return FindNode<T>(node.GetChildren(), recursive) != null;
    }

    /// <summary>
    /// Find a child node of type T
    /// </summary>
    public static T GetComponent<T>(this Node node, bool recursive = true) where T : Node
    {
        return GetNode<T>(node, recursive);
    }

    /// <summary>
    /// Find a child node of type T
    /// </summary>
    public static T GetNode<T>(this Node node, bool recursive = true) where T : Node
    {
        return FindNode<T>(node.GetChildren(), recursive);
    }

    private static T FindNode<T>(Godot.Collections.Array<Node> children, bool recursive = true) where T : Node
    {
        foreach (Node child in children)
        {
            if (child is T type)
            {
                return type;
            }

            if (recursive)
            {
                T val = FindNode<T>(child.GetChildren());

                if (val is not null)
                {
                    return val;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Asynchronously waits for one procress frame.
    /// </summary>
    public async static Task WaitOneFrame(this Node parent)
    {
        await parent.ToSignal(
            source: parent.GetTree(),
            signal: SceneTree.SignalName.ProcessFrame);
    }

    public static void AddChildDeferred(this Node node, Node child)
    {
        node.CallDeferred(Node.MethodName.AddChild, child);
    }

    /// <summary>
    /// Recursively retrieves all nodes of type <typeparamref name="T"/> from <paramref name="node"/>
    /// </summary>
    public static List<T> GetChildren<T>(this Node node, bool recursive = true) where T : Node
    {
        List<T> children = [];
        FindChildrenOfType(node, children, recursive);
        return children;
    }

    private static void FindChildrenOfType<T>(Node node, List<T> children, bool recursive) where T : Node
    {
        foreach (Node child in node.GetChildren())
        {
            if (child is T typedChild)
            {
                children.Add(typedChild);
            }

            if (recursive)
            {
                FindChildrenOfType(child, children, recursive);
            }
        }
    }

    /// <summary>
    /// QueueFree all the children attached to this node.
    /// </summary>
    public static void QueueFreeChildren(this Node parentNode)
    {
        foreach (Node node in parentNode.GetChildren())
        {
            node.QueueFree();
        }
    }

    /// <summary>
    /// Remove all groups this node is attached to.
    /// </summary>
    public static void RemoveAllGroups(this Node node)
    {
        Godot.Collections.Array<StringName> groups = node.GetGroups();

        for (int i = 0; i < groups.Count; i++)
        {
            node.RemoveFromGroup(groups[i]);
        }
    }

    /// <summary>
    /// Recursively traverse the tree, executing <paramref name="code"/> for this <paramref name="node"/> and all its children.
    /// </summary>
    /// <param name="node">The starting node for traversal.</param>
    /// <param name="code">Action to execute for each node.</param>
    public static void TraverseNodes(this Node node, Action<Node> code)
    {
        // Execute the action on the current node
        code(node);

        // Recurse into children
        foreach (Node child in node.GetChildren())
        {
            TraverseNodes(child, code);
        }
    }
}
