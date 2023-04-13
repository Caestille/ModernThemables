using ModernThemables.Charting.Interfaces;

namespace ModernThemables.Charting.Models
{
    /// <summary>
    /// A model for a item in a plot legend.
    /// </summary>
    public struct LegendItem
    {
        /// <summary>
        /// The text element for the legend item to display.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Stroke colour of the display element of the legend item.
        /// </summary>
        public IChartBrush Stroke { get; set; }

		/// <summary>
		/// Fill colour of the display element of the legend item.
		/// </summary>
		public IChartBrush Fill { get; set; }

		/// <summary>
		/// Initialises a new <see cref="LegendItem"/>.
		/// </summary>
		/// <param name="value">The value to display.</param>
		/// <param name="stroke">The stroke colour of the display element of the legend item.</param>
		/// <param name="fill">The fill colour of the display element of the legend item.</param>
		public LegendItem(string value, IChartBrush stroke, IChartBrush fill)
        {
            Value = value;
            Stroke = stroke;
            Fill = fill;
        }
    }
}
