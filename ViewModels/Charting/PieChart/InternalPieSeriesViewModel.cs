using Microsoft.Toolkit.Mvvm.ComponentModel;
using ModernThemables.HelperClasses.Charting.PieChart;
using ModernThemables.ViewModels.Charting.PieChart;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ModernThemables.ViewModels.Charting.CartesianChart
{
	/// <summary>
	/// A view model for an internal representation of a series used by the <see cref="CartesianChart"/>.
	/// </summary>
	internal class InternalPieSeriesViewModel : ObservableObject
	{
		private ObservableCollection<InternalPieWedge> wedges;

		/// <summary>
		/// The data making up the rendered points in pixels scale.
		/// </summary>
		public ObservableCollection<InternalPieWedge> Wedges
		{
			get => wedges;
			set => SetProperty(ref wedges, value);
		}

		/// <summary>
		/// The series name for the legend.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// A <see cref="Func{T1, T2, TResult}"/> used to format the tooltip string.
		/// </summary>
		public Func<IEnumerable<PieWedge>, PieWedge, string> TooltipLabelFormatter;

		/// <summary>
		/// Initialises a new <see cref="InternalPieSeriesViewModel"/>.
		/// </summary>
		/// <param name="name">The series name.</param>
		/// <param name="wedges">The data this series represents.</param>
		/// <param name="tooltipFormatter">The Func used to format the tooltip string.</param>
		public InternalPieSeriesViewModel(
			string name,
			ObservableCollection<InternalPieWedge> wedges,
			Func<IEnumerable<PieWedge>, PieWedge, string> tooltipFormatter)
		{
			Name = name;
			Wedges = wedges;
			TooltipLabelFormatter = tooltipFormatter;
		}
	}
}
