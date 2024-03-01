using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Text.RegularExpressions;

namespace PathViewer.PathCommands;

public partial class EllipticalArc : PathCommand
{
    public const string CommandChar = "a";
    public override string Char { get; protected set; } = CommandChar;
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

    private static readonly Regex _regex = new(@"
        ([Aa])\s*([+-]?[\d]+(?:\.[\d]+)?(?:e[+-][\d]+)?)\s*,?\s*
        ([+-]?[\d]+(?:\.[\d]+)?(?:e[+-][\d]+)?)\s*,?\s*
        ([+-]?[\d]+(?:\.[\d]+)?(?:e[+-][\d]+)?)\s*,?\s*
        ([01])\s*,?\s*
        ([01])\s*,?\s*
        ([+-]?[\d]+(?:\.[\d]+)?(?:e[+-][\d]+)?)\s*,?\s*
        ([+-]?[\d]+(?:\.[\d]+)?(?:e[+-][\d]+)?)",
        RegexOptions.IgnorePatternWhitespace);

    public static new EllipticalArc Parse(string input)
    {
        Match match = _regex.Match(input);
        if (match.Success)
        {
            return new()
            {
                IsAbsolute = match.Groups[1].Value != CommandChar,
                SizeX = double.Parse(match.Groups[2].Value),
                SizeY = double.Parse(match.Groups[3].Value),
                RotationAngle = double.Parse(match.Groups[4].Value),
                IsLargeArc = match.Groups[5].Value == "1",
                IsPositiveSweepDirection = match.Groups[6].Value == "1",
                EndX = double.Parse(match.Groups[7].Value),
                EndY = double.Parse(match.Groups[8].Value)
            };
        }
        throw new ArgumentException($"Not a {nameof(EllipticalArc)} command");
    }

    public override string ToString() =>
        $"{Char}{SizeX},{SizeY},{RotationAngle},{(IsLargeArc ? "1" : "0")},{(IsPositiveSweepDirection ? "1" : "0")},{EndX},{EndY}";
}
