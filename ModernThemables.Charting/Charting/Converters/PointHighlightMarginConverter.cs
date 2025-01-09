using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ModernThemables.Charting.Converters
{
	/// <summary>
	/// Sets the margin for a textblock on an axis
	/// </summary>
	public class PointHighlightMarginConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			double pointRadius = 0;
			if (values[0] is double locationY
				&& values[1] is double locationX
				&& parameter is string pointRadiusString
				&& double.TryParse(pointRadiusString, out pointRadius))
			{
				return new Thickness(locationX - pointRadius, locationY - pointRadius, 0, 0);
			}
			else
			{
				return new Thickness(0);
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return new object[]
			{
				Binding.DoNothing
			};
		}
	}
}
