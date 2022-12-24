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
        /// <summary>
        /// Event raised when a property of the series has changed.
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs> PropertyChanged;

        /// <summary>
        /// Event raised when the series collection of wedges has changed.
        /// </summary>
        public event EventHandler<NotifyCollectionChangedEventArgs> CollectionChanged;

        /// <summary>
        /// The tooltip formatter of the series.
        /// </summary>
        public Func<IEnumerable<PieWedge>, PieWedge, string> TooltipLabelFormatter { get; set; }

        /// <summary>
        /// The series name.
        /// </summary>
        public string Name { get; set; }

        private ObservableCollection<PieWedge> wedges;
        /// <summary>
        /// The collection of <see cref="PieWedge"/>s the series contains.
        /// </summary>
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
