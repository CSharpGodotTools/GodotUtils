using Godot;
using System.IO;
using System;

namespace GodotUtils;

public static class DirectoryUtils
{
    /// <summary>
    /// Recursively traverses a directory tree and invokes a callback for each file encountered.
    /// The callback may control traversal flow (continue, skip directories, or stop traversal).
    /// 
    /// <code>
    /// Traverse("res://", entry => GD.Print(entry.FullPath));
    /// </code>
    /// </summary>
    public static TraverseResult Traverse(string directory, Func<TraverseEntry, TraverseResult> visitor)
    {
        directory = NormalizePath(ProjectSettings.GlobalizePath(directory));

        using DirAccess dir = DirAccess.Open(directory);
        dir.ListDirBegin();

        string nextFileName;

        while ((nextFileName = dir.GetNext()) != string.Empty)
        {
            if (nextFileName.StartsWith('.'))
                continue;

            string fullPath = Path.Combine(directory, nextFileName);
            bool isDir = dir.CurrentIsDir();

            TraverseResult result = visitor(new TraverseEntry(fullPath, isDir));

            if (result == TraverseResult.Stop)
            {
                dir.ListDirEnd();
                return TraverseResult.Stop;
            }

            if (isDir)
            {
                if (result != TraverseResult.SkipDir)
                {
                    TraverseResult childResult = Traverse(fullPath, visitor);

                    if (childResult == TraverseResult.Stop)
                    {
                        dir.ListDirEnd();
                        return TraverseResult.Stop;
                    }
                }
            }
        }

        dir.ListDirEnd();
        return TraverseResult.Continue;
    }

    public readonly struct TraverseEntry(string fullPath, bool isDirectory)
    {
        public string FullPath { get; } = fullPath;
        public bool IsDirectory { get; } = isDirectory;

        public string FileName => Path.GetFileName(FullPath);
    }

    /// <summary>
    /// Recursively searches for the file name and if found returns the full file path to
    /// that file.
    /// 
    /// <code>
    /// string fullPathToPlayer = FindFile("res://", "Player.tscn")
    /// </code>
    /// </summary>
    /// <returns>Returns the full path to the file or null if the file is not found</returns>
    public static string FindFile(string directory, string fileName)
    {
        string foundPath = null;

        Traverse(directory, entry =>
        {
            if (Path.GetFileName(entry.FullPath) == fileName)
            {
                foundPath = entry.FullPath;
                return TraverseResult.Stop;
            }

            return TraverseResult.Continue;
        });

        return foundPath;
    }

    /// <summary>
    /// Normalizes the specified path by replacing all forward slashes ('/') and backslashes ('\') with the system's directory separator character.
    /// </summary>
    /// <param name="path">The path to normalize.</param>
    /// <returns>A normalized path where all directory separators are replaced with the system's directory separator character.</returns>
    /// <remarks>
    /// This method is useful when dealing with paths that may come from different sources (e.g., URLs, different operating systems) and need to be standardized for the current environment.
    /// </remarks>
    public static string NormalizePath(string path)
    {
        return path
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar);
    }

    /// <summary>
    /// Recursively deletes all empty folders in this folder
    /// </summary>
    public static void DeleteEmptyDirectories(string path)
    {
        path = NormalizePath(ProjectSettings.GlobalizePath(path));

        if (Directory.Exists(path))
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                DeleteEmptyDirectories(directory);
                DeleteEmptyDirectory(directory);
            }
        }
    }

    /// <summary>
    /// Checks if the folder is empty and deletes it if it is
    /// </summary>
    private static void DeleteEmptyDirectory(string path)
    {
        path = NormalizePath(ProjectSettings.GlobalizePath(path));

        if (IsDirectoryEmpty(path))
        {
            Directory.Delete(path, recursive: false);
        }
    }

    /// <summary>
    /// Checks if the directory is empty
    /// </summary>
    /// <returns>Returns true if the directory is empty</returns>
    private static bool IsDirectoryEmpty(string path)
    {
        path = NormalizePath(ProjectSettings.GlobalizePath(path));

        return Directory.GetDirectories(path).Length == 0 && Directory.GetFiles(path).Length == 0;
    }
}
