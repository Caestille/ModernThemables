using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ModernThemables.Converters
{
	public class ChartZoomConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] is double zoomLevel && values[1] is double zoomCentre && values[2] is double zoomOffset && values[3] is double chartWidth)
			{
				var remainingGrid = chartWidth * zoomLevel;
				var amount = chartWidth - remainingGrid;
				return new Thickness(-amount * zoomCentre - zoomOffset, 0, -amount * (1 - zoomCentre) + zoomOffset, 0);
			}
			else
			{
				return new Thickness(0);
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return new object[] { Binding.DoNothing, Binding.DoNothing, Binding.DoNothing, Binding.DoNothing, Binding.DoNothing };
		}
	}
}
