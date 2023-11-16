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
        public double LocationX { get; }

		public double LocationY { get; }

        /// <summary>
        /// The fill <see cref="Brush"/> the displayed tooltip point should be filled by.
        /// </summary>
        public Brush Fill { get; }

        /// <summary>
        /// An <see cref="object"/> which can be used to display custom content if a <see cref="DataTemplate"/> is set
        /// in the chart.
        /// </summary>
        public object? TemplatedContent { get; set; }

        /// <summary>
        /// <see cref="DataTemplate"/> to inform how to render the <see cref="TemplatedContent"/>.
        /// </summary>
        public DataTemplate TooltipTemplate { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the tooltip is the nearest one.
		/// </summary>
		public bool IsNearest { get; set; }

		/// <summary>
		/// The tooltip value to display
		/// </summary>
		public string PrimaryValue { get; }

		/// <summary>
		/// The tooltip category
		/// </summary>
		public string SecondaryValue { get; }

		/// <summary>
		/// The tooltip category
		/// </summary>
		public string TertiaryValue { get; }

		private bool resizeTrigger;
		/// <summary>
		/// A <see cref="bool"/> property used to for the series to resize itself when desired by triggering a
		/// converter.
		/// </summary>
		public bool ResizeTrigger
		{
			get => resizeTrigger;
			set => SetProperty(ref resizeTrigger, value);
		}

		/// <summary>
		/// Initialises a new <see cref="TooltipViewModel"/>.
		/// </summary>
		/// <param name="point">The <see cref="InternalChartEntity"/> being represented.</param>
		/// <param name="fill">The point fill if visible.</param>
		/// <param name="primaryValue">The <see cref="string"/> used to make up the tooltip display.</param>
		/// <param name="secondaryValue">The <see cref="string"/> used to make up the tooltip display.</param>
		public TooltipViewModel(
            InternalChartEntity point,
            Brush fill,
            string primaryValue,
            string secondaryValue,
            string tertiaryValue)
        {
			LocationX = point.X;
			LocationY = point.Y;
            Fill = fill;
            PrimaryValue = primaryValue;
			SecondaryValue = secondaryValue;
			TertiaryValue = tertiaryValue;
		}

		public TooltipViewModel(
			double locationX,
			double locationY,
			Brush fill,
			string primaryValue,
			string secondaryValue,
			string tertiaryValue)
		{
			LocationX = locationX;
			LocationY = locationY;
			Fill = fill;
			PrimaryValue = primaryValue;
			SecondaryValue = secondaryValue;
			TertiaryValue = tertiaryValue;
		}
	}
}
