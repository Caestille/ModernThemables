using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ModernThemables.Converters
{
	/// <summary>
	/// Given a <see cref="double"/> value, returns a <see cref="Thickness"/> which is that value across all edges,
	/// unless a converter parameter of the format
	/// [left multiplier]-[top multiplier]-[right multiplier]-[bottom multiplier] is supplied, in which case the value
	/// is multipled by the multiplier for the respective edge.
	/// </summary>
	public class MultiBindingDoubleToBorderThicknessConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var weightingsString = "";

			if (values[0] is double inValue
				&& ((values.Count() > 1 && values[1] is string weightings) || parameter is string weightings2))
			{
				weightingsString = values.Count() > 1 && values[1] is string
					? (string)values[1]
					: parameter != null 
						? (string)parameter 
						: "";

				if (!string.IsNullOrEmpty(weightingsString))
				{
					var weightingsArray = weightingsString.Split('-').Select(x => double.Parse(x)).ToList();
					var leftWeighting = weightingsArray[0];
					var rightWeighting = weightingsArray[2];
					var topWeighting = weightingsArray[1];
					var bottomWeighting = weightingsArray[3];

					return new Thickness(
						inValue * leftWeighting,
						inValue * topWeighting,
						inValue * rightWeighting,
						inValue * bottomWeighting);
				}
				else
				{
					return new Thickness(inValue);
				}
			}

			return null;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
