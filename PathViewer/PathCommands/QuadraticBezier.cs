using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace PathViewer.PathCommands;

public partial class QuadraticBezier : PathCommand
{
    public override string CommandChar => "Q";
    public override ItemType Type => ItemType.QudraticBezier;

    [ObservableProperty]
    private double _controlX;

    [ObservableProperty]
    private double _controlY;

    [ObservableProperty]
    private double _endX;

    [ObservableProperty]
    private double _endY;

    public override void ScalePath(double scaleX, double scaleY)
    {
        ControlX = Normalize(ControlX * scaleX);
        ControlY = Normalize(ControlY * scaleY);
        EndX = Normalize(EndX * scaleX);
        EndY = Normalize(EndY * scaleY);
    }

    public override void MovePath(double distX, double distY)
    {
        ControlX += distX;
        ControlY += distY;
        EndX += distX;
        EndY += distY;
    }

    public override bool HasX => true;
    public override bool HasY => true;
    public override double MinX => Math.Min(ControlX, EndX);
    public override double MinY => Math.Min(ControlY, EndY);
    public override double MaxX => Math.Max(ControlX, EndX);
    public override double MaxY => Math.Max(ControlY, EndY);


    private static readonly Regex _regex = new(
    @"[Qq][\s]*          ([-+]?(?:\d+\.\d+|\.\d+|\d+))(?:\s+,\s+|,\s+|,|\s*){0,1}
        (?:(?<=[,\s])|\b)([-+]?(?:\d+\.\d+|\.\d+|\d+))(?:\s+,\s+|,\s+|,|\s*){0,1}
        (?:(?<=[,\s])|\b)([-+]?(?:\d+\.\d+|\.\d+|\d+))(?:\s+,\s+|,\s+|,|\s*){0,1}
        (?:(?<=[,\s])|\b)([-+]?(?:\d+\.\d+|\.\d+|\d+))",
    RegexOptions.IgnorePatternWhitespace);

    public static bool TryParse(
        string input,
        [MaybeNullWhen(false)] out QuadraticBezier result)
    {
        Match match = _regex.Match(input);
        if (match.Success)
        {
            result = new()
            {
                ControlX = double.Parse(match.Groups[1].Value),
                ControlY = double.Parse(match.Groups[2].Value),
                EndX = double.Parse(match.Groups[3].Value),
                EndY = double.Parse(match.Groups[4].Value)
            };
            return true;
        }
        result = null;
        return false;
    }

    public override string ToString() =>
        $"{CommandChar}{ControlX},{ControlY},{EndX},{EndY}";
}
