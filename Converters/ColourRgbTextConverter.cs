using System;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Data;

namespace ModernThemables.Converters
{
	/// <summary>
	/// For a given <see cref="Color"/>, and <see cref="string"/> parameter dictating the component of the colour to
	/// return (e.g.: 'R', 'G', 'B'), returns the desired component as a string. In reverse the latest of each component is
	/// cached and the resulting colour returned.
	/// </summary>
	public class ColourRgbTextConverter : IValueConverter
	{
		private byte r;
		private byte g;
		private byte b;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var toReturn = (string)parameter;
			var colour = (Color)value;
			switch (toReturn)
			{
				case "R":
					r = colour.R;
					return r.ToString();
				case "G":
					g = colour.G;
					return g.ToString();
				case "B":
					b = colour.B;
					return b.ToString();
			}

			return "0";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var toReturn = (string)parameter;
			var component = (string)value;
			switch (toReturn)
			{
				case "R":
					r = byte.Parse(component);
					break;
				case "G":
					g = byte.Parse(component);
					break;
				case "B":
					b = byte.Parse(component);
					break;
			}

			return Color.FromArgb(255, r, g, b);
		}
	}
}
