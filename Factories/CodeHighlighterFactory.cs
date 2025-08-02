using Godot;

namespace GodotUtils;

public static class CodeHighlighterFactory
{
    private const string Pink         = "ffb6ff";
    private const string LavenderGray = "a8b1d6";
    private const string LightPurple  = "b988ff";
    private const string Periwinkle   = "becaf5";
    private const string DarkGray     = "434048";
    private const string LightLilac   = "a59fff";

    public static CodeHighlighter Create()
    {
        CodeHighlighter editor = new()
        {
            KeywordColors       = [],
            NumberColor         = new Color(Pink),
            SymbolColor         = new Color(LavenderGray),
            FunctionColor       = new Color(LightPurple),
            MemberVariableColor = new Color(Periwinkle),
            ColorRegions        = new Godot.Collections.Dictionary
            {
                { "//", new Color(DarkGray) }
            },
        };

        string[] keywords = ["var", "true", "false", "new", "private", "public", "protected", "internal", "void"];

        foreach (string keyword in keywords)
        {
            editor.KeywordColors.Add(keyword, new Color(LightLilac));
        }

        return editor;
    }
}
