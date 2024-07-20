using System;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Data;

namespace ModernThemables.Converters
{
	public class BrushToColourConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
            return (value is SolidColorBrush brush ? brush.Color : Colors.Red);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
            return Binding.DoNothing;
		}
	}
}
