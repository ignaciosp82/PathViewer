using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Text.RegularExpressions;

namespace PathViewer.PathCommands;

public partial class CubicBezier : PathCommand
{
    public const string CommandChar = "c";

    public override string Char { get; protected set; } = CommandChar;
    public override ItemType Type => ItemType.CubicBezier;

    public override bool HasX => true;
    public override bool HasY => true;
    public override double MinX => Math.Min(Control1X, Math.Min(Control2X, EndX));
    public override double MinY => Math.Min(Control1Y, Math.Min(Control2Y, EndY));
    public override double MaxX => Math.Max(Control1X, Math.Max(Control2X, EndX));
    public override double MaxY => Math.Max(Control1Y, Math.Max(Control2Y, EndY));

    public override void ScalePath(double scaleX, double scaleY)
    {
        Control1X = Normalize(Control1X * scaleX);
        Control1Y = Normalize(Control1Y * scaleY);
        Control2X = Normalize(Control2X * scaleX);
        Control2Y = Normalize(Control2Y * scaleY);
        EndX = Normalize(EndX * scaleX);
        EndY = Normalize(EndY * scaleY);
    }

    public override void MovePath(double distX, double distY)
    {
        Control1X += distX;
        Control1Y += distY;
        Control2X += distX;
        Control2Y += distY;
        EndX += distX;
        EndY += distY;
    }

    [ObservableProperty]
    private double _control1X;

    [ObservableProperty]
    private double _control1Y;

    [ObservableProperty]
    private double _control2X;

    [ObservableProperty]
    private double _control2Y;

    [ObservableProperty]
    private double _endX;

    [ObservableProperty]
    private double _endY;

    private static readonly Regex _regex = new(@"
        ([Cc])\s*([+-]?[\d]+(?:\.[\d]+)?(?:e[+-][\d]+)?)\s*,?\s*
        ([+-]?[\d]+(?:\.[\d]+)?(?:e[+-][\d]+)?)\s*,?\s*
        ([+-]?[\d]+(?:\.[\d]+)?(?:e[+-][\d]+)?)\s*,?\s*
        ([+-]?[\d]+(?:\.[\d]+)?(?:e[+-][\d]+)?)\s*,?\s*
        ([+-]?[\d]+(?:\.[\d]+)?(?:e[+-][\d]+)?)\s*,?\s*
        ([+-]?[\d]+(?:\.[\d]+)?(?:e[+-][\d]+)?)\s*,?\s*
        ([+-]?[\d]+(?:\.[\d]+)?(?:e[+-][\d]+)?)",
        RegexOptions.IgnorePatternWhitespace);

    public static new CubicBezier Parse(string input)
    {
        Match match = _regex.Match(input);
        if (match.Success)
        {
            return new()
            {
                IsAbsolute = match.Groups[1].Value != CommandChar,
                Control1X = double.Parse(match.Groups[2].Value),
                Control1Y = double.Parse(match.Groups[3].Value),
                Control2X = double.Parse(match.Groups[4].Value),
                Control2Y = double.Parse(match.Groups[5].Value),
                EndX = double.Parse(match.Groups[5].Value),
                EndY = double.Parse(match.Groups[6].Value)
            };
        }
        throw new ArgumentException($"Not a {nameof(CubicBezier)} command");
    }

    public override string ToString() =>
        $"{Char}{Control1X},{Control1Y},{Control2X},{Control2Y},{EndX},{EndY}";
}
