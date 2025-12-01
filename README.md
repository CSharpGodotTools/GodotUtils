## What is this?
An ever expanding utils library for Godot C#. This is the library I am using across all my games, now open source for everyone else to enjoy as well.

## Install
Open up the project with `project.godot` and build the game.

Copy `root/bin/GodotUtils.dll` and place it in `res://addons` in your project and add the following to your `.csproj`.
```xml
<ItemGroup>
  <Reference Include="GodotUtils">
    <HintPath>addons/GodotUtils.dll</HintPath>
  </Reference>
</ItemGroup>
```
