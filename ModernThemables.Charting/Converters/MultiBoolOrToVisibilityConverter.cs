namespace ModernThemables.Charting.Converters;

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

/// <summary>
/// Given a set of <see cref="bool"/> bindings, if an OR operation on all <see cref="bool"/>s evaluates to
/// <see cref="true"/>, returns <see cref="Visibility.Visible"/>, else <see cref="Visibility.Collapsed"/>.
/// </summary>
public class MultiBoolOrToVisibilityConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var allow = false;
        foreach (var value in values)
        {
            if (value is bool castValue)
            {
                allow |= castValue;
            }
        }

        return allow ? Visibility.Visible : Visibility.Collapsed;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => [Binding.DoNothing];
}
