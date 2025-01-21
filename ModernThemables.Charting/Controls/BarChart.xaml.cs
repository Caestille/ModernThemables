namespace ModernThemables.Charting.Controls;

using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CoreUtilities.Helpers.Extensions;
using CoreUtilities.Services;
using ModernThemables.Charting.Interfaces;
using ModernThemables.Charting.Models;
using ModernThemables.Charting.Services;
using ModernThemables.Charting.ViewModels;

/// <summary>
/// Interaction logic for BarChart.xaml.
/// </summary>
public partial class BarChart : UserControl
{
    private readonly RefreshTrigger resizeTrigger;
    private readonly BlockingCollection<Action> renderQueue;
    private readonly SeriesWatcherService seriesWatcher;

    private bool isSingleXPoint;

    private bool renderInProgress;
    private bool runRenderThread = true;

    public BarChart()
    {
        this.InitializeComponent();
        this.Loaded += this.WpfChart_Loaded;

        this.seriesWatcher = new SeriesWatcherService(this.QueueRenderChart);

        this.renderQueue = new BlockingCollection<Action>();
        new Thread(new ThreadStart(() =>
        {
            var sleepTimeMs = 16;
            while (this.runRenderThread)
            {
                while (this.renderInProgress)
                {
                    Thread.Sleep(sleepTimeMs);
                }

                if (this.renderQueue.Count != 0)
                {
                    this.renderQueue.Take().Invoke();
                }

                Thread.Sleep(sleepTimeMs);
            }
        })).Start();

        this.TooltipGetterFunc = new Func<Point, IEnumerable<TooltipViewModel>>((point) =>
        {
            var tooltipPoints = new List<TooltipViewModel>();

            foreach (var bar in this.InternalSeries)
            {
                bar.IsMouseOver = (point.X - bar.X) <= this.BarWidth && (point.X - bar.X) >= 0;
            }

            var tooltipBar = this.InternalSeries.FirstOrDefault(x => x.IsMouseOver);

            if (tooltipBar != null)
            {
                var matchingSeries = this.Series.FirstOrDefault(x => x.Values.Any(y => y.Identifier == tooltipBar.Identifier));
                if (matchingSeries != null)
                {
                    var formattedValue = matchingSeries.ValueFormatter != null
                        ? matchingSeries.ValueFormatter(tooltipBar.BackingPoint)
                        : tooltipBar.BackingPoint.YValue.ToString();
                    var matchingBar = matchingSeries.Values.First(x => x.Identifier == tooltipBar.Identifier);
                    var category = matchingSeries.ValueFormatter != null
                        ? matchingSeries.ValueFormatter(matchingBar)
                        : matchingSeries.Name;
                    var formattedDate = tooltipBar.BackingPoint.Name;

                    tooltipPoints.Add(new TooltipViewModel(
                        tooltipBar, tooltipBar?.Fill?.CoreBrush, formattedValue, formattedDate, category ?? string.Empty));
                }
                else
                {
                    tooltipPoints.Clear();
                }
            }

            return tooltipPoints;
        });

        this.resizeTrigger = new RefreshTrigger(() => { this.QueueRenderChart(null, null, true); }, 100);
    }

    private double PlotAreaHeight => this.TooltipControl.ActualHeight;

    private double PlotAreaWidth => this.TooltipControl.ActualWidth;

    private bool HasData => this.Series != null && this.Series.Any(x => x.Values.Any());

    private static void TriggerReRender(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not BarChart chart)
        {
            return;
        }

        chart.QueueRenderChart(null, null, true);
    }

    private static void OnSeriesSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not BarChart chart)
        {
            return;
        }

        chart.seriesWatcher.ProvideSeries(chart.Series);
    }

    private static async void OnLegendLocationSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not BarChart chart)
        {
            return;
        }

        var properties = ChartHelper.GetLegendProperties(chart.LegendLocation);

        chart.LegendGrid.SetValue(Grid.RowProperty, properties.row);
        chart.LegendGrid.SetValue(Grid.ColumnProperty, properties.column);
        chart.LegendGrid.Visibility = properties.visibility;
        chart.LegendGrid.Margin = properties.margin;
        chart.LegendGrid.Orientation = properties.orientation;

        await Task.Delay(1);
        chart.QueueRenderChart(null, null, true);
    }

    private void QueueRenderChart(
        IEnumerable<ISeries>? addedSeries, IEnumerable<ISeries>? removedSeries, bool invalidateAll = false)
            => this.renderQueue.Add(new Action(() => this.RenderChart(addedSeries, removedSeries, invalidateAll)));

    private void RenderChart(
        IEnumerable<ISeries>? addedSeries, IEnumerable<ISeries>? removedSeries, bool invalidateAll = false)
    {
        Application.Current.Dispatcher.Invoke(async () =>
        {
            this.renderInProgress = true;
            await this.SetYAxisLabels();

            var barSep = this.BarSeparationPixels;

            var source = this.Series.ShallowCopy().ToList();

            var groups = this.Series.SelectMany(x => x.Values.Select(y => x.Values.IndexOf(y))).Distinct();
            var groupedBars = groups.Select(
                group => source.Select(
                    series => group < series.Values.Count
                        ? new Tuple<IChartEntity, IChartBrush?, IChartBrush?>(series.Values[group], series.Fill, series.Stroke)
                        : null)
                .Where(y => y != null));
            var labels = groupedBars.Select(x => x.First()?.Item1?.Name ?? string.Empty);
            double barCount = groupedBars.Any() ? groupedBars.Max(x => x.Count()) : 0;

            var groupWidth = groups.Any() ? ((double)this.PlotAreaWidth / groups.Count()) : 0;
            var barWidth = Math.Min(50, groupWidth > 0 ? ((groupWidth - this.BarGroupSeparationPixels) / barCount) - (barSep * ((barCount - 1) / barCount)) : 0);
            var groupSep = groupWidth - (((barWidth + barSep) * barCount) - barSep);

            var collection = await Task.Run(() =>
            {
                var ret = new List<InternalChartEntity>();
                var maxHeight = groupedBars.Any() ? groupedBars.Max(x => x.Max(y => y!.Item1.YValue)) : 0;

                var currentX = groupSep / 2;
                foreach (var group in groupedBars.Select(x => x.ToList()))
                {
                    for (int i = 0; i < barCount; i++)
                    {
                        if (i < group.Count())
                        {
                            var bar = group[i];
                            if (bar == null)
                            {
                                continue;
                            }

                            ret.Add(new InternalChartEntity(
                                currentX,
                                (bar.Item1.YValue / maxHeight) * this.PlotAreaHeight / 1.1d,
                                bar.Item1,
                                bar.Item3,
                                bar.Item2)
                            { Identifier = bar.Item1.Identifier });
                        }

                        currentX += barWidth + barSep;
                    }

                    currentX += groupSep - barSep;
                }

                return ret;
            });

            this.BarWidth = barWidth;
            this.GroupWidth = Math.Floor(groups.Any() ? ((double)this.PlotAreaWidth / groups.Count()) : 0);
            var radius = this.BarWidth * this.BarCornerRadiusFraction / 2;
            this.BarCornerRadius = new CornerRadius(0, 0, radius, radius);
            this.isSingleXPoint = collection.Count < 2;

            this.InternalSeries = new ObservableCollection<InternalChartEntity>(collection);
            this.SetXAxisLabels(labels, (int)barCount, groupSep);

            this.renderInProgress = false;
        });
    }

    private void SetXAxisLabels(IEnumerable<string> labels, int barCount, double groupSep)
    {
        if (!this.HasData)
        {
            this.XAxisLabels.Clear();
            return;
        }

        var labels2 = labels.Select(x => new AxisLabel(
            0,
            this.InternalSeries.First(y => y.BackingPoint.Name == x).X + ((barCount / 2) * this.BarWidth) + (groupSep / 2),
            _ => x));
        this.XAxisLabels = new ObservableCollection<AxisLabel>(labels2);
        if (this.isSingleXPoint)
        {
            this.XAxisLabels = new ObservableCollection<AxisLabel>()
            {
                new AxisLabel(0, this.PlotAreaWidth / 2, _ => labels.First()),
            };
        }
    }

    private async Task SetYAxisLabels()
    {
        if (!this.HasData)
        {
            this.YAxisLabels.Clear();
            return;
        }

        var yMax = this.Series.Max(x => x.Values.Max(y => y.YValue)) * 1.1;
        var yMin = 0;

        var yRange = yMax - yMin;
        var yAxisItemsCount = (int)Math.Max(1, Math.Floor(this.PlotAreaHeight / 50));
        var labels = (await this.GetYSteps(yAxisItemsCount, yMin, yMax)).ToList();
        var labels2 = labels.Select(y => new AxisLabel(
            y,
            ((double)(y - yMin) / (double)yRange) * this.PlotAreaHeight,
            value => this.YAxisFormatter == null
                ? Math.Round(value, 2).ToString()
                : this.YAxisFormatter(value)));
        this.YAxisLabels = new ObservableCollection<AxisLabel>(labels2.Reverse());
    }

    private async Task<List<double>> GetYSteps(int yAxisItemsCount, double yMin, double yMax)
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

    private void Grid_SizeChanged(object sender, SizeChangedEventArgs e) => this.resizeTrigger.Refresh();

    private void MouseCaptureGrid_MouseLeave(object sender, MouseEventArgs e)
    {
        foreach (var bar in this.InternalSeries)
        {
            bar.IsMouseOver = false;
        }
    }

    private void WpfChart_Loaded(object sender, RoutedEventArgs e)
    {
        this.Loaded -= this.WpfChart_Loaded;
        Application.Current.Dispatcher.ShutdownStarted += this.Dispatcher_ShutdownStarted;
        OnLegendLocationSet(this, default);
        this.Coordinator.MouseLeave += this.MouseCaptureGrid_MouseLeave;
    }

    private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
    {
        this.resizeTrigger.Stop();
        this.runRenderThread = false;
        this.seriesWatcher.Dispose();
        this.Coordinator.MouseLeave -= this.MouseCaptureGrid_MouseLeave;
    }
}
