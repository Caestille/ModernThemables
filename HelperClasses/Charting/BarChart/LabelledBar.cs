using ModernThemables.Interfaces;
using System;

namespace ModernThemables.HelperClasses.Charting.BarChart
{
	public class LabelledBar : IChartEntity
	{
		/// <inheritdoc />
		public string Name { get; }

		/// <inheritdoc />
		public double XValue { get; }

		/// <inheritdoc />
		public double YValue { get; }

		/// <inheritdoc />
		public IChartBrush Stroke => throw new NotImplementedException();

		/// <inheritdoc />
		public IChartBrush Fill => throw new NotImplementedException();

		/// <inheritdoc />
		public Guid Identifier => throw new NotImplementedException();

		/// <inheritdoc />
		public bool IsFocused { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public LabelledBar(double value, string label, int position)
		{
			Name = label;
			XValue = position;
			YValue = value;
		}

		/// <inheritdoc />
		public event EventHandler<bool> FocusedChanged;

		/// <inheritdoc />
		public object XValueToImplementation()
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public object XValueToImplementation(double convert)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public object YValueToImplementation()
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public object YValueToImplementation(double convert)
		{
			throw new NotImplementedException();
		}
	}
}
