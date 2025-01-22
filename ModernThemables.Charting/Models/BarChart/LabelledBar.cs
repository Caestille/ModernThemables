namespace ModernThemables.Charting.Models.BarChart;

using System.Windows.Media;
using ModernThemables.Charting.Interfaces;
using ModernThemables.Charting.Models.Brushes;

public class LabelledBar : IChartEntity
{
    public LabelledBar(double value, string label, int position)
    {
        this.Name = label;
        this.XValue = position;
        this.YValue = value;
        this.Stroke = new SolidBrush(Colors.Transparent);
        this.Fill = new SolidBrush(Colors.Transparent);
    }

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

    /// <inheritdoc />
    public object XValueToImplementation() => throw new NotImplementedException();

    /// <inheritdoc />
    public object XValueToImplementation(double convert) => throw new NotImplementedException();

    /// <inheritdoc />
    public object YValueToImplementation() => throw new NotImplementedException();

    /// <inheritdoc />
    public object YValueToImplementation(double convert) => throw new NotImplementedException();
}
