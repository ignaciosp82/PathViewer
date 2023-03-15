using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace PathViewer;

public class PopupBase : UserControl
{
    public PopupBase()
    {
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        if (DataContext is ViewModelBase vm)
        {
            _viewModel = vm;
            _viewModel.CloseEvent += OnClose;
        }
        _parent = Window.GetWindow(this);
        if (_parent != null)
        {
            _parent.Closing += OnClosing;
            Loaded -= OnLoaded;
        }
    }

    private void OnClose(object? sender, CancelEventArgs e)
    {
        if (_parent is not null)
        {
            _parent.DialogResult = !e.Cancel;
        }
    }


    private void OnClosing(object? sender, CancelEventArgs e)
    {
        if (_viewModel != null)
        {
            // Null coalescing doesn't work with events, so we have to do
            // this the long way.
            _viewModel.CloseEvent -= OnClose;
        }
        if (_parent != null)
        {
            _parent.Closing -= OnClosing;
        }
    }

    protected void OnNumericInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
        // e.Handled is set to true when it means the value should be discarded.
        char dec = Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        char thou = Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator);
        char neg = Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.NegativeSign);
        char pos = Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.PositiveSign);
        if (!Regex.IsMatch(e.Text, $"[0-9{dec}{thou}{pos}{neg}]")) e.Handled = true;
    }


    private ViewModelBase? _viewModel;
    private Window? _parent;
}
