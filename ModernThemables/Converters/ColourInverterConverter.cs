using System;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Data;
using CoreUtilities.HelperClasses.Extensions;

namespace ModernThemables.Converters
{
	public class ColourInverterConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
            byte max = 255;
			if (value is Color colour)
            {
                return Color.FromArgb(colour.A, (byte)(max - colour.R), (byte)(max - colour.G), (byte)(max - colour.B));
            }

            return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
            return Binding.DoNothing;
		}
	}
}
