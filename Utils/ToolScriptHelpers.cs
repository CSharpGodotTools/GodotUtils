using Godot;
using System.IO;

namespace GodotUtils.UI;

[Tool]
public partial class ToolScriptHelpers : Node
{
    [Export] 
    public bool RemoveEmptyFolders
    {
        get => false;
        set => DeleteEmptyFolders();
    }

    [Export]
    public bool RemoveOrphanedCSUIDFiles
    {
        get => false;
        set => DeleteOrphanedCSUIDFiles();
    }

    private static void DeleteEmptyFolders()
    {
        if (!Engine.IsEditorHint()) // Do not trigger on game build
            return;

        DirectoryUtils.DeleteEmptyDirectories("res://");

        GD.Print("Removed all empty folders from the project. Restart the game engine or wait some time to see the effect.");
    }

    private static void DeleteOrphanedCSUIDFiles()
    {
        if (!Engine.IsEditorHint())
            return;

        string projectPath = ProjectSettings.GlobalizePath("res://");

        int deletedCount = 0;

        foreach (string file in Directory.GetFiles(projectPath, "*.cs.uid", SearchOption.AllDirectories))
        {
            string directory = Path.GetDirectoryName(file);
            string baseName = Path.GetFileNameWithoutExtension(file).Replace(".cs", ""); // Remove ".cs" from ".cs.uid"
            string csFile = Path.Combine(directory, baseName + ".cs");

            if (!File.Exists(csFile))
            {
                File.Delete(file);
                deletedCount++;
            }
        }

        GD.Print($"Deleted {deletedCount} orphaned .cs.uid files.");
    }
}
