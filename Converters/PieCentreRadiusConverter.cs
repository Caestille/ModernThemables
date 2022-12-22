using System;
using System.Globalization;
using System.Windows.Data;

namespace ModernThemables.Converters
{
	/// <summary>
	/// Moves all but the first series up by the height of their container to make them appear to overlap when using
	/// and items control that would stack them vertically.
	/// </summary>
	public class PieCentreRadiusConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] is double width
				&& values[1] is double height
				&& parameter is string toReturn)
			{
				var centreX = Math.Min(width, height) / 2;
				var centreY = centreX;
				var radius = centreX * 0.9;

				switch (toReturn)
				{
					case "CentreX":
						return centreX;
					case "CentreY":
						return centreY;
					case "Radius":
						return radius;
					default:
						return 0;
				}
			}
			else
			{
				return 0;
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return new object[] { Binding.DoNothing, Binding.DoNothing, Binding.DoNothing };
		}
	}
}
