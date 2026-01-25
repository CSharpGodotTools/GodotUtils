using Godot;

namespace GodotUtils;

public abstract class BaseShapeArea<TArea> where TArea : Node
{
    protected TArea Area => _area;

    private readonly TArea _area;

    protected BaseShapeArea(TArea area)
    {
        _area = area;
    }

    public abstract void SetColor(Color color, bool transparent = false);
    public abstract Color GetColor();

    public static implicit operator TArea(BaseShapeArea<TArea> area) => area._area;
}
