using CommunityToolkit.Mvvm.ComponentModel;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace PathViewer.PathCommands;

public partial class VerticalLine : PathCommand
{
    public const string CommandChar = "v";

    public override string Char { get; protected set; } = CommandChar;
    public override ItemType Type => ItemType.VerticalLine;

    [ObservableProperty]
    private double _endY;

    public override bool HasY => true;
    public override double MinY => EndY;
    public override double MaxY => EndY;

    public override void ScalePath(double scaleX, double scaleY)
    {
        EndY *= scaleY;
    }

    public override void MovePath(double distX, double distY)
    {
        EndY += distY;
    }

    private static readonly Regex _regex = new(@"([Vv])\s*([+-]?[\d]+(?:\.[\d]+)?(?:e[+-][\d]+)?)");

    public static new VerticalLine Parse(string input)
    {
        Match match = _regex.Match(input);
        if (match.Success)
        {
            return new()
            {
                IsAbsolute = match.Groups[1].Value == CommandChar.ToUpper(),
                EndY = double.Parse(match.Groups[2].Value)
            };
        }
        throw new ArgumentException($"Not a {nameof(VerticalLine)} command");
    }

    public override string ToString() =>
        $"{Char}{EndY}";
}
