using System;

namespace ModernThemables.HelperClasses.WpfChart
{
    internal class AxisValues
    {
        public DateTime XMin { get; }
        public DateTime XMax { get; }
        public double YMin { get; }
        public double YMax { get; }
        public DateTime ZoomedXMin { get; }
        public DateTime ZoomedXMax { get; }
        public double ZoomedYMin { get; }
        public double ZoomedYMax { get; }
        public TimeSpan XRange { get; }
        public double YRange { get; }
        public TimeSpan ZoomedXRange { get; }
        public double ZoomedYRange { get; }

        public AxisValues(DateTime xMin, DateTime xMax, double yMin, double yMax, DateTime zoomedXMin, DateTime zoomedXMax, double zoomedYMin, double zoomedYMax)
        {
            XMin = xMin;
            XMax = xMax;
            YMin = yMin;
            YMax = yMax;
            ZoomedXMin = zoomedXMin;
            ZoomedXMax = zoomedXMax;
            ZoomedYMin = zoomedYMin;
            ZoomedYMax = zoomedYMax;
            XRange = xMax - xMin;
            YRange = yMax - yMin;
            ZoomedXRange = zoomedXMax - zoomedXMin;
            ZoomedYRange = zoomedYMax - zoomedYMin;
        }
    }
}
