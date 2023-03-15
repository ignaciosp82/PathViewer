using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace PathViewer.PathCommands;

public partial class SmoothCubicBezier : PathCommand
{
    public override string CommandChar => "S";
    public override ItemType Type => ItemType.SmoothCubicBezier;

    [ObservableProperty]
    private double _control2X;

    [ObservableProperty]
    private double _control2Y;

    [ObservableProperty]
    private double _endX;

    [ObservableProperty]
    private double _endY;

    public override void ScalePath(double scaleX, double scaleY)
    {
        Control2X = Normalize(Control2X * scaleX);
        Control2Y = Normalize(Control2Y * scaleY);
        EndX = Normalize(EndX * scaleX);
        EndY = Normalize(EndY * scaleY);
    }

    public override void MovePath(double distX, double distY)
    {
        Control2X += distX;
        Control2Y += distY;
        EndX += distX;
        EndY += distY;
    }

    public override bool HasX => true;
    public override bool HasY => true;
    public override double MinX => Math.Min(Control2X, EndX);
    public override double MinY => Math.Min(Control2Y, EndY);
    public override double MaxX => Math.Max(Control2X, EndX);
    public override double MaxY => Math.Max(Control2Y, EndY);

    private static readonly Regex _regex = new(
        @"[Ss][\s]*      ([-+]?(?:\d+\.\d+|\.\d+|\d+))(?:\s+,\s+|,\s+|,|\s*){0,1}
        (?:(?<=[,\s])|\b)([-+]?(?:\d+\.\d+|\.\d+|\d+))(?:\s+,\s+|,\s+|,|\s*){0,1}
        (?:(?<=[,\s])|\b)([-+]?(?:\d+\.\d+|\.\d+|\d+))(?:\s+,\s+|,\s+|,|\s*){0,1}
        (?:(?<=[,\s])|\b)([-+]?(?:\d+\.\d+|\.\d+|\d+))",
        RegexOptions.IgnorePatternWhitespace);

    public static bool TryParse(
        string input,
        [MaybeNullWhen(false)] out SmoothCubicBezier result)
    {
        Match match = _regex.Match(input);
        if (match.Success)
        {
            result = new()
            {
                Control2X = double.Parse(match.Groups[1].Value),
                Control2Y = double.Parse(match.Groups[2].Value),
                EndX = double.Parse(match.Groups[3].Value),
                EndY = double.Parse(match.Groups[4].Value)
            };
            return true;
        }
        result = null;
        return false;
    }

    public override string ToString() =>
        $"{CommandChar}{Control2X},{Control2Y},{EndX},{EndY}";
}
