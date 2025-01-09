using ModernThemables.Charting.Interfaces;
using System.Windows;
using System.Windows.Media;

namespace ModernThemables.Charting.Models.Brushes
{
    /// <summary>
    /// A brush with a single color.
    /// </summary>
    public sealed class SolidBrush : IChartBrush
    {
        /// <inheritdoc />
        public Brush CoreBrush { get; private set; }

        private Color colour;

        /// <summary>
        /// Initialises a new <see cref="SolidBrush"/> with a given <see cref="Color"/> which is the colour of the
        /// entire brush.
        /// </summary>
        /// <param name="colour">The brush <see cref="Color"/>.</param>
        public SolidBrush(Color colour)
        {
            this.colour = colour;
            Application.Current.Dispatcher.Invoke(() => { CoreBrush = new SolidColorBrush(colour); });
        }

        /// <inheritdoc />
        public void Reevaluate(double yMax, double yMin, double yCentre, double xMax, double xMin, double xCentre) { }

        /// <inheritdoc />
        public Color ColourAtPoint(double x, double y)
        {
            return colour;
        }
    }
}
