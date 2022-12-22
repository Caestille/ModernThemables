using ModernThemables.HelperClasses.Charting.PieChart.Points;
using ModernThemables.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ModernThemables.HelperClasses.Charting.PieChart
{
    /// <summary>
    /// Series describing a set of pie wedges on a pie chart.
    /// </summary>
    /// <typeparam name="TModel">The point type to model.</typeparam>
    public class PieSeries
    {
        /// <inheritdoc />
        public event EventHandler<PropertyChangedEventArgs> PropertyChanged;

        /// <inheritdoc />
        public event EventHandler<NotifyCollectionChangedEventArgs> CollectionChanged;

        /// <inheritdoc />
        public Func<IEnumerable<PieWedge>, PieWedge, string> TooltipLabelFormatter { get; set; }

        /// <inheritdoc />
        public string Name { get; set; }


        private ObservableCollection<PieWedge> wedges;
        /// <inheritdoc />
        public ObservableCollection<PieWedge> Wedges
        {
            get => wedges;
            set
            {
                if (this.wedges != null)
                {
                    this.wedges.CollectionChanged -= Values_CollectionChanged;
                }
                this.wedges = value;
                this.wedges.CollectionChanged += Values_CollectionChanged;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Wedges)));
            }
        }

        private void Values_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }
    }
}
