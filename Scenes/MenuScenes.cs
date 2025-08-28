using Godot;

namespace GodotUtils.UI;

[GlobalClass]
public partial class MenuScenes : Resource
{
    [Export] public PackedScene MainMenu { get; set; }
    [Export] public PackedScene ModLoader { get; set; }
    [Export] public PackedScene Options { get; set; }
    [Export] public PackedScene Credits { get; set; }
}
