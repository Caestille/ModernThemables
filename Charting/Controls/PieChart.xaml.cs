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
using System.Windows.Threading;
using ModernThemables.Charting.ViewModels.PieChart;
using ModernThemables.Charting.Converters;
using ModernThemables.Charting.Models.PieChart;
using ModernThemables.Charting.Models;
using ModernThemables.Charting.Models.Brushes;
using ModernThemables.Charting.Interfaces;

namespace ModernThemables.Charting.Controls
{
    /// <summary>
    /// Interaction logic for PieChart.xaml
    /// </summary>
    public partial class PieChart : UserControl
	{
		private DateTime timeLastUpdated;
		private TimeSpan updateLimit = TimeSpan.FromMilliseconds(1000 / 60d);
		private List<ISeries> subscribedSeries = new();

		private KeepAliveTriggerService resizeTrigger;

		private Point? lastMouseMovePoint;
		private bool ignoreNextMouseMove;

		private bool tooltipLeft;
		private bool tooltipTop = true;

		private bool preventTrigger;
		
		private IEnumerable<InternalPieWedgeViewModel> allWedges
			=> this.InternalSeries.Aggregate(new List<InternalPieWedgeViewModel>(), (list, series) => { list.AddRange(series.Wedges); return list; });

		private BlockingCollection<Action> renderQueue;
		private bool renderInProgress;

		private Thread renderThread;
		private bool runRenderThread = true;

		public PieChart()
		{
			InitializeComponent();
			this.Loaded += PieChart_Loaded;

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

			resizeTrigger = new KeepAliveTriggerService(QueueRenderChart, 100);
		}

		private static async void OnLegendLocationSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not PieChart chart) return;

			switch (chart.LegendLocation)
			{
				case LegendLocation.Left:
					chart.LegendGrid.SetValue(Grid.RowProperty, 1);
					chart.LegendGrid.SetValue(Grid.ColumnProperty, 0);
					chart.LegendGrid.Visibility = Visibility.Visible;
					chart.LegendGrid.Margin = new Thickness(0, 10, 15, 0);
					chart.Legend.Orientation = Orientation.Vertical;
					break;
				case LegendLocation.Top:
					chart.LegendGrid.SetValue(Grid.RowProperty, 0);
					chart.LegendGrid.SetValue(Grid.ColumnProperty, 1);
					chart.LegendGrid.Visibility = Visibility.Visible;
					chart.LegendGrid.Margin = new Thickness(0, 0, 0, 15);
					chart.Legend.Orientation = Orientation.Horizontal;
					break;
				case LegendLocation.Right:
					chart.LegendGrid.SetValue(Grid.RowProperty, 1);
					chart.LegendGrid.SetValue(Grid.ColumnProperty, 2);
					chart.LegendGrid.Visibility = Visibility.Visible;
					chart.LegendGrid.Margin = new Thickness(15, 10, 0, 0);
					chart.Legend.Orientation = Orientation.Vertical;
					break;
				case LegendLocation.Bottom:
					chart.LegendGrid.SetValue(Grid.RowProperty, 2);
					chart.LegendGrid.SetValue(Grid.ColumnProperty, 1);
					chart.LegendGrid.Visibility = Visibility.Visible;
					chart.LegendGrid.Margin = new Thickness(0, 15, 0, 0);
					chart.Legend.Orientation = Orientation.Horizontal;
					break;
				case LegendLocation.None:
					chart.LegendGrid.Visibility = Visibility.Collapsed;
					break;
			}

			await Task.Delay(1);
			chart.QueueRenderChart();
		}

		#region Subscribe to series'

		private static void OnSeriesSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not PieChart chart) return;

			foreach (var series in chart.subscribedSeries)
			{
				series.PropertyChanged -= chart.Series_PropertyChanged;
				foreach (var wedge in series.Values) wedge.FocusedChanged -= chart.Wedge_FocusedChanged;
			}

			chart.subscribedSeries.Clear();

			chart.Subscribe(chart.Series);

			if (!chart.Series.Any() || !chart.Series.Any()) return;

			chart.QueueRenderChart();
		}

		private void Subscribe(ObservableCollection<ISeries> series)
		{
			series.CollectionChanged += Series_CollectionChanged;
			foreach (ISeries item in series)
			{
				item.PropertyChanged += Series_PropertyChanged;
				foreach (var wedge in item.Values) wedge.FocusedChanged += Wedge_FocusedChanged;
				subscribedSeries.Add(item);
			}
		}

		private async void Series_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				QueueRenderChart();
				return;
			}

			var oldItems = new List<ISeries>();
			if ((e.Action == NotifyCollectionChangedAction.Replace || e.Action == NotifyCollectionChangedAction.Remove)
				&& e.OldItems != null)
			{
				foreach (ISeries series in e.OldItems)
				{
					series.PropertyChanged -= Series_PropertyChanged;
					foreach (var wedge in series.Values) wedge.FocusedChanged -= Wedge_FocusedChanged;
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
					foreach (var wedge in series.Values) wedge.FocusedChanged += Wedge_FocusedChanged;
					newItems.Add(series);
					subscribedSeries.Add(series);
				}
			}

			QueueRenderChart();
		}

		private async void Series_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (sender is ISeries)
			{
				QueueRenderChart();
			}
		}

		#endregion

		private void QueueRenderChart()
		{
			renderQueue.Add(RenderChart);
		}

		private void RenderChart()
		{
			Application.Current.Dispatcher.Invoke(async () =>
			{
				renderInProgress = true;

				var newSeries = new List<InternalPieSeriesViewModel>();

				foreach (var series in Series)
				{
					var wedges = await GetWedgesForSeries(series);

					newSeries.Add(new InternalPieSeriesViewModel(
						series.Name,
						new ObservableCollection<InternalPieWedgeViewModel>(wedges)));

					if (!series.Values.Any()) continue;
				}

				InternalSeries = new ObservableCollection<InternalPieSeriesViewModel>(newSeries);

				foreach (var series in InternalSeries)
				{
					foreach (var wedge in series.Wedges)
					{
						wedge.ResizeTrigger = !wedge.ResizeTrigger;
					}
				}

				renderInProgress = false;
			});
		}

		#region Calculations		

		private async Task<List<InternalPieWedgeViewModel>> GetWedgesForSeries(ISeries? series)
		{
			var convertedSeries = new List<InternalPieWedgeViewModel>();

			if (series == null) return convertedSeries;

			var sum = series.Values.Sum(x => x.XValue);
			var angleSum = 0d;

			foreach (var wedge in series.Values)
			{
				InternalPieWedgeViewModel? matchingWedge = null;

				convertedSeries.Add(new InternalPieWedgeViewModel(
					wedge.Name,
					wedge.Identifier,
					wedge.XValue / sum * 100,
					wedge.XValue,
					angleSum / sum * 360,
					matchingWedge != null
						? matchingWedge.Stroke
						: wedge.Stroke ?? new SolidBrush(ColorExtensions.RandomColour(50)),
					matchingWedge != null
						? matchingWedge.Fill
						: wedge.Fill ?? new SolidBrush(ColorExtensions.RandomColour(50))));

				angleSum += wedge.XValue;
			}

			return convertedSeries;
		}

		#endregion

		#region Grid events

		private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			//resizeTrigger.Refresh();
		}

		#endregion

		#region Mouse events

		private void MouseCaptureGrid_MouseMove(object sender, MouseEventArgs e)
		{
			var converter = new PieCentreRadiusConverter();
			var centreX = (double)converter.Convert(
				new object[] { SeriesItemsControl.ActualWidth, SeriesItemsControl.ActualHeight },
				null, "CentreX", null);
			var centreY = (double)converter.Convert(
				new object[] { SeriesItemsControl.ActualWidth, SeriesItemsControl.ActualHeight },
				null, "CentreY", null);
			var radius = (double)converter.Convert(
				new object[] { SeriesItemsControl.ActualWidth, SeriesItemsControl.ActualHeight },
				null, "Radius", null);

			var mouseLoc = e.GetPosition(SeriesItemsControl);
			mouseLoc = new Point(mouseLoc.X -= SeriesItemsControl.ActualWidth / 2 - radius / 0.9, mouseLoc.Y);

			var hypLength = Math.Sqrt(
				Math.Pow(Math.Abs(mouseLoc.X - centreX), 2)
				+ Math.Pow(Math.Abs(mouseLoc.Y - centreY), 2));

			if (hypLength > radius)
			{
				foreach (var wedge in allWedges) wedge.IsMouseOver = false;
				TooltipWedge = null;
				return;
			}

			var angle = GetMouseAngleFromPoint(mouseLoc, new Point(centreX, centreY));

			foreach (var series in InternalSeries)
			{
				foreach (var wedge in series.Wedges)
				{
					if (angle > wedge.StartAngle
						&& angle < wedge.StartAngle + wedge.Percent * 360d /100d)
					{
						if (!wedge.IsMouseOver)
						{
							wedge.IsMouseOver = true;
							TooltipWedge = wedge;

							var matchingSeries = Series.First(x => x.Values.Any(y => y.Identifier == wedge.Identifier));
							var matchingWedge = matchingSeries.Values.First(x => x.Identifier == wedge.Identifier);
							TooltipString = matchingSeries.TooltipLabelFormatter != null
								? matchingSeries.TooltipLabelFormatter(matchingSeries.Values, matchingWedge)
								: matchingWedge.XValue.ToString();

							var centreAngle = wedge.StartAngle + wedge.Percent / 2 * 360 / 100;

							var angleRad = (Math.PI / 180.0) * (centreAngle - 90);

							double x = radius * Math.Cos(angleRad);
							double y = radius * Math.Sin(angleRad);

							this.Dispatcher.Invoke(DispatcherPriority.Render, delegate () { });

							if (TooltipLocation == TooltipLocation.Points)
							{
								var left = x + (SeriesItemsControl.ActualWidth / 2);
								var top = y + centreY - TooltipGrid.ActualHeight - 20;
								TooltipGrid.Margin = new Thickness(left, top, 0, 0);
							}
						}
					}
					else if (wedge.IsMouseOver)
					{
						wedge.IsMouseOver = false;
					}
				}
			}

			if (TooltipLocation == TooltipLocation.Cursor && TooltipWedge != null)
			{
				var wedge = TooltipWedge;
				var centreAngle = wedge.StartAngle + wedge.Percent / 2 * 360 / 100;

				var angleRad = (Math.PI / 180.0) * (centreAngle - 90);

				double x = radius * Math.Cos(angleRad);
				double y = radius * Math.Sin(angleRad);

				this.Dispatcher.Invoke(DispatcherPriority.Render, delegate () { });

				var left = mouseLoc.X + SeriesItemsControl.ActualWidth / 2 - Math.Min(SeriesItemsControl.ActualWidth, SeriesItemsControl.ActualHeight) / 2 + 5;
				var top = mouseLoc.Y - TooltipGrid.ActualHeight - 5;
				TooltipGrid.Margin = new Thickness(left, top, 0, 0);
			}
		}

		private void MouseCaptureGrid_MouseLeave(object sender, MouseEventArgs e)
		{
			foreach (var series in InternalSeries)
			{
				foreach (var wedge in series.Wedges)
				{
					wedge.IsMouseOver = false;
				}
			}
		}

		private void MouseCaptureGrid_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			
		}

		#endregion

		private bool HasGotData()
		{
			HasData = Series != null && Series.Any();
			return HasData;
		}

		private double GetMouseAngleFromPoint(Point mouseLoc, Point point)
		{
			var hypLength = Math.Sqrt(
				Math.Pow(Math.Abs(mouseLoc.X - point.X), 2)
				+ Math.Pow(Math.Abs(mouseLoc.Y - point.Y), 2));

			var oppLength = Math.Abs(mouseLoc.X - point.X);
			var angle = Math.Asin(oppLength / hypLength) * 180 / Math.PI;

			if (mouseLoc.X <= point.X && mouseLoc.Y <= point.Y)
			{
				angle = 360 - angle;
			}

			if (mouseLoc.Y > point.Y)
			{
				if (mouseLoc.X <= point.X)
				{
					angle = 180 + angle;
				}
				else
				{
					angle = 180 - angle;
				}
			}

			return angle;
		}

		private void YAxisItemsControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			ignoreNextMouseMove = true;
		}

		private void Wedge_FocusedChanged(object? sender, bool e)
		{
			if (sender is not PieWedge wedge) return;
			var id = wedge.Identifier;
			var matchingInternalWedge = InternalSeries.First(x => x.Wedges.Any(y => y.Identifier == id)).Wedges.First(x => x.Identifier == id);
			matchingInternalWedge.IsMouseOver = e;
			TooltipWedge = e ? matchingInternalWedge : null;

			var matchingSeries = Series.First(x => x.Values.Any(y => y.Identifier == wedge.Identifier));
			var matchingWedge = matchingSeries.Values.First(x => x.Identifier == wedge.Identifier);
			TooltipString = matchingSeries.TooltipLabelFormatter != null
				? matchingSeries.TooltipLabelFormatter(matchingSeries.Values, matchingWedge)
				: matchingWedge.XValue.ToString();

			var centreAngle = matchingInternalWedge.StartAngle + matchingInternalWedge.Percent / 2 * 360 / 100;

			var angleRad = (Math.PI / 180.0) * (centreAngle - 90);

			var radius = Math.Min(SeriesItemsControl.ActualWidth, SeriesItemsControl.ActualHeight) * 0.9 / 2;

			double x = radius * Math.Cos(angleRad);
			double y = radius * Math.Sin(angleRad);

			this.Dispatcher.Invoke(DispatcherPriority.Render, delegate () { });

			var converter = new PieCentreRadiusConverter();
			var centreY = (double)converter.Convert(
			new object[] { SeriesItemsControl.ActualWidth, SeriesItemsControl.ActualHeight },
			null, "CentreY", null);

			var left = x + (SeriesItemsControl.ActualWidth / 2);
			var top = y + centreY - TooltipGrid.ActualHeight - 20;
			TooltipGrid.Margin = new Thickness(left, top, 0, 0);
		}

		private void PieChart_Loaded(object sender, RoutedEventArgs e)
		{
			this.Loaded -= PieChart_Loaded;
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
