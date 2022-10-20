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
	public class PointOverlapMarginConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] is HoveredPointViewModel vm && values[1] is ObservableCollection<HoveredPointViewModel> vms && values[2] is double height)
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
			return new object[] { Binding.DoNothing, Binding.DoNothing, Binding.DoNothing, Binding.DoNothing, Binding.DoNothing };
		}
	}
}
