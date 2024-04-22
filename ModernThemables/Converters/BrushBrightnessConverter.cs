using System;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Data;
using CoreUtilities.HelperClasses.Extensions;

namespace ModernThemables.Converters
{
	public class BrushBrightnessConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is SolidColorBrush brush && parameter is double changeFactor)
            {
                return new SolidColorBrush(brush.Color.ChangeColourBrightness((float)changeFactor));
            }

            return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
            return Binding.DoNothing;
		}
	}
}
