using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Text.RegularExpressions;

namespace PathViewer.PathCommands;

public partial class SmoothCubicBezier : PathCommand
{
    public const string CommandChar = "s";

    public override string Char { get; protected set; } = CommandChar;
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

    private static readonly Regex _regex = new(@"
        ([Ss])\s*([+-]?[\d]+(?:\.[\d]+)?(?:e[+-][\d]+)?)\s*,?\s*
        ([+-]?[\d]+(?:\.[\d]+)?(?:e[+-][\d]+)?)\s*,?\s*
        ([+-]?[\d]+(?:\.[\d]+)?(?:e[+-][\d]+)?)\s*,?\s*
        ([+-]?[\d]+(?:\.[\d]+)?(?:e[+-][\d]+)?)",
        RegexOptions.IgnorePatternWhitespace);

    public static new SmoothCubicBezier Parse(string input)
    {
        Match match = _regex.Match(input);
        if (match.Success)
        {
            return new()
            {
                IsAbsolute = match.Groups[1].Value != CommandChar,
                Control2X = double.Parse(match.Groups[2].Value),
                Control2Y = double.Parse(match.Groups[3].Value),
                EndX = double.Parse(match.Groups[4].Value),
                EndY = double.Parse(match.Groups[5].Value)
            };
        }
        throw new ArgumentException($"Not a {nameof(SmoothCubicBezier)} command");
    }

    public override string ToString() =>
        $"{Char}{Control2X},{Control2Y},{EndX},{EndY}";
}
