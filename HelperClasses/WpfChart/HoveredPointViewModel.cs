using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace ModernThemables.HelperClasses.WpfChart
{
	internal class HoveredPointViewModel : ObservableObject
	{
		public InternalChartPoint BackingPoint { get; }
		public Brush Fill { get; }
		public Thickness Margin { get; }
		public Thickness LargeMargin { get; }
		public double IndicatorHeight { get; }
		public string TooltipString { get; }
		public bool IsNearest { get; set; }

		private bool wasClicked;
		public bool WasClicked
		{
			get => wasClicked;
			set => SetProperty(ref wasClicked, value);
		}

		public HoveredPointViewModel(
			InternalChartPoint hoveredPoint,
			Thickness margin,
			Brush fill,
			string tooltipString)
		{
			BackingPoint = hoveredPoint;
			Margin = margin;
			LargeMargin = new Thickness(margin.Left - 10, margin.Top - 10, 0, 0);
			Fill = fill;
			IndicatorHeight = Margin.Top + 5;
			TooltipString = tooltipString;
		}
	}
}
