namespace ModernThemables.Converters;

using CoreUtilities.Helpers.Extensions;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

public class BrushColourInverterConverter : IValueConverter
{

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is SolidColorBrush brush)
        {
            return new SolidColorBrush(brush.Color.Invert());
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}
