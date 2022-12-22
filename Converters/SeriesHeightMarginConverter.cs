using ModernThemables.ViewModels.Charting.CartesianChart;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ModernThemables.Converters
{
    /// <summary>
    /// Scales a series relative to other series according to container size inputs to make them scale relative to each
    /// other correctly when zoomed in the Y direction.
    /// </summary>
    public class SeriesHeightMarginConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] is InternalSeriesViewModel vm
				&& values[1] is ObservableCollection<InternalSeriesViewModel> vms
				&& values[2] is double gridWidth
				&& values[3] is double itemsControlWidth
				&& values[4] is double gridHeight
				&& values[5] is double itemsControlHeight)
			{
				if (!vm.Data.Any() || vms.All(x => !x.Data.Any())) return new Thickness(0);

				var vmsWithData = vms.Where(x => x.Data.Any());
				var minDataYMin = vmsWithData.Min(x => x.Data.Min(x => x.BackingPoint.YValue));
				var maxDataYMax = vmsWithData.Max(x => x.Data.Max(x => x.BackingPoint.YValue));

				var vmDataYMin = vm.Data.Min(x => x.BackingPoint.YValue);
				var vmDataYMax = vm.Data.Max(x => x.BackingPoint.YValue);

				var bottomFrac = (vmDataYMin - minDataYMin) / (maxDataYMax - minDataYMin);
				var topFrac = (maxDataYMax - vmDataYMax) / (maxDataYMax - minDataYMin);

				var minDataXMin = vmsWithData.Min(x => x.Data.Min(x => x.BackingPoint.XValue));
				var maxDataXMax = vmsWithData.Max(x => x.Data.Max(x => x.BackingPoint.XValue));

				var vmDataXMin = vm.Data.Min(x => x.BackingPoint.XValue);
				var vmDataXMax = vm.Data.Max(x => x.BackingPoint.XValue);

				var leftFrac = (vmDataXMin - minDataXMin) / Math.Max(maxDataXMax - minDataXMin, 1);
				var rightFrac = (maxDataXMax - vmDataXMax) / Math.Max(maxDataXMax - minDataXMin, 1);

				return new Thickness(
					leftFrac * itemsControlWidth,
					topFrac * itemsControlHeight,
					rightFrac * itemsControlWidth,
					bottomFrac * itemsControlHeight);
			}
			else
			{
				return new Thickness(0);
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return new object[] { 
				Binding.DoNothing,
				Binding.DoNothing,
				Binding.DoNothing,
				Binding.DoNothing,
				Binding.DoNothing,
				Binding.DoNothing };
		}
	}
}
