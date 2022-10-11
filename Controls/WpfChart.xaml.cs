using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveChartsCore.Defaults;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows.Media;
using System.Windows.Input;
using CoreUtilities.HelperClasses.Extensions;
using System.Windows.Threading;
using CoreUtilities.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Text;

namespace ModernThemables.Controls
{
	public interface IChartBrush
	{
		Brush CoreBrush { get; }
		void Reevaluate(double yMax, double yMin, double yCentre, double xMax, double xMin, double xCentre);
		Color ColourAtPoint(double x, double y);
	}

	public class SwitchBrush : IChartBrush
	{
		public Brush CoreBrush { get; private set; }

		private Color topColour;
		private Color bottomColour;
		private Color topCentreColour;
		private Color bottomCentreColour;

		private double yMax;
		private double yMin;
		private double yCentre;

		public SwitchBrush(Color topColour, Color topCentreColour, Color bottomCentreColour, Color bottomColour)
		{
			CoreBrush = new LinearGradientBrush();
			this.topColour = topColour;
			this.bottomColour = bottomColour;
			this.topCentreColour = topCentreColour;
			this.bottomCentreColour = bottomCentreColour;
		}

		public void Reevaluate(double yMax, double yMin, double yCentre, double xMax, double xMin, double xCentre)
		{
			this.yMax = yMax;
			this.yMin = yMin;
			this.yCentre = yCentre;

			yCentre = Math.Min(Math.Max(yCentre, yMin), yMax);
			var ratio = (double)(1 - (yCentre - yMin) / (yMax - yMin));

			GradientStopCollection collection = new()
			{
				new GradientStop(topColour, 0),
				new GradientStop(topCentreColour, ratio),
				new GradientStop(bottomCentreColour, ratio),
				new GradientStop(bottomColour, 1.0)
			};

			CoreBrush = new LinearGradientBrush(collection, angle: 90);
		}

		public Color ColourAtPoint(double x, double y)
		{
			if (y >= yMax)
			{
				return topColour;
			}
			else if (y < yMax && y >= yCentre)
			{
				var ratio = (double)(1 - (y - yCentre) / (yMax - yCentre));
				return topColour.Combine(topCentreColour, ratio);
			}
			else if (y > yMin && y <= yCentre)
			{
				var ratio = (double)(1 - (y - yMin) / (yCentre - yMin));
				return bottomColour.Combine(bottomCentreColour, ratio);
			}
			else if (y <= yMin)
			{
				return bottomColour;
			}
			else
			{
				return topColour;
			}
		}
	}

	public class LineSeries<TModel>
	{
		public event EventHandler<PropertyChangedEventArgs> PropertyChanged;
		public Func<IEnumerable<TModel>, TModel, string> TooltipLabelFormatter { get; set; }
		public IChartBrush Stroke { get; set; }
		public IChartBrush Fill { get; set; }

		private IEnumerable<TModel> values;
		public IEnumerable<TModel> Values 
		{
			get => values;
			set
			{
				values = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Values)));
			}
		}
	}

	/// <summary>
	/// Interaction logic for WpfChart.xaml
	/// </summary>
	public partial class WpfChart : UserControl
	{
		private struct ValueWithHeight
		{
			public string Value { get; set; }
			public double Height { get; set; }

			public ValueWithHeight(string value, double height)
			{
				Value = value;
				Height = height;
			}
		}

		private class ChartPoint
		{
			public double X { get; set; }
			public double Y { get; set; }
			public DateTimePoint BackingPoint { get; }

			public ChartPoint(double x, double y, DateTimePoint backingPoint)
			{
				X = x;
				Y = y;
				BackingPoint = backingPoint;
			}
		}

		private class WpfChartSeries : ObservableObject
		{
			public IEnumerable<ChartPoint> Data;
			public string PathStrokeData { get; }
			public string PathFillData { get; }
			public IChartBrush Stroke { get; }
			public IChartBrush Fill { get; }
			public double Height => Data.Max(x => x.Y) - Data.Min(x => x.Y);

			public bool PreventTrigger { get; set; }

			private double pseudoZoomLevel = 1;
			public double ZoomLevel
			{
				get => pseudoZoomLevel;
				set => SetProperty(ref pseudoZoomLevel, value);
			}

			private double pseudoZoomCentre = 0.5;
			public double ZoomCentre
			{
				get => pseudoZoomCentre;
				set
				{
					SetProperty(ref pseudoZoomCentre, value);
					SetZoomData();
				}
			}

			public IEnumerable<ChartPoint> ZoomData { get; private set; }

			public WpfChartSeries(IEnumerable<ChartPoint> data, IChartBrush stroke, IChartBrush fill)
			{
				Data = data;
				ZoomData = data;
				Stroke = stroke;
				Fill = fill;

				var sb = new StringBuilder();
				bool setM = true;
				foreach (var point in Data)
				{
					var pointType = setM ? "M" : "L";
					setM = false;
					sb.Append($" {pointType}{point.X} {point.Y}");
				}
				PathStrokeData = sb.ToString();
				PathStrokeData += $" L{Data.Last().X} {Data.First().Y}";

				var dataMin = Data.Min(x => x.BackingPoint.Value).Value;
				var dataMax = Data.Max(x => x.BackingPoint.Value).Value;
				var range = dataMax - dataMin;
				var zero = Math.Min(Math.Max(0d, dataMin), dataMax);
				var ratio = (double)(1 - (zero - dataMin) / range);
				var zeroPoint = ratio * (Data.Max(x => x.Y) - Data.Min(x => x.Y)) * 1.1;
				PathFillData = $"M{Data.First().X} {zeroPoint} {PathStrokeData.Replace("M", "L")} L{Data.Last().X} {zeroPoint}";
			}

			private void SetZoomData()
			{
				var zoomCentreX = Data.Min(x => x.X) + (Data.Max(x => x.X) - Data.Min(x => x.X)) * ZoomCentre;
				var data = new List<ChartPoint>();
				foreach (var point in Data)
				{
					data.Add(new ChartPoint(point.X + ((point.X - zoomCentreX) * (1 / ZoomLevel - 1)), point.Y, point.BackingPoint));
				}
				ZoomData = data;
			}
		}

		private double plotAreaHeight => DrawableChartSectionBorder.ActualHeight;
		private double plotAreaWidth => DrawableChartSectionBorder.ActualWidth;
		private DateTime timeLastUpdated;
		private TimeSpan updateLimit = TimeSpan.FromMilliseconds(1000 / 60d);
		private bool hasSetSeries;
		private List<LineSeries<DateTimePoint>> subscribedSeries = new();

		private KeepAliveTriggerService resizeTrigger;

		private bool tooltipLeft;
		private bool tooltipTop = true;

		public ObservableCollection<LineSeries<DateTimePoint>> Series
		{
			get { return (ObservableCollection<LineSeries<DateTimePoint>>)GetValue(SeriesProperty); }
			set { SetValue(SeriesProperty, value); }
		}
		public static readonly DependencyProperty SeriesProperty = DependencyProperty.Register(
		  "Series", typeof(ObservableCollection<LineSeries<DateTimePoint>>), typeof(WpfChart), new FrameworkPropertyMetadata(null, OnSeriesSet));

		public Func<DateTime, string> XAxisFormatter
		{
			get { return (Func<DateTime, string>)GetValue(XAxisFormatterProperty); }
			set { SetValue(XAxisFormatterProperty, value); }
		}
		public static readonly DependencyProperty XAxisFormatterProperty = DependencyProperty.Register(
		  "XAxisFormatter", typeof(Func<DateTime, string>), typeof(WpfChart), new PropertyMetadata(null));

		public Func<DateTime, string> XAxisCursorLabelFormatter
		{
			get { return (Func<DateTime, string>)GetValue(XAxisCursorLabelFormatterProperty); }
			set { SetValue(XAxisCursorLabelFormatterProperty, value); }
		}
		public static readonly DependencyProperty XAxisCursorLabelFormatterProperty = DependencyProperty.Register(
		  "XAxisCursorLabelFormatter", typeof(Func<DateTime, string>), typeof(WpfChart), new PropertyMetadata(null));

		public Func<double, string> YAxisFormatter
		{
			get { return (Func<double, string>)GetValue(YAxisFormatterProperty); }
			set { SetValue(YAxisFormatterProperty, value); }
		}
		public static readonly DependencyProperty YAxisFormatterProperty = DependencyProperty.Register(
		  "YAxisFormatter", typeof(Func<double, string>), typeof(WpfChart), new PropertyMetadata(null));

		public Func<double, string> YAxisCursorLabelFormatter
		{
			get { return (Func<double, string>)GetValue(YAxisCursorLabelFormatterProperty); }
			set { SetValue(YAxisCursorLabelFormatterProperty, value); }
		}
		public static readonly DependencyProperty YAxisCursorLabelFormatterProperty = DependencyProperty.Register(
		  "YAxisCursorLabelFormatter", typeof(Func<double, string>), typeof(WpfChart), new PropertyMetadata(null));

		public bool ShowXSeparatorLines
		{
			get { return (bool)GetValue(ShowXSeparatorLinesProperty); }
			set { SetValue(ShowXSeparatorLinesProperty, value); }
		}
		public static readonly DependencyProperty ShowXSeparatorLinesProperty = DependencyProperty.Register(
		  "ShowXSeparatorLines", typeof(bool), typeof(WpfChart), new PropertyMetadata(true));

		public bool ShowYSeparatorLines
		{
			get { return (bool)GetValue(ShowYSeparatorLinesProperty); }
			set { SetValue(ShowYSeparatorLinesProperty, value); }
		}
		public static readonly DependencyProperty ShowYSeparatorLinesProperty = DependencyProperty.Register(
		  "ShowYSeparatorLines", typeof(bool), typeof(WpfChart), new PropertyMetadata(true));

		public DataTemplate TooltipTemplate
		{
			get { return (DataTemplate)GetValue(TooltipTemplateProperty); }
			set { SetValue(TooltipTemplateProperty, value); }
		}
		public static readonly DependencyProperty TooltipTemplateProperty = DependencyProperty.Register(
		  "TooltipTemplate", typeof(DataTemplate), typeof(WpfChart), new PropertyMetadata(null));

		private string TooltipString
		{
			get { return (string)GetValue(TooltipStringProperty); }
			set { SetValue(TooltipStringProperty, value); }
		}
		public static readonly DependencyProperty TooltipStringProperty = DependencyProperty.Register(
		  "TooltipString", typeof(string), typeof(WpfChart));

		private DateTimePoint HoveredPoint
		{
			get { return (DateTimePoint)GetValue(HoveredPointProperty); }
			set { SetValue(HoveredPointProperty, value); }
		}
		public static readonly DependencyProperty HoveredPointProperty = DependencyProperty.Register(
		  "HoveredPoint", typeof(DateTimePoint), typeof(WpfChart), new PropertyMetadata(null));

		private ObservableCollection<WpfChartSeries> ConvertedSeries
		{
			get { return (ObservableCollection<WpfChartSeries>)GetValue(ConvertedSeriesProperty); }
			set { SetValue(ConvertedSeriesProperty, value); }
		}
		public static readonly DependencyProperty ConvertedSeriesProperty = DependencyProperty.Register(
		  "ConvertedSeries", typeof(ObservableCollection<WpfChartSeries>), typeof(WpfChart), new PropertyMetadata(new ObservableCollection<WpfChartSeries>()));

		private ObservableCollection<string> XAxisLabels
		{
			get { return (ObservableCollection<string>)GetValue(XAxisLabelsProperty); }
			set { SetValue(XAxisLabelsProperty, value); }
		}
		public static readonly DependencyProperty XAxisLabelsProperty = DependencyProperty.Register(
		  "XAxisLabels", typeof(ObservableCollection<string>), typeof(WpfChart), new PropertyMetadata(new ObservableCollection<string>()));

		private ObservableCollection<ValueWithHeight> YAxisLabels
		{
			get { return (ObservableCollection<ValueWithHeight>)GetValue(YAxisLabelsProperty); }
			set { SetValue(YAxisLabelsProperty, value); }
		}
		public static readonly DependencyProperty YAxisLabelsProperty = DependencyProperty.Register(
		  "YAxisLabels", typeof(ObservableCollection<ValueWithHeight>), typeof(WpfChart), new PropertyMetadata(new ObservableCollection<ValueWithHeight>()));

		private double XAxisLabelsWidth
		{
			get { return (double)GetValue(XAxisLabelsWidthProperty); }
			set { SetValue(XAxisLabelsWidthProperty, value); }
		}
		public static readonly DependencyProperty XAxisLabelsWidthProperty = DependencyProperty.Register(
		  "XAxisLabelsWidth", typeof(double), typeof(WpfChart), new PropertyMetadata(0d));

		public WpfChart()
		{
			InitializeComponent();
			this.Loaded += WpfChart_Loaded;

			resizeTrigger = new KeepAliveTriggerService(() => RenderChart(true), 100);
		}

		private void WpfChart_Loaded(object sender, RoutedEventArgs e)
		{
			RenderChart(false);
			this.Loaded -= WpfChart_Loaded;
			Application.Current.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
		}

		private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
		{
			resizeTrigger.Stop();
			foreach (var series in subscribedSeries)
			{
				series.PropertyChanged -= Series_PropertyChanged;
			}
		}

		private static void OnSeriesSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not WpfChart chart) return;

			if (chart.Series == null)
			{
				foreach (var series in chart.subscribedSeries)
				{
					series.PropertyChanged -= chart.Series_PropertyChanged;
				}

				return;
			}

			chart.Subscribe(chart.Series);
			chart.hasSetSeries = true;

			chart.RenderChart(false);
		}

		private void Subscribe(ObservableCollection<LineSeries<DateTimePoint>> series)
		{
			series.CollectionChanged += Series_CollectionChanged;
			if (!hasSetSeries)
			{
				foreach (LineSeries<DateTimePoint> item in series)
				{
					item.PropertyChanged += Series_PropertyChanged;
					subscribedSeries.Add(item);
				}
			}
			Series_PropertyChanged(this, new PropertyChangedEventArgs(nameof(Series)));
		}

		private void Series_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			foreach (LineSeries<DateTimePoint> series in e.OldItems)
			{
				series.PropertyChanged -= Series_PropertyChanged;
				subscribedSeries.Add(series);
			}

			foreach (LineSeries<DateTimePoint> series in e.NewItems)
			{
				series.PropertyChanged += Series_PropertyChanged;
				subscribedSeries.Remove(series);
			}
			RenderChart(false);
		}

		private void Series_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			RenderChart(false);
		}

		private void RenderChart(bool isResize)
		{
			Application.Current.Dispatcher.BeginInvoke(() =>
			{
				if (Series is null || !Series.Any()) return;

				var series = Series.First();
				if (!series.Values.Any()) return;

				(var xMin, var yMin, var xMax, var yMax, var xRange, var yRange) = GetAxisValues(Series.First());

				SetXAxisLabels(xMin, xRange, isResize);
				SetYAxisLabels(yMin, yMax, yRange, isResize);

				// Force layout update so sizes are correct before rendering points
				this.Dispatcher.Invoke(DispatcherPriority.Render, delegate() { });

				var points = GetPointsForSeries(xMin, xRange, yMin, yRange, Series.First());

				ConvertedSeries = new ObservableCollection<WpfChartSeries>()
					{ new WpfChartSeries(points, Series.First().Stroke, Series.First().Fill), };

				Series.First().Stroke?.Reevaluate(
					series.Values.Max(x => x.Value).Value,
					series.Values.Min(x => x.Value).Value,
					0, xMax.Ticks, xMin.Ticks, 0);
				Series.First().Fill?.Reevaluate(
					series.Values.Max(x => x.Value).Value,
					series.Values.Min(x => x.Value).Value,
					0, xMax.Ticks, xMin.Ticks, 0);
			});
		}

		private (DateTime xMin, double yMin, DateTime xMax, double yMax, TimeSpan xRange, double yRange) 
			GetAxisValues(LineSeries<DateTimePoint> series)
		{
			var xMin = series.Values.Min(x => x.DateTime);
			var xMax = series.Values.Max(x => x.DateTime);
			var yMin = series.Values.Min(x => x.Value).Value;
			var yMax = series.Values.Max(x => x.Value).Value;

			var xRange = xMax - xMin;
			var yRange = yMax - yMin;
			yMin -= yRange * 0.1;
			yMax += yRange * 0.1;
			yRange = yMax - yMin;

			return (xMin, yMin, xMax, yMax, xRange, yRange);
		}

		private void SetXAxisLabels(DateTime xMin, TimeSpan xRange, bool isResize)
		{
			var xAxisItemCount = Math.Floor(plotAreaWidth / 60);
			if (!isResize || XAxisLabels.Count != xAxisItemCount)
			{
				var xLabelStep = xRange / xAxisItemCount;
				XAxisLabels.Clear();

				DateTime currentXStep = xMin;
				for (int i = 0; i < xAxisItemCount; i++)
				{
					currentXStep += i == 0 || i == xAxisItemCount ? xLabelStep / 2 : xLabelStep;
					XAxisLabels.Add(XAxisFormatter == null ? currentXStep.ToString() : XAxisFormatter(currentXStep));
				}
			}
			
			XAxisLabelsWidth = plotAreaWidth / xAxisItemCount;
		}

		private void SetYAxisLabels(double yMin, double yMax, double yRange, bool isResize)
		{
			var yAxisItemsCount = Math.Floor(plotAreaHeight / 50);
			var yLabelStep = yRange / yAxisItemsCount;
			var labels = GetYSteps(yRange, yAxisItemsCount, yMin, yMax).ToList();
			var labels2 = labels.Select(y => new ValueWithHeight()
			{
				Value = YAxisFormatter == null ? Math.Round(y, 2).ToString() : YAxisFormatter(y),
				Height = ((y - yMin) / yRange * plotAreaHeight) - (labels.ToList().IndexOf(y) > 0
					? (labels[labels.ToList().IndexOf(y) - 1] - yMin) / yRange * plotAreaHeight
					: 0),
			});
			YAxisLabels = new ObservableCollection<ValueWithHeight>(labels2.Reverse());
		}

		private List<ChartPoint> GetPointsForSeries(
			DateTime xMin, TimeSpan xRange, double yMin, double yRange, LineSeries<DateTimePoint> series)
		{
			List<ChartPoint> points = new();
			foreach (var point in series.Values/*.OrderBy(x => x.DateTime)*/)
			{
				double x = (double)(point.DateTime - xMin).Ticks / (double)xRange.Ticks * (double)plotAreaWidth;
				double y = plotAreaHeight - (point.Value.Value - yMin) / yRange * plotAreaHeight;
				points.Add(new ChartPoint(x, y, point));
			}
			return points;
		}

		private List<double> GetYSteps(double yRange, double yAxisItemsCount, double yMin, double yMax)
		{
			var idealStep = yRange / yAxisItemsCount;
			double min = double.MaxValue;
			int stepAtMin = 1;
			var roundedSteps = new List<int>() 
				{ 1, 10, 100, 500, 1000, 1500, 2000, 3000, 4000, 5000, 10000, 20000, 50000, 1000000, 10000000 };
			roundedSteps.Reverse();
			foreach (var step in roundedSteps)
			{
				var val = Math.Abs(idealStep - step);
				if (val < min)
				{
					min = val;
					stepAtMin = step;
				}
			}

			List<double> yVals = new();
			bool startAt0 = yMin <= 0 && yMax >= 0;
			if (startAt0)
			{
				double currVal = 0;
				while (currVal > yMin)
				{
					yVals.Insert(0, currVal);
					currVal -= stepAtMin;
				}

				currVal = stepAtMin;

				while (currVal < yMax)
				{
					yVals.Add(currVal);
					currVal += stepAtMin;
				}
			}
			else
			{
				int dir = yMax < 0 ? -1 : 1;
				double currVal = 0;
				bool adding = true;
				bool hasStartedAdding = false;
				while (adding)
				{
					if (currVal < yMax && currVal > yMin)
					{
						hasStartedAdding = true;
						yVals.Add(currVal);
					}
					else if (hasStartedAdding)
					{
						adding = false;
					}

					currVal += dir * stepAtMin;
				}
			}

			return yVals;
		}

		private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			resizeTrigger.Refresh();
		}

		private void DrawableChartSectionBorder_MouseMove(object sender, MouseEventArgs e)
		{
			if (DateTime.Now - timeLastUpdated < updateLimit) return;
			
			timeLastUpdated = DateTime.Now;
			var mouseLoc = e.GetPosition(Grid);
			(var xMin, var yMin, var xMax, var yMax, var xRange, var yRange) = GetAxisValues(Series.First());
			var xPercent = mouseLoc.X / plotAreaWidth;
			var yPercent = mouseLoc.Y / plotAreaHeight;
			var xVal = xMin.Add(xPercent * xRange);
			var yVal = ((1 - yPercent) * yRange + yMin);

			// Move crosshairs
			XCrosshair.Margin = new Thickness(0, mouseLoc.Y, 0, 0);
			YCrosshair.Margin = new Thickness(mouseLoc.X, 0, 0, 0);
			XCrosshairValueDisplay.Margin = new Thickness(mouseLoc.X - 50, 0, 0, -XAxisRow.ActualHeight);
			YCrosshairValueDisplay.Margin = new Thickness(-YAxisColumn.ActualWidth, mouseLoc.Y - 10, 0, 0);

			// Set value displays
			XCrosshairValueLabel.Text
				= XAxisCursorLabelFormatter == null ? xVal.ToString() : XAxisCursorLabelFormatter(xVal);
			YCrosshairValueLabel.Text = YAxisCursorLabelFormatter == null
				? Math.Round(yVal, 2).ToString() 
				: YAxisCursorLabelFormatter(yVal);

			// Get chartpoint to display tooltip for
			var chartPoints = ConvertedSeries.First().ZoomData;
			var nearestPoint = chartPoints.First(x => Math.Abs(x.X - mouseLoc.X) == chartPoints.Min(x => Math.Abs(x.X - mouseLoc.X)));
			var hoveredChartPoints = chartPoints.Where(x => x.X == nearestPoint.X);
			var hoveredChartPoint = hoveredChartPoints.Count() > 1
				? hoveredChartPoints.First(x => Math.Abs(x.Y - mouseLoc.Y) == hoveredChartPoints.Min(x => Math.Abs(x.Y - mouseLoc.Y)))
				: hoveredChartPoints.First();

			if (hoveredChartPoint == null) return;

			// Get tooltip position variables
			if (!tooltipLeft && (plotAreaWidth - mouseLoc.X) < (TooltipBorder.ActualWidth + 10)) tooltipLeft = true;
			if (tooltipLeft && (mouseLoc.X) < (TooltipBorder.ActualWidth + 5)) tooltipLeft = false;
			if (!tooltipTop && (plotAreaHeight - mouseLoc.Y) < (TooltipBorder.ActualHeight + 10)) tooltipTop = true;
			if (tooltipTop && (mouseLoc.Y) < (TooltipBorder.ActualHeight + 5)) tooltipTop = false;

			// Set location of items related to the hoveredPoint
			TooltipBorder.Margin = new Thickness(!tooltipLeft ? mouseLoc.X + 5 : mouseLoc.X - TooltipBorder.ActualWidth - 5, !tooltipTop ? mouseLoc.Y + 5 : mouseLoc.Y - TooltipBorder.ActualHeight - 5, 0, 0);
			HoveredPointEllipse.Margin = new Thickness(hoveredChartPoint.X - 5, hoveredChartPoint.Y - 5, 0, 0);
			HoveredPointEllipse.Fill = new SolidColorBrush(
				ConvertedSeries.First().Stroke.ColourAtPoint(hoveredChartPoint.BackingPoint.DateTime.Ticks, hoveredChartPoint.BackingPoint.Value.Value));
			XPointHighlighter.Margin = new Thickness(0, hoveredChartPoint.Y, 0, 0);
			HoveredPoint = hoveredChartPoint.BackingPoint;
			if (Series.First().TooltipLabelFormatter != null) TooltipString = Series.First().TooltipLabelFormatter(Series.First().Values, HoveredPoint);
		}

		private void DrawableChartSectionBorder_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			var zoomIn = e.Delta > 0;
			var mouseLoc = e.GetPosition(Grid);
			foreach (var series in ConvertedSeries)
			{
				if (zoomIn)
				{
					series.ZoomLevel *= 0.9;
				}
				else
				{
					var zoomLevel = series.ZoomLevel /= 0.9;
					zoomLevel = Math.Min(series.ZoomLevel, 1);
					series.ZoomLevel = zoomLevel;
					if (series.ZoomLevel == 1)
					{
						series.ZoomCentre = 0.5;
					}
				}

				var effectiveMin = Math.Max(plotAreaWidth * series.ZoomCentre - (plotAreaWidth * series.ZoomLevel / 2), 0);
				var effectiveMax = Math.Min(plotAreaWidth * series.ZoomCentre + (plotAreaWidth * series.ZoomLevel / 2), plotAreaWidth);
				var ratio = mouseLoc.X / plotAreaWidth;
				series.ZoomCentre = (effectiveMin + ratio * (effectiveMax - effectiveMin)) / plotAreaWidth;
			}

			HoveredPoint = null;
			TooltipString = string.Empty;
		}
	}
}
