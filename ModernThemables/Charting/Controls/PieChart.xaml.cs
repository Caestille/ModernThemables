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
using ModernThemables.Charting.ViewModels.PieChart;
using ModernThemables.Charting.Converters;
using ModernThemables.Charting.Models;
using ModernThemables.Charting.Models.Brushes;
using ModernThemables.Charting.Interfaces;
using ModernThemables.Charting.ViewModels;
using ModernThemables.Charting.Services;

namespace ModernThemables.Charting.Controls
{
	/// <summary>
	/// Interaction logic for PieChart.xaml
	/// </summary>
	public partial class PieChart : UserControl
	{
		private readonly RefreshTrigger resizeTrigger;

		private readonly SeriesWatcherService seriesWatcher;

		private IEnumerable<InternalPieWedgeViewModel> allWedges
			=> InternalSeries.Aggregate(new List<InternalPieWedgeViewModel>(), (list, series) => { list.AddRange(series.Wedges); return list; });

		private readonly BlockingCollection<Action> renderQueue;
		private bool renderInProgress;

		private readonly Thread renderThread;
		private bool runRenderThread = true;

		public PieChart()
		{
			InitializeComponent();
			Loaded += PieChart_Loaded;

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

			TooltipGetterFunc = new Func<Point, IEnumerable<TooltipViewModel>>((mouseLoc) =>
			{
				var tooltipPoints = new List<TooltipViewModel>();

				var centreX = PieCentreRadiusConverter.ConvertLocally(SeriesItemsControl.ActualWidth, SeriesItemsControl.ActualHeight, PieCentreRadiusConverter.PieConverterReturnType.CentreX);
				var centreY = PieCentreRadiusConverter.ConvertLocally(SeriesItemsControl.ActualWidth, SeriesItemsControl.ActualHeight, PieCentreRadiusConverter.PieConverterReturnType.CentreY);
				var radius = PieCentreRadiusConverter.ConvertLocally(SeriesItemsControl.ActualWidth, SeriesItemsControl.ActualHeight, PieCentreRadiusConverter.PieConverterReturnType.Radius);

				mouseLoc = new Point(mouseLoc.X -= SeriesItemsControl.ActualWidth / 2 - radius / 0.9, mouseLoc.Y);

				var hypLength = Math.Sqrt(
					Math.Pow(Math.Abs(mouseLoc.X - centreX), 2)
					+ Math.Pow(Math.Abs(mouseLoc.Y - centreY), 2));

				if (hypLength > radius)
				{
					foreach (var wedge in allWedges) wedge.IsMouseOver = false;
					return new List<TooltipViewModel>();
				}

				var angle = GetMouseAngleFromPoint(mouseLoc, new Point(centreX, centreY));

				foreach (var series in InternalSeries)
				{
					foreach (var wedge in series.Wedges)
					{
						if (angle > wedge.StartAngle
							&& angle < wedge.StartAngle + wedge.Percent * 360d / 100d)
						{
							if (!wedge.IsMouseOver)
							{
								wedge.IsMouseOver = true;
							}

							var matchingSeries = Series.First(x => x.Values.Any(y => y.Identifier == wedge.Identifier));
							var matchingWedge = matchingSeries.Values.First(x => x.Identifier == wedge.Identifier);
							var formattedValue = matchingSeries.ValueFormatter != null
								? matchingSeries.ValueFormatter(matchingWedge)
								: matchingWedge.XValue.ToString();
							var formattedPercent = $"{Math.Round(wedge.Percent, 1)} %";

							var x = 0d;
							var y = 0d;

							if (TooltipLocation == TooltipLocation.Points)
							{
								var centreAngle = wedge.StartAngle + wedge.Percent / 2 * 360 / 100;
								var angleRad = (Math.PI / 180.0) * (centreAngle - 90);
								x = radius * Math.Cos(angleRad);
								y = radius * Math.Sin(angleRad);
								x = x + (SeriesItemsControl.ActualWidth / 2);
								y = y + centreY - 20;
							}

							var tooltip = new TooltipViewModel(x, y, wedge?.Fill?.CoreBrush, formattedValue, matchingWedge.Name, formattedPercent);
							tooltipPoints.Add(tooltip);
						}
						else if (wedge.IsMouseOver)
						{
							wedge.IsMouseOver = false;
						}
					}
				}

				return tooltipPoints;
			});

			resizeTrigger = new RefreshTrigger(() => QueueRenderChart(null, null, true), 100);
		}

		private static async void OnLegendLocationSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not PieChart chart) return;

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
			if (sender is not PieChart chart) return;

			chart.seriesWatcher.ProvideSeries(chart.Series);
		}

		private void QueueRenderChart(
			IEnumerable<ISeries>? addedSeries, IEnumerable<ISeries>? removedSeries, bool invalidateAll = false)
		{
			renderQueue.Add(RenderChart);
		}

		private void RenderChart()
		{
			Application.Current.Dispatcher.Invoke(async () =>
			{
                if (Series == null)
                {
                    return;
                }

				renderInProgress = true;

				var newSeries = new List<InternalPieSeriesViewModel>();

				foreach (var series in Series.Where(x => x.Values != null && x.Values.Any()))
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
			return await Task.Run(() => { 
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
			});
		}

		#endregion

		#region Mouse events

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

		#endregion

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
			//ignoreNextMouseMove = true;
		}

		private void PieChart_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= PieChart_Loaded;
			Application.Current.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
			OnLegendLocationSet(this, new DependencyPropertyChangedEventArgs());
			Coordinator.MouseLeave += MouseCaptureGrid_MouseLeave;
		}

		private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
		{
			resizeTrigger.Stop();
			runRenderThread = false;
			seriesWatcher.Dispose();
			Coordinator.MouseLeave -= MouseCaptureGrid_MouseLeave;
		}
	}
}
