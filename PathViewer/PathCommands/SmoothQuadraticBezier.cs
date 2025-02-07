using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Text.RegularExpressions;

namespace PathViewer.PathCommands;

public partial class SmoothQuadraticBezier : PathCommand
{
    public const string CommandChar = "t";

    public override string Char { get; protected set; } = CommandChar;
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

    private static readonly Regex _regex = new(@"
        ([Tt])\s*([+-]?[\d]+(?:\.[\d]+)?(?:e[+-][\d]+)?)\s*,?\s*
        ([+-]?[\d]+(?:\.[\d]+)?(?:e[+-][\d]+)?)",
        RegexOptions.IgnorePatternWhitespace);

    public static new SmoothQuadraticBezier Parse(string input)
    {
        Match match = _regex.Match(input);
        if (match.Success)
        {
            return new()
            {
                IsAbsolute = match.Groups[1].Value != CommandChar,
                EndX = double.Parse(match.Groups[2].Value),
                EndY = double.Parse(match.Groups[3].Value)
            };
        }
        throw new ArgumentException($"Not a {nameof(SmoothQuadraticBezier)} command");
    }

    public override string ToString() =>
        $"{Char}{EndX},{EndY}";
}
