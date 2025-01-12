namespace ModernThemables.Charting.ViewModels.PieChart;

using CommunityToolkit.Mvvm.ComponentModel;
using ModernThemables.Charting.Interfaces;

/// <summary>
/// An internal representation of a chart point for rendering the actual series with.
/// </summary>
internal class InternalPieWedgeViewModel : ObservableObject
{
    private string? name;
    /// <summary>
    /// The name of the pie wedge.
    /// </summary>
    public string? Name
    {
        get => this.name;
        set => this.SetProperty(ref this.name, value);
    }

    private double percent;
    /// <double>
    /// The value of the pie wedge in percent of a full circle.
    /// </summary>
    public double Percent
    {
        get => this.percent;
        set => this.SetProperty(ref this.percent, value);
    }

    private double val;
    /// <double>
    /// The value of the pie wedge.
    /// </summary>
    public double Value
    {
        get => this.val;
        set => this.SetProperty(ref this.val, value);
    }

    private double startAngle;
    /// <summary>
    /// The start angle of the wedge.
    /// </summary>
    public double StartAngle
    {
        get => this.startAngle;
        set => this.SetProperty(ref this.startAngle, value);
    }

    private IChartBrush? stroke;
    /// <summary>
    /// The wedge stroke.
    /// </summary>
    public IChartBrush? Stroke
    {
        get => this.stroke;
        set => this.SetProperty(ref this.stroke, value);
    }

    private IChartBrush? fill;
    /// <summary>
    /// The wedge fill.
    /// </summary>
    public IChartBrush? Fill
    {
        get => this.fill;
        set => this.SetProperty(ref this.fill, value);
    }

    private Guid identifier;
    /// <summary>
    /// The wedge unique identifier.
    /// </summary>
    public Guid Identifier
    {
        get => this.identifier;
        set => this.SetProperty(ref this.identifier, value);
    }

    private bool resizeTrigger;
    /// <summary>
    /// A <see cref="bool"/> property used to for the series to resize itself when desired by triggering a
    /// converter.
    /// </summary>
    public bool ResizeTrigger
    {
        get => this.resizeTrigger;
        set => this.SetProperty(ref this.resizeTrigger, value);
    }

    private bool isMouseOver;
    /// <summary>
    /// A <see cref="bool"/> indicating whether the mouse is over this wedge.
    /// </summary>
    public bool IsMouseOver
    {
        get => this.isMouseOver;
        set => this.SetProperty(ref this.isMouseOver, value);
    }

    /// <summary>
    /// Initialises a new <see cref="InternalPieWedgeViewModel"/>.
    /// </summary>
    /// <param name="name">The wedge name.</param>
    /// <param name="percent">The wedge value in percent of a full circle.</param>
    /// <param name="value">The wedge value.</param>
    /// <param name="startAngle">The wedge start angle.</param>
    /// <param name="stroke">The wedge stroke.</param>
    /// <param name="fill">The wedge fill.</param>
    public InternalPieWedgeViewModel(string name, Guid identifier, double percent, double value, double startAngle, IChartBrush? stroke, IChartBrush? fill)
    {
        this.Name = name;
        this.Identifier = identifier;
        this.Percent = percent;
        this.Value = value;
        this.StartAngle = startAngle;
        this.Stroke = stroke;
        this.Fill = fill;
    }
}
