using ModernThemables.HelperClasses.WpfChart;
using ModernThemables.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ModernThemables.Converters
{
	public class SeriesHeightMarginConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] is WpfChartSeriesViewModel vm && values[1] is ObservableCollection<WpfChartSeriesViewModel> vms && values[2] is double gridWidth && values[3] is double itemsControlWidth && values[4] is double gridHeight && values[5] is double itemsControlHeight)
			{
				var minDataYMin = vms.Min(x => x.Data.Min(x => x.Y));
				var maxDataYMax = vms.Max(x => x.Data.Max(x => x.Y));

				var vmDataYMin = vm.Data.Min(x => x.Y);
				var vmDataYMax = vm.Data.Max(x => x.Y);

				var bottomFrac = (vmDataYMin - minDataYMin) / (maxDataYMax - minDataYMin);
				var topFrac = (maxDataYMax - vmDataYMax) / (maxDataYMax - minDataYMin);

				var maxYRange = maxDataYMax - minDataYMin;

				var minDataXMin = vms.Min(x => x.Data.Min(x => x.X));
				var maxDataXMax = vms.Max(x => x.Data.Max(x => x.X));

				var vmDataXMin = vm.Data.Min(x => x.X);
				var vmDataXMax = vm.Data.Max(x => x.X);

				var leftFrac = (vmDataXMin - minDataXMin) / (maxDataXMax - minDataXMin);
				var rightFrac = (maxDataXMax - vmDataXMax) / (maxDataXMax - minDataXMin);

				var maxXRange = maxDataXMax - minDataXMin;

				var xScale = itemsControlWidth / gridWidth;
				var yScale = itemsControlHeight / gridHeight;

				return new Thickness(
					leftFrac * maxXRange * xScale,
					bottomFrac * maxYRange * yScale,
					rightFrac * maxXRange * xScale,
					topFrac * maxYRange * yScale);
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
