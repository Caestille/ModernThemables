using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Threading;
using CoreUtilities.Services;
using ModernThemables.HelperClasses.WpfChart;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ModernThemables.Interfaces;

namespace ModernThemables.Controls
{
	/// <summary>
	/// Interaction logic for WpfChart.xaml
	/// </summary>
	public partial class WpfChart : UserControl
	{
		public event EventHandler<IChartPoint> PointClicked;
		public event EventHandler<Tuple<IChartPoint, IChartPoint>> PointRangeSelected;

		private double plotAreaHeight => DrawableChartSectionBorder.ActualHeight;
		private double plotAreaWidth => DrawableChartSectionBorder.ActualWidth;
		private DateTime timeLastUpdated;
		private TimeSpan updateLimit = TimeSpan.FromMilliseconds(1000 / 60d);
		private bool hasSetSeries;
		private List<ISeries> subscribedSeries = new();

		private KeepAliveTriggerService resizeTrigger;

		private Point? lastMouseMovePoint;
		private double currentZoomLevel = 1;

		private bool tooltipLeft;
		private bool tooltipTop = true;

		private bool isMouseDown;
		private bool isUserDragging;
		private bool userCouldBePanning;
		private bool isUserPanning;
		private IChartPoint lowerSelection;
		private IChartPoint upperSelection;

		#region Dependecy Properties

		public ObservableCollection<ISeries> Series
		{
			get { return (ObservableCollection<ISeries>)GetValue(SeriesProperty); }
			set { SetValue(SeriesProperty, value); }
		}
		public static readonly DependencyProperty SeriesProperty = DependencyProperty.Register(
		  "Series", typeof(ObservableCollection<ISeries>), typeof(WpfChart), new FrameworkPropertyMetadata(null, OnSeriesSet));

		public Func<object, string> XAxisFormatter
		{
			get { return (Func<object, string>)GetValue(XAxisFormatterProperty); }
			set { SetValue(XAxisFormatterProperty, value); }
		}
		public static readonly DependencyProperty XAxisFormatterProperty = DependencyProperty.Register(
		  "XAxisFormatter", typeof(Func<object, string>), typeof(WpfChart), new PropertyMetadata(null));

		public Func<object, string> XAxisCursorLabelFormatter
		{
			get { return (Func<object, string>)GetValue(XAxisCursorLabelFormatterProperty); }
			set { SetValue(XAxisCursorLabelFormatterProperty, value); }
		}
		public static readonly DependencyProperty XAxisCursorLabelFormatterProperty = DependencyProperty.Register(
		  "XAxisCursorLabelFormatter", typeof(Func<object, string>), typeof(WpfChart), new PropertyMetadata(null));

		public Func<object, string> YAxisFormatter
		{
			get { return (Func<object, string>)GetValue(YAxisFormatterProperty); }
			set { SetValue(YAxisFormatterProperty, value); }
		}
		public static readonly DependencyProperty YAxisFormatterProperty = DependencyProperty.Register(
		  "YAxisFormatter", typeof(Func<object, string>), typeof(WpfChart), new PropertyMetadata(null));

		public Func<object, string> YAxisCursorLabelFormatter
		{
			get { return (Func<object, string>)GetValue(YAxisCursorLabelFormatterProperty); }
			set { SetValue(YAxisCursorLabelFormatterProperty, value); }
		}
		public static readonly DependencyProperty YAxisCursorLabelFormatterProperty = DependencyProperty.Register(
		  "YAxisCursorLabelFormatter", typeof(Func<object, string>), typeof(WpfChart), new PropertyMetadata(null));

		public Func<double, bool> YAxisLabelIdentifier
		{
			get { return (Func<double, bool>)GetValue(YAxisLabelIdentifierProperty); }
			set { SetValue(YAxisLabelIdentifierProperty, value); }
		}
		public static readonly DependencyProperty YAxisLabelIdentifierProperty = DependencyProperty.Register(
		  "YAxisLabelIdentifier", typeof(Func<double, bool>), typeof(WpfChart), new PropertyMetadata(null));

		public Func<double, bool> XAxisLabelIdentifier
		{
			get { return (Func<double, bool>)GetValue(XAxisLabelIdentifierProperty); }
			set { SetValue(XAxisLabelIdentifierProperty, value); }
		}
		public static readonly DependencyProperty XAxisLabelIdentifierProperty = DependencyProperty.Register(
		  "XAxisLabelIdentifier", typeof(Func<double, bool>), typeof(WpfChart), new PropertyMetadata(null));

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
		  "IsZoomed", typeof(bool), typeof(WpfChart), new PropertyMetadata(false));

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

		private InternalChartPointRepresentation HoveredPoint
		{
			get { return (InternalChartPointRepresentation)GetValue(HoveredPointProperty); }
			set { SetValue(HoveredPointProperty, value); }
		}
		public static readonly DependencyProperty HoveredPointProperty = DependencyProperty.Register(
		  "HoveredPoint", typeof(InternalChartPointRepresentation), typeof(WpfChart), new PropertyMetadata(null));

		private ObservableCollection<WpfChartSeriesViewModel> ConvertedSeries
		{
			get { return (ObservableCollection<WpfChartSeriesViewModel>)GetValue(ConvertedSeriesProperty); }
			set { SetValue(ConvertedSeriesProperty, value); }
		}
		public static readonly DependencyProperty ConvertedSeriesProperty = DependencyProperty.Register(
		  "ConvertedSeries", typeof(ObservableCollection<WpfChartSeriesViewModel>), typeof(WpfChart), new PropertyMetadata(new ObservableCollection<WpfChartSeriesViewModel>()));

		private ObservableCollection<ValueWithHeight> XAxisLabels
		{
			get { return (ObservableCollection<ValueWithHeight>)GetValue(XAxisLabelsProperty); }
			set { SetValue(XAxisLabelsProperty, value); }
		}
		public static readonly DependencyProperty XAxisLabelsProperty = DependencyProperty.Register(
		  "XAxisLabels", typeof(ObservableCollection<ValueWithHeight>), typeof(WpfChart), new PropertyMetadata(new ObservableCollection<ValueWithHeight>()));

		private ObservableCollection<ValueWithHeight> YAxisLabels
		{
			get { return (ObservableCollection<ValueWithHeight>)GetValue(YAxisLabelsProperty); }
			set { SetValue(YAxisLabelsProperty, value); }
		}
		public static readonly DependencyProperty YAxisLabelsProperty = DependencyProperty.Register(
		  "YAxisLabels", typeof(ObservableCollection<ValueWithHeight>), typeof(WpfChart), new PropertyMetadata(new ObservableCollection<ValueWithHeight>()));

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

		private ZoomState CurrentZoomState
		{
			get { return (ZoomState)GetValue(CurrentZoomStateProperty); }
			set { SetValue(CurrentZoomStateProperty, value); }
		}
		public static readonly DependencyProperty CurrentZoomStateProperty = DependencyProperty.Register(
		  "CurrentZoomState", typeof(ZoomState), typeof(WpfChart), new PropertyMetadata(new ZoomState(0, 0, 0)));

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
			var binding = SeriesMultiBinding;
			SeriesItemsControl.Margin = new Thickness(0);
			UpdateZoom(Series.Min(x => x.Values.Min(y => y.XValue)), Series.Max(x => x.Values.Max(y => y.XValue)), 0);
			SeriesItemsControl.SetBinding(ItemsControl.MarginProperty, binding);

			IsZoomed = false;

			RenderChart(false);
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

			chart.UpdateZoom(chart.Series.Min(x => x.Values.Min(y => y.XValue)), chart.Series.Max(x => x.Values.Max(y => y.XValue)), 0);

			chart.Subscribe(chart.Series);
			chart.hasSetSeries = true;

			chart.RenderChart(false);
		}

		private void Subscribe(ObservableCollection<ISeries> series)
		{
			series.CollectionChanged += Series_CollectionChanged;
			if (!hasSetSeries)
			{
				foreach (ISeries item in series)
				{
					item.PropertyChanged += Series_PropertyChanged;
					subscribedSeries.Add(item);
				}
			}
			Series_PropertyChanged(this, new PropertyChangedEventArgs(nameof(Series)));
		}

		private void Series_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			foreach (ISeries series in e.OldItems)
			{
				series.PropertyChanged -= Series_PropertyChanged;
				subscribedSeries.Add(series);
			}

			foreach (ISeries series in e.NewItems)
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

				var axisValues = GetDataBounds();

				SetXAxisLabels(CurrentZoomState.Min, CurrentZoomState.Max);
				SetYAxisLabels(axisValues.YMin, axisValues.YMax);

				// Force layout update so sizes are correct before rendering points
				this.Dispatcher.Invoke(DispatcherPriority.Render, delegate() { });

				var points = GetPointsForSeries(axisValues.XMin, axisValues.XRange, axisValues.YMin, axisValues.YRange, Series.First());

				ConvertedSeries = new ObservableCollection<WpfChartSeriesViewModel>()
					{ new WpfChartSeriesViewModel(points, Series.First().Stroke, Series.First().Fill), };

				Series.First().Stroke?.Reevaluate(
					series.Values.Max(x => x.YValue),
					series.Values.Min(x => x.YValue),
					0, axisValues.XMax, axisValues.XMin, 0);
				Series.First().Fill?.Reevaluate(
					series.Values.Max(x => x.YValue),
					series.Values.Min(x => x.YValue),
					0, axisValues.XMax, axisValues.XMin, 0);
			});
		}

		private DataBounds GetDataBounds()
		{
			var xMin = Series.Min(x => x.Values.Min(y => y.XValue));
			var xMax = Series.Min(x => x.Values.Max(x => x.XValue));
			var yMin = Series.Min(x => x.Values.Min(x => x.YValue));
			var yMax = Series.Min(x => x.Values.Max(x => x.YValue));

			var yRange = yMax - yMin;
			yMin -= yRange * 0.1;
			yMax += yRange * 0.1;

			return new DataBounds(xMin, xMax, yMin, yMax);
		}

		private void SetXAxisLabels(double xMin, double xMax)
		{
			var xRange = xMax - xMin;
			var xAxisItemCount = Math.Floor(plotAreaWidth / 60);
			var labels = GetXSteps(xAxisItemCount, xMin, xMax);
			var labels2 = labels.Select(x => new ValueWithHeight()
			{
				Value = XAxisFormatter == null ? x.ToString("MMM YY") : XAxisFormatter(Series.First().Values.First().XValueToImplementation(x)),
				Height = ((x - xMin) / xRange * plotAreaWidth) - (labels.ToList().IndexOf(x) > 0
					? (labels[labels.ToList().IndexOf(x) - 1] - xMin) / xRange * plotAreaWidth
					: 0),
			});
			XAxisLabels = new ObservableCollection<ValueWithHeight>(labels2);
		}

		private void SetYAxisLabels(double yMin, double yMax)
		{
			var yRange = yMax - yMin;
			var yAxisItemsCount = Math.Max(1, Math.Floor(plotAreaHeight / 50));
			var labels = GetYSteps(yAxisItemsCount, yMin, yMax).ToList();
			var labels2 = labels.Select(y => new ValueWithHeight()
			{
				Value = YAxisFormatter == null ? Math.Round(y, 2).ToString() : YAxisFormatter(Series.First().Values.First().YValueToImplementation(y)),
				Height = ((y - yMin) / yRange * plotAreaHeight) - (labels.ToList().IndexOf(y) > 0
					? (labels[labels.ToList().IndexOf(y) - 1] - yMin) / yRange * plotAreaHeight
					: 0),
			});
			YAxisLabels = new ObservableCollection<ValueWithHeight>(labels2.Reverse());
		}

		private List<InternalChartPointRepresentation> GetPointsForSeries(
			double xMin, double xRange, double yMin, double yRange, ISeries series)
		{
			List<InternalChartPointRepresentation> points = new();
			foreach (var point in series.Values)
			{
				double x = (double)(point.XValue - xMin) / (double)xRange * (double)plotAreaWidth;
				double y = plotAreaHeight - (point.YValue - yMin) / yRange * plotAreaHeight;
				points.Add(new InternalChartPointRepresentation(x, y, point));
			}
			return points;
		}

		private List<double> GetXSteps(double xAxisItemsCount, double xMin, double xMax)
		{
			var xRange = xMax - xMin;
			var idealStep = xRange / xAxisItemsCount;
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

			List<double> xVals = new();

			double currVal = xMin;
			while (currVal < xMax)
			{
				currVal += stepAtMin;
				xVals.Add(currVal);
			}

			int fracOver = (int)Math.Ceiling(xVals.Count() / (decimal)Math.Round(xAxisItemsCount));

			return xVals.Where(x => xVals.IndexOf(x) % fracOver == 0).ToList();
		}

		private List<double> GetYSteps(double yAxisItemsCount, double yMin, double yMax)
		{
			var yRange = yMax - yMin;
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

		private void UpdateZoom(double min, double max, double zoomOffset)
		{
			CurrentZoomState = new ZoomState(min, max, zoomOffset);
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
			IsZoomed = SeriesItemsControl.Margin.Left != 0 || SeriesItemsControl.Margin.Right != 0;

			var mouseLoc = e.GetPosition(Grid);

			if (isMouseDown)
			{
				isUserDragging = true;
				IsUserSelectingRange = !(userCouldBePanning || isUserPanning);
			}

			DataBounds? axisValues = null;
			if (userCouldBePanning)
			{
				isUserPanning = true;
				if (lastMouseMovePoint != null)
				{
					axisValues = GetDataBounds();
					var zoomOffset = lastMouseMovePoint.Value.X - mouseLoc.X;
					UpdateZoom(CurrentZoomState.Min, CurrentZoomState.Max, zoomOffset);
					SetXAxisLabels(CurrentZoomState.Min, CurrentZoomState.Max);
				}
			}
			lastMouseMovePoint = mouseLoc;

			if (DateTime.Now - timeLastUpdated < updateLimit) return;

			axisValues = axisValues ?? GetDataBounds();
			timeLastUpdated = DateTime.Now;
			var xPercent = mouseLoc.X / plotAreaWidth;
			var yPercent = mouseLoc.Y / plotAreaHeight;
			var xVal = CurrentZoomState.Min + (xPercent * (CurrentZoomState.Max - CurrentZoomState.Min));
			var yVal = ((1 - yPercent) * axisValues.YRange + axisValues.YMin);

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
					= XAxisCursorLabelFormatter == null ? xVal.ToString() : XAxisCursorLabelFormatter(Series.First().Values.First().XValueToImplementation(xVal));
				YCrosshairValueLabel.Text = YAxisCursorLabelFormatter == null
					? Math.Round(yVal, 2).ToString()
					: YAxisCursorLabelFormatter(Series.First().Values.First().YValueToImplementation(yVal));
			}

			var translatedMouseLoc = e.GetPosition(SeriesItemsControl);
			var hoveredChartPoint = ConvertedSeries.First().GetChartPointUnderTranslatedMouse(translatedMouseLoc.X, translatedMouseLoc.Y, SeriesItemsControl.ActualWidth, -SeriesItemsControl.Margin.Left);

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
					ConvertedSeries.First().Stroke.ColourAtPoint(hoveredChartPoint.BackingPoint.XValue, hoveredChartPoint.BackingPoint.YValue));
				XPointHighlighter.Margin = new Thickness(0, hoveredChartPoint.Y, 0, 0);
			}	

			if (IsUserSelectingRange)
			{
				SelectionRangeBorder.Width = Math.Max(HoveredPoint.X - SelectionRangeBorder.Margin.Left, 0);
			}
		}

		private void MouseCaptureGrid_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			IsZoomed = SeriesItemsControl.Margin.Left != 0 || SeriesItemsControl.Margin.Right != 0;

			var min = CurrentZoomState.Min;
			var max = CurrentZoomState.Max;
			var zoomOffset = CurrentZoomState.Offset;

			var zoomStep = e.Delta > 0 ? 0.9d : 1d / 0.9d;
			currentZoomLevel /= zoomStep;
			if (Math.Round(currentZoomLevel, 1) == 1)
			{
				ResetZoom();
			}

			var zoomCentre = e.GetPosition(Grid).X / plotAreaWidth;

			var currentRange = max - min;
			var newRange = currentRange * zoomStep;
			var diff = currentRange - newRange;
			min = min + diff * zoomCentre;
			max = max - diff * (1 - zoomCentre);

			if (Math.Round(currentZoomLevel, 1) != 1)
			{
				UpdateZoom(min, max, zoomOffset);
			}

			SetXAxisLabels(min, max);

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
				PointRangeSelected?.Invoke(this, new Tuple<IChartPoint, IChartPoint>(lowerSelection, upperSelection));
				isUserDragging = false;
				isUserPanning = false;
				userCouldBePanning = false;
				return;
			}

			PointClicked?.Invoke(this, lowerSelection);
			PointSelectionEllipse.Opacity = 1;
			ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(1, 3, TimeSpan.FromMilliseconds(300)));
			ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(1, 3, TimeSpan.FromMilliseconds(300)));
			PointSelectionEllipse.BeginAnimation(Ellipse.OpacityProperty, new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300)));
			PointSelectionEllipse.BeginAnimation(Ellipse.StrokeThicknessProperty, new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300)));

			e.Handled = true;
		}

		#endregion
	}
}
