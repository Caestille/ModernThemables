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

		private double xMin => Series.Min(x => x.Values.Min(y => y.XValue));
		private double xMax => Series.Max(x => x.Values.Max(x => x.XValue));
		private double yMin => Series.Min(x => x.Values.Min(x => x.YValue)) 
			- (Series.Max(x => x.Values.Max(x => x.YValue))
				- Series.Min(x => x.Values.Min(x => x.YValue))) * 0.1;
		private double yMax => Series.Max(x => x.Values.Max(x => x.YValue)) 
			+ (Series.Max(x => x.Values.Max(x => x.YValue))
				- Series.Min(x => x.Values.Min(x => x.YValue))) * 0.1;
		private double xDataOffset => CurrentZoomState.XOffset / plotAreaWidth * (xMax - xMin);

		public WpfChart()
		{
			InitializeComponent();
			this.Loaded += WpfChart_Loaded;

			NameScope.SetNameScope(ContextMenu, NameScope.GetNameScope(this));

			resizeTrigger = new KeepAliveTriggerService(() => RenderChart(), 100);
		}

		private void WpfChart_Loaded(object sender, RoutedEventArgs e)
		{
			RenderChart();
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
			CurrentZoomState = new ZoomState(
				Series.Min(x => x.Values.Min(y => y.XValue)),
				Series.Max(x => x.Values.Max(y => y.XValue)),
				Series.Min(x => x.Values.Min(y => y.YValue)),
				Series.Max(x => x.Values.Max(y => y.YValue)),
				0);
			SeriesItemsControl.SetBinding(MarginProperty, binding);
			IsZoomed = false;
			RenderChart();
		}

		#region Subscribe to series'

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

			chart.CurrentZoomState = new ZoomState(
				chart.Series.Min(x => x.Values.Min(y => y.XValue)),
				chart.Series.Max(x => x.Values.Max(y => y.XValue)),
				chart.Series.Min(x => x.Values.Min(y => y.YValue)),
				chart.Series.Max(x => x.Values.Max(y => y.YValue)),
				0);

			chart.Subscribe(chart.Series);
			chart.hasSetSeries = true;

			chart.RenderChart();
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
			RenderChart();
		}

		private void Series_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			RenderChart();
		}

		#endregion

		private void RenderChart()
		{
			Application.Current.Dispatcher.BeginInvoke(() =>
			{
				if (Series is null || !Series.Any()) return;

				var collection = new ObservableCollection<WpfChartSeriesViewModel>();
				foreach (var series in Series)
				{
					if (!series.Values.Any()) continue;

					var yMin = Series.Min(
						x => x.Values.Where(y => y.XValue <= xMax && y.XValue >= xMin).Min(z => z.YValue));
					var yMax = Series.Max(
						x => x.Values.Where(y => y.XValue <= xMax && y.XValue >= xMin).Max(z => z.YValue));

					if (Math.Round(currentZoomLevel, 1) != 1)
					{
						CurrentZoomState = new ZoomState(xMin, xMax, yMin, yMax, CurrentZoomState.XOffset);
					}

					SetXAxisLabels(CurrentZoomState.XMin + xDataOffset, CurrentZoomState.XMax + xDataOffset);
					SetYAxisLabels(CurrentZoomState.YMin, CurrentZoomState.YMax);

					// Force layout update so sizes are correct before rendering points
					this.Dispatcher.Invoke(DispatcherPriority.Render, delegate () { });

					var points = GetPointsForSeries(xMin, xMax - xMin, this.yMin, this.yMax - this.yMin, series);

					collection.Add(new WpfChartSeriesViewModel(points, series.Stroke, series.Fill));
					ConvertedSeries = collection;

					series.Stroke?.Reevaluate(yMax, yMin, 0, xMax, xMin, 0);
					series.Fill?.Reevaluate(yMax, yMin, 0, xMax, xMin, 0);
				}
			});
		}

		#region Calculations

		private void SetXAxisLabels(double xMin, double xMax)
		{
			var xRange = xMax - xMin;
			var xAxisItemCount = Math.Floor(plotAreaWidth / 60);
			var labels = GetXSteps(xAxisItemCount, xMin, xMax);
			var labels2 = labels.Select(x => new ValueWithHeight()
			{
				Value = XAxisFormatter == null 
					? x.ToString("MMM YY")
					: XAxisFormatter(Series.First().Values.First().XValueToImplementation(x)),
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
				Value = YAxisFormatter == null 
					? Math.Round(y, 2).ToString()
					: YAxisFormatter(Series.First().Values.First().YValueToImplementation(y)),
				Height = ((y - yMin) / yRange * plotAreaHeight) - (labels.ToList().IndexOf(y) > 0
					? (labels[labels.ToList().IndexOf(y) - 1] - yMin) / yRange * plotAreaHeight
					: 0),
			});
			YAxisLabels = new ObservableCollection<ValueWithHeight>(labels2.Reverse());
		}

		private List<double> GetXSteps(double xAxisItemsCount, double xMin, double xMax)
		{
			List<double> xVals = new();

			if (XAxisLabelIdentifier != null)
			{
				var currVal = xMin;
				while (currVal < xMax)
				{
					if (XAxisLabelIdentifier(Series.First().Values.First().XValueToImplementation(currVal)))
					{
						xVals.Add(currVal);
					}
					currVal++;
				}
			}
			else
			{
				var xRange = xMax - xMin;
				var idealStep = xRange / xAxisItemsCount;
				var min = double.MaxValue;
				var stepAtMin = 1;
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

				var currVal = xMin;
				while (currVal < xMax)
				{
					currVal += stepAtMin;
					xVals.Add(currVal);
				}
			}

			var fracOver = (int)Math.Ceiling(xVals.Count() / (decimal)Math.Round(xAxisItemsCount));

			return xVals.Where(x => xVals.IndexOf(x) % fracOver == 0).ToList();
		}

		private List<double> GetYSteps(double yAxisItemsCount, double yMin, double yMax)
		{
			List<double> yVals = new();
			
			if (YAxisLabelIdentifier != null)
			{
				var currVal = yMin;
				while (currVal < yMax)
				{
					if (XAxisLabelIdentifier(Series.First().Values.First().YValueToImplementation(currVal)))
					{
						yVals.Add(currVal);
					}
					currVal++;
				}
			}
			else
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
			}			

			return yVals;
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

		#endregion

		#region Grid events

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

		#endregion

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

			if (userCouldBePanning)
			{
				isUserPanning = true;
				if (lastMouseMovePoint != null)
				{
					var zoomOffset = CurrentZoomState.XOffset + lastMouseMovePoint.Value.X - mouseLoc.X;
					CurrentZoomState = new ZoomState(
						CurrentZoomState.XMin,
						CurrentZoomState.XMax,
						CurrentZoomState.YMin,
						CurrentZoomState.YMax,
						zoomOffset,
						false);
					SetXAxisLabels(CurrentZoomState.XMin + xDataOffset, CurrentZoomState.XMax + xDataOffset);
				}
			}
			lastMouseMovePoint = mouseLoc;

			if (DateTime.Now - timeLastUpdated < updateLimit) return;

			timeLastUpdated = DateTime.Now;
			var xPercent = mouseLoc.X / plotAreaWidth;
			var yPercent = 1 - mouseLoc.Y / plotAreaHeight;
			var xVal = CurrentZoomState.XMin 
				+ (xPercent * (CurrentZoomState.XMax - CurrentZoomState.XMin)) + xDataOffset;
			var yVal = CurrentZoomState.YMin + (yPercent * (CurrentZoomState.YMax - CurrentZoomState.YMin));

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
				XCrosshairValueLabel.Text = XAxisCursorLabelFormatter == null 
						? xVal.ToString() 
						: XAxisCursorLabelFormatter(Series.First().Values.First().XValueToImplementation(xVal));
				YCrosshairValueLabel.Text = YAxisCursorLabelFormatter == null
					? Math.Round(yVal, 2).ToString()
					: YAxisCursorLabelFormatter(Series.First().Values.First().YValueToImplementation(yVal));
			}

			var translatedMouseLoc = e.GetPosition(SeriesItemsControl);
			var hoveredChartPoint = ConvertedSeries.First().GetChartPointUnderTranslatedMouse(
				translatedMouseLoc.X,
				translatedMouseLoc.Y,
				SeriesItemsControl.ActualWidth,
				SeriesItemsControl.ActualHeight,
				-SeriesItemsControl.Margin.Left,
				-SeriesItemsControl.Margin.Top);

			if (hoveredChartPoint == null) return;

			HoveredPoint = hoveredChartPoint;

			if (IsTooltipVisible)
			{
				// Get tooltip position variables
				if (!tooltipLeft && (plotAreaWidth - mouseLoc.X) < (TooltipBorder.ActualWidth + 10)) 
					tooltipLeft = true;
				if (tooltipLeft && (mouseLoc.X) < (TooltipBorder.ActualWidth + 5)) 
					tooltipLeft = false;
				if (!tooltipTop && (plotAreaHeight - mouseLoc.Y) < (TooltipBorder.ActualHeight + 10)) 
					tooltipTop = true;
				if (tooltipTop && (mouseLoc.Y) < (TooltipBorder.ActualHeight + 5)) 
					tooltipTop = false;

				TooltipBorder.Margin = new Thickness(
					!tooltipLeft ? mouseLoc.X + 5 : mouseLoc.X - TooltipBorder.ActualWidth - 5,
					!tooltipTop ? mouseLoc.Y + 5 : mouseLoc.Y - TooltipBorder.ActualHeight - 5,
					0, 0);
				if (Series.First().TooltipLabelFormatter != null) 
					TooltipString = Series.First().TooltipLabelFormatter(
						Series.First().Values, HoveredPoint.BackingPoint);
			}

			if (IsPointIndicatorsVisible)
			{
				// Set location of items related to the hoveredPoint
				HoveredPointEllipse.Margin = new Thickness(
					hoveredChartPoint.X - 5,
					hoveredChartPoint.Y - 5,
					0, 0);
				HoveredPointEllipse.Fill = new SolidColorBrush(
					ConvertedSeries.First().Stroke.ColourAtPoint(
						hoveredChartPoint.BackingPoint.XValue,
						hoveredChartPoint.BackingPoint.YValue));
				XPointHighlighter.Margin = new Thickness(0, hoveredChartPoint.Y, 0, 0);
			}	

			if (IsUserSelectingRange)
				SelectionRangeBorder.Width = Math.Max(HoveredPoint.X - SelectionRangeBorder.Margin.Left, 0);
		}

		private void MouseCaptureGrid_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			IsZoomed = SeriesItemsControl.Margin.Left != 0 || SeriesItemsControl.Margin.Right != 0;

			var xMin = CurrentZoomState.XMin;
			var xMax = CurrentZoomState.XMax;
			var zoomOffset = CurrentZoomState.XOffset;

			var zoomStep = e.Delta > 0 ? 0.9d : 1d / 0.9d;
			currentZoomLevel /= zoomStep;
			if (Math.Round(currentZoomLevel, 1) == 1)
				ResetZoom();

			var zoomCentre = e.GetPosition(Grid).X / plotAreaWidth;

			var currXRange = xMax - xMin;
			var newXRange = currXRange * zoomStep;
			var xDiff = currXRange - newXRange;
			xMin = xMin + xDiff * zoomCentre;
			xMax = xMax - xDiff * (1 - zoomCentre);
			var yMin = Series.Min(
				x => x.Values.Where(y => y.XValue <= xMax && y.XValue >= xMin).Min(z => z.YValue));
			var yMax = Series.Max(
				x => x.Values.Where(y => y.XValue <= xMax && y.XValue >= xMin).Max(z => z.YValue));

			if (Math.Round(currentZoomLevel, 1) != 1)
			{
				CurrentZoomState = new ZoomState(xMin, xMax, yMin, yMax, zoomOffset);
			}

			SetXAxisLabels(xMin + xDataOffset, xMax + xDataOffset);
			SetYAxisLabels(CurrentZoomState.YMin, CurrentZoomState.YMax);

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
				PointRangeSelected?.Invoke(
					this, new Tuple<IChartPoint, IChartPoint>(lowerSelection, upperSelection));
				isUserDragging = false;
				isUserPanning = false;
				userCouldBePanning = false;
				return;
			}

			PointClicked?.Invoke(this, lowerSelection);
			PointSelectionEllipse.Opacity = 1;
			ScaleTransform.BeginAnimation(
				ScaleTransform.ScaleXProperty, new DoubleAnimation(1, 3, TimeSpan.FromMilliseconds(300)));
			ScaleTransform.BeginAnimation(
				ScaleTransform.ScaleYProperty, new DoubleAnimation(1, 3, TimeSpan.FromMilliseconds(300)));
			PointSelectionEllipse.BeginAnimation(
				Ellipse.OpacityProperty, new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300)));
			PointSelectionEllipse.BeginAnimation(
				Ellipse.StrokeThicknessProperty, new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300)));

			e.Handled = true;
		}

		#endregion
	}
}
