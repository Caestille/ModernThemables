using LiveChartsCore.Defaults;

namespace ModernThemables.HelperClasses.WpfChart
{
    internal class ChartPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public DateTimePoint BackingPoint { get; }

        public ChartPoint(double x, double y, DateTimePoint backingPoint)
        {
            X = x;
            Y = y;
            BackingPoint = backingPoint;
        }
    }
}
