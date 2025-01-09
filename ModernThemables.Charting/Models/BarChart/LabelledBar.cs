namespace ModernThemables.Charting.Models.BarChart
{
    using ModernThemables.Charting.Interfaces;
    using ModernThemables.Charting.Models.Brushes;
    using System.Windows.Media;

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
            this.Name = label;
            this.XValue = position;
            this.YValue = value;
            this.Stroke = new SolidBrush(Colors.Transparent);
            this.Fill = new SolidBrush(Colors.Transparent);
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
