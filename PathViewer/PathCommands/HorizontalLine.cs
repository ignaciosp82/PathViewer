using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Text.RegularExpressions;

namespace PathViewer.PathCommands;

public partial class HorizontalLine : PathCommand
{
    public const string CommandChar = "h";

    public override string Char { get; protected set; } = CommandChar;
    public override ItemType Type => ItemType.HorizontalLine;

    [ObservableProperty]
    private double _endX;

    public override bool HasX => true;
    public override double MaxX => EndX;
    public override double MinX => EndX;

    public override void ScalePath(double scaleX, double scaleY)
    {
        EndX = Normalize(EndX * scaleX);
    }

    public override void MovePath(double distX, double distY)
    {
        EndX += distX;
    }

    private static readonly Regex _regex = new(@"([Hh])\s*([+-]?[\d]+(?:\.[\d]+)?(?:e[+-][\d]+)?)");

    public static new HorizontalLine Parse(string input)
    {
        Match match = _regex.Match(input);
        if (match.Success)
        {
            return new()
            {
                IsAbsolute = match.Groups[1].Value != CommandChar,
                EndX = double.Parse(match.Groups[2].Value)
            };
        }
        throw new ArgumentException($"Not a {nameof(HorizontalLine)} command");
    }

    public override string ToString() =>
        $"{Char}{EndX}";
}
