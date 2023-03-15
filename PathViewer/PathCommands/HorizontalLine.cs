using CommunityToolkit.Mvvm.ComponentModel;

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Windows;

namespace PathViewer.PathCommands;

public partial class HorizontalLine : PathCommand
{
    public override string CommandChar => "H";
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

    private static readonly Regex _regex = new(@"[Hh]([-+]?(?:\d+\.\d+|\.\d+|\d+))");

    public static bool TryParse(
        string input,
        [MaybeNullWhen(false)] out HorizontalLine result)
    {
        Match match= _regex.Match(input);
        if (match.Success)
        {
            result = new()
            {
                EndX = double.Parse(match.Groups[1].Value)
            };
            return true;
        }
        result = null;
        return false;
    }

    public override string ToString() =>
        $"{CommandChar}{EndX}";
}
