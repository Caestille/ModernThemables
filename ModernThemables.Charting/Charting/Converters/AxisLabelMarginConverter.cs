using CoreUtilities.Converters;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ModernThemables.Charting.Converters
{
	/// <summary>
	/// Sets the margin for a textblock on an axis
	/// </summary>
	public class AxisLabelMarginConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] is string text
				&& values[1] is double fontSize
				&& values[2] is FontFamily fontFamily
				&& values[3] is FontStyle fontStyle
				&& values[4] is FontWeight fontWeight
				&& values[5] is FontStretch fontStretch
				&& values[6] is HorizontalAlignment alignment
				&& values[7] is Orientation orientation
                && values[8] is double angle)
			{
				var left = 0d;
				var top = orientation == Orientation.Vertical ? 0d : 10d;

				if (alignment != HorizontalAlignment.Left && orientation == Orientation.Horizontal)
                {
                    var mult = orientation == Orientation.Vertical
                        ? Math.Cos(angle * Math.PI / 180)
                        : Math.Sin(angle * Math.PI / 180);

                    var textWidth = StringWidthGetterConverter.MeasureString(text, fontSize, fontFamily, fontStyle, fontWeight, fontStretch).Width * (1 - mult);
					left = -textWidth / 2;
				}

				return new Thickness(left, top, 0, 0);
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
				Binding.DoNothing };
		}
	}
}
