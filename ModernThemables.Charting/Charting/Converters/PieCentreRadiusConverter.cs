using System;
using System.Globalization;
using System.Windows.Data;

namespace ModernThemables.Charting.Converters
{
    /// <summary>
    /// Moves all but the first series up by the height of their container to make them appear to overlap when using
    /// an items control that would stack them vertically.
    /// </summary>
    public class PieCentreRadiusConverter : IMultiValueConverter
    {
		public enum PieConverterReturnType
		{
			CentreX,
			CentreY,
			Radius,
		}

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is double width
                && values[1] is double height
                && parameter is string toReturn
				&& Enum.TryParse<PieConverterReturnType>(toReturn, out var returnType))
            {
                return ConvertLocally(width, height, returnType);
            }
            else
            {
                return Binding.DoNothing;
            }
        }

		public static double ConvertLocally(double width, double height, PieConverterReturnType toReturn)
		{
			switch (toReturn)
			{
				case PieConverterReturnType.CentreX:
					return Math.Min(width, height) / 2;
				case PieConverterReturnType.CentreY:
					return Math.Min(width, height) / 2;
				case PieConverterReturnType.Radius:
					return (Math.Min(width, height) / 2) * 0.9;
				default:
					return 0;
			}
		}

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { Binding.DoNothing, Binding.DoNothing, Binding.DoNothing };
        }
    }
}
