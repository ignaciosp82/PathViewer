using CommunityToolkit.Mvvm.ComponentModel;

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace PathViewer.PathCommands;

public partial class Move : PathCommand
{
    public override string CommandChar => "M";
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

    private static readonly Regex _regex = new(
    @"[Mm][\s]*      ([-+]?(?:\d+\.\d+|\.\d+|\d+))(?:\s+,\s+|,\s+|,|\s*){0,1}
        (?:(?<=[,\s])|\b)([-+]?(?:\d+\.\d+|\.\d+|\d+))",
    RegexOptions.IgnorePatternWhitespace);

    public static bool TryParse(
    string input,
    [MaybeNullWhen(false)] out Move result)
    {
        Match match = _regex.Match(input);
        if (match.Success)
        {
            result = new()
            {
                X = double.Parse(match.Groups[1].Value),
                Y = double.Parse(match.Groups[2].Value)
            };
            return true;
        }
        result = null;
        return false;
    }

    public override string ToString() =>
        $"{CommandChar}{X},{Y}";
}
