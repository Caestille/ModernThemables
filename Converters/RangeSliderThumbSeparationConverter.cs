﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Win10Themables.Converters
{
	public class RangeSliderThumbSeparationConverter : IMultiValueConverter
	{
		public DateTime LowerLimit { get; private set; }
		public DateTime UpperLimit { get; private set; }

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			LowerLimit = (DateTime)values[0];
			UpperLimit = (DateTime)values[1];

			return (UpperLimit - LowerLimit).TotalMilliseconds * 0.02;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return new[] { Binding.DoNothing, Binding.DoNothing };
		}
	}
}