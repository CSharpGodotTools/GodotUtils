using Godot;
using System;
using Environment = Godot.Environment;

namespace GodotUtils.UI;

public partial class OptionsGraphics : Control
{
    public event Action<int> AntialiasingChanged;

    private ResourceOptions _options;
    private OptionButton _antialiasing;

    public override void _Ready()
    {
        _options = OptionsManager.GetOptions();

        SetupQualityPreset();
        SetupAntialiasing();
    }

    private void _OnQualityModeItemSelected(int index)
    {
        _options.QualityPreset = (QualityPreset)index;
    }

    private void _OnAntialiasingItemSelected(int index)
    {
        _options.Antialiasing = index;
        AntialiasingChanged?.Invoke(index);
    }

    private void SetupQualityPreset()
    {
        OptionButton optionBtnQualityPreset = GetNode<OptionButton>("%QualityMode");
        optionBtnQualityPreset.Select((int)_options.QualityPreset);
    }

    private void SetupAntialiasing()
    {
        _antialiasing = GetNode<OptionButton>("%Antialiasing");
        _antialiasing.Select(_options.Antialiasing);
    }

    private static Label CreateLabel(string text)
    {
        return new Label
        {
            Text = text,
            CustomMinimumSize = new Vector2(200, 0)
        };
    }
}

public enum QualityPreset
{
    Low,
    Medium,
    High
}
