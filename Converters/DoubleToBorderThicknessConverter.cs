using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ModernThemables.Converters
{
	public class DoubleToBorderThicknessConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var inValue = (double)value;

			if (parameter != null)
			{
				var weightings = (string)parameter;
				var weightingsArray = weightings.Split('-').Select(x => double.Parse(x)).ToList();
				var leftWeighting = weightingsArray[0];
				var rightWeighting = weightingsArray[2];
				var topWeighting = weightingsArray[1];
				var bottomWeighting = weightingsArray[3];

				return new Thickness(inValue * leftWeighting, inValue * topWeighting, inValue * rightWeighting, inValue * bottomWeighting);
			}
			else
			{
				var leftWeighting = 1;
				var rightWeighting = 1;
				var topWeighting = 1;
				var bottomWeighting = 1;

				return new Thickness(inValue * leftWeighting, inValue * topWeighting, inValue * rightWeighting, inValue * bottomWeighting);
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}
