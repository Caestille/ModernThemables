namespace ModernThemables.Icons.Converters;

using System;
using System.Globalization;
using System.Windows;

public class DataTypeValueConverter : MarkupConverter
{
    private static DataTypeValueConverter? instance;

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static DataTypeValueConverter()
    {
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => instance ?? (instance = new DataTypeValueConverter());

    protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value?.GetType()!;

    protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => DependencyProperty.UnsetValue;
}
