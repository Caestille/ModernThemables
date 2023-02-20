using ModernThemables.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ModernThemables.HelperClasses.Charting
{
    /// <summary>
    /// Generic series.
    /// </summary>
    public class Series : ISeries
    {
        /// <inheritdoc />
        public event EventHandler<PropertyChangedEventArgs> PropertyChanged;

        /// <inheritdoc />
        public event EventHandler<NotifyCollectionChangedEventArgs> CollectionChanged;

        /// <inheritdoc />
        public Func<IEnumerable<IChartEntity>, IChartEntity, string> TooltipLabelFormatter { get; set; }

        /// <inheritdoc />
        public IChartBrush Stroke { get; set; }

        /// <inheritdoc />
        public IChartBrush Fill { get; set; }

        /// <inheritdoc />
        public Guid Identifier { get; } = Guid.NewGuid();

        /// <inheritdoc />
        public string Name { get; set; }

        private ObservableCollection<IChartEntity> values;
        /// <inheritdoc />
        public ObservableCollection<IChartEntity> Values
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
