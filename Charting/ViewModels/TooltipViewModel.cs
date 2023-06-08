using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace ModernThemables.Charting.ViewModels
{
    /// <summary>
    /// A view model represnting a tooltip aligned to a point on a chart.
    /// </summary>
    public class TooltipViewModel : ObservableObject
    {
        /// <summary>
        /// The <see cref="InternalChartEntity"/> this tooltip represents.
        /// </summary>
        public InternalChartEntity Point { get; }

        /// <summary>
        /// The fill <see cref="Brush"/> the displayed tooltip point should be filled by.
        /// </summary>
        public Brush Fill { get; }

        /// <summary>
        /// The <see cref="Thickness"/> used to place the point.
        /// </summary>
        public Thickness Margin { get; set; }

        /// <summary>
        /// An <see cref="object"/> which can be used to display custom content if a <see cref="DataTemplate"/> is set
        /// in the chart.
        /// </summary>
        public object TemplatedContent { get; }

        /// <summary>
        /// <see cref="DataTemplate"/> to inform how to render the <see cref="TemplatedContent"/>.
        /// </summary>
        public DataTemplate TooltipTemplate { get; }

        /// <summary>
        /// The tooltip category
        /// </summary>
        public string Category { get; }

		/// <summary>
		/// The tooltip value to display
		/// </summary>
		public string FormattedValue { get; }

		/// <summary>
		/// Initialises a new <see cref="TooltipViewModel"/>.
		/// </summary>
		/// <param name="point">The <see cref="InternalChartEntity"/> being represented.</param>
		/// <param name="fill">The point fill if visible.</param>
		/// <param name="formattedValue">The <see cref="string"/> used to make up the tooltip display.</param>
		/// <param name="category">The <see cref="string"/> used to make up the tooltip display.</param>
		public TooltipViewModel(
            InternalChartEntity point,
            Brush fill,
            string formattedValue,
            string category)
        {
            Point = point;
            Fill = fill;
            FormattedValue = formattedValue;
			Category = category;
        }
    }
}
