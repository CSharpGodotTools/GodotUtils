> [!NOTE]
> This project has been archived because its files has been merged into [CSharpGodotTools/Template](https://github.com/CSharpGodotTools/Template).

## What is this?
An ever expanding utils library for Godot C#. This is the library I am using across all my games, now open source for everyone else to enjoy as well.

## Install
1. Open `project.godot` and build the game
2. Go to `root/bin`, copy `GodotUtils.dll`, `GodotUtils.xml` and place them in your project under `res://addons`
3. Add the following to your `.csproj`
```xml
<ItemGroup>
  <Reference Include="GodotUtils">
    <HintPath>addons/GodotUtils.dll</HintPath>
  </Reference>
</ItemGroup>
```
