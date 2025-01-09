using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ModernThemables.Charting.Interfaces
{
	/// <summary>
	/// Interface for a generic chart series.
	/// </summary>
	public interface ISeries
    {
        /// <summary>
        /// Event fired when a property of the series changes.
        /// </summary>
        event EventHandler<PropertyChangedEventArgs>? PropertyChanged;

        /// <summary>
        /// Event fired when the series points change.
        /// </summary>
        event EventHandler<NotifyCollectionChangedEventArgs>? CollectionChanged;

        /// <summary>
        /// A <see cref="Func{T1, TResult}"/> Which given a point, returns a string which is the formatted value.
        /// </summary>
        public Func<IChartEntity, string>? ValueFormatter { get; set; }

        /// <summary>
        /// The series line stroke.
        /// </summary>
        IChartBrush? Stroke { get; set; }

        /// <summary>
        /// The series line fill.
        /// </summary>
        IChartBrush? Fill { get; set; }

        /// <summary>
        /// The series values.
        /// </summary>
        ObservableCollection<IChartEntity> Values { get; set; }

        /// <summary>
        /// A unique identifier for the series.
        /// </summary>
        Guid Identifier { get; }

        /// <summary>
        /// The series name.
        /// </summary>
        string? Name { get; set; }
    }
}
