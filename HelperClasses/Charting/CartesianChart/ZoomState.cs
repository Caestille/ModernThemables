using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace ModernThemables.HelperClasses.Charting.PieChart
{
    /// <summary>
    /// An object describing the zoom state for a chart.
    /// </summary>
    internal class ZoomState : ObservableObject
    {
        /// <summary>
        /// The minimum X value to be visible on the chart.
        /// </summary>
        public double XMin { get; }

        /// <summary>
        /// The maximum X value to be visible on the chart.
        /// </summary>
        public double XMax { get; }

        /// <summary>
        /// The minimum Y value to be visible on the chart.
        /// </summary>
        public double YMin { get; }

        /// <summary>
        /// The maximum Y value to be visible on the chart.
        /// </summary>
        public double YMax { get; }

        /// <summary>
        /// The distance in relative scale by which X should be offset by for panning purposes.
        /// </summary>
        public double XOffset { get; }

        /// <summary>
        /// Initialises a new <see cref="ZoomState"/>. If <paramref name="expandY"/> is <see cref="true"/>, the given Y
        /// values will be expanded by their distance apart multiplied by the <paramref name="yBuffer"/>, for the
        /// purpose of having a buffer distance in which the max/minima of the chart do not occupy (prevents y values
        /// reaching the edge of the chart).
        /// </summary>
        /// <param name="xMin">The minimum X value to be visible on the chart.</param>
        /// <param name="xMax">The maximum X value to be visible on the chart.</param>
        /// <param name="yMin">The minimum Y value to be visible on the chart.</param>
        /// <param name="yMax">The maximum Y value to be visible on the chart.</param>
        /// <param name="xOffset">The distance in relative scale by which X should be offset by for panning purposes.
        /// </param>
        /// <param name="yBuffer">The fractional distance by which the Y points will be expanded by if
        /// <paramref name="expandY"/> is <see cref="true"/>.</param>
        /// <param name="expandY">Whether the expand the Y points by the <paramref name="yBuffer"/> fraction.</param>
        public ZoomState(double xMin, double xMax, double yMin, double yMax, double xOffset, double yBuffer, bool expandY = true)
        {
            XMin = xMin;
            XMax = xMax;
            if (expandY)
            {
                var yRange = yMax - yMin;
                YMin = yMin - yRange * yBuffer;
                YMax = yMax + yRange * yBuffer;
            }
            else
            {
                YMin = yMin;
                YMax = yMax;
            }
            XOffset = xOffset;
        }

        /// <summary>
        /// Given a point in the same scale, indicates whether it would be visible.
        /// </summary>
        /// <param name="x">The X component of the point.</param>
        /// <param name="y">The Y component of the point.</param>
        /// <returns>A <see cref="bool"/> indicating whether the given point is contained by the current zoom level.
        /// </returns>
        public bool IsPointInBounds(double x, double y)
        {
            return x <= XMax && x >= XMin && y <= YMax && y >= YMin;
        }
    }
}
