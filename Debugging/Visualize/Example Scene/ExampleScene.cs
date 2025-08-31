#if DEBUG
using Godot;

namespace GodotUtils.Debugging.Visualize;

public partial class ExampleScene : Node
{
    [Export] private int _cameraSpeed = 5;
    [Export] private PackedScene _spriteExampleScene;

    private Camera2D _camera;

    public override void _Ready()
    {
        _camera = GetNode<Camera2D>("Camera2D");

        VisualizeExampleSprite sprite = _spriteExampleScene.Instantiate<VisualizeExampleSprite>();

        // As you can see the visualize info is created at the moment of node creation
        _ = new GodotTween(this)
            .Delay(1)
            .Callback(() =>
            {
                AddChild(sprite);
                sprite.GlobalPosition = new Vector2(200, 100);
            });
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 dir = Input.GetVector(InputActions.MoveLeft, InputActions.MoveRight, InputActions.MoveUp, InputActions.MoveDown);
        
        _camera.Position += dir * _cameraSpeed;
    }
}
#endif
