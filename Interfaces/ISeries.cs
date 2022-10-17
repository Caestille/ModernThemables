using ModernThemables.HelperClasses.WpfChart;
using System;
using System.Collections.Generic;
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
		IEnumerable<IChartPoint> Values { get; set; }
	}
}
