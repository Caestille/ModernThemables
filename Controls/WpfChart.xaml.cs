using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.Defaults;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows.Media;
using System.Windows.Input;

namespace ModernThemables.Controls
{
	public struct ValueWithHeight
	{
		public string Value { get; set; }
		public double Height { get; set; }

		public ValueWithHeight(string value, double height)
		{
			Value = value;
			Height = height;
		}
	}

	public struct WpfSeries
	{
		public string PathData { get; set; }
		public Brush Stroke { get; set; }
		public Brush Fill { get; set; }

		public WpfSeries(string pathData, Brush stroke, Brush fill)
		{
			PathData = pathData;
			Stroke = stroke;
			Fill = fill;
		}
	}

	/// <summary>
	/// Interaction logic for WpfChart.xaml
	/// </summary>
	public partial class WpfChart : UserControl
	{
		private double zoomFactor = 1;
		private double zoomCentre;
		private double plotAreaHeight => DrawableChartSectionBorder.ActualHeight;
		private double plotAreaWidth => DrawableChartSectionBorder.ActualWidth;

		private DateTime timeLastUpdated;
		private TimeSpan updateLimit = TimeSpan.FromMilliseconds(1000 / 60d);

		private bool hasSetSeries;

		private List<LineSeries<DateTimePoint>> subscribedSeries = new();

		public ObservableCollection<LineSeries<DateTimePoint>> Series
		{
			get { return (ObservableCollection<LineSeries<DateTimePoint>>)GetValue(SeriesProperty); }
			set { SetValue(SeriesProperty, value); }
		}
		public static readonly DependencyProperty SeriesProperty = DependencyProperty.Register(
		  "Series", typeof(ObservableCollection<LineSeries<DateTimePoint>>), typeof(WpfChart), new FrameworkPropertyMetadata(null, SetChartValues));

		public ObservableCollection<WpfSeries> ConvertedSeries
		{
			get { return (ObservableCollection<WpfSeries>)GetValue(ConvertedSeriesProperty); }
			set { SetValue(ConvertedSeriesProperty, value); }
		}
		public static readonly DependencyProperty ConvertedSeriesProperty = DependencyProperty.Register(
		  "ConvertedSeries", typeof(ObservableCollection<WpfSeries>), typeof(WpfChart), new PropertyMetadata(new ObservableCollection<WpfSeries>()));

		public ObservableCollection<string> XAxisLabels
		{
			get { return (ObservableCollection<string>)GetValue(XAxisLabelsProperty); }
			set { SetValue(XAxisLabelsProperty, value); }
		}
		public static readonly DependencyProperty XAxisLabelsProperty = DependencyProperty.Register(
		  "XAxisLabels", typeof(ObservableCollection<string>), typeof(WpfChart), new PropertyMetadata(new ObservableCollection<string>()));

		public ObservableCollection<ValueWithHeight> YAxisLabels
		{
			get { return (ObservableCollection<ValueWithHeight>)GetValue(YAxisLabelsProperty); }
			set { SetValue(YAxisLabelsProperty, value); }
		}
		public static readonly DependencyProperty YAxisLabelsProperty = DependencyProperty.Register(
		  "YAxisLabels", typeof(ObservableCollection<ValueWithHeight>), typeof(WpfChart), new PropertyMetadata(new ObservableCollection<ValueWithHeight>()));

		public double XAxisLabelsWidth
		{
			get { return (double)GetValue(XAxisLabelsWidthProperty); }
			set { SetValue(XAxisLabelsWidthProperty, value); }
		}
		public static readonly DependencyProperty XAxisLabelsWidthProperty = DependencyProperty.Register(
		  "XAxisLabelsWidth", typeof(double), typeof(WpfChart), new PropertyMetadata(0d));

		public string PathData
		{
			get { return (string)GetValue(PathDataProperty); }
			set { SetValue(PathDataProperty, value); }
		}
		public static readonly DependencyProperty PathDataProperty = DependencyProperty.Register(
		  "PathData", typeof(string), typeof(WpfChart), new PropertyMetadata(""));

		public WpfChart()
		{
			InitializeComponent();
		}

		private static void SetChartValues(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not WpfChart chart) return;

			if (chart.Series == null)
			{
				foreach (var series in chart.subscribedSeries)
				{
					series.PropertyChanged -= chart.Series_PropertyChanged;
                }
			}

			chart.Subscribe(chart.Series);
			chart.hasSetSeries = true;
		}

		private void Subscribe(ObservableCollection<LineSeries<DateTimePoint>> series)
		{
			series.CollectionChanged += Series_CollectionChanged;
			if (!hasSetSeries)
			{
				foreach (LineSeries<DateTimePoint> item in series)
				{
					item.PropertyChanged += Series_PropertyChanged;
					subscribedSeries.Add(item);
                }
			}
			Series_PropertyChanged(this, new PropertyChangedEventArgs(nameof(Series)));
		}

		private void Series_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			foreach (LineSeries<DateTimePoint> series in e.OldItems)
			{
				series.PropertyChanged -= Series_PropertyChanged;
                subscribedSeries.Add(series);
            }

			foreach (LineSeries<DateTimePoint> series in e.NewItems)
			{
				series.PropertyChanged += Series_PropertyChanged;
                subscribedSeries.Remove(series);
            }
		}

		private void Series_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			Application.Current.Dispatcher.BeginInvoke(() =>
			{
				var series = Series;
				var chart = this;

				if (Series is null) return;

				var xAxisItemCount = Math.Floor(plotAreaWidth / 60);
				var yAxisItemsCount = Math.Floor(plotAreaHeight / 50);

				var xMin = series.First().Values.Min(x => x.DateTime);
				var xMax = series.First().Values.Max(x => x.DateTime);
				var yMin = series.First().Values.Min(x => x.Value).Value;
				var yMax = series.First().Values.Max(x => x.Value).Value;

				var xRange = xMax - xMin;
				var yRange = yMax - yMin;
				yMin -= yRange * 0.1;
				yMax += yRange * 0.1;
				yRange = yMax - yMin;

				var xLabelStep = xRange / xAxisItemCount;
				var yLabelStep = yRange / yAxisItemsCount;
				chart.XAxisLabels.Clear();
				DateTime currentXStep = xMin;
				double currentYStep = yMin;
				for (int i = 0; i < xAxisItemCount; i++)
				{
					currentXStep += i == 0 || i == xAxisItemCount ? xLabelStep / 2 : xLabelStep;
					chart.XAxisLabels.Add(currentXStep.ToString("MMM yy"));
				}

				var labels = chart.GetYSteps(yRange, yAxisItemsCount, yMin, yMax).ToList();
				var labels2 = labels.Select(y => new ValueWithHeight() 
					{ 
						Value = Math.Round(y / 1000, 2).ToString() + " K",
						Height = ((y - yMin) / yRange * plotAreaHeight) - (labels.ToList().IndexOf(y) > 0 
							? (labels[labels.ToList().IndexOf(y) - 1] - yMin) / yRange * plotAreaHeight 
							: 0),
					});
				chart.YAxisLabels = new ObservableCollection<ValueWithHeight>(labels2.Reverse());

				chart.XAxisLabelsWidth = plotAreaWidth / xAxisItemCount;

				string pathData = string.Empty;
				bool setM = true;
				foreach (var point in series.First().Values.OrderBy(x => x.DateTime))
				{
					var pointType = setM ? "M" : "L";
					setM = false;
					double x = (double)(point.DateTime - xMin).Ticks / (double)xRange.Ticks * (double)plotAreaWidth;
					double y = plotAreaHeight - (point.Value.Value - yMin) / yRange * plotAreaHeight;
					pathData += $" {pointType}{x} {y}";
				}

				ConvertedSeries = new ObservableCollection<WpfSeries>() 
				{ 
					new WpfSeries(pathData,
						new LinearGradientBrush(),
						new LinearGradientBrush()) 
				};

				PathData = pathData;
			});
		}

		private List<double> GetYSteps(double yRange, double yAxisItemsCount, double yMin, double yMax)
		{
			var idealStep = yRange / yAxisItemsCount;
			double min = double.MaxValue;
			int stepAtMin = 1;
			var roundedSteps 
				= new List<int>() 
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

		private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Series_PropertyChanged(this, new PropertyChangedEventArgs(nameof(Series)));
		}

		private void DrawableChartSectionBorder_MouseMove(object sender, MouseEventArgs e)
		{
			if (DateTime.Now - timeLastUpdated > updateLimit)
			{
				timeLastUpdated = DateTime.Now;
				var mouseLoc = e.GetPosition(Grid);
				XCrosshair.Margin = new Thickness(0, mouseLoc.Y, 0, 0);
				YCrosshair.Margin = new Thickness(mouseLoc.X, 0, 0, 0);
				XCrosshairValueDisplay.Margin = new Thickness(mouseLoc.X - 50, 0, 0, -20);
				YCrosshairValueDisplay.Margin = new Thickness(-35, mouseLoc.Y - 10, 0, 0);
			}
		}
	}
}
