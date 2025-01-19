namespace ModernThemables.Charting.Models.Brushes;

using System.Windows;
using System.Windows.Media;
using CoreUtilities.Helpers.Extensions;
using ModernThemables.Charting.Interfaces;

/// <summary>
/// A brush with a gradient between two <see cref="Color"/>s from top to bottom.
/// <see cref="Reevaluate(double, double, double, double, double, double)"/> adjusts the midpoint of this gradient.
/// </summary>
public sealed class SingleGradientBrush : IChartBrush
{
    /// <inheritdoc />
    public Brush? CoreBrush { get; private set; }

    private Color topColour;
    private Color bottomColour;

    private double yMax;
    private double yMin;

    /// <summary>
    /// Initialises a new <see cref="SingleGradientBrush"/>.
    /// </summary>
    /// <param name="topColour">The <see cref="Color"/> at the top of the brush.</param>
    /// <param name="bottomColour">The <see cref="Color"/> at the bottom of the brush.</param>
    public SingleGradientBrush(Color topColour, Color bottomColour)
    {
        this.topColour = topColour;
        this.bottomColour = bottomColour;

        Application.Current.Dispatcher.Invoke(() =>
        {
            GradientStopCollection collection = new()
            {
                new GradientStop(topColour, 0),
                new GradientStop(bottomColour, 1.0),
            };
            this.CoreBrush = new LinearGradientBrush(collection, angle: 90);
        });
    }

    /// <inheritdoc />
    public void Reevaluate(double yMax, double yMin, double yCentre, double xMax, double xMin, double xCentre)
    {
        this.yMax = yMax;
        this.yMin = yMin;

        GradientStopCollection collection = new()
        {
            new GradientStop(this.topColour, 0),
            new GradientStop(this.bottomColour, 1.0),
        };

        this.CoreBrush = new LinearGradientBrush(collection, angle: 90);
    }

    /// <inheritdoc />
    public Color ColourAtPoint(double x, double y)
    {
        if (y >= this.yMax)
        {
            return this.topColour;
        }
        else if (y < this.yMax && y > this.yMin)
        {
            var ratio = (double)(1 - (y - this.yMin) / (this.yMax - this.yMin));
            return this.topColour.Combine(this.bottomColour, ratio);
        }
        else if (y <= this.yMin)
        {
            return this.bottomColour;
        }
        else
        {
            return this.topColour;
        }
    }
}
