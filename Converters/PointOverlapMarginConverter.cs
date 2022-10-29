using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ModernThemables.ViewModels.WpfChart;

namespace ModernThemables.Converters
{
	/// <summary>
	/// Moves all but the first item up by the height of the container to allow them to appear to overlap using an
	/// items control.
	/// </summary>
	public class PointOverlapMarginConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] is TooltipPointViewModel vm
				&& values[1] is ObservableCollection<TooltipPointViewModel> vms
				&& values[2] is double height)
			{
				if (vms.IndexOf(vm) == 0)
				{
					return new Thickness(0);
				}
				else
				{
					return new Thickness(0, -height, 0, 0);
				}
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
