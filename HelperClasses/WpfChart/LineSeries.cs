using LiveChartsCore.Defaults;
using ModernThemables.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ModernThemables.HelperClasses.WpfChart
{
    public class LineSeries
    {
        public event EventHandler<PropertyChangedEventArgs> PropertyChanged;
        public Func<IEnumerable<DateTimePoint>, DateTimePoint, string> TooltipLabelFormatter { get; set; }
        public IChartBrush Stroke { get; set; }
        public IChartBrush Fill { get; set; }

        private IEnumerable<DateTimePoint> values;
        public IEnumerable<DateTimePoint> Values
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
