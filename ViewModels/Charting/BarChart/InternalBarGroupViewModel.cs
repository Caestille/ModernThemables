using ModernThemables.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ModernThemables.ViewModels.Charting
{
    /// <summary>
    /// An internal representation of a chart point for rendering the actual series with.
    /// </summary>
    internal class InternalBarGroupViewModel
    {
        /// <summary>
        /// The group name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The bars this group contains.
        /// </summary>
        public ObservableCollection<InternalChartEntity> Bars { get; set; }

		/// <summary>
		/// Initialises a new <see cref="InternalChartEntity"/>.
		/// </summary>
		/// <param name="name">The group name.</param>
		/// <param name="bars">The contained bars</param>
		public InternalBarGroupViewModel(string name, IList<InternalChartEntity> bars)
        {
            Name = name;
            Bars = new ObservableCollection<InternalChartEntity>(bars);
        }
    }
}
