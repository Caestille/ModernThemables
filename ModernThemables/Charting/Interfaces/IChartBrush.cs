using System.Windows.Media;

namespace ModernThemables.Charting.Interfaces
{
    /// <summary>
    /// An interface for brushes working with the <see cref="CartesianChart"/>.
    /// </summary>
    public interface IChartBrush
    {
        /// <summary>
        /// The underlying <see cref="Brush"/>.
        /// </summary>
        Brush CoreBrush { get; }

        /// <summary>
        /// As the brush can be non-constant, called when desired to re-evaluate the brush properties from the given
        /// inputs.
        /// </summary>
        /// <param name="yMax">The maximum Y value on the chart.</param>
        /// <param name="yMin">The minimum Y value on the chart.</param>
        /// <param name="yCentre">The Y centre point. This is not necessarily the mid point between the maximum
        /// and minimum, but anywhere inbetween. For example, for a chart where the bottom 10% of the series line is
        /// a different colour, this would be 10% of the distance from the minimum to the maximum.</param>
        /// <param name="xMax">The maximum X value on the chart.</param>
        /// <param name="xMin">The minimum X value on the chart.</param>
        /// <param name="xCentre">The X centre point. Similar to the Y centre point, this is not necessarily the direct
        /// mid point.</param>
        void Reevaluate(double yMax, double yMin, double yCentre, double xMax, double xMin, double xCentre);

        /// <summary>
        /// Given an input X and Y point, returns the <see cref="Color"/> of the series line at that point.
        /// </summary>
        /// <param name="x">The input X point.</param>
        /// <param name="y">The input Y point.</param>
        /// <returns>Returns a <see cref="Color"/> at the given input point.</returns>
        Color ColourAtPoint(double x, double y);
    }
}
