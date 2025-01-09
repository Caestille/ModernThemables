namespace ModernThemables.Charting.Models.Brushes
{
    using CoreUtilities.HelperClasses.Extensions;
    using ModernThemables.Charting.Interfaces;
    using System.Windows;
    using System.Windows.Media;

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
            Application.Current.Dispatcher.Invoke(() => { this.CoreBrush = new LinearGradientBrush(); });
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
                new GradientStop(this.topColour, 0),
                new GradientStop(this.topCentreColour, ratio),
                new GradientStop(this.bottomCentreColour, ratio),
                new GradientStop(this.bottomColour, 1.0)
            };

            Application.Current.Dispatcher.Invoke(
                () => { this.CoreBrush = new LinearGradientBrush(collection, angle: 90); });
        }

        /// <inheritdoc />
        public Color ColourAtPoint(double x, double y)
        {
            if (y >= this.yMax)
            {
                return this.topColour;
            }
            else if (y < this.yMax && y >= this.yCentre)
            {
                var ratio = (double)(1 - (y - this.yCentre) / (this.yMax - this.yCentre));
                return this.topColour.Combine(this.topCentreColour, ratio);
            }
            else if (y > this.yMin && y <= this.yCentre)
            {
                var ratio = (double)(1 - (y - this.yMin) / (this.yCentre - this.yMin));
                return this.bottomColour.Combine(this.bottomCentreColour, ratio);
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
}
