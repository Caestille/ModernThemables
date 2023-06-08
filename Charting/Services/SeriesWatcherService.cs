using ModernThemables.Charting.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModernThemables.Charting.Services
{
	public class SeriesWatcherService
	{
		private Action<IList<ISeries>, IList<ISeries>, bool> onSeriesUpdated;
		private bool hasSetSeries;
		private List<ISeries> subscribedSeries = new();

		public SeriesWatcherService(Action<IList<ISeries>, IList<ISeries>, bool> onSeriesUpdated)
		{
			this.onSeriesUpdated = onSeriesUpdated;
		}

		public void ProvideSeries(ObservableCollection<ISeries> newSeries)
		{
			if (newSeries == null)
			{
				foreach (var series in subscribedSeries)
				{
					series.PropertyChanged -= Series_PropertyChanged;
				}

				return;
			}

			Subscribe(newSeries);
			hasSetSeries = true;

			if (!newSeries.Any() || !newSeries.Any(x => x.Values.Any())) return;

			onSeriesUpdated(null, null, true);
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
				onSeriesUpdated(null, null, true);
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

			onSeriesUpdated(newItems, oldItems, false);
		}

		private async void Series_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (sender is ISeries series)
			{
				var list = new List<ISeries>() { series };
				onSeriesUpdated(list, list, false);
			}
		}

		public void Dispose()
		{
			foreach (var series in subscribedSeries)
			{
				series.PropertyChanged -= Series_PropertyChanged;
			}
		}
	}
}
