using Microsoft.Toolkit.Mvvm.ComponentModel;
using ModernThemables.HelperClasses.WpfChart;
using System.Windows;
using System.Windows.Media;

namespace ModernThemables.ViewModels.WpfChart
{
	internal class TooltipPointViewModel : ObservableObject
	{
		public InternalChartPoint Point { get; }

		public Brush Fill { get; }

		public Thickness Margin { get; }

		public Thickness LargeMargin { get; }

		public Thickness TooltipMargin { get; private set; }

		public double IndicatorHeight { get; }

		public string TooltipString { get; }

		public bool IsNearest { get; set; }

		private bool wasClicked;
		public bool WasClicked
		{
			get => wasClicked;
			set => SetProperty(ref wasClicked, value);
		}

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
