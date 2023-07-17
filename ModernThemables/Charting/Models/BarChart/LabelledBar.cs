using ModernThemables.Charting.Interfaces;
using System;

namespace ModernThemables.Charting.Models.BarChart
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
        public IChartBrush Stroke { get; }

        /// <inheritdoc />
        public IChartBrush Fill { get; }

        /// <inheritdoc />
        public Guid Identifier { get; } = Guid.NewGuid();

        public LabelledBar(double value, string label, int position)
        {
            Name = label;
            XValue = position;
            YValue = value;
        }

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
