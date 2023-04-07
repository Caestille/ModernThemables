using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ModernThemables.Charting.Converters
{
    /// <summary>
    /// Moves all but the first series up by the height of their container to make them appear to overlap when using
    /// and items control that would stack them vertically.
    /// </summary>
    public class PieCentrererConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is double width
                && values[1] is double height)
            {
                var left = width > height ? (width - height) / 2 : 0;
                return new Thickness(left, 0, 0, 0);
            }
            else
            {
                return new Thickness(0);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { Binding.DoNothing, Binding.DoNothing, Binding.DoNothing };
        }
    }
}
