namespace ModernThemables.Charting.Converters;

using System;
using System.Globalization;
using System.Windows.Data;

public class MultiBoolOrConverter : IMultiValueConverter
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

        return allow;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => [Binding.DoNothing];
}
