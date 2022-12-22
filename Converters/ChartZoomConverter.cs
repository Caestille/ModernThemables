using ModernThemables.HelperClasses.Charting.PieChart;
using ModernThemables.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ModernThemables.Converters
{
    /// <summary>
    /// Given a <see cref="ZoomState"/>, collection of <see cref="ISeries"/>' and a host 
    /// <see cref="FrameworkElement"/>, returns a <see cref="Thickness"/> which is the margin of a hosted item
    /// displaying the rendered <see cref="ISeries"/> in order to scale them according to the <see cref="ZoomState"/>
    /// properties.
    /// </summary>
    public class ChartZoomConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] is ZoomState zoom
				&& values[1] is ObservableCollection<ISeries> series
				&& values[2] is FrameworkElement grid && series.Any())
			{
				var displayAreaWidth = grid.ActualWidth;

				if (!series.Any(x => x.Values.Any())) return new Thickness(0);

				var xMin = series.Min(x => x.Values.Min(y => y.XValue));
				var xMax = series.Max(x => x.Values.Max(y => y.XValue));

				var leftFrac = (zoom.XMin - xMin) / Math.Max(xMax - xMin, 1);
				var rightFrac = (xMax - zoom.XMax) / Math.Max(xMax - xMin, 1);

				var newWidth = displayAreaWidth / (1 - (leftFrac + rightFrac));

				var leftDiff = newWidth * leftFrac;
				var rightDiff = newWidth * rightFrac;

				var displayAreaHeight = grid.ActualHeight;

				var yMin = series.Min(x => x.Values.Min(y => y.YValue));
				var yMax = series.Max(x => x.Values.Max(y => y.YValue));

				var bottomFrac = (zoom.YMin - yMin) / (yMax - yMin);
				var topFrac = (yMax - zoom.YMax) / (yMax - yMin);

				var newHeight = displayAreaHeight / (1 - (topFrac + bottomFrac));

				var topDiff = newHeight * topFrac;
				var bottomDiff = newHeight * bottomFrac;

				return new Thickness(-leftDiff - zoom.XOffset, -topDiff, -rightDiff + zoom.XOffset, -bottomDiff);
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
