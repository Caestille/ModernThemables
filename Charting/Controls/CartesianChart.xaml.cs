using CoreUtilities.HelperClasses.Extensions;
using CoreUtilities.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ModernThemables.Charting.ViewModels.CartesianChart;
using ModernThemables.Charting.ViewModels;
using ModernThemables.Charting.Models;
using ModernThemables.Charting.Models.Brushes;
using ModernThemables.Charting.Interfaces;

namespace ModernThemables.Charting.Controls
{
    /// <summary>
    /// Interaction logic for CartesianChart.xaml
    /// </summary>
    public partial class CartesianChart : UserControl
	{
		public event EventHandler<IChartEntity>? PointClicked;
		public event EventHandler<Tuple<IChartEntity, IChartEntity>>? PointRangeSelected;

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

		private bool preventTrigger;

		private bool isMouseDown;
		private bool isUserDragging;
		private bool userCouldBePanning;
		private bool isUserPanning;
		private InternalChartEntity? lowerSelection;
		private InternalChartEntity? upperSelection;

		private static double yBuffer = 0.1;

		private double xMin;
		private double xMax;
		private double yMin;
		private double yMax;
		private double yMinExpanded;
		private double yMaxExpanded;
		private double xDataOffset;
		private double xRange => Math.Max(xMax - xMin, 1);
		private bool isSingleXPoint => xMax - xMin == 0;

		private BlockingCollection<Action> renderQueue;
		private bool renderInProgress;

		private Thread renderThread;
		private bool runRenderThread = true;

		public CartesianChart()
		{
			InitializeComponent();
			this.Loaded += WpfChart_Loaded;

			renderQueue = new BlockingCollection<Action>();
			renderThread = new Thread(new ThreadStart(() =>
			{
				while (runRenderThread)
				{
					while (renderInProgress)
					{
						Thread.Sleep(1);
					}
					if (renderQueue.Any())
						renderQueue.Take().Invoke();
					Thread.Sleep(1);
				}
			}));
			renderThread.Start();

			NameScope.SetNameScope(ContextMenu, NameScope.GetNameScope(this));

			resizeTrigger = new KeepAliveTriggerService(() => { QueueRenderChart(null, null, true); }, 100);
		}

		public void ResetZoom()
		{
			CurrentZoomState = new ZoomState(xMin, xMax, yMin, yMax, 0, yBuffer, true);
			IsZoomed = false;
		}

		private static void OnTooltipLocationSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not CartesianChart chart) return;

			chart.IsTooltipByCursor = chart.TooltipLocation == TooltipLocation.Cursor;
		}

		private static void OnSetMinMax(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not CartesianChart chart
				|| chart.Min == -1d
				|| chart.Max == -1d
				|| chart.Series == null
				|| !chart.Series.Any()
				|| chart.preventTrigger) return;

			chart.CurrentZoomState = new ZoomState(chart.Min, chart.Max, chart.yMin, chart.yMax, 0, yBuffer, true);
		}

		private static void OnSetZoomState(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not CartesianChart chart) return;
			if (chart.Series == null) return;

			bool setY = false;

			var seriesWithInRange = chart.Series.Select(x =>
			{
				var list = new List<IChartEntity>();
				list.AddRange(
					x.Values.Where(
						y => y.XValue <= chart.CurrentZoomState.XMax && y.XValue >= chart.CurrentZoomState.XMin));
				return list;
			}).Where(x => x.Any());

			if (!seriesWithInRange.Any()) return;

			var yMin = seriesWithInRange.Min(x => x.Min(y => y.YValue));
			var yMax = seriesWithInRange.Max(x => x.Max(y => y.YValue));

			var expand = (yMax - yMin) * yBuffer;
			if (Math.Abs(yMin - (chart.CurrentZoomState.YMin + expand)) > 0.0001
				|| Math.Abs(yMax - (chart.CurrentZoomState.YMax - expand)) > 0.0001)
			{
				setY = true;
				chart.CurrentZoomState = new ZoomState(
					chart.CurrentZoomState.XMin,
					chart.CurrentZoomState.XMax,
					yMin,
					yMax,
					chart.CurrentZoomState.XOffset,
					yBuffer,
					true);
			}

			if (setY) return;

			if (Math.Abs(chart.Min - chart.CurrentZoomState.XMin) > 0.0001
				|| Math.Abs(chart.Max - chart.CurrentZoomState.XMax) > 0.0001)
			{
				chart.preventTrigger = true;
				chart.Min = chart.CurrentZoomState.XMin;
				chart.Max = chart.CurrentZoomState.XMax;
				chart.preventTrigger = false;
			}

			chart.xDataOffset
				= chart.CurrentZoomState.XOffset / chart.SeriesItemsControl.ActualWidth * (chart.xMax - chart.xMin);

			_ = chart.SetXAxisLabels(
					chart.CurrentZoomState.XMin + chart.xDataOffset, chart.CurrentZoomState.XMax + chart.xDataOffset);
			_ = chart.SetYAxisLabels(chart.CurrentZoomState.YMin, chart.CurrentZoomState.YMax);

			chart.MouseOverPoint = null;
			chart.TooltipPoints.Clear();

			chart.IsZoomed = chart.SeriesItemsControl.Margin.Left != 0 || chart.SeriesItemsControl.Margin.Right != 0;
			chart.currentZoomLevel = (chart.xMax - chart.xMin) / (chart.Max - chart.Min);
		}

		private static async void OnLegendLocationSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not CartesianChart chart) return;

			switch (chart.LegendLocation)
			{
				case LegendLocation.Left:
					chart.LegendGrid.SetValue(Grid.RowProperty, 1);
					chart.LegendGrid.SetValue(Grid.ColumnProperty, 0);
					chart.LegendGrid.Visibility = Visibility.Visible;
					chart.LegendGrid.Margin = new Thickness(0, 10, 15, 0);
					chart.LegendItemsControl.ItemsPanel = (ItemsPanelTemplate)chart.Resources["StackTemplate"];
					break;
				case LegendLocation.Top:
					chart.LegendGrid.SetValue(Grid.RowProperty, 0);
					chart.LegendGrid.SetValue(Grid.ColumnProperty, 1);
					chart.LegendGrid.Visibility = Visibility.Visible;
					chart.LegendGrid.Margin = new Thickness(0, 0, 0, 15);
					chart.LegendItemsControl.ItemsPanel = (ItemsPanelTemplate)chart.Resources["WrapTemplate"];
					break;
				case LegendLocation.Right:
					chart.LegendGrid.SetValue(Grid.RowProperty, 1);
					chart.LegendGrid.SetValue(Grid.ColumnProperty, 2);
					chart.LegendGrid.Visibility = Visibility.Visible;
					chart.LegendGrid.Margin = new Thickness(15, 10, 0, 0);
					chart.LegendItemsControl.ItemsPanel = (ItemsPanelTemplate)chart.Resources["StackTemplate"];
					break;
				case LegendLocation.Bottom:
					chart.LegendGrid.SetValue(Grid.RowProperty, 2);
					chart.LegendGrid.SetValue(Grid.ColumnProperty, 1);
					chart.LegendGrid.Visibility = Visibility.Visible;
					chart.LegendGrid.Margin = new Thickness(0, 15, 0, 0);
					chart.LegendItemsControl.ItemsPanel = (ItemsPanelTemplate)chart.Resources["WrapTemplate"];
					break;
				case LegendLocation.None:
					chart.LegendGrid.Visibility = Visibility.Collapsed;
					break;
			}

			await Task.Delay(1);
			chart.QueueRenderChart(null, null, true);
		}

		#region Subscribe to series'

		private static void OnSeriesSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not CartesianChart chart) return;

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

			chart.CacheDataLimits();

			chart.CurrentZoomState = new ZoomState(
				chart.Series.Min(x => x.Values.Min(y => y.XValue)),
				chart.Series.Max(x => x.Values.Max(y => y.XValue)),
				chart.Series.Min(x => x.Values.Min(y => y.YValue)),
				chart.Series.Max(x => x.Values.Max(y => y.YValue)),
				0,
				yBuffer);

			chart.QueueRenderChart(null, null, true);
		}

		private void CacheDataLimits()
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				xMin = Series != null && Series.Where(x => x.Values.Any()).Any()
					? Series.Where(x => x.Values.Any()).Min(x => x.Values.Min(y => y.XValue)) : 0;
				xMax = Series != null && Series.Where(x => x.Values.Any()).Any()
					? Series.Where(x => x.Values.Any()).Max(x => x.Values.Max(y => y.XValue)) : 0;
				yMin = Series != null && Series.Where(x => x.Values.Any()).Any()
					? Series.Where(x => x.Values.Any()).Min(x => x.Values.Min(y => y.YValue)) : 0;
				yMax = Series != null && Series.Where(x => x.Values.Any()).Any()
					? Series.Where(x => x.Values.Any()).Max(x => x.Values.Max(y => y.YValue)) : 0;
				yMinExpanded = Series != null && Series.Where(x => x.Values.Any()).Any()
					? Series.Where(x => x.Values.Any()).Min(x => x.Values.Min(y => y.YValue)) - (yMax - yMin) * yBuffer : 0;
				yMaxExpanded = Series != null && Series.Where(x => x.Values.Any()).Any()
					? Series.Where(x => x.Values.Any()).Max(x => x.Values.Max(y => y.YValue)) + (yMax - yMin) * yBuffer : 0;
			});
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
				QueueRenderChart(null, null, true);
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

			CacheDataLimits();

			QueueRenderChart(newItems, oldItems);
		}

		private async void Series_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			CacheDataLimits();
			if (sender is ISeries series)
			{
				var list = new List<ISeries>() { series };
				QueueRenderChart(list, list);
			}
		}

		#endregion

		private void QueueRenderChart(
			IEnumerable<ISeries>? addedSeries, IEnumerable<ISeries>? removedSeries, bool invalidateAll = false)
		{
			renderQueue.Add(new Action(() => RenderChart(addedSeries, removedSeries, invalidateAll)));
		}

		private void RenderChart(
			IEnumerable<ISeries>? addedSeries, IEnumerable<ISeries>? removedSeries, bool invalidateAll = false)
		{
			Application.Current.Dispatcher.Invoke(async () =>
			{
				renderInProgress = true;
				var collection = InternalSeries.Clone().ToList();

				if (invalidateAll)
				{
					collection.Clear();
				}
				else
				{
					foreach (var series in (removedSeries ?? new List<ISeries>()))
					{
						collection.Remove(collection.First(x => x.Identifier == series.Identifier));
					}
				}

				foreach (var series in invalidateAll
					? Series ?? new ObservableCollection<ISeries>()
					: addedSeries ?? new List<ISeries>())
				{
					var points = await GetPointsForSeries(
						xMin, xRange, yMinExpanded, yMaxExpanded - yMinExpanded, series);

					var matchingSeries = InternalSeries.FirstOrDefault(x => x.Identifier == series.Identifier);

					collection.Add(new InternalPathSeriesViewModel(
						series.Name,
						series.Identifier,
						points,
						invalidateAll
							? matchingSeries != null
								? matchingSeries.Stroke
								: series.Stroke ?? new SolidBrush(ColorExtensions.RandomColour(50))
							: series.Stroke ?? new SolidBrush(ColorExtensions.RandomColour(50)),
						invalidateAll
							? matchingSeries != null ? matchingSeries.Fill : series.Fill
							: series.Fill,
						yBuffer,
						series.TooltipLabelFormatter));

					if (!series.Values.Any()) continue;

					var seriesYMin = series.Values.Min(z => z.YValue);
					var seriesYMax = series.Values.Max(z => z.YValue);

					series.Stroke?.Reevaluate(seriesYMax, seriesYMin, 0, xMax, xMin, 0);
					series.Fill?.Reevaluate(seriesYMax, seriesYMin, 0, xMax, xMin, 0);
				}

				if (CurrentZoomState.XMax == 0 && CurrentZoomState.XMin == 0)
					ResetZoom();

				foreach (var series in collection)
				{
					var matchingSeries = Series.FirstOrDefault(x => x.Identifier == series.Identifier);

					if (matchingSeries == null) continue;

					var points = await GetPointsForSeries(
						xMin, xRange, yMinExpanded, yMaxExpanded - yMinExpanded, matchingSeries);
					series.UpdatePoints(points);
				}

				InternalSeries = new ObservableCollection<InternalPathSeriesViewModel>(collection);

				foreach (var series in InternalSeries)
				{
					series.ResizeTrigger = !series.ResizeTrigger;
				}

				this.Dispatcher.Invoke(DispatcherPriority.Render, delegate () { });

				if (IsZoomed)
				{
					CurrentZoomState = new ZoomState(
						CurrentZoomState.XMin, CurrentZoomState.XMax, yMin, yMax, CurrentZoomState.XOffset, yBuffer, true);
				}
				else
				{
					CurrentZoomState = new ZoomState(xMin, xMax, yMin, yMax, 0, yBuffer, true);
				}

				renderInProgress = false;
			});
		}

		#region Calculations

		private async Task SetXAxisLabels(double xMin, double xMax)
		{
			if (!HasGotData()) return;

			var xRange = xMax - xMin;
			var xAxisItemCount = Math.Floor(plotAreaWidth / 60);
			var labels = await GetXSteps(xAxisItemCount, xMin, xMax);
			var labels2 = labels.Select(x => new AxisLabel()
			{
				Value = XAxisFormatter == null
					? x.ToString()
					: XAxisFormatter(Series.First().Values.First().XValueToImplementation(x)),
				Height = ((x - xMin) / xRange * plotAreaWidth) - (labels.ToList().IndexOf(x) > 0
					? (labels[labels.ToList().IndexOf(x) - 1] - xMin) / xRange * plotAreaWidth
					: 0),
			});
			XAxisLabels = new ObservableCollection<AxisLabel>(labels2);
			if (isSingleXPoint)
			{
				XAxisLabels = new ObservableCollection<AxisLabel>()
				{
					new AxisLabel(
						XAxisFormatter == null
							? xMin.ToString()
							: XAxisFormatter(Series.First().Values.First().XValueToImplementation(xMin)),
						plotAreaWidth / 2)
				};
			}
		}

		private async Task SetYAxisLabels(double yMin, double yMax)
		{
			if (!HasGotData()) return;

			var yRange = yMax - yMin;
			var yAxisItemsCount = Math.Max(1, Math.Floor(plotAreaHeight / 50));
			var labels = (await GetYSteps(yAxisItemsCount, yMin, yMax)).ToList();
			var labels2 = labels.Select(y => new AxisLabel()
			{
				Value = YAxisFormatter == null
					? Math.Round(y, 2).ToString()
					: YAxisFormatter(Series.First().Values.First().YValueToImplementation(y)),
				Height = ((y - yMin) / yRange * plotAreaHeight) - (labels.ToList().IndexOf(y) > 0
					? (labels[labels.ToList().IndexOf(y) - 1] - yMin) / yRange * plotAreaHeight
					: 0),
			});
			YAxisLabels = new ObservableCollection<AxisLabel>(labels2.Reverse());
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
					if (YAxisLabelIdentifier(Series.First().Values.First().YValueToImplementation(currVal)))
					{
						yVals.Add(currVal);
					}
					currVal++;
				}
			}
			else
			{
				await Task.Run(() =>
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
				});
			}

			return yVals;
		}

		private async Task<List<InternalChartEntity>> GetPointsForSeries(
			double xMin, double xRange, double yMin, double yRange, ISeries? series)
		{
			if (series == null) return new List<InternalChartEntity>();

			return await Task.Run(() =>
			{
				List<InternalChartEntity> points = new();
				foreach (var point in series.Values)
				{
					double x = (double)(point.XValue - xMin) / (double)xRange * (double)plotAreaWidth;
					double y = plotAreaHeight - (point.YValue - yMin) / yRange * plotAreaHeight;
					points.Add(new InternalChartEntity(x, y, point));
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

			//IsZoomed = SeriesItemsControl.Margin.Left != 0 || SeriesItemsControl.Margin.Right != 0;

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
				}
			}
			lastMouseMovePoint = mouseLoc;
			#endregion

			if (DateTime.Now - timeLastUpdated < updateLimit || isUserPanning) return;

			timeLastUpdated = DateTime.Now;
			var xVal = CurrentZoomState.XMin
				+ (mouseLoc.X / plotAreaWidth * (CurrentZoomState.XMax - CurrentZoomState.XMin)) + xDataOffset;
			var yVal = CurrentZoomState.YMin
				+ ((1 - mouseLoc.Y / plotAreaHeight) * (CurrentZoomState.YMax - CurrentZoomState.YMin));

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
					Math.Max(InternalSeries.Max(x => x.Data.Max(y => y.X)) - InternalSeries.Min(x => x.Data.Min(y => y.X)), 1),
					Math.Max(InternalSeries.Max(x => x.Data.Max(y => y.Y)) - InternalSeries.Min(x => x.Data.Min(y => y.Y)), 1),
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
							InternalSeries.Max(
								x => x.Data.Max(y => y.X)) - InternalSeries.Min(x => x.Data.Min(y => y.X)),
							translatedMouseLoc.X,
							SeriesItemsControl.ActualWidth)) continue;

				if (isSingleXPoint) hoveredChartPoint.X += (plotAreaWidth / 2);

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
					plotAreaHeight));
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
				nearestPoint.IsNearest = pointsUnderMouse.Count() > 1;

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

			if (Math.Round(currentZoomLevel, 1) != 1)
			{
				CurrentZoomState = new ZoomState(xMin, xMax, yMin, yMax, zoomOffset, yBuffer);
			}

			IsZoomed = SeriesItemsControl.Margin.Left != 0 || SeriesItemsControl.Margin.Right != 0;
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
					? new Tuple<IChartEntity, IChartEntity>(lowerSelection.BackingPoint, upperSelection.BackingPoint)
					: new Tuple<IChartEntity, IChartEntity>(upperSelection.BackingPoint, lowerSelection.BackingPoint);
				PointRangeSelected?.Invoke(this, eventData);
				isUserDragging = false;
				isUserPanning = false;
				userCouldBePanning = false;
				return;
			}

			if (TooltipPoints != null && TooltipPoints.Any())
			{
				var tooltipPoint = TooltipPoints.Count() == 1 
					? TooltipPoints.First() : TooltipPoints.FirstOrDefault(x => x.IsNearest);

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

		private void WpfChart_Loaded(object sender, RoutedEventArgs e)
		{
			this.Loaded -= WpfChart_Loaded;
			Application.Current.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
			OnLegendLocationSet(this, new DependencyPropertyChangedEventArgs());
		}

		private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
		{
			resizeTrigger.Stop();
			runRenderThread = false;
			foreach (var series in subscribedSeries)
			{
				series.PropertyChanged -= Series_PropertyChanged;
			}
		}
	}
}
