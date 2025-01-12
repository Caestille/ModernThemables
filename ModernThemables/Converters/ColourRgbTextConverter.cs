namespace ModernThemables.Converters;

using System;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Data;

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
                this.r = colour.R;
                return this.r.ToString();
            case "G":
                this.g = colour.G;
                return this.g.ToString();
            case "B":
                this.b = colour.B;
                return this.b.ToString();
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
                this.r = byte.Parse(component);
                break;
            case "G":
                this.g = byte.Parse(component);
                break;
            case "B":
                this.b = byte.Parse(component);
                break;
        }

        return Color.FromArgb(255, this.r, this.g, this.b);
    }
}
