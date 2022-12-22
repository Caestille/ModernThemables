using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ModernThemables.Converters
{
	/// <summary>
	/// Sets the margin for a textblock to appear to be placed within a pie wedge.
	/// </summary>
	public class PieLabelMarginConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] is double textHeight
				&& values[1] is double textWidth
				&& values[2] is double startAngle
				&& values[3] is double percent
				&& values[4] is double labelRadiusFrac
				&& values[5] is double pieHeight
				&& values[6] is double pieWidth
				&& values[7] is bool isMouseOver)
			{
				var centreX = Math.Min(pieWidth, pieHeight) / 2;
				var centreY = centreX;
				var radius = centreX * 0.9;

				var angle = startAngle + percent / 2 * 360 / 100;
				var translatedAngle = angle;
				if (angle > 90 && angle <= 180)
				{
					translatedAngle = 180 - angle;
				}
				else if (angle > 180 && angle <= 270)
				{
					translatedAngle = angle - 180;
				}
				else if (angle > 270)
				{
					translatedAngle = 360 - angle;
				}

				var oOverH = Math.Sin(translatedAngle * Math.PI / 180);
				var h = radius * (isMouseOver ? 1 : labelRadiusFrac);
				var o = oOverH * h;
				var a = Math.Sqrt(Math.Pow(h, 2) - Math.Pow(o, 2));

				var desiredX = 0d;
				var desiredY = 0d;

				if (angle > 90 && angle <= 180)
				{
					desiredX = centreX + o;
					desiredY = centreY + a;
				}
				else if (angle > 180 && angle <= 270)
				{
					desiredX = centreX - o;
					desiredY = centreY + a;
				}
				else if (angle > 270)
				{
					desiredX = centreX - o;
					desiredY = centreY - a;
				}
				else
				{
					desiredX = centreX + o;
					desiredY = centreY - a;
				}

				var offsetX = desiredX - textWidth / 2;
				var offsetY = desiredY - textHeight / 2;

				return new Thickness(offsetX, offsetY, 0, 0);
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
