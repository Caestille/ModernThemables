using ModernThemables.HelperClasses.WpfChart;
using ModernThemables.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ModernThemables.Converters
{
	public class ChartZoomConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] is ZoomState zoom && values[1] is ObservableCollection<ISeries> series && values[2] is FrameworkElement grid)
			{
				var displayAreaWidth = grid.ActualWidth;

				var min = series.Min(x => x.Values.Min(y => y.XValue));
				var max = series.Max(x => x.Values.Max(y => y.XValue));

				var leftFrac = (zoom.Min - min) / (max - min);
				var rightFrac = (max - zoom.Max) / (max - min);

				var newWidth = displayAreaWidth / (1 - (leftFrac + rightFrac));

				var leftDiff = newWidth * leftFrac;
				var rightDiff = newWidth * rightFrac;

				return new Thickness(-leftDiff - zoom.Offset, 0, -rightDiff + zoom.Offset, 0);
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
