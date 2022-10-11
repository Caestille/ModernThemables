using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ModernThemables.Converters
{
	public class ChartZoomConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var zoomLevel = (double)values[0];
			var zoomCentre = (double)values[1];
			var chartWidth = (double)values[2];

			var remainingGrid = chartWidth * zoomLevel;
			var amount = chartWidth - remainingGrid;
			var thicc = new Thickness(-amount * zoomCentre, 0, -amount * (1 - zoomCentre), 0);
			return thicc;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return new object[] { Binding.DoNothing, Binding.DoNothing, Binding.DoNothing };
		}
	}
}
