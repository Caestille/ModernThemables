namespace ModernThemables.Converters;

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using CoreUtilities.Helpers.Extensions;

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

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}
