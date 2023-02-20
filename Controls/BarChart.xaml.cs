using CoreUtilities.HelperClasses.Extensions;
using CoreUtilities.Services;
using ModernThemables.HelperClasses.Charting.Brushes;
using ModernThemables.HelperClasses.Charting;
using ModernThemables.Interfaces;
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
using ModernThemables.ViewModels.Charting.CartesianChart;
using ModernThemables.HelperClasses.Charting.CartesianChart;
using ModernThemables.ViewModels.Charting;

namespace ModernThemables.Controls
{
	/// <summary>
	/// Interaction logic for BarChart.xaml
	/// </summary>
	public partial class BarChart : UserControl
	{
		private double plotAreaHeight => DrawableChartSectionBorder.ActualHeight;
		private double plotAreaWidth => DrawableChartSectionBorder.ActualWidth;
		private DateTime timeLastUpdated;
		private TimeSpan updateLimit = TimeSpan.FromMilliseconds(1000 / 60d);
		private bool hasSetSeries;
		private List<ISeries> subscribedSeries = new();

		private KeepAliveTriggerService resizeTrigger;

		private Point? lastMouseMovePoint;
		private bool ignoreNextMouseMove;

		private bool tooltipLeft;
		private bool tooltipTop = true;

		private bool preventTrigger;

		private bool isSingleXPoint;

		private BlockingCollection<Action> renderQueue;
		private bool renderInProgress;

		private Thread renderThread;
		private bool runRenderThread = true;

		public BarChart()
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

		private static void OnTooltipLocationSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not BarChart chart) return;

			chart.IsTooltipByCursor = chart.TooltipLocation == TooltipLocation.Cursor;
		}

		private static async void OnLegendLocationSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not BarChart chart) return;

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
			if (sender is not BarChart chart) return;

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

			chart.QueueRenderChart(null, null, true);
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

			QueueRenderChart(newItems, oldItems);
		}

		private async void Series_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
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
				var collection = new List<InternalBarGroupViewModel>();// InternalSeries.Clone().ToList();

				//if (invalidateAll)
				//{
				//	collection.Clear();
				//}
				//else
				//{
				//	foreach (var series in (removedSeries ?? new List<ISeries>()))
				//	{
				//		collection.Remove(collection.First(x => x.Identifier == series.Identifier));
				//	}
				//}

				var source = Series;/*invalidateAll
					? Series ?? new ObservableCollection<ISeries>()
					: addedSeries ?? new List<ISeries>();*/

				var groups = source.SelectMany(x => x.Values.Select(y => (y.XValue, y.Name))).Distinct();

				foreach (var group in groups)
				{
					var bars = await GetBarsForGroup(group.XValue, source);

					collection.Add(bars);

					//if (!series.Values.Any()) continue;

					//var seriesYMin = series.Values.Min(z => z.YValue);
					//var seriesYMax = series.Values.Max(z => z.YValue);

					//series.Stroke?.Reevaluate(seriesYMax, seriesYMin, 0, xMax, xMin, 0);
					//series.Fill?.Reevaluate(seriesYMax, seriesYMin, 0, xMax, xMin, 0);
				}

				//foreach (var series in collection)
				//{
				//	var matchingSeries = Series.FirstOrDefault(x => x.Identifier == series.Identifier);

				//	if (matchingSeries == null) continue;

				//	var points = await GetPointsForSeries(matchingSeries);
				//	series.UpdatePoints(points);
				//}

				InternalSeries = new ObservableCollection<InternalBarGroupViewModel>(collection);

				//foreach (var series in InternalSeries)
				//{
				//	series.ResizeTrigger = !series.ResizeTrigger;
				//}

				_ = SetXAxisLabels();
				_ = SetYAxisLabels();

				renderInProgress = false;
			});
		}

		#region Calculations

		private async Task SetXAxisLabels()
		{
			if (!HasGotData()) return;

			var labels = InternalSeries.Select(x => x.Name);
			var labels2 = labels.Select(x => new AxisLabel()
			{
				Value = x.ToString(),
				Height = (labels.ToList().IndexOf(x) / labels.Count() * plotAreaWidth),
			});
			XAxisLabels = new ObservableCollection<AxisLabel>(labels2);
			if (isSingleXPoint)
			{
				XAxisLabels = new ObservableCollection<AxisLabel>()
				{
					new AxisLabel(labels.First(), plotAreaWidth / 2)
				};
			}
		}

		private async Task SetYAxisLabels()
		{
			if (!HasGotData()) return;

			var yMax = InternalSeries.Max(x => x.Bars.Max(y => y.Y));
			var yMin = InternalSeries.Min(x => x.Bars.Min(y => y.Y));

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

		private async Task<InternalBarGroupViewModel?> GetBarsForGroup(double group, IEnumerable<ISeries> series)
		{
			if (series == null || !series.Any()) return null;

			var barsInGroup = series.SelectMany(x => x.Values.Where(y => y.XValue == group));
			return await Task.Run(() =>
			{
				return new InternalBarGroupViewModel(
					barsInGroup.First().Name,
					barsInGroup.Select(x => new InternalChartEntity(group, x.YValue, x, x.Stroke, x.Fill)).ToList());
			});
		}

		#endregion

			#region Grid events

		private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			resizeTrigger.Refresh();
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

			var mouseLoc = e.GetPosition(Grid);
			var translatedMouseLoc = e.GetPosition(SeriesItemsControl);

			if (DateTime.Now - timeLastUpdated < updateLimit) return;

			timeLastUpdated = DateTime.Now;

			#region Find points under mouse
			var pointsUnderMouse = new List<TooltipPointViewModel>();
			//foreach (var series in InternalSeries)
			//{
			//	var hoveredChartPoint = series.GetChartPointUnderTranslatedMouse(
			//		Math.Max(InternalSeries.Max(x => x.Data.Max(y => y.X)) - InternalSeries.Min(x => x.Data.Min(y => y.X)), 1),
			//		Math.Max(InternalSeries.Max(x => x.Data.Max(y => y.Y)) - InternalSeries.Min(x => x.Data.Min(y => y.Y)), 1),
			//		translatedMouseLoc.X,
			//		translatedMouseLoc.Y,
			//		SeriesItemsControl.ActualWidth,
			//		SeriesItemsControl.ActualHeight,
			//		-SeriesItemsControl.Margin.Left,
			//		-SeriesItemsControl.Margin.Top,
			//		0/*yBuffer*/);

			//	if (hoveredChartPoint == null
			//		//|| !CurrentZoomState.IsPointInBounds(
			//		//		hoveredChartPoint.BackingPoint.XValue,
			//		//		hoveredChartPoint.BackingPoint.YValue)
			//		|| !series.IsTranslatedMouseInBounds(
			//				InternalSeries.Max(
			//					x => x.Data.Max(y => y.X)) - InternalSeries.Min(x => x.Data.Min(y => y.X)),
			//				translatedMouseLoc.X,
			//				SeriesItemsControl.ActualWidth)) continue;

			//	if (isSingleXPoint) hoveredChartPoint.X += (plotAreaWidth / 2);

			//	pointsUnderMouse.Add(new TooltipPointViewModel(
			//		hoveredChartPoint,
			//		new Thickness(hoveredChartPoint.X - 5, hoveredChartPoint.Y - 5, 0, 0),
			//		new SolidColorBrush(series.Stroke != null
			//			? series.Stroke.ColourAtPoint(
			//				hoveredChartPoint.BackingPoint.XValue, hoveredChartPoint.BackingPoint.YValue)
			//			: Colors.Red),
			//		series.TooltipLabelFormatter != null
			//			? series.TooltipLabelFormatter(
			//				series.Data.Select(x => x.BackingPoint), hoveredChartPoint.BackingPoint)
			//			: hoveredChartPoint.BackingPoint.YValue.ToString(),
			//		plotAreaHeight
			//		));
			//}
			#endregion

			#region Tooltip
			var nearestPoint = pointsUnderMouse.FirstOrDefault(
				x => Math.Abs(x.Point.Y - mouseLoc.Y)
					== pointsUnderMouse.Min(x => Math.Abs(x.Point.Y - mouseLoc.Y)));

			if (nearestPoint != null)
				nearestPoint.IsNearest = pointsUnderMouse.Count() > 1;

			if (IsTooltipVisible && TooltipLocation == TooltipLocation.Cursor)
			{
				//// Get tooltip position variables
				//if (!tooltipLeft && (plotAreaWidth - mouseLoc.X) < (TooltipItemsControl.ActualWidth + 10))
				//	tooltipLeft = true;
				//if (tooltipLeft && (mouseLoc.X) < (TooltipItemsControl.ActualWidth + 5))
				//	tooltipLeft = false;
				//if (!tooltipTop && (plotAreaHeight - mouseLoc.Y) < (TooltipItemsControl.ActualHeight + 10))
				//	tooltipTop = true;
				//if (tooltipTop && (mouseLoc.Y) < (TooltipItemsControl.ActualHeight + 5))
				//	tooltipTop = false;

				//TooltipItemsControl.Margin = new Thickness(
				//	!tooltipLeft ? mouseLoc.X + 5 : mouseLoc.X - TooltipItemsControl.ActualWidth - 5,
				//	!tooltipTop ? mouseLoc.Y + 5 : mouseLoc.Y - TooltipItemsControl.ActualHeight - 5,
				//	0, 0);
			}
			#endregion
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
