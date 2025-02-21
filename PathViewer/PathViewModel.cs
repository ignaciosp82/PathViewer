using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PathViewer.PathCommands;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace PathViewer;

public partial class PathViewModel : ViewModelBase
{
    public PathViewModel()
    {
        // CommunityToolkit.MVVM doesn't subscribe to RequestSuggested,
        // so that makes change notifications a manual thing. This subverts that.
        FindRelayCommands();

        ParseData(Data);
        Bounds = Geometry.Parse(Data).Bounds;
    }

    private void ParseData(string data)
    {
        PathCommands.Clear();
        Match match = _regex.Match(data);
        while (match.Success)
        {
            try
            {
                PathCommands.Add(PathCommand.Parse(match.Groups[0].Value));
            }
            catch (Exception ex)
            {
                PathError = ex.Message;
                return;
            }
            match = match.NextMatch();
        }
    }

    private static Regex _regex = new(@"([A-Za-z])(?:\s*([+\-]?\d*\.?\d+(?:[eE][+\-]?\d+)?)(?:\s*,\s*|\s+|\b(?![,\.\s])))*");

    #region Commands
    [RelayCommand(CanExecute = nameof(CanReset))]
    private void ResetZoom() => Zoom = 1;

    [RelayCommand]
    private void AddItem() => AddItemAt(-1);

    private void AddItemAt(int location = -1)
    {
        using AddOrEditViewModel add = new();
        if (ShowModal(
            add,
            (location == -1 ? "Insert" : "Add") + " Path Command"))
        {
            if (location != -1)
            {
                PathCommands.Insert(location, add.Command);
            }
            else
            {
                PathCommands.Add(add.Command);
            }
            Data = GeneratedData;
        }
    }

    [RelayCommand(CanExecute = nameof(SomethingSelected))]
    private void InsertItem()
    {
        if (SelectedIndex == -1) return;
        AddItemAt(SelectedIndex);
    }

    [RelayCommand(CanExecute = nameof(SomethingSelected))]
    private void Edit(PathCommand? command = null)
    {
        command ??= SelectedItem;
        if (command is null) return;
        using AddOrEditViewModel edit = new(command);
        if (ShowModal(edit, "Edit Path Command"))
        {
            // Easier to insert new (edited) item than
            // copy properties.
            int target = SelectedIndex + 1;
            PathCommands.Insert(SelectedIndex, edit.Command);
            PathCommands.RemoveAt(target);
            SelectedIndex = target;

            Data = GeneratedData;
        }
    }

    [RelayCommand(CanExecute = nameof(SomethingSelected))]
    private void Delete()
    {
        if (SelectedItem is null) return;
        int target = SelectedIndex;
        PathCommands.RemoveAt(target);
        if (PathCommands.Count > 0)
        {
            SelectedIndex =
                PathCommands.Count >= target - 1
                ? target
                : PathCommands.Count - 1;
        }
        Data = GeneratedData;
    }

    [RelayCommand(CanExecute = nameof(HasCommands))]
    private void Clear()
    {
        PathCommands.Clear();

        Data = GeneratedData;
    }

    [RelayCommand(CanExecute = nameof(CanMoveUp))]
    private void MoveUp()
    {
        if (SelectedItem is null) return;
        PathCommand cmd = SelectedItem;
        int target = SelectedIndex - 1;
        PathCommands.RemoveAt(SelectedIndex);
        PathCommands.Insert(target, cmd);
        SelectedIndex = target;

        Data = GeneratedData;
    }

    [RelayCommand(CanExecute = nameof(CanMoveDown))]
    private void MoveDown()
    {
        if (SelectedItem is null) return;
        PathCommand cmd = SelectedItem;
        int target = SelectedIndex + 1;
        PathCommands.RemoveAt(SelectedIndex);
        PathCommands.Insert(target, cmd);
        SelectedIndex = target;

        Data = GeneratedData;
    }

    [RelayCommand]
    private void ScalePath()
    {
        ScaleOrMoveViewModel vm = new(PathWidth, PathHeight, true);
        if (ShowModal(vm, "ScalePath Path"))
        {
            double scaleX = vm.Width / PathWidth;
            double scaleY = vm.Height / PathHeight;
            foreach (PathCommand cmd in PathCommands)
            {
                cmd.ScalePath(scaleX, scaleY);
            }
            Data = GeneratedData;
        }
    }

    [RelayCommand]
    private void MovePath()
    {
        ScaleOrMoveViewModel vm = new(-Origin.X, -Origin.Y, false);
        if (ShowModal(vm, "ScalePath Path"))
        {
            foreach (PathCommand cmd in PathCommands)
            {
                cmd.MovePath(vm.Width, vm.Height);
            }
            Data = GeneratedData;
        }
    }

    #region Predicates
    private bool CanReset => Zoom != 1;
    private bool SomethingSelected => SelectedIndex != -1;
    private bool HasCommands => PathCommands.Count > 0;
    private bool CanMoveUp => SelectedIndex > 0;
    private bool CanMoveDown => SelectedIndex < PathCommands.Count - 1;
    #endregion
    #endregion

    #region Helpers
    private string GeneratedData => string.Join(" ", PathCommands.Select(p => p.ToString()));

    private bool _isUpdating;
    #endregion

    #region View Model Properties
    private string _data =
        "M-50,90 "
        + "A90,90 0 0 0 180,90 "
        + "M30,30 "
        + "A15,15 0 0 0 60,30 "
        + "M30,30 "
        + "A15,15 0 0 1 60,30 "
        + "M120,30 "
        + "A15,15 0 0 0 150,30 "
        + "M120,30 "
        + "A15,15 0 0 1 150,30";
    public string Data
    {
        get => _data;
        set
        {
            if (!_isUpdating && SetProperty(ref _data, value))
            {
                try
                {
                    int curSel = SelectedIndex;
                    _isUpdating = true;
                    Bounds = Geometry.Parse(_data).Bounds;
                    PathError = null;
                    ParseData(_data);
                    SelectedIndex = curSel;
                }
                catch (FormatException ex)
                {
                    PathError = ex.Message;
                }
                finally
                {
                    _isUpdating = false;
                }
            }
        }
    }

    [ObservableProperty]
    private bool _showOriginLines = true;

    [ObservableProperty]
    private bool _showBoundingBox = true;

    [ObservableProperty]
    private double _zoom = 1.0;

    public List<string> ColorsList => typeof(Colors).GetProperties().Select(p => p.Name).ToList();

    public static List<int> Thicknesses => Enumerable.Range(1, 20).ToList();

    [ObservableProperty]
    private string _strokeColor = nameof(Colors.Black);

    [ObservableProperty]
    private int _strokeThickness = 2;

    [ObservableProperty]
    private string _fillColor = nameof(Colors.Transparent);

    public ObservableCollection<PathCommand> PathCommands { get; } = new();

    [ObservableProperty]
    private PathCommand? _selectedItem;

    [ObservableProperty]
    private int _selectedIndex;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanvasSize))]
    private Size _viewportSize;

    public Size CanvasSize =>
        new(Math.Max(ViewportSize.Width, Bounds.Width + PathMargin * 2),
            Math.Max(ViewportSize.Height, Bounds.Height + PathMargin * 2));

    [ObservableProperty]
    private double _pathMargin = 20;

    [ObservableProperty]
    private string? _pathError;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PathWidth))]
    [NotifyPropertyChangedFor(nameof(PathHeight))]
    [NotifyPropertyChangedFor(nameof(Origin))]
    [NotifyPropertyChangedFor(nameof(Extent))]
    [NotifyPropertyChangedFor(nameof(PathStartX))]
    [NotifyPropertyChangedFor(nameof(PathStartY))]
    [NotifyPropertyChangedFor(nameof(CanvasSize))]
    [NotifyPropertyChangedFor(nameof(Size))]
    [NotifyPropertyChangedFor(nameof(ZeroOrigin))]
    [NotifyPropertyChangedFor(nameof(TopLabel))]
    [NotifyPropertyChangedFor(nameof(BottomLabel))]
    [NotifyPropertyChangedFor(nameof(LeftLabel))]
    [NotifyPropertyChangedFor(nameof(RightLabel))]
    private Rect _bounds;

    public double PathWidth => Bounds.Width;

    public double PathHeight => Bounds.Height;

    public Size Size => Bounds.Size;

    public Point Origin => Bounds.TopLeft;

    public Point Extent => Bounds.BottomRight;

    public double DrawWidth => Bounds.Width + PathMargin * 2;
    public double DrawHeight => Bounds.Height + PathMargin * 2;

    public double PathStartX => -Bounds.X + PathMargin;

    public double PathStartY => -Bounds.Y + PathMargin;

    public Point ZeroOrigin => new(-Bounds.X, -Bounds.Y);

    public double LeftLabel => PathMargin / 2;
    public double RightLabel => Bounds.Width + PathMargin * 1.5;
    public double TopLabel => PathMargin / 2;
    public double BottomLabel => Bounds.Height + PathMargin * 1.5;
    #endregion
}
