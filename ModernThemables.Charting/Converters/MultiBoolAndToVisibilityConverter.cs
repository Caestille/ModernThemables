namespace ModernThemables.Charting.Converters;

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

/// <summary>
/// Given a set of <see cref="bool"/> bindings, if an AND operation on all <see cref="bool"/>s evaluates to
/// <see cref="true"/>, returns <see cref="Visibility.Visible"/>, else <see cref="Visibility.Collapsed"/>.
/// </summary>
public class MultiBoolAndToVisibilityConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var type = Visibility.Collapsed;
        if (parameter != null && parameter is string visType && (visType == "hidden" || visType == "Hidden"))
        {
            type = Visibility.Hidden;
        }

        var allow = true;
        foreach (var value in values)
        {
            if (value is bool castValue)
            {
                allow &= castValue;
            }
        }

        return allow ? Visibility.Visible : type;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => new[] { Binding.DoNothing };
}
