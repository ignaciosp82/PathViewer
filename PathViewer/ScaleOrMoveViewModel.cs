using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using System;
using System.Windows;

namespace PathViewer;

public partial class ScaleOrMoveViewModel : ViewModelBase
{
    // Design mode requires an empty contructor.
    public ScaleOrMoveViewModel() : this(100, 100, false) { }

    public ScaleOrMoveViewModel(
        double width,
        double height,
        bool isScaleMode)
    {
        IsScaleMode = isScaleMode;
        _updating = true;
        Width = width;
        Height = height;
        _updating = false;
    }

    private bool _updating;

    [ObservableProperty]
    private bool _isScaleMode;

    private double _width;
    public double Width
    {
        get => _width;
        set
        {
            double from = _width;
            if (SetProperty(ref _width, value)
                && IsScaleMode
                && IsAspectLocked
                && !_updating)
            {
                    _updating = true;
                    double scale = _width / from;
                    Height *= scale;
                    _updating = false;
            }
        }
    }

    private double _height;
    public double Height
    {
        get => _height;
        set
        {
            double from = _height;
            if (SetProperty(ref _height, value)
                && IsScaleMode
                && IsAspectLocked
                && !_updating)
            {
                _updating = true;
                double scale = _height / from;
                Width *= scale;
                _updating = false;
            }
        }
    }


    [ObservableProperty]
    private bool _isAspectLocked = true;

    public string WidthLabel =>
        IsScaleMode
        ? "PathWidth"
        : "X";

    public string HeightLabel =>
        IsScaleMode
        ? "PathHeight"
        : "Y";

    [RelayCommand]
    public void Ok()
    {
        if (IsScaleMode && (Width <= 0 || Height <= 0))
        {
            if (MessageBoxResult.Cancel == MessageBox.Show(
                "Your height and width must be greater than zero." + _eol + _eol
                + "Press [OK] to edit values or [Cancel] to cancel the operation.",
                "Invalid Parameters",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Exclamation))
            {
                OnClose(true);
            }
            return;
        }
        OnClose();
    }
}
