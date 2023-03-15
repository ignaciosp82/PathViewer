using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PathViewer
{
    internal class ValueConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool reverse = parameter is string s && (s.ToLower() == "reverse" || s.StartsWith("!"));
            if (targetType == typeof(Visibility))
            {
                Visibility ifTrue = reverse
                    ? Visibility.Collapsed
                    : Visibility.Visible;
                Visibility ifFalse = reverse
                    ? Visibility.Visible
                    : Visibility.Collapsed;
                // If it's a bool, we'll assume true means visible unless reverse.
                if (value is bool b)
                    return 
                        b
                        ? ifTrue
                        : ifFalse;
                // If it's not a bool, then we'll assume non-null means visible unless reverse.
                return
                    value is not null
                    ? ifTrue
                    : ifFalse;
            }
            return null;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        Convert(value, targetType, parameter, culture);
    }
}
