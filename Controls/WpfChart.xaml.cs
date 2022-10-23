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
using ModernThemables.Interfaces;
using CoreUtilities.HelperClasses.Extensions;
using System.Threading.Tasks;
using System.Threading;
using ModernThemables.ViewModels.WpfChart;
using System.Diagnostics;

namespace ModernThemables.Controls
{
	/// <summary>
	/// Interaction logic for WpfChart.xaml
	/// </summary>
	public partial class WpfChart : UserControl
	{
		public event EventHandler<IChartPoint>? PointClicked;
		public event EventHandler<Tuple<IChartPoint, IChartPoint>>? PointRangeSelected;

		private double plotAreaHeight => DrawableChartSectionBorder.ActualHeight;
		private double plotAreaWidth => DrawableChartSectionBorder.ActualWidth;
		private DateTime timeLastUpdated;
		private TimeSpan updateLimit = TimeSpan.FromMilliseconds(1000 / 60d);
		private bool hasSetSeries;
		private List<ISeries> subscribedSeries = new();

		private KeepAliveTriggerService resizeTrigger;

		private Point? lastMouseMovePoint;
		private double currentZoomLevel = 1;
		private bool ignoreNextMouseMove;

		private bool tooltipLeft;
		private bool tooltipTop = true;

		private bool isMouseDown;
		private bool isUserDragging;
		private bool userCouldBePanning;
		private bool isUserPanning;
		private InternalChartPoint? lowerSelection;
		private InternalChartPoint? upperSelection;

		private static double yBuffer => 0.1;

		private CancellationTokenSource tokenSource = new CancellationTokenSource();

		private double xMin => Series != null && Series.Where(x => x.Values.Any()).Any()
			? Series.Where(x => x.Values.Any()).Min(x => x.Values.Min(y => y.XValue)) : 0;
		private double xMax => Series != null && Series.Where(x => x.Values.Any()).Any()
			? Series.Where(x => x.Values.Any()).Max(x => x.Values.Max(y => y.XValue)) : 0;
		private double yMin => Series != null && Series.Where(x => x.Values.Any()).Any()
			? Series.Where(x => x.Values.Any()).Min(x => x.Values.Min(y => y.YValue)) : 0;
		private double yMax => Series != null && Series.Where(x => x.Values.Any()).Any()
			? Series.Where(x => x.Values.Any()).Max(x => x.Values.Max(y => y.YValue)) : 0;
		private double yMinExpanded => Series != null && Series.Where(x => x.Values.Any()).Any()
			? Series.Where(x => x.Values.Any()).Min(x => x.Values.Min(y => y.YValue)) - (yMax - yMin) * yBuffer : 0;
		private double yMaxExpanded => Series != null && Series.Where(x => x.Values.Any()).Any()
			? Series.Where(x => x.Values.Any()).Max(x => x.Values.Max(y => y.YValue)) + (yMax - yMin) * yBuffer : 0;
		private double xDataOffset => CurrentZoomState.XOffset / plotAreaWidth * (xMax - xMin);

		public WpfChart()
		{
			InitializeComponent();
			this.Loaded += WpfChart_Loaded;

			NameScope.SetNameScope(ContextMenu, NameScope.GetNameScope(this));

			resizeTrigger = new KeepAliveTriggerService(async () => await RenderChart(null, null, true), 100);
		}

		private void WpfChart_Loaded(object sender, RoutedEventArgs e)
		{
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
			CurrentZoomState = new ZoomState(xMin, xMax, yMin, yMax, 0, yBuffer, true);
			IsZoomed = false;
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

			chart.Subscribe(chart.Series);
			chart.hasSetSeries = true;

			if (!chart.Series.Any() || !chart.Series.Any(x => x.Values.Any())) return;

			chart.CurrentZoomState = new ZoomState(
				chart.Series.Min(x => x.Values.Min(y => y.XValue)),
				chart.Series.Max(x => x.Values.Max(y => y.XValue)),
				chart.Series.Min(x => x.Values.Min(y => y.YValue)),
				chart.Series.Max(x => x.Values.Max(y => y.YValue)),
				0,
				yBuffer);

			_ = chart.RenderChart(null, null, true);
		}

		private static void OnTooltipLocationSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not WpfChart chart) return;

			chart.IsTooltipByCursor = chart.TooltipLocation == TooltipLocation.Cursor;
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
		}

		private async void Series_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				await RenderChart(null, null, true);
				return;
			}

			var oldItems = new List<ISeries>();
			if ((e.Action == NotifyCollectionChangedAction.Replace || e.Action == NotifyCollectionChangedAction.Remove)
				&& e.OldItems != null)
			{
				foreach (ISeries series in e.OldItems)
				{
					series.PropertyChanged -= Series_PropertyChanged;
					oldItems.Add(series);
					subscribedSeries.Remove(series);
				}
			}

			var newItems = new List<ISeries>();
			if ((e.Action == NotifyCollectionChangedAction.Replace || e.Action == NotifyCollectionChangedAction.Add)
				&& e.NewItems != null)
			{
				foreach (ISeries series in e.NewItems)
				{
					series.PropertyChanged += Series_PropertyChanged;
					newItems.Add(series);
					subscribedSeries.Add(series);
				}
			}

			if (Series != null && Series.Any() && Series.Any(x => x.Values.Any()))
			{
				CurrentZoomState = new ZoomState(
					Series.Min(x => x.Values.Min(y => y.XValue)),
					Series.Max(x => x.Values.Max(y => y.XValue)),
					Series.Min(x => x.Values.Min(y => y.YValue)),
					Series.Max(x => x.Values.Max(y => y.YValue)),
					0,
					yBuffer);
			}

			await RenderChart(newItems, oldItems);
		}

		private async void Series_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (sender is ISeries series)
			{
				var list = new List<ISeries>() { series };
				await RenderChart(list, list);
			}
		}

		#endregion

		private async Task RenderChart(
			IEnumerable<ISeries>? addedSeries, IEnumerable<ISeries>? removedSeries, bool invalidateAll = false)
		{
			tokenSource.Cancel();
			tokenSource = new CancellationTokenSource();

			try
			{
				await Task.Run(async () =>
				{
					await Application.Current.Dispatcher.InvokeAsync(async () =>
					{
						_ = SetXAxisLabels(CurrentZoomState.XMin + xDataOffset, CurrentZoomState.XMax + xDataOffset);
						_ = SetYAxisLabels(CurrentZoomState.YMin, CurrentZoomState.YMax);

						var collection = new List<InternalSerieViewModel>();
						foreach (var series in invalidateAll ? Series : (addedSeries ?? new List<ISeries>()))
						{
							if (!series.Values.Any()) continue;

							var points = await GetPointsForSeries(
								xMin, xMax - xMin, yMinExpanded, yMaxExpanded - yMinExpanded, series);

							collection.Add(new InternalSerieViewModel(
								series.Guid, points, series.Stroke, series.Fill, yBuffer, series.TooltipLabelFormatter));

							var seriesYMin = series.Values.Min(z => z.YValue);
							var seriesYMax = series.Values.Max(z => z.YValue);

							series.Stroke?.Reevaluate(seriesYMax, seriesYMin, 0, xMax, xMin, 0);
							series.Fill?.Reevaluate(seriesYMax, seriesYMin, 0, xMax, xMin, 0);
						}

						if (Series.Any() && Math.Round(currentZoomLevel, 1) != 1)
						{
							var zoomYMin = Series.Min(
							x => x.Values.Where(y => y.XValue <= xMax && y.XValue >= xMin).Min(z => z.YValue));
							var zoomYMax = Series.Max(
								x => x.Values.Where(y => y.XValue <= xMax && y.XValue >= xMin).Max(z => z.YValue));
							CurrentZoomState
								= new ZoomState(xMin, xMax, zoomYMin, zoomYMax, CurrentZoomState.XOffset, yBuffer);
						}

						// Force layout update so sizes are correct before rendering points
						this.Dispatcher.Invoke(DispatcherPriority.Render, delegate () { });

						if (invalidateAll)
						{
							InternalSeries.Clear();
						}
						else
						{
							foreach (var series in (removedSeries ?? new List<ISeries>()))
							{
								InternalSeries.Remove(InternalSeries.First(x => x.Identifier == series.Guid));
							}
						}

						foreach (var series in InternalSeries)
						{
							var points = await GetPointsForSeries(
								xMin, xMax - xMin, yMinExpanded, yMaxExpanded - yMinExpanded, Series.First(
										x => x.Values.Count == series.Data.Count()));
							series.UpdatePoints(points);
						}

						foreach (var series in collection)
						{
							InternalSeries.Add(series);
						}

						foreach (var series in InternalSeries)
						{
							series.ResizeTrigger = !series.ResizeTrigger;
						}
					});
				}).AsCancellable(tokenSource.Token);
			}
			catch (TaskCanceledException) { }
		}

		#region Calculations

		private async Task SetXAxisLabels(double xMin, double xMax)
		{
			if (!HasGotData()) return;

			var xRange = xMax - xMin;
			var xAxisItemCount = Math.Floor(plotAreaWidth / 60);
			var labels = await GetXSteps(xAxisItemCount, xMin, xMax);
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

		private async Task SetYAxisLabels(double yMin, double yMax)
		{
			if (!HasGotData()) return;

			var yRange = yMax - yMin;
			var yAxisItemsCount = Math.Max(1, Math.Floor(plotAreaHeight / 50));
			var labels = (await GetYSteps(yAxisItemsCount, yMin, yMax)).ToList();
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

		private async Task<List<double>> GetXSteps(double xAxisItemsCount, double xMin, double xMax)
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
				await Task.Run(() =>
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
				});
			}

			var fracOver = (int)Math.Ceiling(xVals.Count() / (decimal)Math.Round(xAxisItemsCount));

			return xVals.Where(x => xVals.IndexOf(x) % fracOver == 0).ToList();
		}

		private async Task<List<double>> GetYSteps(double yAxisItemsCount, double yMin, double yMax)
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
				await Task.Run(() => { 
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
				});
			}			

			return yVals;
		}

		private async Task<List<InternalChartPoint>> GetPointsForSeries(
			double xMin, double xRange, double yMin, double yRange, ISeries series)
		{
			return await Task.Run(() =>
			{
				List<InternalChartPoint> points = new();
				foreach (var point in series.Values)
				{
					double x = (double)(point.XValue - xMin) / (double)xRange * (double)plotAreaWidth;
					double y = plotAreaHeight - (point.YValue - yMin) / yRange * plotAreaHeight;
					points.Add(new InternalChartPoint(x, y, point));
				}
				return points;
			});
		}

		#endregion

		#region Grid events

		private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			resizeTrigger.Refresh();
		}

		private void Grid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			if (!HasGotData()) { e.Handled = true; return; }

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
			if (!HasGotData() || ignoreNextMouseMove)
			{
				ignoreNextMouseMove = false;
				return;
			}

			IsZoomed = SeriesItemsControl.Margin.Left != -1 || SeriesItemsControl.Margin.Right != 0;

			var mouseLoc = e.GetPosition(Grid);
			var translatedMouseLoc = e.GetPosition(SeriesItemsControl);

			if (isMouseDown)
			{
				isUserDragging = true;
				IsUserSelectingRange = !(userCouldBePanning || isUserPanning);
			}

			#region Chart panning
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
						yBuffer,
						false);
					_ = SetXAxisLabels(CurrentZoomState.XMin + xDataOffset, CurrentZoomState.XMax + xDataOffset);
				}
			}
			lastMouseMovePoint = mouseLoc;
			#endregion

			if (DateTime.Now - timeLastUpdated < updateLimit) return;

			timeLastUpdated = DateTime.Now;
			var xVal = CurrentZoomState.XMin 
				+ (mouseLoc.X / plotAreaWidth * (CurrentZoomState.XMax - CurrentZoomState.XMin)) + xDataOffset;
			var yVal = CurrentZoomState.YMin + ((1 - mouseLoc.Y / plotAreaHeight) * (CurrentZoomState.YMax - CurrentZoomState.YMin));

			#region Crosshairs
			if (IsCrosshairVisible)
			{
				// Move crosshairs
				XCrosshair.Margin = new Thickness(0, mouseLoc.Y, 0, 0);
				YCrosshair.Margin = new Thickness(mouseLoc.X, 0, 0, 0);
			}
			#endregion

			#region Axis value indicators
			if (IsAxisIndicatorsVisible)
			{
				XCrosshairValueDisplay.Margin = new Thickness(mouseLoc.X - 50, 0, -100, -XAxisRow.ActualHeight);
				YCrosshairValueDisplay.Margin = new Thickness(-YAxisColumn.ActualWidth, mouseLoc.Y - 10, 0, 0);

				// Set value displays
				XCrosshairValueLabel.Text = XAxisCursorLabelFormatter == null
						? xVal.ToString() 
						: XAxisCursorLabelFormatter(Series.First().Values.First().XValueToImplementation(xVal));
				YCrosshairValueLabel.Text = YAxisCursorLabelFormatter == null
					? Math.Round(yVal, 2).ToString()
					: YAxisCursorLabelFormatter(Series.First().Values.First().YValueToImplementation(yVal));
			}
			#endregion

			#region Find points under mouse
			var pointsUnderMouse = new List<TooltipPointViewModel>();
			foreach (var series in InternalSeries)
			{
				var hoveredChartPoint = series.GetChartPointUnderTranslatedMouse(
					InternalSeries.Max(x => x.Data.Max(y => y.X)) - InternalSeries.Min(x => x.Data.Min(y => y.X)),
					InternalSeries.Max(x => x.Data.Max(y => y.Y)) - InternalSeries.Min(x => x.Data.Min(y => y.Y)),
					translatedMouseLoc.X,
					translatedMouseLoc.Y,
					SeriesItemsControl.ActualWidth,
					SeriesItemsControl.ActualHeight,
					-SeriesItemsControl.Margin.Left,
					-SeriesItemsControl.Margin.Top,
					yBuffer);

				if (hoveredChartPoint == null 
					|| !CurrentZoomState.IsPointInBounds(
							hoveredChartPoint.BackingPoint.XValue,
							hoveredChartPoint.BackingPoint.YValue)
					|| !series.IsTranslatedMouseInBounds(
							InternalSeries.Max(x => x.Data.Max(y => y.X)) - InternalSeries.Min(x => x.Data.Min(y => y.X)),
							translatedMouseLoc.X,
							SeriesItemsControl.ActualWidth)) continue;

				pointsUnderMouse.Add(new TooltipPointViewModel(
					hoveredChartPoint,
					new Thickness(hoveredChartPoint.X - 5, hoveredChartPoint.Y - 5, 0, 0),
					new SolidColorBrush(series.Stroke != null 
						? series.Stroke.ColourAtPoint(
							hoveredChartPoint.BackingPoint.XValue, hoveredChartPoint.BackingPoint.YValue)
						: Colors.Red),
					series.TooltipLabelFormatter != null
						? series.TooltipLabelFormatter(
							series.Data.Select(x => x.BackingPoint), hoveredChartPoint.BackingPoint)
						: hoveredChartPoint.BackingPoint.YValue.ToString(),
					plotAreaHeight
					));
			}
			#endregion

			#region Tooltip
			var nearestPoint = pointsUnderMouse.FirstOrDefault(
				x => Math.Abs(x.Point.Y - mouseLoc.Y)
					== pointsUnderMouse.Min(x => Math.Abs(x.Point.Y - mouseLoc.Y)));

			switch (TooltipFindingStrategy)
			{
				case TooltipFindingStrategy.None:
					TooltipPoints.Clear();
					break;
				case TooltipFindingStrategy.NearestXAllY:
					TooltipPoints = new ObservableCollection<TooltipPointViewModel>(pointsUnderMouse);
					break;
				case TooltipFindingStrategy.NearestXNearestY:
					if (nearestPoint != null)
						TooltipPoints = new ObservableCollection<TooltipPointViewModel>() { nearestPoint };
					else
						TooltipPoints.Clear();
					break;
				case TooltipFindingStrategy.NearestXWithinThreshold:
					TooltipPoints = new ObservableCollection<TooltipPointViewModel>(
						pointsUnderMouse.Where(
							x => Math.Abs(x.Point.X - mouseLoc.X) <= TooltipLocationThreshold));
					break;
			}

			if (nearestPoint != null)
				nearestPoint.IsNearest = true;

			if (IsTooltipVisible && TooltipLocation == TooltipLocation.Cursor)
			{
				// Get tooltip position variables
				if (!tooltipLeft && (plotAreaWidth - mouseLoc.X) < (TooltipItemsControl.ActualWidth + 10))
					tooltipLeft = true;
				if (tooltipLeft && (mouseLoc.X) < (TooltipItemsControl.ActualWidth + 5))
					tooltipLeft = false;
				if (!tooltipTop && (plotAreaHeight - mouseLoc.Y) < (TooltipItemsControl.ActualHeight + 10))
					tooltipTop = true;
				if (tooltipTop && (mouseLoc.Y) < (TooltipItemsControl.ActualHeight + 5))
					tooltipTop = false;

				TooltipItemsControl.Margin = new Thickness(
					!tooltipLeft ? mouseLoc.X + 5 : mouseLoc.X - TooltipItemsControl.ActualWidth - 5,
					!tooltipTop ? mouseLoc.Y + 5 : mouseLoc.Y - TooltipItemsControl.ActualHeight - 5,
					0, 0);
			}
			#endregion

			#region Selected range
			MouseOverPoint = pointsUnderMouse.FirstOrDefault(
				x => Math.Abs(x.Point.X - mouseLoc.X) == 
					pointsUnderMouse.Min(x => Math.Abs(x.Point.X - mouseLoc.X)))?.Point;

			if (IsUserSelectingRange && MouseOverPoint != null && lowerSelection != null)
			{
				var negative = MouseOverPoint.X < lowerSelection.X;
				var margin = SelectionRangeBorder.Margin;
				margin.Left = negative ? MouseOverPoint.X : lowerSelection.X;
				SelectionRangeBorder.Margin = margin;
				SelectionRangeBorder.Width = negative
					? Math.Max(lowerSelection.X - MouseOverPoint.X, 0)
					: Math.Max(MouseOverPoint.X - lowerSelection.X, 0);
			}
			#endregion
		}

		private void MouseCaptureGrid_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (!HasGotData()) return;

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
			var seriesWithInRange = Series.Select(x => 
				{ 
					var list = new List<IChartPoint>();
					list.AddRange(x.Values.Where(y => y.XValue <= xMax && y.XValue >= xMin));
					return list; 
				}).Where(x => x.Any());
			var yMin = seriesWithInRange.Min(x => x.Min(y => y.YValue));
			var yMax = seriesWithInRange.Max(x => x.Max(y => y.YValue));

			if (Math.Round(currentZoomLevel, 1) != 1)
			{
				CurrentZoomState = new ZoomState(xMin, xMax, yMin, yMax, zoomOffset, yBuffer);
			}

			_ = SetXAxisLabels(xMin + xDataOffset, xMax + xDataOffset);
			_ = SetYAxisLabels(CurrentZoomState.YMin, CurrentZoomState.YMax);

			MouseOverPoint = null;
			TooltipPoints.Clear();

			IsZoomed = SeriesItemsControl.Margin.Left != -1 || SeriesItemsControl.Margin.Right != 0;
		}

		private void MouseCaptureGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!HasGotData()) return;

			isMouseDown = true;
			if (e.ChangedButton == MouseButton.Left && MouseOverPoint != null)
			{
				e.Handled = true;
				lowerSelection = MouseOverPoint;
				SelectionRangeBorder.Margin = new Thickness(MouseOverPoint.X, 0, 0, 0);
			}
			else if (e.ChangedButton == MouseButton.Right)
			{
				userCouldBePanning = true;
			}
		}

		private void MouseCaptureGrid_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (!HasGotData()) return;

			isMouseDown = false;
			IsUserSelectingRange = false;

			if (e.ChangedButton != MouseButton.Left)
			{
				isUserDragging = false;
				return;
			}

			if (isUserDragging && lowerSelection != null && MouseOverPoint != null)
			{
				upperSelection = MouseOverPoint;
				var eventData = upperSelection.X > lowerSelection.X
					? new Tuple<IChartPoint, IChartPoint>(lowerSelection.BackingPoint, upperSelection.BackingPoint)
					: new Tuple<IChartPoint, IChartPoint>(upperSelection.BackingPoint, lowerSelection.BackingPoint);
				PointRangeSelected?.Invoke(this, eventData);
				isUserDragging = false;
				isUserPanning = false;
				userCouldBePanning = false;
				return;
			}

			if (TooltipPoints != null && TooltipPoints.Any())
			{
				var tooltipPoint = TooltipPoints.FirstOrDefault(x => x.IsNearest);

				if (tooltipPoint != null)
				{
					tooltipPoint.WasClicked = true;
					tooltipPoint.WasClicked = false;
					if (lowerSelection != null)
						PointClicked?.Invoke(this, lowerSelection.BackingPoint);
					tooltipPoint.WasClicked = false;
				}
			}

			e.Handled = true;
		}

		#endregion

		private bool HasGotData()
		{
			HasData = Series != null && Series.Any(x => x.Values.Any());
			return HasData;
		}

		private void YAxisItemsControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			ignoreNextMouseMove = true;
		}
	}
}
