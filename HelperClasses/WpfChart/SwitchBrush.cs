using CoreUtilities.HelperClasses.Extensions;
using ModernThemables.Interfaces;
using System;
using System.Windows.Media;

namespace ModernThemables.HelperClasses.WpfChart
{
    public class SwitchBrush : IChartBrush
    {
        public Brush CoreBrush { get; private set; }


        private Color topColour;
        private Color bottomColour;
        private Color topCentreColour;
        private Color bottomCentreColour;

        private double yMax;
        private double yMin;
        private double yCentre;

        public SwitchBrush(Color topColour, Color topCentreColour, Color bottomCentreColour, Color bottomColour)
        {
            CoreBrush = new LinearGradientBrush();
            this.topColour = topColour;
            this.bottomColour = bottomColour;
            this.topCentreColour = topCentreColour;
            this.bottomCentreColour = bottomCentreColour;
        }

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

            CoreBrush = new LinearGradientBrush(collection, angle: 90);
        }

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

        Color IChartBrush.ColourAtPoint(double x, double y)
        {
            throw new NotImplementedException();
        }
    }
}
