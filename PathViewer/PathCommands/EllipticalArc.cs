using CommunityToolkit.Mvvm.ComponentModel;

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Windows;

namespace PathViewer.PathCommands;

public partial class EllipticalArc : PathCommand
{
    public override string CommandChar => "A";
    public override ItemType Type => ItemType.EllipticalArc;

    public override bool HasX => true;
    public override bool HasY => true;
    // If we were being clever, we could calculate the extents using all of the
    // parameters, but that's way out of scope.
    // If anyone ever decides to do this, please calculate them when the parameters
    // change, and not in the getter for the properties!
    public override double MinX => EndX;
    public override double MinY => EndY;
    public override double MaxX => EndX;
    public override double MaxY => EndY;

    public override void ScalePath(double scaleX, double scaleY)
    {
        SizeX = Normalize(SizeX * scaleX);
        SizeY = Normalize(SizeY * scaleY);
        EndX = Normalize(EndX * scaleX);
        EndY = Normalize(EndY * scaleY);
    }

    public override void MovePath(double distX, double distY)
    {
        EndX += distX;
        EndY += distY;
    }

    [ObservableProperty]
    private double _sizeX;

    [ObservableProperty]
    private double _sizeY;

    [ObservableProperty]
    private double _rotationAngle;

    [ObservableProperty]
    private bool _isLargeArc;

    [ObservableProperty]
    private bool _isPositiveSweepDirection;

    [ObservableProperty]
    private double _endX;

    [ObservableProperty]
    private double _endY;

    private static readonly Regex _regex = new(
        @"[Aa][\s]*      ([-+]?(?:\d+\.\d+|\.\d+|\d+))(?:\s+,\s+|,\s+|,|\s*){0,1}
        (?:(?<=[,\s])|\b)([-+]?(?:\d+\.\d+|\.\d+|\d+))(?:\s+,\s+|,\s+|,|\s*){0,1}
        (?:(?<=[,\s])|\b)([-+]?(?:\d+\.\d+|\.\d+|\d+))(?:\s+,\s+|,\s+|,|\s*){0,1}
        (?:(?<=[,\s])|\b)(1|0)(?:\s+,\s+|,\s+|,|\s*){0,1}
        (?:(?<=[,\s])|\b)(1|0)(?:\s+,\s+|,\s+|,|\s*){0,1}
        (?:(?<=[,\s])|\b)([-+]?(?:\d+\.\d+|\.\d+|\d+))(?:\s+,\s+|,\s+|,|\s*){0,1}
        (?:(?<=[,\s])|\b)([-+]?(?:\d+\.\d+|\.\d+|\d+))",
        RegexOptions.IgnorePatternWhitespace);

    public static bool TryParse(
    string input,
    [MaybeNullWhen(false)] out EllipticalArc result)
    {
        Match match = _regex.Match(input);
        if (match.Success)
        {
            result = new()
            {
                SizeX = double.Parse(match.Groups[1].Value),
                SizeY = double.Parse(match.Groups[2].Value),
                RotationAngle = double.Parse(match.Groups[3].Value),
                IsLargeArc = match.Groups[4].Value == "1",
                IsPositiveSweepDirection = match.Groups[5].Value == "1",
                EndX = double.Parse(match.Groups[6].Value),
                EndY = double.Parse(match.Groups[7].Value)
            };
            return true;
        }
        result = null;
        return false;
    }


    public override string ToString() =>
        $"{CommandChar}{SizeX},{SizeY},{RotationAngle},{(IsLargeArc ? "1" : "0")},{(IsPositiveSweepDirection ? "1" : "0")},{EndX},{EndY}";
}
