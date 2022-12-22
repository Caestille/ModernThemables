using Microsoft.Toolkit.Mvvm.ComponentModel;
using ModernThemables.HelperClasses.Charting.PieChart;
using System.Windows;
using System.Windows.Media;

namespace ModernThemables.ViewModels.Charting.CartesianChart
{
    /// <summary>
    /// A view model represnting a tooltip aligned to a point on a chart.
    /// </summary>
    internal class TooltipPointViewModel : ObservableObject
    {
        /// <summary>
        /// The <see cref="InternalChartPoint"/> this tooltip represents.
        /// </summary>
        public InternalChartPoint Point { get; }

        /// <summary>
        /// The fill <see cref="Brush"/> the displayed tooltip point should be filled by.
        /// </summary>
        public Brush Fill { get; }

        /// <summary>
        /// The <see cref="Thickness"/> used to place the point.
        /// </summary>
        public Thickness Margin { get; }

        /// <summary>
        /// The <see cref="Thickness"/> used to place a large border around the point.
        /// </summary>
        public Thickness LargeMargin { get; }

        /// <summary>
        /// The <see cref="Thickness"/> used to place the tooltip itself.
        /// </summary>
        public Thickness TooltipMargin { get; private set; }

        /// <summary>
        /// The height of the represented point in pixels.
        /// </summary>
        public double IndicatorHeight { get; }

        /// <summary>
        /// The string the tooltip should display. If a <see cref="DataTemplate"/> is set in the chart, this should be
        /// consumed by the template, this this need not always be human readable, and can be used to create a more
        /// complex tooltip display.
        /// </summary>
        public string TooltipString { get; }

        /// <summary>
        /// A <see cref="bool"/> indicating if this point is the nearest point to the cursor of all visible tooltips.
        /// </summary>
        public bool IsNearest { get; set; }

        private bool wasClicked;
        /// <summary>
        /// A <see cref="bool"/> indicating if this point was just clicked on by the user.
        /// </summary>
        public bool WasClicked
        {
            get => wasClicked;
            set => SetProperty(ref wasClicked, value);
        }

        /// <summary>
        /// Initialises a new <see cref="TooltipPointViewModel"/>.
        /// </summary>
        /// <param name="point">The <see cref="InternalChartPoint"/> being represented.</param>
        /// <param name="margin">The <see cref="Thickness"/> of the point to be displayed.</param>
        /// <param name="fill">The point fill if visible.</param>
        /// <param name="tooltipString">The <see cref="string"/> used to make up the tooltip display.</param>
        /// <param name="chartHeight">The height of the hosting chart, in pixels for calculation purposes.</param>
        public TooltipPointViewModel(
            InternalChartPoint point,
            Thickness margin,
            Brush fill,
            string tooltipString,
            double chartHeight)
        {
            Point = point;
            Margin = margin;
            LargeMargin = new Thickness(margin.Left - 10, margin.Top - 10, 0, 0);
            TooltipMargin = new Thickness(margin.Left + 10, -1000, -1000, chartHeight - margin.Top);
            Fill = fill;
            IndicatorHeight = Margin.Top + 5;
            TooltipString = tooltipString;
        }
    }
}
