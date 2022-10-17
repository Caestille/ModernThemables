using ModernThemables.Interfaces;
using System;
using System.Collections.Generic;
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

        private IEnumerable<IChartPoint> values;
        public IEnumerable<IChartPoint> Values
        {
            get => values;
            set
            {
                values = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Values)));
            }
        }
    }
}
