namespace ModernThemables.Charting.Controls;

using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CoreUtilities.Helpers.Extensions;
using CoreUtilities.Services;
using ModernThemables.Charting.Interfaces;
using ModernThemables.Charting.Models;
using ModernThemables.Charting.Models.Brushes;
using ModernThemables.Charting.Services;
using ModernThemables.Charting.ViewModels;
using ModernThemables.Charting.ViewModels.CartesianChart;

/// <summary>
/// Interaction logic for CartesianChart.xaml.
/// </summary>
public partial class CartesianChart : UserControl
{
    private readonly RefreshTrigger resizeTrigger;
    private readonly SeriesWatcherService seriesWatcher;
    private readonly BlockingCollection<Action> renderQueue;
    private readonly Thread renderThread;
    private bool renderInProgress;
    private bool runRenderThread = true;

    public CartesianChart()
    {
        this.InitializeComponent();
        this.Loaded += this.WpfChart_Loaded;

        this.seriesWatcher = new SeriesWatcherService(this.QueueRenderChart);

        this.renderQueue = new BlockingCollection<Action>();
        this.renderThread = new Thread(new ThreadStart(() =>
        {
            while (this.runRenderThread)
            {
                while (this.renderInProgress)
                {
                    Thread.Sleep(1);
                }

                var sw = Stopwatch.StartNew();
                if (this.renderQueue.Count != 0)
                {
                    this.renderQueue.Take().Invoke();
                }

                while (this.renderQueue.Count != 0)
                {
                    _ = this.renderQueue.Take();
                }

                sw.Stop();

                Thread.Sleep((int)Math.Max(0, 16 - sw.ElapsedMilliseconds));
            }
        }));
        this.renderThread.Start();

        this.TooltipControl.TooltipGetterFunc = new Func<Point, IEnumerable<ChartTooltipViewModel>>(point =>
        {
            var pointsUnderMouse = this.GetPointsUnderMouse(point);

            var tooltips = pointsUnderMouse.Select(x => new ChartTooltipViewModel(
                x.Point,
                new SolidColorBrush(x.Series.Stroke != null
                    ? x.Series.Stroke.ColourAtPoint(
                        x.Point.BackingPoint.XValue, x.Point.BackingPoint.YValue)
                    : Colors.Red),
                string.Empty,
                string.Empty,
                string.Empty)
            {
                TooltipTemplate = this.TooltipTemplate,
                TemplatedContent = this.TooltipContentGetter != null
                        ? this.TooltipContentGetter(x.Series.Data.Select(x => x.BackingPoint), x.Point.BackingPoint)
                        : null,
            }).ToList();

            switch (this.TooltipFindingStrategy)
            {
                case TooltipFindingStrategy.None:
                    tooltips.Clear();
                    break;
                case TooltipFindingStrategy.NearestXNearestY:
                    var nearestPoint = tooltips.FirstOrDefault(
                        x => Math.Abs(x.LocationY - point.Y)
                            == tooltips.Min(x => Math.Abs(x.LocationY - point.Y)));
                    if (nearestPoint != null)
                    {
                        tooltips = new List<ChartTooltipViewModel>() { nearestPoint };
                    }
                    else
                    {
                        tooltips.Clear();
                    }

                    break;
                case TooltipFindingStrategy.NearestXWithinThreshold:
                    tooltips = new List<ChartTooltipViewModel>(
                        tooltips.Where(
                            x => Math.Abs(x.LocationX - point.X) <= this.TooltipLocationThreshold));
                    break;
            }

            return tooltips;
        });

        this.Zoom.GetDataHeightPixelsInBounds = new Func<(double, double)>(() =>
        {
            var allPoints = this.InternalSeries.SelectMany(x => x.Data);
            if (!allPoints.Any())
            {
                return (0, 1);
            }

            var min = allPoints.Min(x => x.X);
            var max = allPoints.Max(x => x.X);
            var range = max - min;
            var boundedXMax = max - (this.Zoom.RightFraction * range) + ((this.Zoom.PanOffsetFraction * range * this.Coordinator.ActualWidth) / this.Zoom.ActualWidth);
            var boundedXMin = min + (this.Zoom.LeftFraction * range) + ((this.Zoom.PanOffsetFraction * range * this.Coordinator.ActualWidth) / this.Zoom.ActualWidth);
            var pointsInRange = allPoints.Where(x => x.X >= boundedXMin && x.X <= boundedXMax);
            var boundedYMax = pointsInRange.Any() ? pointsInRange.Max(x => x.Y) : allPoints.Max(x => x.Y);
            var boundedYMin = pointsInRange.Any() ? pointsInRange.Min(x => x.Y) : allPoints.Min(x => x.Y);
            return (boundedYMin, boundedYMax);
        });

        this.resizeTrigger = new RefreshTrigger(() => { this.QueueRenderChart(null, null, true); }, 100);
    }

    public event EventHandler<IChartEntity>? PointClicked;

    public event EventHandler<Tuple<IChartEntity, IChartEntity>>? PointRangeSelected;

    private bool HasData => this.Series != null && this.Series.Any(x => x.Values?.Any() ?? false);

    private double PlotAreaHeight => this.TooltipControl.ActualHeight;

    private double PlotAreaWidth => this.TooltipControl.ActualWidth;

    private double DataXMin => this.SafeMinMax(true, (point) => point.XValue);

    private double DataXMax => this.SafeMinMax(false, (point) => point.XValue);

    private double DataYMin => this.SafeMinMax(true, (point) => point.YValue);

    private double DataYMax => this.SafeMinMax(false, (point) => point.YValue);

    public void ResetZoom() => this.Zoom.ResetZoom();

    private static async void OnLegendLocationSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not CartesianChart chart)
        {
            return;
        }

        var properties = ChartHelper.GetLegendProperties(chart.LegendLocation);

        chart.LegendGrid.SetValue(Grid.RowProperty, properties.Row);
        chart.LegendGrid.SetValue(Grid.ColumnProperty, properties.Column);
        chart.LegendGrid.Visibility = properties.Visibility;
        chart.LegendGrid.Margin = properties.Margin;
        chart.LegendGrid.Orientation = properties.Orientation;

        await Task.Delay(1);
        chart.QueueRenderChart(null, null, true);
    }

    private static void OnSeriesSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not CartesianChart chart)
        {
            return;
        }

        chart.seriesWatcher.ProvideSeries(chart.Series);
    }

    private void QueueRenderChart(
        IEnumerable<ISeries>? addedSeries,
        IEnumerable<ISeries>? removedSeries,
        bool invalidateAll = false) => this.renderQueue.Add(
            new Action(() => this.RenderChart(addedSeries, removedSeries, invalidateAll)));

    private void RenderChart(
        IEnumerable<ISeries>? addedSeries, IEnumerable<ISeries>? removedSeries, bool invalidateAll = false)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var sw = Stopwatch.StartNew();
            this.renderInProgress = true;
            var collection = this.InternalSeries.ShallowCopy().Select(series => (series, false)).ToList();

            if (invalidateAll)
            {
                collection.Clear();
            }
            else if (removedSeries != null && removedSeries.Any())
            {
                foreach (var series in removedSeries.Where(x => collection.Any(y => y.series.Identifier == x.Identifier)))
                {
                    collection.Remove(collection.First(x => x.series.Identifier == series.Identifier));
                }
            }

            var xMax = this.DataXMax;
            var xMin = this.DataXMin;
            foreach (var series in invalidateAll
                ? this.Series ?? new ObservableCollection<ISeries>()
                : addedSeries ?? new List<ISeries>())
            {
                var clonedValues = series.Values?.ShallowCopy();
                if (clonedValues == null || !clonedValues.Any())
                {
                    continue;
                }

                var points = this.GetPointsForSeries(series);

                var matchingSeries = this.InternalSeries.FirstOrDefault(x => x.Identifier == series.Identifier);

                var stroke = invalidateAll
                    ? matchingSeries != null
                        ? matchingSeries.Stroke
                        : series.Stroke ?? new SolidBrush(ColorExtensions.RandomColour(50))
                    : series.Stroke ?? new SolidBrush(ColorExtensions.RandomColour(50));
                var fill = invalidateAll
                    ? matchingSeries != null ? matchingSeries.Fill : series.Fill
                    : series.Fill;
                collection.Add((
                    new InternalPathSeriesViewModel(series.Name, series.Identifier, points, stroke, fill),
                    true));

                if (!clonedValues.Any())
                {
                    continue;
                }

                var seriesYMin = clonedValues.Min(z => z.YValue);
                var seriesYMax = clonedValues.Max(z => z.YValue);

                series.Stroke?.Reevaluate(seriesYMax, seriesYMin, 0, xMax, xMin, 0);
                series.Fill?.Reevaluate(seriesYMax, seriesYMin, 0, xMax, xMin, 0);
            }

            var yMax = this.DataYMax;
            var yMin = this.DataYMin;
            foreach (var series in collection.Where(x => x.Item2).Select(x => x.series))
            {
                if (this.Series == null || !this.Series.Any())
                {
                    break;
                }

                var matchingSeries = this.Series.FirstOrDefault(x => x.Identifier == series.Identifier);
                if (matchingSeries == null)
                {
                    continue;
                }

                series.UpdatePoints(this.GetPointsForSeries(matchingSeries));

                var cloned = matchingSeries.Values.ShallowCopy();
                if (!cloned.Any())
                {
                    continue;
                }

                var seriesYMax = cloned.Max(x => x.YValue);
                var seriesYMin = cloned.Min(x => x.YValue);
                var seriesYRange = seriesYMax - seriesYMin;
                var seriesXMax = cloned.Max(x => x.XValue);
                var seriesXMin = cloned.Min(x => x.XValue);
                var seriesXRange = seriesXMax - seriesXMin;

                var topMargin = (yMax - seriesYMax) / seriesYRange;
                var bottomMargin = (seriesYMin - yMin) / seriesYRange;
                var rightMargin = (xMax - seriesXMax) / seriesXRange;
                var leftMargin = (seriesXMin - xMin) / seriesXRange;

                series.SetMargins(topMargin, bottomMargin, leftMargin, rightMargin);
            }

            _ = this.SetXAxisLabels();
            _ = this.SetYAxisLabels();

            this.InternalSeries = new ObservableCollection<InternalPathSeriesViewModel>(collection.Select(x => x.series));

            this.Zoom.InvalidateArrange();

            this.renderInProgress = false;
            sw.Stop();
        });
    }

    private async Task SetXAxisLabels()
    {
        if (!this.HasData)
        {
            return;
        }

        var range = this.DataXMax - this.DataXMin;
        var xMax = this.DataXMax - (this.Zoom.RightFraction * range) + ((this.Zoom.PanOffsetFraction * range * this.Coordinator.ActualWidth) / this.Zoom.ActualWidth);
        var xMin = this.DataXMin + (this.Zoom.LeftFraction * range) + ((this.Zoom.PanOffsetFraction * range * this.Coordinator.ActualWidth) / this.Zoom.ActualWidth);

        var first = this.Series.First().Values.First();
        var xRange = xMax - xMin;
        var xAxisItemCount = (int)Math.Floor(this.PlotAreaWidth / 60);
        var labels = await this.GetXSteps(xAxisItemCount, xMin, xMax);
        var labels2 = labels.Select(xValue => new AxisLabel(
            xValue,
            (xValue - xMin) / xRange * this.PlotAreaWidth,
            value => this.XAxisFormatter == null ? value.ToString() : this.XAxisFormatter(first.XValueToImplementation(value)),
            value => this.XAxisCursorLabelFormatter(first.XValueToImplementation(value))));
        this.XAxisLabels = new ObservableCollection<AxisLabel>(labels2);
        if (xRange == 0)
        {
            this.XAxisLabels = new ObservableCollection<AxisLabel>()
            {
                new AxisLabel(
                    xMin,
                    this.PlotAreaWidth / 2,
                    value => this.XAxisFormatter == null ? value.ToString() : this.XAxisFormatter(first.XValueToImplementation(value)),
                    this.XAxisCursorLabelFormatter != null ? value => this.XAxisCursorLabelFormatter(first.XValueToImplementation(value)) : null),
            };
        }
    }

    private async Task SetYAxisLabels()
    {
        if (!this.HasData)
        {
            return;
        }

        var range = this.DataYMax - this.DataYMin;
        var yMax = this.DataYMax - (this.Zoom.TopFraction * range);
        var yMin = this.DataYMin + (this.Zoom.BottomFraction * range);

        var first = this.Series.First().Values.First();
        var yRange = yMax - yMin;
        var yAxisItemsCount = (int)Math.Max(1, Math.Floor(this.PlotAreaHeight / 50));
        var labels = (await this.GetYSteps(yAxisItemsCount, yMax, yMin)).ToList();
        var labels2 = labels.Select(yValue => new AxisLabel(
            yValue,
            (yValue - yMin) / yRange * this.PlotAreaHeight,
            value => this.YAxisFormatter == null ? Math.Round(value, 2).ToString() : this.YAxisFormatter(first.YValueToImplementation(value)),
            this.YAxisCursorLabelFormatter != null ? value => this.YAxisCursorLabelFormatter(first.YValueToImplementation(value)) : null));
        this.YAxisLabels = new ObservableCollection<AxisLabel>(labels2.Reverse());
    }

    private async Task<List<double>> GetXSteps(int xAxisItemsCount, double xMin, double xMax)
    {
        List<double> xVals = new();

        if (this.XAxisLabelIdentifier != null)
        {
            var currVal = xMin;
            while (currVal < xMax)
            {
                if (this.XAxisLabelIdentifier(this.Series.First().Values.First().XValueToImplementation(currVal)))
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

        if (this.YAxisLabelIdentifier != null)
        {
            var currVal = yMin;
            while (currVal < yMax)
            {
                if (this.YAxisLabelIdentifier(this.Series.First().Values.First().YValueToImplementation(currVal)))
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
        if (series == null)
        {
            return new List<InternalChartEntity>();
        }

        var xMin = this.DataXMin;
        var xRange = this.DataXMax - xMin;
        var yMin = this.DataYMin;
        var yRange = this.DataYMax - yMin;

        List<InternalChartEntity> points = new();
        foreach (var point in series.Values.ShallowCopy())
        {
            double x = (double)(point.XValue - xMin) / (double)xRange * (double)this.PlotAreaWidth;
            double y = this.PlotAreaHeight - ((point.YValue - yMin) / (yRange * this.PlotAreaHeight));
            points.Add(new InternalChartEntity(x, y, point));
        }

        return points;
    }

    private List<(InternalChartEntity Point, InternalPathSeriesViewModel Series)> GetPointsUnderMouse(Point point)
    {
        var xMax = this.DataXMax;
        var xMin = this.DataXMin;
        var xRange = xMax - xMin;

        var translatedMouseLoc = this.TooltipControl.TranslatePoint(point, this.Zoom);
        var pointsUnderMouse = new List<(InternalChartEntity Point, InternalPathSeriesViewModel Series)>();
        foreach (var series in this.InternalSeries)
        {
            var data = this.InternalSeries.SelectMany(x => x.Data);
            var xZoom = this.Zoom.ActualWidth / Math.Max(data.Max(y => y.X) - data.Min(y => y.X), 1);
            var yZoom = this.Zoom.ActualHeight / Math.Max(data.Max(y => y.Y) - data.Min(y => y.Y), 1);
            var hoveredChartPoint = series.GetChartPointUnderTranslatedMouse(
                translatedMouseLoc,
                xZoom,
                yZoom,
                -this.Zoom.Margin.Left,
                -this.Zoom.Margin.Top);

            if (hoveredChartPoint == null
                || !series.IsTranslatedMouseInBounds(
                        this.InternalSeries.Max(
                            x => x.Data.Max(y => y.X)) - this.InternalSeries.Min(x => x.Data.Min(y => y.X)),
                        translatedMouseLoc.X,
                        this.SeriesItemsControl.ActualWidth))
            {
                continue;
            }

            if (xRange == 0)
            {
                hoveredChartPoint.X += this.PlotAreaWidth / 2;
            }

            pointsUnderMouse.Add((hoveredChartPoint, series));
        }

        return pointsUnderMouse;
    }

    private double SafeMinMax(bool isMin, Func<IChartEntity, double> valueGetter)
    {
        if (this.Series is null)
        {
            return 0;
        }

        var cloned = this.Series.Where(x => x.Values?.Any() ?? false).Select(x => x.Values.ShallowCopy()).ToList();
        if (!cloned.Any())
        {
            return 0;
        }

        var all = cloned.SelectMany(x => x).ToList();

        if (!all.Any())
        {
            return 0;
        }

        return isMin
            ? all.Min(y => valueGetter(y))
            : all.Max(y => valueGetter(y));
    }

    private void Grid_SizeChanged(object sender, SizeChangedEventArgs e) => this.resizeTrigger.Refresh();

    private void WpfChart_Loaded(object sender, RoutedEventArgs e)
    {
        this.Loaded -= this.WpfChart_Loaded;
        Application.Current.Dispatcher.ShutdownStarted += this.Dispatcher_ShutdownStarted;
        OnLegendLocationSet(this, default(DependencyPropertyChangedEventArgs));
        this.Coordinator.PointClicked += this.Coordinator_PointClicked;
        this.Coordinator.PointRangeSelected += this.Coordinator_PointRangeSelected;
        this.Zoom.ZoomChanged += this.Zoom_ZoomChanged;
    }

    private void Zoom_ZoomChanged(object? sender, EventArgs e)
    {
        _ = this.SetXAxisLabels();
        _ = this.SetYAxisLabels();
    }

    private void Coordinator_PointRangeSelected(object? sender, (Point LowerValue, Point UpperValue) e)
    {
        var lowerPoints = this.GetPointsUnderMouse(e.LowerValue).Select(x => x.Point.BackingPoint);
        var upperPoints = this.GetPointsUnderMouse(e.UpperValue).Select(x => x.Point.BackingPoint);

        var nearestLower = lowerPoints
            .FirstOrDefault(x => Math.Abs(x.YValue - e.LowerValue.Y)
                    == lowerPoints.Min(x => Math.Abs(x.YValue - e.LowerValue.Y)));

        var nearestUpper = upperPoints
            .FirstOrDefault(x => Math.Abs(x.YValue - e.UpperValue.Y)
                    == upperPoints.Min(x => Math.Abs(x.YValue - e.UpperValue.Y)));

        if (nearestLower != null && nearestUpper != null)
        {
            this.PointRangeSelected?.Invoke(this, new Tuple<IChartEntity, IChartEntity>(nearestLower, nearestUpper));
        }
    }

    private void Coordinator_PointClicked(object? sender, Point e)
    {
        var pointsUnderMouse = this.GetPointsUnderMouse(e).Select(x => x.Point.BackingPoint);
        var nearestPoint = pointsUnderMouse
            .FirstOrDefault(x => Math.Abs(x.YValue - e.Y)
                    == pointsUnderMouse.Min(x => Math.Abs(x.YValue - e.Y)));

        if (nearestPoint != null)
        {
            this.PointClicked?.Invoke(this, nearestPoint);
        }
    }

    private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
    {
        this.resizeTrigger.Stop();
        this.runRenderThread = false;
        this.seriesWatcher.Dispose();
    }
}
