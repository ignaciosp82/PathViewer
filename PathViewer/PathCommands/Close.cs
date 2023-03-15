using System.Diagnostics.CodeAnalysis;

namespace PathViewer.PathCommands;

public class Close : PathCommand
{
    public override string CommandChar => "Z";
    public override ItemType Type => ItemType.Close;

    public static bool TryParse(
        string input,
        [MaybeNullWhen(false)] out Close result)
    {
        if (input.Trim().ToLower() == "z")
        {
            result = new();
            return true;
        }
        result = null;
        return false;
    }

    public override string ToString() => CommandChar;
}
