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
using System.Windows.Threading;
using CoreUtilities.Services;
using ModernThemables.HelperClasses.WpfChart;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ModernThemables.Converters;
using System.Threading.Tasks;

namespace ModernThemables.Controls
{
	/// <summary>
	/// Interaction logic for WpfChart.xaml
	/// </summary>
	public partial class WpfChart : UserControl
	{
		public event EventHandler<DateTimePoint> PointClicked;
		public event EventHandler<Tuple<DateTimePoint, DateTimePoint>> PointRangeSelected;

		private double plotAreaHeight => DrawableChartSectionBorder.ActualHeight;
		private double plotAreaWidth => DrawableChartSectionBorder.ActualWidth;
		private DateTime timeLastUpdated;
		private TimeSpan updateLimit = TimeSpan.FromMilliseconds(1000 / 60d);
		private bool hasSetSeries;
		private List<LineSeries<DateTimePoint>> subscribedSeries = new();

		private KeepAliveTriggerService resizeTrigger;

		private ChartPoint cachedPoint;
		private Point? lastMouseMovePoint;

		private bool tooltipLeft;
		private bool tooltipTop = true;

		private bool isMouseDown;
		private bool isUserDragging;
		private bool userCouldBePanning;
		private bool isUserPanning;
		private DateTimePoint lowerSelection;
		private DateTimePoint upperSelection;

		#region Dependecy Properties

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

		public bool IsZoomed
		{
			get { return (bool)GetValue(IsZoomedProperty); }
			set { SetValue(IsZoomedProperty, value); }
		}
		public static readonly DependencyProperty IsZoomedProperty = DependencyProperty.Register(
		  "IsZoomed", typeof(bool), typeof(WpfChart), new PropertyMetadata(true));

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

		private ChartPoint HoveredPoint
		{
			get { return (ChartPoint)GetValue(HoveredPointProperty); }
			set { SetValue(HoveredPointProperty, value); }
		}
		public static readonly DependencyProperty HoveredPointProperty = DependencyProperty.Register(
		  "HoveredPoint", typeof(ChartPoint), typeof(WpfChart), new PropertyMetadata(null));

		private ObservableCollection<WpfChartSeriesViewModel> ConvertedSeries
		{
			get { return (ObservableCollection<WpfChartSeriesViewModel>)GetValue(ConvertedSeriesProperty); }
			set { SetValue(ConvertedSeriesProperty, value); }
		}
		public static readonly DependencyProperty ConvertedSeriesProperty = DependencyProperty.Register(
		  "ConvertedSeries", typeof(ObservableCollection<WpfChartSeriesViewModel>), typeof(WpfChart), new PropertyMetadata(new ObservableCollection<WpfChartSeriesViewModel>()));

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

		private bool IsCrosshairVisible
		{
			get { return (bool)GetValue(IsCrosshairVisibleProperty); }
			set { SetValue(IsCrosshairVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsCrosshairVisibleProperty = DependencyProperty.Register(
		  "IsCrosshairVisible", typeof(bool), typeof(WpfChart), new PropertyMetadata(true));

		private bool IsTooltipVisible
		{
			get { return (bool)GetValue(IsTooltipVisibleProperty); }
			set { SetValue(IsTooltipVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsTooltipVisibleProperty = DependencyProperty.Register(
		  "IsTooltipVisible", typeof(bool), typeof(WpfChart), new PropertyMetadata(true));

		private bool IsAxisIndicatorsVisible
		{
			get { return (bool)GetValue(IsAxisIndicatorsVisibleProperty); }
			set { SetValue(IsAxisIndicatorsVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsAxisIndicatorsVisibleProperty = DependencyProperty.Register(
		  "IsAxisIndicatorsVisible", typeof(bool), typeof(WpfChart), new PropertyMetadata(true));

		private bool IsPointIndicatorsVisible
		{
			get { return (bool)GetValue(IsPointIndicatorsVisibleProperty); }
			set { SetValue(IsPointIndicatorsVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsPointIndicatorsVisibleProperty = DependencyProperty.Register(
		  "IsPointIndicatorsVisible", typeof(bool), typeof(WpfChart), new PropertyMetadata(true));

		private bool IsUserSelectingRange
		{
			get { return (bool)GetValue(IsUserSelectingRangeProperty); }
			set { SetValue(IsUserSelectingRangeProperty, value); }
		}
		public static readonly DependencyProperty IsUserSelectingRangeProperty = DependencyProperty.Register(
		  "IsUserSelectingRange", typeof(bool), typeof(WpfChart), new PropertyMetadata(false));

		private double ZoomLevel
		{
			get { return (double)GetValue(ZoomLevelProperty); }
			set { SetValue(ZoomLevelProperty, value); }
		}
		public static readonly DependencyProperty ZoomLevelProperty = DependencyProperty.Register(
		  "ZoomLevel", typeof(double), typeof(WpfChart), new FrameworkPropertyMetadata(1d, OnZoomSet));

		private double ZoomCentre
		{
			get { return (double)GetValue(ZoomCentreProperty); }
			set { SetValue(ZoomCentreProperty, value); }
		}
		public static readonly DependencyProperty ZoomCentreProperty = DependencyProperty.Register(
		  "ZoomCentre", typeof(double), typeof(WpfChart), new FrameworkPropertyMetadata(0.5d, OnZoomSet));

		private double ZoomOffset
		{
			get { return (double)GetValue(ZoomOffsetProperty); }
			set { SetValue(ZoomOffsetProperty, value); }
		}
		public static readonly DependencyProperty ZoomOffsetProperty = DependencyProperty.Register(
		  "ZoomOffset", typeof(double), typeof(WpfChart), new FrameworkPropertyMetadata(0d, OnZoomSet));

		#endregion

		public WpfChart()
		{
			InitializeComponent();
			this.Loaded += WpfChart_Loaded;

			NameScope.SetNameScope(ContextMenu, NameScope.GetNameScope(this));

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

		public void ResetZoom()
		{
			ZoomCentre = 0.5;
			ZoomLevel = 1;
			ZoomOffset = 0;
			IsZoomed = false;
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

		private static void OnZoomSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not WpfChart chart) return;

			foreach (var series in chart.ConvertedSeries)
			{
				series.SetZoomData(chart.ZoomLevel, chart.ZoomCentre, chart.ZoomOffset);
			}
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

				ConvertedSeries = new ObservableCollection<WpfChartSeriesViewModel>()
					{ new WpfChartSeriesViewModel(points, Series.First().Stroke, Series.First().Fill), };

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
			var yAxisItemsCount = Math.Max(1, Math.Floor(plotAreaHeight / 50));
			var yLabelStep = Math.Max(1, yRange / yAxisItemsCount);
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

		private void Grid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			if (isUserPanning)
			{
				e.Handled = true;
			}
			isUserPanning = false;
			userCouldBePanning = false;
		}

		#region Mouse events

		private void MouseCaptureGrid_MouseMove(object sender, MouseEventArgs e)
		{
			IsZoomed = ZoomLevel != 1 || ZoomOffset != 0;

			var mouseLoc = e.GetPosition(Grid);

			if (isMouseDown)
			{
				isUserDragging = true;
				IsUserSelectingRange = !(userCouldBePanning || isUserPanning);
			}

			if (userCouldBePanning)
			{
				isUserPanning = true;
				if (lastMouseMovePoint != null)
				{
					ZoomOffset += lastMouseMovePoint.Value.X - mouseLoc.X;
				}
			}
			lastMouseMovePoint = mouseLoc;

			if (DateTime.Now - timeLastUpdated < updateLimit) return;
			
			timeLastUpdated = DateTime.Now;
			(var xMin, var yMin, var xMax, var yMax, var xRange, var yRange) = GetAxisValues(Series.First());
			var xPercent = mouseLoc.X / plotAreaWidth;
			var yPercent = mouseLoc.Y / plotAreaHeight;
			var xVal = xMin.Add(xPercent * xRange);
			var yVal = ((1 - yPercent) * yRange + yMin);

			if (IsCrosshairVisible)
			{
				// Move crosshairs
				XCrosshair.Margin = new Thickness(0, mouseLoc.Y, 0, 0);
				YCrosshair.Margin = new Thickness(mouseLoc.X, 0, 0, 0);
			}

			if (IsAxisIndicatorsVisible)
			{
				XCrosshairValueDisplay.Margin = new Thickness(mouseLoc.X - 50, 0, 0, -XAxisRow.ActualHeight);
				YCrosshairValueDisplay.Margin = new Thickness(-YAxisColumn.ActualWidth, mouseLoc.Y - 10, 0, 0);

				// Set value displays
				XCrosshairValueLabel.Text
					= XAxisCursorLabelFormatter == null ? xVal.ToString() : XAxisCursorLabelFormatter(xVal);
				YCrosshairValueLabel.Text = YAxisCursorLabelFormatter == null
					? Math.Round(yVal, 2).ToString()
					: YAxisCursorLabelFormatter(yVal);
			}

			// Get chartpoint to display tooltip for
			var chartPoints = ConvertedSeries.First().ZoomData;
			var nearestPoint = chartPoints.First(x => Math.Abs(x.X - mouseLoc.X) == chartPoints.Min(x => Math.Abs(x.X - mouseLoc.X)));
			var hoveredChartPoints = chartPoints.Where(x => x.X == nearestPoint.X);
			var hoveredChartPoint = hoveredChartPoints.Count() > 1
				? hoveredChartPoints.First(x => Math.Abs(x.Y - mouseLoc.Y) == hoveredChartPoints.Min(x => Math.Abs(x.Y - mouseLoc.Y)))
				: hoveredChartPoints.First();

			if (hoveredChartPoint == null) return;

			HoveredPoint = hoveredChartPoint;

			if (IsTooltipVisible)
			{
				// Get tooltip position variables
				if (!tooltipLeft && (plotAreaWidth - mouseLoc.X) < (TooltipBorder.ActualWidth + 10)) tooltipLeft = true;
				if (tooltipLeft && (mouseLoc.X) < (TooltipBorder.ActualWidth + 5)) tooltipLeft = false;
				if (!tooltipTop && (plotAreaHeight - mouseLoc.Y) < (TooltipBorder.ActualHeight + 10)) tooltipTop = true;
				if (tooltipTop && (mouseLoc.Y) < (TooltipBorder.ActualHeight + 5)) tooltipTop = false;

				TooltipBorder.Margin = new Thickness(!tooltipLeft ? mouseLoc.X + 5 : mouseLoc.X - TooltipBorder.ActualWidth - 5, !tooltipTop ? mouseLoc.Y + 5 : mouseLoc.Y - TooltipBorder.ActualHeight - 5, 0, 0);
				if (Series.First().TooltipLabelFormatter != null) TooltipString = Series.First().TooltipLabelFormatter(Series.First().Values, HoveredPoint.BackingPoint);
			}

			if (IsPointIndicatorsVisible)
			{
				// Set location of items related to the hoveredPoint
				HoveredPointEllipse.Margin = new Thickness(hoveredChartPoint.X - 5, hoveredChartPoint.Y - 5, 0, 0);
				HoveredPointEllipse.Fill = new SolidColorBrush(
					ConvertedSeries.First().Stroke.ColourAtPoint(hoveredChartPoint.BackingPoint.DateTime.Ticks, hoveredChartPoint.BackingPoint.Value.Value));
				XPointHighlighter.Margin = new Thickness(0, hoveredChartPoint.Y, 0, 0);
			}	

			if (IsUserSelectingRange)
			{
				SelectionRangeBorder.Width = Math.Max(HoveredPoint.X - SelectionRangeBorder.Margin.Left, 0);
			}
		}

		private void MouseCaptureGrid_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			IsZoomed = ZoomLevel != 1 || ZoomOffset != 0;
			cachedPoint = HoveredPoint ?? cachedPoint;

			var zoomIn = e.Delta > 0;

			if (zoomIn)
			{
				ZoomLevel *= 0.9;
			}
			else
			{
				var zoomLevel = ZoomLevel / 0.9;
				zoomLevel = Math.Min(zoomLevel, 1);
				ZoomLevel = zoomLevel;
				if (ZoomLevel == 1)
				{
					ZoomCentre = 0.5;
					ZoomOffset = 0;
				}
			}

			(var xMin, var yMin, var xMax, var yMax, var xRange, var yRange) = GetAxisValues(Series.First());
			var prevZoomCentre = ZoomCentre;
			var newZoomCentre =
				(cachedPoint.BackingPoint.DateTime - xMin).TotalMilliseconds / (xMax - xMin).TotalMilliseconds;
			ZoomCentre = (prevZoomCentre * 3 + newZoomCentre) / 4;

			HoveredPoint = null;
			TooltipString = string.Empty;
		}

		private void MouseCaptureGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			isMouseDown = true;
			if (e.ChangedButton == MouseButton.Left)
			{
				e.Handled = true;
				lowerSelection = HoveredPoint.BackingPoint;
				SelectionRangeBorder.Margin = new Thickness(HoveredPoint.X, 0, 0, 0);
			}
			else if (e.ChangedButton == MouseButton.Right)
			{
				userCouldBePanning = true;
			}
		}

		private void MouseCaptureGrid_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			isMouseDown = false;
			IsUserSelectingRange = false;

			if (e.ChangedButton != MouseButton.Left)
			{
				isUserDragging = false;
				return;
			}

			if (isUserDragging)
			{
				upperSelection = HoveredPoint.BackingPoint;
				PointRangeSelected?.Invoke(this, new Tuple<DateTimePoint, DateTimePoint>(lowerSelection, upperSelection));
				isUserDragging = false;
				isUserPanning = false;
				userCouldBePanning = false;
				return;
			}

			PointClicked?.Invoke(this, lowerSelection);
			PointSelectionEllipse.Opacity = 1;
			ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(1, 3, TimeSpan.FromMilliseconds(200)));
			ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(1, 3, TimeSpan.FromMilliseconds(200)));
			PointSelectionEllipse.BeginAnimation(Ellipse.OpacityProperty, new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200)));
			PointSelectionEllipse.BeginAnimation(Ellipse.StrokeThicknessProperty, new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200)));

			e.Handled = true;
		}

		#endregion
	}
}
