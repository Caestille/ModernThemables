namespace ModernThemables.Charting.Converters;

using System;
using System.Globalization;
using System.Windows.Data;

public class MultiBoolOrConverter : IMultiValueConverter
{
    /// <summary>
    /// Given a set of <see cref="bool"/> bindings, returns the result of an OR operation on all <see cref="bool"/>
    /// values given.
    /// </summary>
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

        return allow;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => new[] { Binding.DoNothing };
}
