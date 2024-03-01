
using CommunityToolkit.Mvvm.ComponentModel;

using System;

namespace PathViewer.PathCommands;

public abstract class PathCommand : ObservableObject
{
    public abstract string Char { get; protected set; }

    public abstract ItemType Type { get; }

    private bool _isAbsolute;
    public bool IsAbsolute
    {
        get => _isAbsolute;
        set
        {
            if (SetProperty(ref _isAbsolute, value))
            {
                Char = value ? Char.ToUpper() : Char.ToLower();
            }
        }
    }

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

    public static PathCommand Parse(string command) =>
        command[0].ToString().ToLower() switch
        {
            Close.CommandChar => Close.Parse(command),
            CubicBezier.CommandChar => CubicBezier.Parse(command),
            EllipticalArc.CommandChar => EllipticalArc.Parse(command),
            HorizontalLine.CommandChar => HorizontalLine.Parse(command),
            Line.CommandChar => Line.Parse(command),
            Move.CommandChar => Move.Parse(command),
            QuadraticBezier.CommandChar => QuadraticBezier.Parse(command),
            SmoothCubicBezier.CommandChar => SmoothCubicBezier.Parse(command),
            SmoothQuadraticBezier.CommandChar => SmoothQuadraticBezier.Parse(command),
            VerticalLine.CommandChar => VerticalLine.Parse(command),
            _ => throw new ArgumentException(
                $"Unable to parse [{command}] into a PathCommand",
                nameof(command)),
        };
}
