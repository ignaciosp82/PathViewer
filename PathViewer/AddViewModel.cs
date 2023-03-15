using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PathViewer.PathCommands;

using System;
using System.Collections.Generic;
using System.Linq;

namespace PathViewer;

public partial class AddOrEditViewModel : ViewModelBase
{
    public AddOrEditViewModel()
    {
        // Hook up change notifications.
        FindRelayCommands();

        for (int i = 0; i < ValueCount; ++i)
        {
            ValueItem item = new();
            item.PropertyChanged += OnDataChanged;
            Values.Add(item);
            if (i < FlagCount)
            {
                FlagItem flag = new();
                flag.PropertyChanged += OnDataChanged;
                Flags.Add(flag);
            }
        }

        // The compiler isn't convinced that setting the property
        // sets the field, so we'll just do this to make the compiler
        // less grumpy.
        _type = ItemTypes[0];
        Type = ItemTypes.Where(t => t.Type == ItemType.Move).First();
    }

    public AddOrEditViewModel(PathCommand command) : this()
    {
        Type = (from t in ItemTypes where t.Type == command.Type select t).First();
        switch (command.Type)
        {
            case ItemType.Close:
                break;
            case ItemType.CubicBezier:
                var cb = (CubicBezier)command;
                Values[0].Value = cb.Control1X;
                Values[1].Value = cb.Control1Y;
                Values[2].Value = cb.Control2X;
                Values[3].Value = cb.Control2Y;
                Values[4].Value = cb.EndX;
                Values[5].Value = cb.EndY;
                break;
            case ItemType.EllipticalArc:
                var ea = (EllipticalArc)command;
                Values[0].Value = ea.SizeX;
                Values[1].Value = ea.SizeY;
                Values[3].Value = ea.RotationAngle;
                Flags[0].Value = ea.IsLargeArc;
                Flags[1].Value = ea.IsPositiveSweepDirection;
                Values[4].Value = ea.EndX;
                Values[5].Value= ea.EndY;
                break;
            case ItemType.HorizontalLine:
                Values[0].Value = ((HorizontalLine)command).EndX;
                break;
            case ItemType.Line:
                var l = (Line)command;
                Values[0].Value = l.EndX;
                Values[1].Value = l.EndY;
                break;
            case ItemType.Move:
                var m = (Move)command;
                Values[0].Value = m.X;
                Values[1].Value = m.Y;
                break;
            case ItemType.QudraticBezier:
                var qb = (QuadraticBezier)command;
                Values[0].Value = qb.ControlX;
                Values[1].Value = qb.ControlY;
                Values[2].Value = qb.EndX;
                Values[3].Value = qb.EndY;
                break;
            case ItemType.SmoothCubicBezier:
                var scb = (SmoothCubicBezier)command;
                Values[0].Value = scb.Control2X;
                Values[1].Value = scb.Control2Y;
                Values[2].Value = scb.EndX;
                Values[3].Value = scb.EndY;
                break;
            case ItemType.SmoothQuadraticBezier:
                var sqb = (SmoothQuadraticBezier)command;
                Values[0].Value = sqb.EndX;
                Values[1].Value = sqb.EndY;
                break;
            case ItemType.VerticalLine:
                var vl = (VerticalLine)command;
                Values[0].Value = vl.EndY;
                break;
            default:
                break;
        }
    }

    private void OnDataChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Result));
    }

    public override void Dispose()
    {
        foreach (var item in Values)
        {
            item.PropertyChanged -= OnDataChanged;
        }
        foreach (var flag in Flags)
        {
            flag.PropertyChanged -= OnDataChanged;
        }
        base.Dispose();
    }

    private const int ValueCount = 6;
    private const int FlagCount = 2;

    #region View Model Properties
    public ItemTypeItem[] ItemTypes { get; } = new[]
    {
        new ItemTypeItem("Close", ItemType.Close),
        new ItemTypeItem(
            "Cubic Bezier",
            ItemType.CubicBezier,
            new[]
            {
                "Control Point 1 X",
                "Control Point 1 Y",
                "Control Point 2 X",
                "Control Point 2 Y",
                "End X",
                "End Y"
            }),
        new ItemTypeItem(
            "Elliptical Arc",
            ItemType.EllipticalArc,
            new[] { "Size X", "Size Y", "Rotation Angle", "End X", "End Y" },
            new[] { "Large Arc", "Positive Sweep Direction"}),
        new ItemTypeItem(
            "Horizontal Line",
            ItemType.HorizontalLine,
            new[] { "End X" }),
        new ItemTypeItem(
            "Line",
            ItemType.Line,
            new[] { "End X", "End Y"}),
        new ItemTypeItem(
            "MovePath",
            ItemType.Move,
            new[] { "X", "Y" }),
        new ItemTypeItem(
            "Qudratic Bezier",
            ItemType.QudraticBezier,
            new[] { "Control X", "Control Y", "End X", "End Y" }),
        new ItemTypeItem(
            "Smooth Cubic Bezier",
            ItemType.SmoothCubicBezier,
            new[] { "Control 2 X", "Control 2 Y", "End X", "End Y"}),
        new ItemTypeItem(
            "Smooth Quadratic Bezier",
            ItemType.SmoothQuadraticBezier,
            new[] { "End X", "End Y" }),
        new ItemTypeItem(
            "Vertical Line",
            ItemType.VerticalLine,
            new[] { "End Y" })
    };

    private ItemTypeItem _type;
    public ItemTypeItem Type
    {
        get => _type;
        set
        {
            if (SetProperty(ref _type, value))
            {
                for (int i = 0; i < ValueCount; ++i)
                {
                    ValueLabels[i] =
                        _type.Values.Count > i
                        ? _type.Values[i]
                        : null;
                    if (i < FlagCount)
                    {
                        FlagLabels[i] =
                            _type.Flags.Count > i
                            ? _type.Flags[i]
                            : null;
                    }
                }
                OnPropertyChanged(nameof(ValueLabels));
                OnPropertyChanged(nameof(FlagLabels));
                OnPropertyChanged(nameof(Result));
            }
        }
    }

    public List<ValueItem> Values { get; } = new();
    public List<FlagItem> Flags { get; } = new();

    [ObservableProperty]
    private string?[] _valueLabels =
        Enumerable.Repeat<string?>(null, ValueCount).ToArray();

    [ObservableProperty]
    private string?[] _flagLabels =
        Enumerable.Repeat<string?>(null, FlagCount).ToArray();

    public string? Result =>
        Command?.ToString() ?? string.Empty;
    #endregion

    #region Commands
    [RelayCommand(CanExecute = nameof(IsValidated))]
    private void Ok()
    {
        OnClose();
    }

    #region Command Predicates
    public bool IsValidated => true;
    #endregion
    #endregion

    public PathCommand Command => (Type?.Type ?? ItemType.Close) switch
    {
        ItemType.Close => new Close(),
        ItemType.CubicBezier => new CubicBezier
        {
            Control1X = Values[0].Value,
            Control1Y = Values[1].Value,
            Control2X = Values[2].Value,
            Control2Y = Values[3].Value,
            EndX = Values[4].Value,
            EndY = Values[5].Value
        },
        ItemType.EllipticalArc => new EllipticalArc
        {
            SizeX = Values[0].Value,
            SizeY = Values[1].Value,
            RotationAngle = Values[2].Value,
            IsLargeArc = Flags[0].Value,
            IsPositiveSweepDirection = Flags[1].Value,
            EndX = Values[3].Value,
            EndY = Values[4].Value
        },
        ItemType.HorizontalLine => new HorizontalLine { EndX = Values[0].Value },
        ItemType.Line => new Line { EndX = Values[0].Value, EndY = Values[1].Value },
        ItemType.Move => new Move { X = Values[0].Value, Y = Values[1].Value },
        ItemType.QudraticBezier => new QuadraticBezier
        {
            ControlX = Values[0].Value,
            ControlY = Values[1].Value,
            EndX = Values[2].Value,
            EndY = Values[3].Value
        },
        ItemType.SmoothCubicBezier => new SmoothCubicBezier
        {
            Control2X = Values[0].Value,
            Control2Y = Values[1].Value,
            EndX = Values[2].Value,
            EndY = Values[3].Value
        },
        ItemType.SmoothQuadraticBezier => new SmoothQuadraticBezier
        {
            EndX = Values[0].Value,
            EndY = Values[1].Value,
        },
        ItemType.VerticalLine => new VerticalLine { EndY = Values[0].Value },
        _ => throw new InvalidOperationException($"Undefined path command [{(Type?.Type.ToString() ?? "(null)")}]")
    };


    public class ItemTypeItem
    {
        public ItemTypeItem(
            string name,
            ItemType type,
            IList<string>? values = null,
            IList<string>? flags = null)
        {
            Name = name;
            Type = type;
            Values = values ?? new List<string>();
            Flags = flags ?? new List<string>();
        }
        public string Name { get; }
        public ItemType Type { get; }
        public IList<string> Values { get; } = Array.Empty<string>();
        public IList<string> Flags { get; } = Array.Empty<string>();
    }

    // In order to get change notifications for items in an ObservableCollection,
    // I have to have objects that notify of change.
    public partial class ValueItem : ObservableObject
    {
        [ObservableProperty]
        private double _value;
        public override string ToString() => Value.ToString();
    }
    public partial class FlagItem : ObservableObject
    {
        [ObservableProperty]
        private bool _value;
        public override string ToString() => Value.ToString();
    }
}
