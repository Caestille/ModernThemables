using ModernThemables.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ModernThemables.HelperClasses.WpfChart
{
	public class PathSeries<TModel> : ISeries where TModel : IChartPoint
	{
		public event EventHandler<PropertyChangedEventArgs> PropertyChanged;
		public event EventHandler<NotifyCollectionChangedEventArgs> CollectionChanged;

		public Func<IEnumerable<IChartPoint>, IChartPoint, string> TooltipLabelFormatter { get; set; }
		public IChartBrush Stroke { get; set; }
		public IChartBrush Fill { get; set; }

		private ObservableCollection<IChartPoint> values;
		public ObservableCollection<IChartPoint> Values
		{
			get => values;
			set
			{
				if (values != null)
				{
					values.CollectionChanged -= Values_CollectionChanged;
				}
				values = value;
				values.CollectionChanged += Values_CollectionChanged;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Values)));
			}
		}

		private void Values_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			CollectionChanged?.Invoke(this, e);
		}
	}
}
