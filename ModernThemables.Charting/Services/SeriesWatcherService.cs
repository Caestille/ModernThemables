namespace ModernThemables.Charting.Services;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using ModernThemables.Charting.Interfaces;

public class SeriesWatcherService
{
    private readonly Action<IList<ISeries>?, IList<ISeries>?, bool> onSeriesUpdated;
    private readonly List<ISeries> subscribedSeries = new();
    private bool hasSetSeries;

    public SeriesWatcherService(Action<IList<ISeries>?, IList<ISeries>?, bool> onSeriesUpdated)
    {
        this.onSeriesUpdated = onSeriesUpdated;
    }

    public void ProvideSeries(ObservableCollection<ISeries> newSeries)
    {
        if (newSeries == null)
        {
            foreach (var series in this.subscribedSeries)
            {
                series.PropertyChanged -= this.Series_PropertyChanged;
            }

            return;
        }

        this.Subscribe(newSeries);
        this.hasSetSeries = true;

        if (!newSeries.Any() /*|| !newSeries.Any(x => x.Values?.Any() ?? false)*/)
        {
            return;
        }

        this.onSeriesUpdated(null, null, true);
    }

    public void Dispose()
    {
        foreach (var series in this.subscribedSeries)
        {
            series.PropertyChanged -= this.Series_PropertyChanged;
        }
    }

    private void Subscribe(ObservableCollection<ISeries> series)
    {
        series.CollectionChanged += this.Series_CollectionChanged;
        if (!this.hasSetSeries)
        {
            foreach (ISeries item in series)
            {
                item.PropertyChanged += this.Series_PropertyChanged;
                item.CollectionChanged += this.Series_ValuesChanged;
                item.Values.CollectionChanged += this.Series_ValuesChanged;
                this.subscribedSeries.Add(item);
            }
        }
    }

    private void Series_ValuesChanged(object? sender, NotifyCollectionChangedEventArgs e) => this.onSeriesUpdated(null, null, true);

    private void Series_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            this.onSeriesUpdated(null, null, true);
            return;
        }

        var oldItems = new List<ISeries>();
        if ((e.Action == NotifyCollectionChangedAction.Replace || e.Action == NotifyCollectionChangedAction.Remove)
            && e.OldItems != null)
        {
            foreach (ISeries series in e.OldItems)
            {
                series.PropertyChanged -= this.Series_PropertyChanged;
                series.CollectionChanged -= this.Series_ValuesChanged;
                series.Values.CollectionChanged -= this.Series_ValuesChanged;
                oldItems.Add(series);
                this.subscribedSeries.Remove(series);
            }
        }

        var newItems = new List<ISeries>();
        if ((e.Action == NotifyCollectionChangedAction.Replace || e.Action == NotifyCollectionChangedAction.Add)
            && e.NewItems != null)
        {
            foreach (ISeries series in e.NewItems)
            {
                series.PropertyChanged += this.Series_PropertyChanged;
                series.CollectionChanged += this.Series_ValuesChanged;
                series.Values.CollectionChanged += this.Series_ValuesChanged;
                newItems.Add(series);
                this.subscribedSeries.Add(series);
            }
        }

        this.onSeriesUpdated(newItems, oldItems, false);
    }

    private void Series_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is ISeries series)
        {
            var list = new List<ISeries>() { series };
            this.onSeriesUpdated(list, list, false);
        }
    }
}
