using ModernThemables.Charting.Interfaces;
using System;

namespace ModernThemables.Charting.Models.PieChart
{
    /// <summary>
    /// A point representing a wedge on a pie chart with a <see cref="string"/> name and <see cref="double"/> value
    /// </summary>
    public class PieWedge : IChartEntity
    {
        /// <summary>
        /// The name of the pie wedge.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The value of the pie wedge.
        /// </summary>
        public double Value => XValue;

        /// <inheritdoc />
        public IChartBrush Stroke { get; }

        /// <inheritdoc />
        public IChartBrush Fill { get; }

        /// <inheritdoc />
        public Guid Identifier { get; } = Guid.NewGuid();

        public double XValue { get; }

        public double YValue => throw new NotImplementedException();

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
            XValue = value;
            Stroke = stroke;
            Fill = fill;
        }

        /// <inheritdoc />
        public object XValueToImplementation()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public object YValueToImplementation()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public object XValueToImplementation(double convert)
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
