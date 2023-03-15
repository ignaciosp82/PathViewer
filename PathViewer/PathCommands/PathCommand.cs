
using CommunityToolkit.Mvvm.ComponentModel;

using System;

namespace PathViewer.PathCommands;

public abstract class PathCommand : ObservableObject
{
    public abstract string CommandChar { get; }

    public abstract ItemType Type { get; }

    // Used to calculate size and position.
    public virtual bool HasX => false;
    public virtual bool HasY => false;
    public virtual double MinX => throw new NotImplementedException("Item doesn't have X coordinates");
    public virtual double MinY => throw new NotImplementedException("Item doesn't have Y coordinates");
    public virtual double MaxX => throw new NotImplementedException("Item doesn't have X coordinates");
    public virtual double MaxY => throw new NotImplementedException("Item doesn't have Y coordinates");

    public virtual void ScalePath(double scaleX, double scaleY) { }

    public virtual void MovePath(double distX, double distY) { }

    protected static double Normalize(double n)
    {
        int digits =
            n >= 100 ? 0
            : n >= 10 ? 1
            : n >= 1 ? 2
            : 3;
        return Math.Round(n, digits);
    }

    public static PathCommand Parse(string command)
    {
        if (Close.TryParse(command, out var close))
            return close;
        else if (CubicBezier.TryParse(command, out var curve))
            return curve;
        else if (EllipticalArc.TryParse(command, out var arc))
            return arc;
        else if (HorizontalLine.TryParse(command, out var hl))
            return hl;
        else if (Line.TryParse(command, out var line))
            return line;
        else if (Move.TryParse(command, out var move))
            return move;
        else if (QuadraticBezier.TryParse(command, out var qb))
            return qb;
        else if (SmoothCubicBezier.TryParse(command, out var scb))
            return scb;
        else if (SmoothQuadraticBezier.TryParse(command, out var sqb))
            return sqb;
        else if (VerticalLine.TryParse(command, out var vl))
            return vl;

        throw new ArgumentException(
            $"Unable to parse [{command}] into a PathCommand",
            nameof(command));
    }
}
