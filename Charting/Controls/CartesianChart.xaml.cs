using CoreUtilities.HelperClasses.Extensions;
using CoreUtilities.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ModernThemables.Charting.Services;

namespace ModernThemables.Charting.Controls
{
	/// <summary>
	/// Interaction logic for CartesianChart.xaml
	/// </summary>
	public partial class CartesianChart : UserControl
	{
		public event EventHandler<IChartEntity>? PointClicked;
		public event EventHandler<Tuple<IChartEntity, IChartEntity>>? PointRangeSelected;

		private KeepAliveTriggerService resizeTrigger;

		private double currentZoomLevel = 1;

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

		private SeriesWatcherService seriesWatcher;

		private bool hasData => Series != null && Series.Any(x => x.Values.Any());
		private double plotAreaHeight => TooltipControl.ActualHeight;
		private double plotAreaWidth => TooltipControl.ActualWidth;

		public CartesianChart()
		{
			InitializeComponent();
			this.Loaded += WpfChart_Loaded;

			seriesWatcher = new SeriesWatcherService((added, removed, invalidateAll) =>
			{
				CacheDataLimits();
				QueueRenderChart(added, removed, invalidateAll);
			});

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

			TooltipGetterFunc = new Func<Point, IEnumerable<TooltipViewModel>>((point =>
			{
				var pointsUnderMouse = GetPointsUnderMouse(point);

				var	tooltips = pointsUnderMouse.Select(x => new TooltipViewModel(
					x.point,
					new SolidColorBrush(x.series.Stroke != null
						? x.series.Stroke.ColourAtPoint(
							x.point.BackingPoint.XValue, x.point.BackingPoint.YValue)
						: Colors.Red),
					"", "", "")
					{
						TooltipTemplate = this.TooltipTemplate,
						TemplatedContent = TooltipContentGetter != null
							? TooltipContentGetter(x.series.Data.Select(x => x.BackingPoint), x.point.BackingPoint)
							: null
					}).ToList();				

				switch (TooltipFindingStrategy)
				{
					case TooltipFindingStrategy.None:
						tooltips.Clear();
						break;
					case TooltipFindingStrategy.NearestXNearestY:
						var nearestPoint = tooltips.FirstOrDefault(
							x => Math.Abs(x.LocationY - point.Y)
								== tooltips.Min(x => Math.Abs(x.LocationY - point.Y)));
						if (nearestPoint != null)
							tooltips = new List<TooltipViewModel>() { nearestPoint };
						else
							tooltips.Clear();
						break;
					case TooltipFindingStrategy.NearestXWithinThreshold:
						tooltips = new List<TooltipViewModel>(
							tooltips.Where(
								x => Math.Abs(x.LocationX - point.X) <= TooltipLocationThreshold));
						break;
				}

				return tooltips;
			}));

			resizeTrigger = new KeepAliveTriggerService(() => { QueueRenderChart(null, null, true); }, 100);
		}

		public void ResetZoom()
		{
			CurrentZoomState = new ZoomState(xMin, xMax, yMin, yMax, 0, yBuffer, true);
			IsZoomed = false;
		}

		private static void OnSetZoomState(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not CartesianChart chart || chart.Series == null) return;

			bool doSetY = false;

			var seriesWithValuesInZoomLimits = chart.Series
				.Select(x => x.Values)
				.Where(x => x.Any(chart.CurrentZoomState.IsPointInBounds));

			if (!seriesWithValuesInZoomLimits.Any()) return;

			var yMin = seriesWithValuesInZoomLimits.Min(x => x.Min(y => y.YValue));
			var yMax = seriesWithValuesInZoomLimits.Max(x => x.Max(y => y.YValue));

			var bufferSize = (yMax - yMin) * yBuffer;
			if (Math.Abs(yMin - (chart.CurrentZoomState.YMin + bufferSize)) > 0.0001
				|| Math.Abs(yMax - (chart.CurrentZoomState.YMax - bufferSize)) > 0.0001)
			{
				doSetY = true;
				chart.CurrentZoomState = new ZoomState(
					chart.CurrentZoomState.XMin,
					chart.CurrentZoomState.XMax,
					yMin,
					yMax,
					chart.CurrentZoomState.XOffset,
					yBuffer,
					true);
			}

			if (doSetY) return;

			chart.xDataOffset
				= chart.CurrentZoomState.XOffset / chart.SeriesItemsControl.ActualWidth * (chart.xMax - chart.xMin);

			_ = chart.SetXAxisLabels(
					chart.CurrentZoomState.XMin + chart.xDataOffset, chart.CurrentZoomState.XMax + chart.xDataOffset);
			_ = chart.SetYAxisLabels(chart.CurrentZoomState.YMin, chart.CurrentZoomState.YMax);

			chart.IsZoomed = chart.SeriesItemsControl.Margin.Left != 0 || chart.SeriesItemsControl.Margin.Right != 0;
		}

		private static async void OnLegendLocationSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not CartesianChart chart) return;

			var properties = ChartHelper.GetLegendProperties(chart.LegendLocation);

			chart.LegendGrid.SetValue(Grid.RowProperty, properties.row);
			chart.LegendGrid.SetValue(Grid.ColumnProperty, properties.column);
			chart.LegendGrid.Visibility = properties.visibility;
			chart.LegendGrid.Margin = properties.margin;

			await Task.Delay(1);
			chart.QueueRenderChart(null, null, true);
		}

		private static void OnSeriesSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not CartesianChart chart) return;

			if (chart.Series.Any())
			{
				chart.CurrentZoomState = new ZoomState(
				chart.Series.Min(x => x.Values.Min(y => y.XValue)),
				chart.Series.Max(x => x.Values.Max(y => y.XValue)),
				chart.Series.Min(x => x.Values.Min(y => y.YValue)),
				chart.Series.Max(x => x.Values.Max(y => y.YValue)),
				0,
				yBuffer);
			}

			chart.seriesWatcher.ProvideSeries(chart.Series);
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
							: series.Fill));

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

				CurrentZoomState = IsZoomed
					? new ZoomState(CurrentZoomState.XMin, CurrentZoomState.XMax, yMin, yMax, CurrentZoomState.XOffset, yBuffer, true)
					: new ZoomState(xMin, xMax, yMin, yMax, 0, yBuffer, true);

				renderInProgress = false;
			});
		}

		#region Calculations

		private async Task SetXAxisLabels(double xMin, double xMax)
		{
			if (!hasData) return;

			var xRange = xMax - xMin;
			var xAxisItemCount = (int)Math.Floor(plotAreaWidth / 60);
			var labels = await GetXSteps(xAxisItemCount, xMin, xMax);
			var labels2 = labels.Select(x => new AxisLabel()
			{
				Value = XAxisFormatter == null
					? x.ToString()
					: XAxisFormatter(Series.First().Values.First().XValueToImplementation(x)),
				Location = (x - xMin) / xRange * plotAreaWidth,
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
			if (!hasData) return;

			var yRange = yMax - yMin;
			var yAxisItemsCount = (int)Math.Max(1, Math.Floor(plotAreaHeight / 50));
			var labels = (await GetYSteps(yAxisItemsCount, yMin, yMax)).ToList();
			var labels2 = labels.Select(y => new AxisLabel()
			{
				Value = YAxisFormatter == null
					? Math.Round(y, 2).ToString()
					: YAxisFormatter(Series.First().Values.First().YValueToImplementation(y)),
				Location = (y - yMin) / yRange * plotAreaHeight,
			});
			YAxisLabels = new ObservableCollection<AxisLabel>(labels2.Reverse());
		}

		private async Task<List<double>> GetXSteps(int xAxisItemsCount, double xMin, double xMax)
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
				await Task.Run(() => xVals = ChartHelper.IdealAxisSteps(xAxisItemsCount, yMin, yMax));
			}

			var fracOver = (int)Math.Ceiling(xVals.Count() / (decimal)xAxisItemsCount);

			return xVals.Where(x => xVals.IndexOf(x) % fracOver == 0).ToList();
		}

		private async Task<List<double>> GetYSteps(int yAxisItemsCount, double yMin, double yMax)
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
				await Task.Run(() => yVals = ChartHelper.IdealAxisSteps(yAxisItemsCount, yMin, yMax));
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

		private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			resizeTrigger.Refresh();
		}

		#region Mouse events

		private void MouseCaptureGrid_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (!hasData) return;

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

		#endregion

		private void WpfChart_Loaded(object sender, RoutedEventArgs e)
		{
			this.Loaded -= WpfChart_Loaded;
			Application.Current.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
			OnLegendLocationSet(this, new DependencyPropertyChangedEventArgs());
			Coordinator.MouseWheel += MouseCaptureGrid_MouseWheel;
			Coordinator.MouseMove += Coordinator_MouseMove;
			Coordinator.PointClicked += Coordinator_PointClicked;
			Coordinator.PointRangeSelected += Coordinator_PointRangeSelected;
		}

		private void Coordinator_PointRangeSelected(object? sender, (Point lowerValue, Point upperValue) e)
		{
			var lowerPoints = GetPointsUnderMouse(e.lowerValue).Select(x => x.point.BackingPoint);
			var upperPoints = GetPointsUnderMouse(e.upperValue).Select(x => x.point.BackingPoint);

			var nearestLower = lowerPoints
				.FirstOrDefault(x => Math.Abs(x.YValue - e.lowerValue.Y)
						== lowerPoints.Min(x => Math.Abs(x.YValue - e.lowerValue.Y)));

			var nearestUpper = upperPoints
				.FirstOrDefault(x => Math.Abs(x.YValue - e.upperValue.Y)
						== upperPoints.Min(x => Math.Abs(x.YValue - e.upperValue.Y)));

			PointRangeSelected?.Invoke(this, new Tuple<IChartEntity, IChartEntity>(nearestLower, nearestUpper));
		}

		private void Coordinator_PointClicked(object? sender, Point e)
		{
			var pointsUnderMouse = GetPointsUnderMouse(e).Select(x => x.point.BackingPoint);
			var nearestPoint = pointsUnderMouse
				.FirstOrDefault(x => Math.Abs(x.YValue - e.Y)
						== pointsUnderMouse.Min(x => Math.Abs(x.YValue - e.Y)));

			PointClicked?.Invoke(this, nearestPoint);
		}

		private void Coordinator_MouseMove(object? sender, (bool isUserDragging, bool isUserPanning, Point? lowerSelection, Point lastMousePoint, MouseEventArgs args) e)
		{
			if (e.isUserPanning)
			{
				var mouseLoc = e.args.GetPosition(Coordinator);
				var zoomOffset = CurrentZoomState.XOffset + e.lastMousePoint.X - mouseLoc.X;
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

		private List<(InternalChartEntity point, InternalPathSeriesViewModel series)> GetPointsUnderMouse(Point point)
		{
			var translatedMouseLoc = TooltipControl.TranslatePoint(point, SeriesItemsControl);
			var pointsUnderMouse = new List<(InternalChartEntity point, InternalPathSeriesViewModel series)>();
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

				pointsUnderMouse.Add((hoveredChartPoint, series));
			}

			return pointsUnderMouse;
		}

		private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
		{
			resizeTrigger.Stop();
			runRenderThread = false;
			seriesWatcher.Dispose();
			Coordinator.MouseWheel -= MouseCaptureGrid_MouseWheel;
		}
	}
}
