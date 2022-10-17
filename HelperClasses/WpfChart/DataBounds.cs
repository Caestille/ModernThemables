using System;

namespace ModernThemables.HelperClasses.WpfChart
{
    internal class DataBounds
    {
        public double XMin { get; }
        public double XMax { get; }
        public double YMin { get; }
        public double YMax { get; }
        public double XRange { get; }
        public double YRange { get; }

        public DataBounds(double xMin, double xMax, double yMin, double yMax)
        {
            XMin = xMin;
            XMax = xMax;
            YMin = yMin;
            YMax = yMax;
            XRange = xMax - xMin;
            YRange = yMax - yMin;
        }
    }
}
