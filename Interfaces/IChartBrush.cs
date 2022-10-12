using System.Windows.Media;

namespace ModernThemables.Interfaces
{
    public interface IChartBrush
    {
        Brush CoreBrush { get; }
        void Reevaluate(double yMax, double yMin, double yCentre, double xMax, double xMin, double xCentre);
        Color ColourAtPoint(double x, double y);
    }
}
