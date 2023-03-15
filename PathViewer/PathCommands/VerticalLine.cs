using CommunityToolkit.Mvvm.ComponentModel;

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace PathViewer.PathCommands;

public partial class VerticalLine : PathCommand
{
    public override string CommandChar => "V";
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

    private static readonly Regex _regex = new(@"[Vv]([-+]?(?:\d+\.\d+|\.\d+|\d+))");

    public static bool TryParse(
        string input, 
        [MaybeNullWhen(false)] out VerticalLine value)
    {
        Match match = _regex.Match(input);
        if (match.Success)
        {
            value = new()
            {
                EndY = double.Parse(match.Groups[1].Value)
            };
            return true;
        }
        value = null;
        return false;
    }

    public override string ToString() =>
        $"{CommandChar}{EndY}";
}
