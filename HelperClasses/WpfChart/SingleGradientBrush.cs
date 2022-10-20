using CoreUtilities.HelperClasses.Extensions;
using ModernThemables.Interfaces;
using System;
using System.Windows.Media;

namespace ModernThemables.HelperClasses.WpfChart
{
    public sealed class SingleGradientBrush : IChartBrush
    {
        public Brush CoreBrush { get; private set; }

        private Color topColour;
        private Color bottomColour;

        private double yMax;
        private double yMin;

        public SingleGradientBrush(Color topColour, Color bottomColour)
        {
            this.topColour = topColour;
            this.bottomColour = bottomColour;

			GradientStopCollection collection = new()
			{
				new GradientStop(topColour, 0),
				new GradientStop(bottomColour, 1.0)
			};

			CoreBrush = new LinearGradientBrush(collection, angle: 90);
		}

        public void Reevaluate(double yMax, double yMin, double yCentre, double xMax, double xMin, double xCentre)
        {
            this.yMax = yMax;
            this.yMin = yMin;

            GradientStopCollection collection = new()
            {
                new GradientStop(topColour, 0),
                new GradientStop(bottomColour, 1.0)
            };

            CoreBrush = new LinearGradientBrush(collection, angle: 90);
        }

        public Color ColourAtPoint(double x, double y)
        {
			if (y >= yMax)
			{
				return topColour;
			}
			else if (y < yMax && y > yMin)
			{
				var ratio = (double)(1 - (y - yMin) / (yMax - yMin));
				return topColour.Combine(bottomColour, ratio);
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
