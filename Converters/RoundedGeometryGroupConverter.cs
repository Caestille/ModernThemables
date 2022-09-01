using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ModernThemables.Converters
{
	public class RoundedGeometryGroupConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var cornerRadius = double.Parse((string)(parameter ?? "5"));
			var size = new Size((double)values[0], (double)values[1]);
			GeometryGroup group = new GeometryGroup();
			group.Children.Add(new EllipseGeometry(new Point(cornerRadius, cornerRadius), cornerRadius, cornerRadius));
			group.Children.Add(new EllipseGeometry(new Point(size.Width - cornerRadius, cornerRadius), cornerRadius, cornerRadius));
			group.Children.Add(new EllipseGeometry(new Point(size.Width - cornerRadius, size.Height - cornerRadius), cornerRadius, cornerRadius));
			group.Children.Add(new EllipseGeometry(new Point(cornerRadius, size.Height - cornerRadius), cornerRadius, cornerRadius));
			group.Children.Add(new RectangleGeometry(new Rect(cornerRadius, 0, Math.Max(0, size.Width - 2 * cornerRadius), Math.Max(0, size.Height))));
			group.Children.Add(new RectangleGeometry(new Rect(0, cornerRadius, Math.Max(0, size.Width), Math.Max(0, size.Height - 2 * cornerRadius))));
			return group.Children;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
