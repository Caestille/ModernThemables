using ModernThemables.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ModernThemables.HelperClasses.WpfChart
{
    public class LineSeries<TModel>
    {
        public event EventHandler<PropertyChangedEventArgs> PropertyChanged;
        public Func<IEnumerable<TModel>, TModel, string> TooltipLabelFormatter { get; set; }
        public IChartBrush Stroke { get; set; }
        public IChartBrush Fill { get; set; }

        private IEnumerable<TModel> values;
        public IEnumerable<TModel> Values
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
