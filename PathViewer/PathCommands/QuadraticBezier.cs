using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Text.RegularExpressions;

namespace PathViewer.PathCommands;

public partial class QuadraticBezier : PathCommand
{
    public const string CommandChar = "q";

    public override string Char { get; protected set; } = CommandChar;
    public override ItemType Type => ItemType.QuadraticBezier;

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


    private static readonly Regex _regex = new(@"
        ([Qq])\s*([+-]?[\d]+(?:\.[\d]+)?(?:e[+-][\d]+)?)\s*,?\s*
        ([+-]?[\d]+(?:\.[\d]+)?(?:e[+-][\d]+)?)\s*,?\s*
        ([+-]?[\d]+(?:\.[\d]+)?(?:e[+-][\d]+)?)\s*,?\s*
        ([+-]?[\d]+(?:\.[\d]+)?(?:e[+-][\d]+)?)",
        RegexOptions.IgnorePatternWhitespace);

    public static new QuadraticBezier Parse(string input)
    {
        Match match = _regex.Match(input);
        if (match.Success)
        {
            return new()
            {
                IsAbsolute = match.Groups[1].Value != CommandChar,
                ControlX = double.Parse(match.Groups[2].Value),
                ControlY = double.Parse(match.Groups[3].Value),
                EndX = double.Parse(match.Groups[4].Value),
                EndY = double.Parse(match.Groups[5].Value)
            };
        }
        throw new ArgumentException($"Not a {nameof(QuadraticBezier)} command");
    }

    public override string ToString() =>
        $"{Char}{ControlX},{ControlY},{EndX},{EndY}";
}
