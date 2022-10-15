using System;

namespace ModernThemables.HelperClasses.WpfChart
{
    internal class DataBounds
    {
        public DateTime XMin { get; }
        public DateTime XMax { get; }
        public double YMin { get; }
        public double YMax { get; }
        public TimeSpan XRange { get; }
        public double YRange { get; }

        public DataBounds(DateTime xMin, DateTime xMax, double yMin, double yMax)
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
