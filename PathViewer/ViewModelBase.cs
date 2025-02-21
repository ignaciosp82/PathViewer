using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;


namespace PathViewer;

public class ViewModelBase : ObservableObject, IDisposable
{
    public ViewModelBase()
    {
        // Need the null check for design mode.
        if (Application.Current?.MainWindow is not null)
        {
            Application.Current.MainWindow.Closing += OnMainWindowClosing;
        }
    }

    public virtual void Dispose()
    {
        CommandManager.RequerySuggested -= OnRequerySuggested;
        if (Application.Current?.MainWindow is not null)
        {
            Application.Current.MainWindow.Closing -= OnMainWindowClosing;
        }
    }

    protected bool ShowModal(ViewModelBase vm, string title)
    {
        bool result;
        Window window = new()
        {
            Content = vm,
            Title = title,
            SizeToContent = SizeToContent.WidthAndHeight,
            Owner = Application.Current.MainWindow,
            ResizeMode = ResizeMode.NoResize,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        try
        {
            window.KeyUp += OnKey;
            result = window.ShowDialog() ?? false;
        }
        finally
        {
            window.KeyUp -= OnKey;
        }

        return result;

        void OnKey(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                window.DialogResult = false;
                window.Close();
            }
        }
    }

    protected virtual void OnMainWindowClosing(object? sender, CancelEventArgs e)
    {
        // Checking e.Cancel just in case there's ever a dialog to make sure.
        if (!e.Cancel)
        {
            Dispose();
        }
    }

    #region Close Notification
    public event CancelEventHandler? CloseEvent;
    protected void OnClose(bool cancel = false) => CloseEvent?.Invoke(this, new CancelEventArgs { Cancel = cancel });
    #endregion


    protected void FindRelayCommands()
    {
        // CommunityToolkit doesn't subscribe to CommandManager.RequerySuggested,
        // and we need those notifications to go to our commands.
        CommandManager.RequerySuggested += OnRequerySuggested;
        foreach (PropertyInfo cmdInfo in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var obj = cmdInfo.GetValue(this);
            if (obj is IRelayCommand cmd) _commands.Add(cmd);
        }
    }

    private void OnRequerySuggested(object? sender, EventArgs e)
    {
        foreach (IRelayCommand command in _commands)
        {
            command.NotifyCanExecuteChanged();
        }
    }
    private readonly List<IRelayCommand> _commands = new();

    protected static readonly string _eol = Environment.NewLine;
}
