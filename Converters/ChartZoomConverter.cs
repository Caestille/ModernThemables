using ModernThemables.Controls;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ModernThemables.Converters
{
	public class ChartZoomConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] is ZoomStep zoomDetails && values[1] is FrameworkElement chart)
			{
				var widthDiff = chart.ActualWidth / zoomDetails.Step - chart.ActualWidth;
				var leftDiff = widthDiff * zoomDetails.Centre;
				var rightDiff = widthDiff * (1 - zoomDetails.Centre);

				return new Thickness(chart.Margin.Left - leftDiff - zoomDetails.Offset, 0, chart.Margin.Right - rightDiff + zoomDetails.Offset, 0);
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
