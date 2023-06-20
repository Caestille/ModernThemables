using System;

namespace ModernThemables.Charting.Models
{
	/// <summary>
	/// A label for axis displays
	/// </summary>
	public struct AxisLabel
	{
		/// <summary>
		/// The actual axis value.
		/// </summary>
		public double Value { get; }

		/// <summary>
		/// A func to format the given value into a display value
		/// </summary>
		public Func<double, string> ValueFormatter { get; }

		/// <summary>
		/// A func to format the given value into a display value
		/// </summary>
		public Func<double, string> IndicatorFormatter { get; }

		/// <summary>
		/// The absolute position of the label along the axis.
		/// </summary>
		public double Location { get; }

		/// <summary>
		/// The actual axis value.
		/// </summary>
		public string FormattedValue { get; }

		/// <summary>
		/// Initialises a new <see cref="AxisLabel"/>.
		/// </summary>
		/// <param name="value">The actual value the formatted value represents if needed.</param>
		/// <param name="location">The absolute position of the label along the axis.</param>
		/// <param name="valueFormatter">The formatter to format the given value into a display value.</param>
		/// <param name="indicatorFormatter">The formatter to format a value into a value to display on the indicator.</param>
		public AxisLabel(double value, double location, Func<double, string> valueFormatter, Func<double, string> indicatorFormatter = null)
		{
			Value = value;
			Location = location;
			ValueFormatter = valueFormatter;
			IndicatorFormatter = indicatorFormatter;
			FormattedValue = valueFormatter(value);
		}
	}
}
