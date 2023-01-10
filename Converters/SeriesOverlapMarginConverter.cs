using ModernThemables.ViewModels.Charting.CartesianChart;
using ModernThemables.ViewModels.Charting.PieChart;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ModernThemables.Converters
{
	/// <summary>
	/// Moves all but the first series up by the height of their container to make them appear to overlap when using
	/// and items control that would stack them vertically.
	/// </summary>
	public class SeriesOverlapMarginConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] is InternalPathSeriesViewModel seriesVm
				&& values[1] is ObservableCollection<InternalPathSeriesViewModel> seriesVms
				&& values[2] is double seriesHeight)
			{
				return seriesVms.IndexOf(seriesVm) == 0 ? new Thickness(0) : new Thickness(0, -seriesHeight, 0, 0);
			}
			else if (values[0] is TooltipPointViewModel tooltipVm
				&& values[1] is ObservableCollection<TooltipPointViewModel> tooltipVms
				&& values[2] is double tooltipHeight)
			{
				return tooltipVms.IndexOf(tooltipVm) == 0 ? new Thickness(0) : new Thickness(0, -tooltipHeight, 0, 0);
			}
			else if (values[0] is InternalPieWedgeViewModel wedgeVm
				&& values[1] is ObservableCollection<InternalPieSeriesViewModel> wedgeVms
				&& values[2] is double wedgeHeight)
			{
				return wedgeVms.First(x => x.Wedges.Contains(wedgeVm)).Wedges.IndexOf(wedgeVm) == 0
					? new Thickness(0)
					: new Thickness(0, -wedgeHeight, 0, 0);
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
