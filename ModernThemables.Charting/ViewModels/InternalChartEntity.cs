namespace ModernThemables.Charting.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using ModernThemables.Charting.Interfaces;

/// <summary>
/// An internal representation of a chart point for rendering the actual series with.
/// </summary>
public class InternalChartEntity : ObservableObject
{
    /// <summary>
    /// The X point in pixels.
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// The Y point in pixels.
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    /// The unscaled point this represents.
    /// </summary>
    public IChartEntity BackingPoint { get; }

    /// <summary>
    /// The <see cref="IChartBrush"/> the path stroke uses to colour itself.
    /// </summary>
    public IChartBrush? Stroke { get; }

    /// <summary>
    /// The <see cref="IChartBrush"/> the path fill uses to colour itself.
    /// </summary>
    public IChartBrush? Fill { get; }

    /// <summary>
    /// The identifier.
    /// </summary>
    public Guid? Identifier { get; set; }

    private bool isMouseOver;
    public bool IsMouseOver
    {
        get => this.isMouseOver;
        set => this.SetProperty(ref this.isMouseOver, value);
    }

    /// <summary>
    /// Initialises a new <see cref="InternalChartEntity"/>.
    /// </summary>
    /// <param name="x">The x point which the point will be plotted on in pixels.</param>
    /// <param name="y">The y point which the point will be plotted on in pixels. </param>
    /// <param name="backingPoint">The actual, unscaled point this represents.</param>
    public InternalChartEntity(double x, double y, IChartEntity backingPoint)
    {
        this.X = x;
        this.Y = y;
        this.BackingPoint = backingPoint;
    }

    /// <summary>
    /// Initialises a new <see cref="InternalChartEntity"/>.
    /// </summary>
    /// <param name="x">The x point which the point will be plotted on in pixels.</param>
    /// <param name="y">The y point which the point will be plotted on in pixels. </param>
    /// <param name="backingPoint">The actual, unscaled point this represents.</param>
    /// <param name="stroke">The <see cref="IChartBrush"/> entity stroke.</param>
    /// <param name="fill">The <see cref="IChartBrush"/> entity fill.</param>
    public InternalChartEntity(double x, double y, IChartEntity backingPoint, IChartBrush? stroke, IChartBrush? fill)
    {
        this.X = x;
        this.Y = y;
        this.BackingPoint = backingPoint;
        this.Stroke = stroke;
        this.Fill = fill;
    }
}
