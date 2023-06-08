using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ModernThemables.Charting.Converters
{
    /// <summary>
    /// Places a tooltip based on the tooltip size, location and placement within the owning chart
    /// </summary>
    public class PointToolTipConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is double locationX
                && values[1] is double locationY
				&& values[2] is double tooltipWidth
                && tooltipWidth > 0
                && values[3] is double tooltipHeight
                && tooltipHeight > 0
                && values[4] is double plotWidth
                && values[5] is double plotHeight
                && values[6] is bool invertY
                && values[7] is double offsetX
                && values[8] is double offsetY)
            {
                locationY = invertY ? plotHeight - locationY : locationY;

                var left = locationX + tooltipWidth > plotWidth - 5;
                var bottom = locationY < tooltipHeight + 5;

                return new Thickness(
                    (!left ? locationX + 5 : locationX + 5 - tooltipWidth) + offsetX,
                    (bottom ? locationY + 5 : locationY - 5 - tooltipHeight) + offsetY,
                    0,
                    0);
            }
            else
            {
                return Binding.DoNothing;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}