namespace ModernThemables.Charting.Models.CartesianChart;

using ModernThemables.Charting.Interfaces;

/// <summary>
/// A point representing a <see cref="double"/> in the Y axis and a <see cref="System.DateTime"/> on the X axis.
/// </summary>
public class DateTimePoint : IChartEntity
{
    /// <summary>
    /// Initialises a new <see cref="DateTimePoint"/> with a given <see cref="System.DateTime"/> and
    /// <see cref="double"/>.
    /// </summary>
    /// <param name="dateTime">The input <see cref="System.DateTime"/> X axis value.</param>
    /// <param name="value">The input <see cref="double"/> Y axis value.</param>
    public DateTimePoint(DateTime dateTime, double value)
    {
        this.DateTime = dateTime;
        this.Value = value;
    }

    /// <inheritdoc />
    public string Name => throw new NotImplementedException();

    /// <summary>
    /// The <see cref="System.DateTime"/> X axis value.
    /// </summary>
    public DateTime DateTime { get; }

    /// <summary>
    /// The <see cref="double"/> Y Axis value.
    /// </summary>
    public double Value { get; }

    /// <inheritdoc />
    public double XValue => new TimeSpan(this.DateTime.Ticks).TotalDays;

    /// <inheritdoc />
    public double YValue => this.Value;

    /// <inheritdoc />
    public IChartBrush Stroke => throw new NotImplementedException();

    /// <inheritdoc />
    public IChartBrush Fill => throw new NotImplementedException();

    /// <inheritdoc />
    public Guid Identifier => throw new NotImplementedException();

    /// <inheritdoc />
    public bool IsFocused { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    /// <inheritdoc />
    public object XValueToImplementation() => new DateTime(TimeSpan.FromDays(this.XValue).Ticks);

    /// <inheritdoc />
    public object YValueToImplementation() => this.YValue;

    /// <inheritdoc />
    public object XValueToImplementation(double convert) => new DateTime(TimeSpan.FromDays(convert).Ticks);

    /// <inheritdoc />
    public object YValueToImplementation(double convert) => convert;
}
