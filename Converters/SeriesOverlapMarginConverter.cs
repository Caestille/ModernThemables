using ModernThemables.ViewModels.WpfChart;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ModernThemables.Converters
{
	public class SeriesOverlapMarginConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] is InternalSerieViewModel vm
				&& values[1] is ObservableCollection<InternalSerieViewModel> vms
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
