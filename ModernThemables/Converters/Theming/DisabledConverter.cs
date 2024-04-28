﻿using System;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Data;
using CoreUtilities.HelperClasses.Extensions;
using System.Windows;

namespace ModernThemables.Converters.Theming
{
    public class DisabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                return new SolidColorBrush(brush.Color.ChangeColourBrightness((float)ThemeManager.DisabledModifier));
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
