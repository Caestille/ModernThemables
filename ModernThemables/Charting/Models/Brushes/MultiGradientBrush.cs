using CoreUtilities.HelperClasses.Extensions;
using ModernThemables.Charting.Interfaces;
using System;
using System.Windows;
using System.Windows.Media;

namespace ModernThemables.Charting.Models.Brushes
{
    /// <summary>
    /// A brush with 4 <see cref="Color"/>s across its vertical span.
    /// <see cref="Reevaluate(double, double, double, double, double, double)"/> Adjusts the crossing over point
    /// between the top two and bottomr two <see cref="Color"/>s.
    /// </summary>
    public sealed class MultiGradientBrush : IChartBrush
    {
        /// <inheritdoc />
        public Brush CoreBrush { get; private set; }

        private Color topColour;
        private Color bottomColour;
        private Color topCentreColour;
        private Color bottomCentreColour;

        private double yMax;
        private double yMin;
        private double yCentre;

        /// <summary>
        /// Initialises a new <see cref="MultiGradientBrush"/>.
        /// </summary>
        /// <param name="topColour">The <see cref="Color"/> at the top of the brush.</param>
        /// <param name="topCentreColour">The <see cref="Color"/> at the upper side of the crossing over point of the
        /// brush.</param>
        /// <param name="bottomCentreColour">The <see cref="Color"/> at the lower side of the crossing over point of
        /// the brush.</param>
        /// <param name="bottomColour">The <see cref="Color"/> at the bottom of the brush.</param>
        public MultiGradientBrush(Color topColour, Color topCentreColour, Color bottomCentreColour, Color bottomColour)
        {
            Application.Current.Dispatcher.Invoke(() => { CoreBrush = new LinearGradientBrush(); });
            this.topColour = topColour;
            this.bottomColour = bottomColour;
            this.topCentreColour = topCentreColour;
            this.bottomCentreColour = bottomCentreColour;
        }

        /// <inheritdoc />
        public void Reevaluate(double yMax, double yMin, double yCentre, double xMax, double xMin, double xCentre)
        {
            this.yMax = yMax;
            this.yMin = yMin;
            this.yCentre = yCentre;

            yCentre = Math.Min(Math.Max(yCentre, yMin), yMax);
            var ratio = (double)(1 - (yCentre - yMin) / (yMax - yMin));

            GradientStopCollection collection = new()
            {
                new GradientStop(topColour, 0),
                new GradientStop(topCentreColour, ratio),
                new GradientStop(bottomCentreColour, ratio),
                new GradientStop(bottomColour, 1.0)
            };

            Application.Current.Dispatcher.Invoke(
                () => { CoreBrush = new LinearGradientBrush(collection, angle: 90); });
        }

        /// <inheritdoc />
        public Color ColourAtPoint(double x, double y)
        {
            if (y >= yMax)
            {
                return topColour;
            }
            else if (y < yMax && y >= yCentre)
            {
                var ratio = (double)(1 - (y - yCentre) / (yMax - yCentre));
                return topColour.Combine(topCentreColour, ratio);
            }
            else if (y > yMin && y <= yCentre)
            {
                var ratio = (double)(1 - (y - yMin) / (yCentre - yMin));
                return bottomColour.Combine(bottomCentreColour, ratio);
            }
            else if (y <= yMin)
            {
                return bottomColour;
            }
            else
            {
                return topColour;
            }
        }
    }
}
