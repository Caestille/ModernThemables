using ModernThemables.Interfaces;

namespace ModernThemables.HelperClasses.WpfChart
{
    internal class InternalChartPointRepresentation
    {
        public double X { get; set; }
        public double Y { get; set; }
        public IChartPoint BackingPoint { get; }

        public InternalChartPointRepresentation(double x, double y, IChartPoint backingPoint)
        {
            X = x;
            Y = y;
            BackingPoint = backingPoint;
        }
    }
}
