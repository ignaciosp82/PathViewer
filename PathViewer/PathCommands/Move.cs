using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace PathViewer.PathCommands;

public partial class Move : PathCommand
{
    public const string CommandChar = "m";

    public override string Char { get; protected set; } = CommandChar;
    public override ItemType Type => ItemType.Move;

    [ObservableProperty]
    private double _x;

    [ObservableProperty]
    private double _y;

    public override bool HasX => true;
    public override bool HasY => true;
    public override double MinX => X;
    public override double MinY => Y;
    public override double MaxX => X;
    public override double MaxY => Y;

    public override void ScalePath(double scaleX, double scaleY)
    {
        X = Normalize(X * scaleX);
        Y = Normalize(Y * scaleY);
    }

    public override void MovePath(double distX, double distY)
    {
        X += distX;
        Y += distY;
    }

    private static readonly Regex _regex = new(@"
        ([Mm])\s*([+-]?[\d]+(?:\.[\d]+)?(?:e[+-][\d]+)?)\s*,?\s*
        ([+-]?[\d]+(?:\.[\d]+)?(?:e[+-][\d]+)?)",
        RegexOptions.IgnorePatternWhitespace);

    public static new Move Parse(string input)
    {
        Match match = _regex.Match(input);
        if (match.Success)
        {
            return new()
            {
                IsAbsolute = match.Groups[1].Value != CommandChar,
                X = double.Parse(match.Groups[2].Value),
                Y = double.Parse(match.Groups[3].Value)
            };
        }
        throw new ArgumentException($"Not a {nameof(Move)} command");
    }

    public override string ToString() =>
        $"{Char}{X},{Y}";
}
