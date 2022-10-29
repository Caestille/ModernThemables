using ModernThemables.Interfaces;

namespace ModernThemables.HelperClasses.WpfChart
{
    /// <summary>
    /// An internal representation of a chart point for rendering the actual series with.
    /// </summary>
    internal class InternalChartPoint
    {
        /// <summary>
        /// The X point in pixels.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// The Y point in pixels.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// The unscaled point this represents.
        /// </summary>
        public IChartPoint BackingPoint { get; }

        /// <summary>
        /// Initialises a new <see cref="InternalChartPoint"/>.
        /// </summary>
        /// <param name="x">The x point which the point will be plotted on in pixels.</param>
        /// <param name="y">The y point which the point will be plotted on in pixels. </param>
        /// <param name="backingPoint">The actual, unscaled point this represents.</param>
        public InternalChartPoint(double x, double y, IChartPoint backingPoint)
        {
            X = x;
            Y = y;
            BackingPoint = backingPoint;
        }
    }
}
