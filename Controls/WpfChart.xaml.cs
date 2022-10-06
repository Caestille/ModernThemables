using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Input;
using System.Threading.Tasks;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.Defaults;

namespace ModernThemables.Controls
{
	public record PriceData(
		decimal Price,
		DateTimeOffset Timestamp,
		string Date,
		string Time
	);

	public record TimeRange(TimeSpan Range, string Name);

	public class DecimalToVariablePrecision : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var price = (decimal)value;

			if (price <= 0) return string.Empty;

			int precision;
			if (price >= 1) precision = 2;
			else if (price > 0.099M) precision = 4;
			else precision = (int)Math.Log10((double)(1M / price)) * 2;
			return string.Format(CultureInfo.CurrentCulture, "{0:N" + precision + "}", price);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Interaction logic for WpfChart.xaml
	/// </summary>
	public partial class WpfChart : UserControl
	{
		public Thickness YOpenMargin
		{
			get { return (Thickness)GetValue(YOpenMarginProperty); }
			set { SetValue(YOpenMarginProperty, value); }
		}
		public static readonly DependencyProperty YOpenMarginProperty = DependencyProperty.Register(
		  "YOpenMargin", typeof(Thickness), typeof(WpfChart), new PropertyMetadata(new Thickness(0)));

		public Thickness YAxisMargin
		{
			get { return (Thickness)GetValue(YAxisMarginProperty); }
			set { SetValue(YAxisMarginProperty, value); }
		}
		public static readonly DependencyProperty YAxisMarginProperty = DependencyProperty.Register(
		  "YAxisMargin", typeof(Thickness), typeof(WpfChart), new PropertyMetadata(new Thickness(0)));

		public string YPointPrice
		{
			get { return (string)GetValue(YPointPriceProperty); }
			set { SetValue(YPointPriceProperty, value); }
		}
		public static readonly DependencyProperty YPointPriceProperty = DependencyProperty.Register(
		  "YPointPrice", typeof(string), typeof(WpfChart), new PropertyMetadata(""));

		public Point ChartPoint
		{
			get { return (Point)GetValue(ChartPointProperty); }
			set { SetValue(ChartPointProperty, value); }
		}
		public static readonly DependencyProperty ChartPointProperty = DependencyProperty.Register(
		  "ChartPoint", typeof(Point), typeof(WpfChart), new PropertyMetadata(new Point(0, 0)));

		public ObservableCollection<string> Timesteps
		{
			get { return (ObservableCollection<string>)GetValue(TimestepsProperty); }
			set { SetValue(TimestepsProperty, value); }
		}
		public static readonly DependencyProperty TimestepsProperty = DependencyProperty.Register(
		  "Timesteps", typeof(ObservableCollection<string>), typeof(WpfChart), new PropertyMetadata(new ObservableCollection<string>()));

		public TimeRange TimeRange
		{
			get { return (TimeRange)GetValue(TimeRangeProperty); }
			set { SetValue(TimeRangeProperty, value); }
		}
		public static readonly DependencyProperty TimeRangeProperty = DependencyProperty.Register(
		  "TimeRange", typeof(TimeRange), typeof(WpfChart), new PropertyMetadata(new TimeRange(TimeSpan.Zero, "")));

		public string FormattedOpen
		{
			get { return FormatMixed(open, CultureInfo.CurrentCulture); }
		}
		public static readonly DependencyProperty FormattedOpenProperty = DependencyProperty.Register(
		  "FormattedOpen", typeof(string), typeof(WpfChart), new PropertyMetadata(""));

		public string ChartPointFill
		{
			get { return PriceAtPoint?.Price >= open ? "#16C784" : "#EA3943"; }
		}
		public static readonly DependencyProperty ChartPointFillProperty = DependencyProperty.Register(
		  "ChartPointFill", typeof(string), typeof(WpfChart), new PropertyMetadata(""));

		public PriceData PriceAtPoint
		{
			get
			{
				var price = priceData.GetValueOrDefault(decimal.Round((decimal)ChartPoint.X, 3));
				if (price is not null)
				{
					priceAtPoint = price;
				}

				return priceAtPoint;
			}
		}
		public static readonly DependencyProperty PriceAtPointProperty = DependencyProperty.Register(
		  "PriceAtPoint", typeof(PriceData), typeof(WpfChart), new PropertyMetadata(null));

		public string Sparkline
		{
			get
			{
				if (Series is null || !resyncChart) return string.Empty;

				decimal index = 0;
				var scale = horizontalResolution / (max - min);

				return Series.Values.Aggregate("", (path, price) =>
				{
					var instruction = index == 0 ? 'M' : 'L';
					path = string.Format(
						CultureInfo.InvariantCulture,
						"{0} {1}{2} {3:0.###}",
						path,
						instruction,
						index,
						decimal.Round((max - (decimal?)price?.Value ?? 0) * scale, 3)
					);
					index += step;
					return path;
				}) +
				string.Format(
					CultureInfo.InvariantCulture,
					" {0}{1} {2:0.###}",
					'L',
					index,
					(max - open) * scale
				);
			}
		}
		public static readonly DependencyProperty SparklineProperty = DependencyProperty.Register(
		  "Sparkline", typeof(string), typeof(WpfChart), new PropertyMetadata(""));

		public Brush Stroke
		{
			get
			{
				if (Series is null) return Brushes.Transparent;

				var ratio = (double)(1 - (open - min) / (max - min));

				var green = (Color)ColorConverter.ConvertFromString("#16C784")!;
				var red = (Color)ColorConverter.ConvertFromString("#EA3943")!;

				GradientStopCollection collection = new()
			{
				new GradientStop(green, 0),
				new GradientStop(green, ratio),
				new GradientStop(red, ratio),
				new GradientStop(red, 1.0)
			};

				return new LinearGradientBrush(collection, angle: 90);
			}
		}
		public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
		  "Stroke", typeof(Brush), typeof(WpfChart), new PropertyMetadata(null));

		public Brush Fill
		{
			get
			{
				if (Series is null) return Brushes.Transparent;

				var ratio = (double)(1 - (open - min) / (max - min));

				var green = (Color)ColorConverter.ConvertFromString("#16C784");
				var red = (Color)ColorConverter.ConvertFromString("#EA3943");

				GradientStopCollection collection = new();

				collection.Add(new GradientStop(green, -0.95));
				collection.Add(new GradientStop(Color.FromArgb(3, 131, 214, 183), ratio));
				collection.Add(new GradientStop(Color.FromArgb(3, 247, 153, 159), ratio));
				collection.Add(new GradientStop(red, 1.95));

				return new LinearGradientBrush(collection, angle: 90);
			}
		}
		public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
		  "Fill", typeof(Brush), typeof(WpfChart), new PropertyMetadata(null));

		public List<string> YPrices
		{
			get
			{
				if (Series is null) return new();

				var step = (max - min) / 6;

				return Enumerable.Range(0, 7)
					.Select(i => max - i * step)
					.Select(v => FormatMixed(v, CultureInfo.CurrentCulture))
					.ToList();
			}
		}
		public static readonly DependencyProperty YPricesProperty = DependencyProperty.Register(
		  "YPrices", typeof(List<string>), typeof(WpfChart), new PropertyMetadata(null));

		public LineSeries<DateTimePoint> Series
		{
			get { return (LineSeries<DateTimePoint>)GetValue(SeriesProperty); }
			set { SetValue(SeriesProperty, value); SetChartValues(); }
		}
		public static readonly DependencyProperty SeriesProperty = DependencyProperty.Register(
		  "Series", typeof(LineSeries<DateTimePoint>), typeof(WpfChart), new PropertyMetadata(null));

		private Dictionary<int, Point> pathPoints = new();
		private decimal min;
		private decimal max;
		private decimal open;
		private bool resyncChart;
		private const decimal horizontalResolution = 360;
		private const decimal verticalResolution = 740;
		private Dictionary<decimal, PriceData> priceData = new();
		private PriceData priceAtPoint;
		private decimal step = 2.75m;
		private readonly DecimalToVariablePrecision variablePrecisionConverter = new();

		public WpfChart()
		{
			InitializeComponent();
		}

		string FormatMixed(decimal number, CultureInfo culture)
		{
			return number switch
			{
				> 9_999 => number.ToString("0,.00K", culture),
				> 999 => number.ToString("N0", culture),
				_ => (string)variablePrecisionConverter.Convert(number, typeof(string), null, culture)
			};
		}

		private async Task SetChartValues()
		{
			var from = DateTimeOffset.UtcNow - TimeRange.Range;
			var to = DateTimeOffset.UtcNow;

			step = verticalResolution / Series.Values.Count();

			var open = (decimal)Series.Values.First().Value;
			var max = (decimal)Series.Values.Max(x => x.Value);
			var min = (decimal)Series.Values.Min(x => x.Value);
			var avg = (decimal)Series.Values.Average(x => x.Value);

			var dateTimeCulture = new CultureInfo("en-US");

			await Dispatcher.InvokeAsync(() =>
			{
				resyncChart = false;
				priceData = new();

				for (var i = 0; i < Series.Values.Count(); i++)
				{
					var timestamp = DateTimeOffset.FromUnixTimeMilliseconds((long)Series.Values.ToList()[i].DateTime.Ticks) + TimeSpan.FromHours(2);
					priceData.Add(
						decimal.Round(i * step, 3),
						new PriceData(
							(decimal)Series.Values.ToList()[i].Value,
							timestamp,
							timestamp.DateTime.ToString("d", dateTimeCulture),
							timestamp.DateTime.ToString("T", dateTimeCulture)
						)
					);
				}

				this.open = open;
				this.max = max;
				this.min = min;
				YAxisMargin = new Thickness(0, 0, 0, (double)decimal.Round(horizontalResolution / (max - min) * (max - min) / 7, 3) + 2);
				YOpenMargin = new Thickness(0, (double)(horizontalResolution / (max - min)) * (double)(max - open) + 24, 0, 0);

				resyncChart = true;

				pathPoints = ChartPath.Data
				.GetFlattenedPathGeometry()
				.Figures.SelectMany(f => f.Segments)
				.SelectMany(s => ((PolyLineSegment)s).Points)
				.DistinctBy(p => (int)p.X)
				.ToDictionary(p => (int)p.X, p => p);

				int steps = TimeRange.Range.TotalDays switch
				{
					>= 365 => 12,
					>= 7 => 7,
					1 or _ => 8
				};

				var timestep = (to - from) / steps;

				var timeFormat = TimeRange.Range.TotalDays switch
				{
					1 => "h:mm tt",
					>= 7 => "MMM d",
					_ => "T"
				};

				Timesteps.Clear();

				var dateTimeSteps = Enumerable.Range(0, steps + 1)
								  .Select(i => from + TimeSpan.FromHours(3) + i * timestep)
								  .ToList();

				for (var i = 0; i < dateTimeSteps.Count - 1; i++)
				{
					if (TimeRange.Range >= TimeSpan.FromDays(7) &&
						dateTimeSteps[i].Month != dateTimeSteps[i + 1].Month)
					{
						Timesteps.Add(dateTimeSteps[i + 1].ToString("MMM"));
					}
					else if (TimeRange.Range == TimeSpan.FromDays(1) &&
							 dateTimeSteps[i].DayOfYear != dateTimeSteps[i + 1].DayOfYear)
					{
						Timesteps.Add(dateTimeSteps[i + 1].ToString("MMM d"));
					}
					else
					{
						Timesteps.Add(dateTimeSteps[i].ToString(timeFormat, dateTimeCulture));
					}
				}
			});
		}

		private void ChartMouseMove(object sender, MouseEventArgs e)
		{
			var position = e.GetPosition(ChartPath);
			var pathPoint = pathPoints.GetValueOrDefault((int)position.X);

			double width = ChartPanel.ActualWidth;
			double timestampWidth = Math.Max(XAxisTimestampPanel.ActualWidth, 150);

			if (pathPoint != default)
			{
				ChartPoint = pathPoint;

				var xTimestampMargin = XAxisTimestampPanel.Margin;
				xTimestampMargin.Left = Math.Clamp(
					position.X - timestampWidth / 2,
					0,
					width - timestampWidth - 6
				);
				XAxisTimestampPanel.Margin = xTimestampMargin;
			}

			var yPointMargin = YPointPricePanel.Margin;
			var chartTooltipMargin = ChartTooltip.Margin;

			if (position.X > width - 280)
			{
				chartTooltipMargin.Left = position.X - 220;
			}
			else
			{
				chartTooltipMargin.Left = position.X + 80;
			}

			if (position.Y < 110)
			{
				chartTooltipMargin.Top = position.Y + 52;
			}
			else
			{
				chartTooltipMargin.Top = position.Y - 92;
			}

			HorizontalChartLine.Y1 = HorizontalChartLine.Y2 = yPointMargin.Top = position.Y + 24;

			ChartTooltipWrapper.Margin = chartTooltipMargin;
			ChartTooltip.Margin = chartTooltipMargin;
			YPointPricePanel.Margin = yPointMargin;

			YPointPrice = FormatMixed(
				min + (max - min) *
				(1 - (decimal)(position.Y / ChartPath.ActualHeight)),
				CultureInfo.CurrentCulture
			);
		}
	}
}
