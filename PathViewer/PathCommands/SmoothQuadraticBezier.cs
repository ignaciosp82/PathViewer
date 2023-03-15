using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace PathViewer.PathCommands;

public partial class SmoothQuadraticBezier : PathCommand
{
    public override string CommandChar => "T";
    public override ItemType Type => ItemType.SmoothQuadraticBezier;
    [ObservableProperty]
    private double _endX;

    [ObservableProperty]
    private double _endY;

    public override bool HasX => true;
    public override bool HasY => true;
    public override double MinX => EndX;
    public override double MinY => EndY;
    public override double MaxX => EndX;
    public override double MaxY => EndY;

    public override void ScalePath(double scaleX, double scaleY)
    {
        EndX = Normalize(EndX * scaleX);
        EndY = Normalize(EndY * scaleY);
    }

    public override void MovePath(double distX, double distY)
    {
        EndX += distX;
        EndY += distY;
    }

    private static readonly Regex _regex = new(
        @"[Tt][\s]*      ([-+]?(?:\d+\.\d+|\.\d+|\d+))(?:\s+,\s+|,\s+|,|\s*){0,1}
        (?:(?<=[,\s])|\b)([-+]?(?:\d+\.\d+|\.\d+|\d+))",
        RegexOptions.IgnorePatternWhitespace);

    public static bool TryParse(
        string input,
        [MaybeNullWhen(false)] out SmoothQuadraticBezier value)
    {
        Match match = _regex.Match(input);
        if (match.Success)
        {
            value = new()
            {
                EndX = double.Parse(match.Groups[1].Value),
                EndY = double.Parse(match.Groups[2].Value)
            };
            return true;
        }
        value = null;
        return false;
    }

    public override string ToString() =>
        $"{CommandChar}{EndX},{EndY}";
}
