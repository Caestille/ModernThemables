using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ModernThemables.Interfaces
{
	public interface ISeries
	{
		event EventHandler<PropertyChangedEventArgs> PropertyChanged;
		event EventHandler<NotifyCollectionChangedEventArgs> CollectionChanged;
		Func<IEnumerable<IChartPoint>, IChartPoint, string> TooltipLabelFormatter { get; set; }
		IChartBrush Stroke { get; set; }
		IChartBrush Fill { get; set; }
		ObservableCollection<IChartPoint> Values { get; set; }
		Guid Identifier { get; }
		string Name { get; set; }
	}
}
