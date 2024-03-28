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
using System.Windows.Media;
using ModernThemables.Charting.ViewModels.CartesianChart;
using ModernThemables.Charting.ViewModels;
using ModernThemables.Charting.Models;
using ModernThemables.Charting.Models.Brushes;
using ModernThemables.Charting.Interfaces;
using ModernThemables.Charting.Services;
using ModernThemables.Charting.Controls.ChartComponents;
using System.Diagnostics;

namespace ModernThemables.Charting.Controls
{
	/// <summary>
	/// Interaction logic for CartesianChart.xaml
	/// </summary>
	public partial class CartesianChart : UserControl
	{
		public event EventHandler<IChartEntity>? PointClicked;
		public event EventHandler<Tuple<IChartEntity, IChartEntity>>? PointRangeSelected;

		private readonly RefreshTrigger resizeTrigger;

		private readonly BlockingCollection<Action> renderQueue;
		private bool renderInProgress;

		private readonly Thread renderThread;
		private bool runRenderThread = true;

		private readonly SeriesWatcherService seriesWatcher;

		private bool hasData => Series != null && Series.Any(x => x.Values?.Any() ?? false);
		private double plotAreaHeight => TooltipControl.ActualHeight;
		private double plotAreaWidth => TooltipControl.ActualWidth;

		private double dataXMin => SafeMinMax(true, (point) => point.XValue);
		private double dataXMax => SafeMinMax(false, (point) => point.XValue);
		public double dataYMin => SafeMinMax(true, (point) => point.YValue);
		public double dataYMax => SafeMinMax(false, (point) => point.YValue);

		public CartesianChart()
		{
			InitializeComponent();
			Loaded += WpfChart_Loaded;

			seriesWatcher = new SeriesWatcherService(QueueRenderChart);

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

			TooltipControl.TooltipGetterFunc = new Func<Point, IEnumerable<TooltipViewModel>>((point =>
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
						TooltipTemplate = TooltipTemplate,
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
			Zoom.GetDataHeightPixelsInBounds = new Func<(double, double)>(() =>
			{
				var allPoints = InternalSeries.SelectMany(x => x.Data);
				var min = allPoints.Min(x => x.X);
				var max = allPoints.Max(x => x.X);
				var range = max - min;
				var boundedXMax = max - Zoom.RightFraction * range + Zoom.PanOffsetFraction * range * Coordinator.ActualWidth / Zoom.ActualWidth;
				var boundedXMin = min + Zoom.LeftFraction * range + Zoom.PanOffsetFraction * range * Coordinator.ActualWidth / Zoom.ActualWidth;
				var pointsInRange = allPoints.Where(x => x.X > boundedXMin && x.X < boundedXMax);
				var boundedYMax = pointsInRange.Any() ? pointsInRange.Min(x => x.Y) : allPoints.Min(x => x.Y);
				var boundedYMin = pointsInRange.Any() ? pointsInRange.Max(x => x.Y) : allPoints.Max(x => x.Y);
				return (boundedYMin, boundedYMax);
			});

			resizeTrigger = new RefreshTrigger(() => { QueueRenderChart(null, null, true); }, 100);
		}

		public void ResetZoom()
		{
			Zoom.ResetZoom();
		}

		private static async void OnLegendLocationSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not CartesianChart chart) return;

			var properties = ChartHelper.GetLegendProperties(chart.LegendLocation);

			chart.LegendGrid.SetValue(Grid.RowProperty, properties.row);
			chart.LegendGrid.SetValue(Grid.ColumnProperty, properties.column);
			chart.LegendGrid.Visibility = properties.visibility;
			chart.LegendGrid.Margin = properties.margin;
			chart.LegendGrid.Orientation = properties.orientation;

			await Task.Delay(1);
			chart.QueueRenderChart(null, null, true);
		}

		private static void OnSeriesSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not CartesianChart chart) return;

			chart.seriesWatcher.ProvideSeries(chart.Series);
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
				var sw = Stopwatch.StartNew();
				renderInProgress = true;
				var collection = InternalSeries.Clone().Select(x => (x, false)).ToList();

				if (invalidateAll)
				{
					collection.Clear();
				}
				else
				{
					foreach (var series in (removedSeries ?? new List<ISeries>()).Where(x => collection.Any(y => y.Item1.Identifier == x.Identifier)))
					{
						collection.Remove(collection.First(x => x.Item1.Identifier == series.Identifier));
					}
				}

				var xMax = dataXMax;
				var xMin = dataXMin;
				foreach (var series in invalidateAll
					? Series ?? new ObservableCollection<ISeries>()
					: addedSeries ?? new List<ISeries>())
				{
					if (series.Values == null || !series.Values.Any()) continue;

					var points = GetPointsForSeries(series);

					var matchingSeries = InternalSeries.FirstOrDefault(x => x.Identifier == series.Identifier);

					collection.Add((new InternalPathSeriesViewModel(
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
							: series.Fill), true));

					if (!series.Values.Any()) continue;

					var seriesYMin = series.Values.Min(z => z.YValue);
					var seriesYMax = series.Values.Max(z => z.YValue);

					series.Stroke?.Reevaluate(seriesYMax, seriesYMin, 0, xMax, xMin, 0);
					series.Fill?.Reevaluate(seriesYMax, seriesYMin, 0, xMax, xMin, 0);
				}

				var yMax = dataYMax;
				var yMin = dataYMin;
				foreach (var series in collection.Where(x => x.Item2).Select(x => x.Item1))
				{
					if (Series == null || !Series.Any()) break;

					var matchingSeries = Series.FirstOrDefault(x => x.Identifier == series.Identifier);
					if (matchingSeries == null) continue;
					series.UpdatePoints(GetPointsForSeries(matchingSeries));

					if (!matchingSeries.Values.Any()) continue;

					var seriesYMax = matchingSeries.Values.Max(x => x.YValue);
					var seriesYMin = matchingSeries.Values.Min(x => x.YValue);
					var seriesYRange = seriesYMax - seriesYMin;
					var seriesXMax = matchingSeries.Values.Max(x => x.XValue);
					var seriesXMin = matchingSeries.Values.Min(x => x.XValue);
					var seriesXRange = seriesXMax - seriesXMin;

					var topMargin = ((yMax - seriesYMax) / seriesYRange);
					var bottomMargin = ((seriesYMin - yMin) / seriesYRange);
					var rightMargin = ((xMax - seriesXMax) / seriesXRange);
					var leftMargin = ((seriesXMin - xMin) / seriesXRange);

					series.SetMargins(topMargin, bottomMargin, leftMargin, rightMargin);
				}

				_ = SetXAxisLabels();
				_ = SetYAxisLabels();

				InternalSeries = new ObservableCollection<InternalPathSeriesViewModel>(collection.Select(x => x.Item1));

				Zoom.InvalidateArrange();

				renderInProgress = false;
				sw.Stop();
			});
		}

		#region Calculations

		private async Task SetXAxisLabels()
		{
			if (!hasData) return;

			var range = dataXMax - dataXMin;
			var xMax = dataXMax - Zoom.RightFraction * range + Zoom.PanOffsetFraction * range * Coordinator.ActualWidth / Zoom.ActualWidth;
			var xMin = dataXMin + Zoom.LeftFraction * range + Zoom.PanOffsetFraction * range * Coordinator.ActualWidth / Zoom.ActualWidth;

			var first = Series.First().Values.First();
			var xRange = xMax - xMin;
			var xAxisItemCount = (int)Math.Floor(plotAreaWidth / 60);
			var labels = await GetXSteps(xAxisItemCount, xMin, xMax);
			var labels2 = labels.Select(xValue => new AxisLabel(
				xValue,
				(xValue - xMin) / xRange * plotAreaWidth,
				value => XAxisFormatter == null ? value.ToString() : XAxisFormatter(first.XValueToImplementation(value)),
				value => XAxisCursorLabelFormatter(first.XValueToImplementation(value))));
			XAxisLabels = new ObservableCollection<AxisLabel>(labels2);
			if (xRange == 0)
			{
				XAxisLabels = new ObservableCollection<AxisLabel>()
				{
					new AxisLabel(
						xMin,
						plotAreaWidth / 2,
						value => XAxisFormatter == null ? value.ToString() : XAxisFormatter(first.XValueToImplementation(value)),
						XAxisCursorLabelFormatter != null ? value => XAxisCursorLabelFormatter(first.XValueToImplementation(value)) : null)
				};
			}
		}

		private async Task SetYAxisLabels()
		{
			if (!hasData) return;

			var range = dataYMax - dataYMin;
			var yMax = dataYMax - Zoom.TopFraction * range;
			var yMin = dataYMin + Zoom.BottomFraction * range;

			var first = Series.First().Values.First();
			var yRange = yMax - yMin;
			var yAxisItemsCount = (int)Math.Max(1, Math.Floor(plotAreaHeight / 50));
			var labels = (await GetYSteps(yAxisItemsCount, yMax, yMin)).ToList();
			var labels2 = labels.Select(yValue => new AxisLabel(
				yValue,
				(yValue - yMin) / yRange * plotAreaHeight,
				value => YAxisFormatter == null ? Math.Round(value, 2).ToString() : YAxisFormatter(first.YValueToImplementation(value)),
				YAxisCursorLabelFormatter != null ? value =>  YAxisCursorLabelFormatter(first.YValueToImplementation(value)) : null));
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
				await Task.Run(() => xVals = ChartHelper.IdealAxisSteps(xAxisItemsCount, xMin, xMax));
			}

			var fracOver = (int)Math.Ceiling(xVals.Count() / (decimal)xAxisItemsCount);

			return xVals.Where(x => xVals.IndexOf(x) % fracOver == 0).ToList();
		}

		private async Task<List<double>> GetYSteps(int yAxisItemsCount, double yMax, double yMin)
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

		private List<InternalChartEntity> GetPointsForSeries(ISeries? series)
		{
			if (series == null) return new List<InternalChartEntity>();

			var xMin = dataXMin;
			var xRange = dataXMax - xMin;
			var yMin = dataYMin;
			var yRange = dataYMax - yMin;
			
			List<InternalChartEntity> points = new();
			foreach (var point in series.Values)
			{
				double x = (double)(point.XValue - xMin) / (double)xRange * (double)plotAreaWidth;
				double y = plotAreaHeight - (point.YValue - yMin) / yRange * plotAreaHeight;
				points.Add(new InternalChartEntity(x, y, point));
			}
			return points;
		}

		private List<(InternalChartEntity point, InternalPathSeriesViewModel series)> GetPointsUnderMouse(Point point)
		{
			var xMax = dataXMax;
			var xMin = dataXMin;
			var xRange = xMax - xMin;

			var translatedMouseLoc = TooltipControl.TranslatePoint(point, Zoom);
			var pointsUnderMouse = new List<(InternalChartEntity point, InternalPathSeriesViewModel series)>();
			foreach (var series in InternalSeries)
			{
				var data = InternalSeries.SelectMany(x => x.Data);
				var xZoom = Zoom.ActualWidth / Math.Max(data.Max(y => y.X) - data.Min(y => y.X), 1);
				var yZoom = Zoom.ActualHeight / Math.Max(data.Max(y => y.Y) - data.Min(y => y.Y), 1);
				var hoveredChartPoint = series.GetChartPointUnderTranslatedMouse(
					translatedMouseLoc,
					xZoom,
					yZoom,
					-Zoom.Margin.Left,
					-Zoom.Margin.Top);

				if (hoveredChartPoint == null
					|| !series.IsTranslatedMouseInBounds(
							InternalSeries.Max(
								x => x.Data.Max(y => y.X)) - InternalSeries.Min(x => x.Data.Min(y => y.X)),
							translatedMouseLoc.X,
							SeriesItemsControl.ActualWidth)) continue;

				if (xRange == 0) hoveredChartPoint.X += (plotAreaWidth / 2);

				pointsUnderMouse.Add((hoveredChartPoint, series));
			}

			return pointsUnderMouse;
		}

		private double SafeMinMax(bool isMin, Func<IChartEntity, double> valueGetter)
		{
			if (isMin)
			{
				return Series != null && Series.Where(x => x.Values?.Any() ?? false).Any()
					? Series.Where(x => x.Values.Any()).SelectMany(x => x.Values).Min(y => valueGetter(y))
					: 0;
			}
			else
			{
				return Series != null && Series.Where(x => x.Values?.Any() ?? false).Any()
					? Series.Where(x => x.Values?.Any() ?? false).SelectMany(x => x.Values).Max(y => valueGetter(y))
					: 0;
			}
		}

		#endregion

		private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			resizeTrigger.Refresh();
		}

		private void WpfChart_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= WpfChart_Loaded;
			Application.Current.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
			OnLegendLocationSet(this, new DependencyPropertyChangedEventArgs());
			Coordinator.PointClicked += Coordinator_PointClicked;
			Coordinator.PointRangeSelected += Coordinator_PointRangeSelected;
			Zoom.ZoomChanged += Zoom_ZoomChanged;
		}

		private void Zoom_ZoomChanged(object? sender, EventArgs e)
		{
			_ = SetXAxisLabels();
			_ = SetYAxisLabels();
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

			if (nearestLower != null && nearestUpper != null) PointRangeSelected?.Invoke(this, new Tuple<IChartEntity, IChartEntity>(nearestLower, nearestUpper));
		}

		private void Coordinator_PointClicked(object? sender, Point e)
		{
			var pointsUnderMouse = GetPointsUnderMouse(e).Select(x => x.point.BackingPoint);
			var nearestPoint = pointsUnderMouse
				.FirstOrDefault(x => Math.Abs(x.YValue - e.Y)
						== pointsUnderMouse.Min(x => Math.Abs(x.YValue - e.Y)));

			if (nearestPoint != null) PointClicked?.Invoke(this, nearestPoint);
		}

		private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
		{
			resizeTrigger.Stop();
			runRenderThread = false;
			seriesWatcher.Dispose();
		}
	}
}
