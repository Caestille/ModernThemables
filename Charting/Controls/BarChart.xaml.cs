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
using CoreUtilities.Converters;
using System.Windows.Threading;
using ModernThemables.Charting.ViewModels;
using ModernThemables.Charting.Models;
using ModernThemables.Charting.Interfaces;

namespace ModernThemables.Charting.Controls
{
    /// <summary>
    /// Interaction logic for BarChart.xaml
    /// </summary>
    public partial class BarChart : UserControl
	{
		private double plotAreaHeight => MouseCaptureGrid.ActualHeight;
		private double plotAreaWidth => MouseCaptureGrid.ActualWidth;
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

		private static void TriggerReRender(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not BarChart chart) return;

			chart.QueueRenderChart(null, null, true);
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
				this.Dispatcher.Invoke(DispatcherPriority.Render, SetYAxisLabels);

				var barSep = BarSeparationPixels;
				var groupSep = BarGroupSeparationPixels;

				var source = Series.Clone().ToList();

				var groups = Series.SelectMany(x => x.Values.Select(y => x.Values.IndexOf(y))).Distinct();
				var groupedBars = groups.Select(x => source.Select(y => x < y.Values.Count ? new Tuple<IChartEntity, IChartBrush, IChartBrush>(y.Values[x], y.Fill, y.Stroke) : null).Where(y => y != null));
				var labels = groupedBars.Select(x => x.First()?.Item1?.Name ?? string.Empty);
				double barCount = groupedBars.Any() ? groupedBars.Max(x => x.Count()) : 0;

				var groupWidth = groups.Any() ? ((double)plotAreaWidth / (double)groups.Count()) - groupSep : 0;
				var barWidth = groupWidth > 0 ? (groupWidth / barCount) - barSep : 0;

				XAxisLabelHeight = labels.Any() ? labels.Max(x => (double)(new StringWidthGetterConverter().Convert(new object[] { "", x, FontSize, FontFamily, FontStyle, FontWeight, FontStretch }, null, null, null))) * Math.Sin(XAxisLabelRotation * Math.PI / 180) + 10 : 0;
				this.Dispatcher.Invoke(DispatcherPriority.Render, delegate () { });

				var collection = await Task.Run(() =>
				{
					var ret = new List<InternalChartEntity>();
					var maxHeight = groupedBars.Any() ? groupedBars.Max(x => x.Max(y => y.Item1.YValue)) : 0;

					var currentX = groupSep / 2;
					foreach (var group in groupedBars.Select(x => x.ToList()))
					{
						for (int i = 0; i < barCount; i++)
						{
							if (i < group.Count())
							{
								var bar = group[i];
								ret.Add(new InternalChartEntity(currentX, (bar.Item1.YValue / maxHeight) * plotAreaHeight * (1 - 0.1), bar.Item1, bar.Item3, bar.Item2) { Identifier = bar.Item1.Identifier });
							}
							currentX += (barWidth + barSep);
						}
						currentX += groupSep;
					}

					return ret;
				});

				BarWidth = barWidth;
				GroupWidth = Math.Floor(groups.Any() ? ((double)plotAreaWidth / (double)groups.Count()) : 0);
				var radius = BarWidth * BarCornerRadiusFraction / 2;
				BarCornerRadius = new CornerRadius(0, 0, radius, radius);

				await Task.Delay(18);
				this.Dispatcher.Invoke(DispatcherPriority.Render, () => { });
				await this.Dispatcher.BeginInvoke(DispatcherPriority.Render, () => { InternalSeries = new ObservableCollection<InternalChartEntity>(collection); });
				await this.Dispatcher.BeginInvoke(DispatcherPriority.Render, () => SetXAxisLabels(labels));

				renderInProgress = false;
			});
		}

		#region Calculations

		private void SetXAxisLabels(IEnumerable<string> labels)
		{
			if (!HasGotData()) return;

			var labels2 = labels.Select(x => new AxisLabel()
			{
				Value = x.ToString(),
				Location = InternalSeries.First(y => y.BackingPoint.Name == x).X + GroupWidth / 2,
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

		private async void SetYAxisLabels()
		{
			if (!HasGotData()) return;

			var yMax = Series.Max(x => x.Values.Max(y => y.YValue)) * 1.1;
			var yMin = Series.Min(x => x.Values.Min(y => y.YValue));

			var yRange = yMax - yMin;
			var yAxisItemsCount = Math.Max(1, Math.Floor(plotAreaHeight / 50));
			var labels = (await GetYSteps(yAxisItemsCount, yMin, yMax)).ToList();
			var labels2 = labels.Select(y => new AxisLabel()
			{
				Value = YAxisFormatter == null
					? Math.Round(y, 2).ToString()
					: YAxisFormatter(y),
				Location = ((double)(y - yMin) / (double)yRange) * plotAreaHeight,
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
						{ 1, 10, 20, 100, 500, 1000, 1500, 2000, 3000, 4000, 5000, 10000, 20000, 50000, 1000000, 10000000 };
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
			translatedMouseLoc = new Point(translatedMouseLoc.X, plotAreaHeight - translatedMouseLoc.Y);

			if (DateTime.Now - timeLastUpdated < updateLimit) return;

			timeLastUpdated = DateTime.Now;

			#region Find points under mouse
			
			foreach (var bar in InternalSeries)
			{
				bar.IsMouseOver = (translatedMouseLoc.X - bar.X) <= BarWidth && (translatedMouseLoc.X - bar.X) >= 0 /*&& translatedMouseLoc.Y <= bar.Y*/;
			}

			#endregion

			#region Tooltip

			if (InternalSeries.Any(x => x.IsMouseOver))
			{
				TooltipBar = InternalSeries.First(x => x.IsMouseOver);

				var matchingSeries = Series.First(x => x.Values.Any(y => y.Identifier == TooltipBar.Identifier));
				TooltipBar.DisplayValue = matchingSeries.ValueFormatter(TooltipBar.BackingPoint);
				var matchingWedge = matchingSeries.Values.First(x => x.Identifier == TooltipBar.Identifier);
				TooltipString = matchingSeries.TooltipLabelFormatter != null
					? matchingSeries.TooltipLabelFormatter(matchingSeries.Values, matchingWedge)
					: matchingSeries.Name;

				this.Dispatcher.Invoke(DispatcherPriority.Render, delegate () { });

				if (TooltipLocation == TooltipLocation.Points)
				{
					var effectiveY = plotAreaHeight - TooltipBar.Y - 60;

					// Get tooltip position variables
					if (!tooltipLeft && TooltipBar.X > (plotAreaWidth - 10 - TooltipGrid.ActualWidth))
						tooltipLeft = true;
					if (tooltipLeft && TooltipBar.X < (10 + TooltipGrid.ActualWidth))
						tooltipLeft = false;
					tooltipTop = effectiveY < (10 + TooltipGrid.ActualHeight);

					TooltipGrid.Margin = new Thickness(
						!tooltipLeft ? TooltipBar.X : TooltipBar.X - TooltipGrid.ActualWidth + BarWidth + 5,
						!tooltipTop ? effectiveY : effectiveY,
						0, 0);
				}
				else if (TooltipLocation == TooltipLocation.Cursor)
				{
					// Get tooltip position variables
					if (!tooltipLeft && translatedMouseLoc.X > (plotAreaWidth - 10 - TooltipGrid.ActualWidth))
						tooltipLeft = true;
					if (tooltipLeft && translatedMouseLoc.X < (10 + TooltipGrid.ActualWidth))
						tooltipLeft = false;
					if (!tooltipTop && translatedMouseLoc.Y > (plotAreaHeight - 10 - TooltipGrid.ActualHeight))
						tooltipTop = true;
					if (tooltipTop && (translatedMouseLoc.Y) < (10 + TooltipGrid.ActualHeight))
						tooltipTop = false;

					TooltipGrid.Margin = new Thickness(
						!tooltipLeft ? translatedMouseLoc.X + 5 : translatedMouseLoc.X - TooltipGrid.ActualWidth - 5,
						!tooltipTop ? translatedMouseLoc.Y - 5 : translatedMouseLoc.Y - TooltipGrid.ActualHeight - 5,
						0, 0);
				}
			}
			else
			{
				TooltipBar = null;
			}

			#endregion
		}

		private void MouseCaptureGrid_MouseLeave(object sender, MouseEventArgs e)
		{
			foreach (var bar in InternalSeries)
			{
				bar.IsMouseOver = false;
			}
			TooltipBar = null;
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
