using ModernThemables.Interfaces;
using System;

namespace ModernThemables.HelperClasses.Charting.PieChart
{
	/// <summary>
	/// A point representing a wedge on a pie chart with a <see cref="string"/> name and <see cref="double"/> value
	/// </summary>
	public class PieWedge
	{
		public event EventHandler<bool> FocusedChanged;

		/// <summary>
		/// The name of the pie wedge.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// The value of the pie wedge.
		/// </summary>
		public double Value { get; }

		/// <inheritdoc />
		public IChartBrush Stroke { get; }

		/// <inheritdoc />
		public IChartBrush Fill { get; }

		/// <inheritdoc />
		public Guid Identifier { get; } = Guid.NewGuid();

		private bool isFocused;
		public bool IsFocused
		{
			get => isFocused;
			set
			{
				isFocused = value;
				FocusedChanged?.Invoke(this, value);
			}
		}

		/// <summary>
		/// Initialises a new <see cref="PieWedge"/> with a given <see cref="string"/> and
		/// <see cref="double"/> name and value.
		/// </summary>
		/// <param name="name">The input name.</param>
		/// <param name="value">The input value.</param>
		/// <param name="stroke">The wedge stroke.</param>
		/// <param name="fill">The wedge fill.</param>
		public PieWedge(string name, double value, IChartBrush stroke, IChartBrush fill)
		{
			Name = name;
			Value = value;
			Stroke = stroke;
			Fill = fill;
		}
	}
}
