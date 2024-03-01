using System;

namespace PathViewer.PathCommands;

public class Close : PathCommand
{
    public const string CommandChar = "z";

    public override string Char { get; protected set; } = CommandChar;
    public override ItemType Type => ItemType.Close;

    public static new Close Parse(string input)
    {
        if (input.Trim().ToLower() == CommandChar)
        {
            return new();
        }
        throw new ArgumentException($"Not a {nameof(Close)} command");
    }

    public override string ToString() => CommandChar;
}
