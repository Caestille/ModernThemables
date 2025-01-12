namespace ModernThemables.Charting.Controls;

using ModernThemables.Charting.Interfaces;
using ModernThemables.Charting.Models;
using ModernThemables.Charting.ViewModels.CartesianChart;
using System.Collections.ObjectModel;
using System.Windows;

public partial class CartesianChart // .DependencyProperties
{
    #region Public properties

    public ObservableCollection<ISeries> Series
    {
        get => (ObservableCollection<ISeries>)this.GetValue(SeriesProperty);
        set => this.SetValue(SeriesProperty, value);
    }
    public static readonly DependencyProperty SeriesProperty = DependencyProperty.Register(
        "Series",
        typeof(ObservableCollection<ISeries>),
        typeof(CartesianChart),
        new FrameworkPropertyMetadata(null, OnSeriesSet));

    public Func<object, string> XAxisFormatter
    {
        get => (Func<object, string>)this.GetValue(XAxisFormatterProperty);
        set => this.SetValue(XAxisFormatterProperty, value);
    }
    public static readonly DependencyProperty XAxisFormatterProperty = DependencyProperty.Register(
        "XAxisFormatter",
        typeof(Func<object, string>),
        typeof(CartesianChart),
        new PropertyMetadata(null));

    public Func<object, string> XAxisCursorLabelFormatter
    {
        get => (Func<object, string>)this.GetValue(XAxisCursorLabelFormatterProperty);
        set => this.SetValue(XAxisCursorLabelFormatterProperty, value);
    }
    public static readonly DependencyProperty XAxisCursorLabelFormatterProperty = DependencyProperty.Register(
        "XAxisCursorLabelFormatter",
        typeof(Func<object, string>),
        typeof(CartesianChart),
        new PropertyMetadata(null));

    public Func<object, string> YAxisFormatter
    {
        get => (Func<object, string>)this.GetValue(YAxisFormatterProperty);
        set => this.SetValue(YAxisFormatterProperty, value);
    }
    public static readonly DependencyProperty YAxisFormatterProperty = DependencyProperty.Register(
        "YAxisFormatter",
        typeof(Func<object, string>),
        typeof(CartesianChart),
        new PropertyMetadata(null));

    public Func<object, string> YAxisCursorLabelFormatter
    {
        get => (Func<object, string>)this.GetValue(YAxisCursorLabelFormatterProperty);
        set => this.SetValue(YAxisCursorLabelFormatterProperty, value);
    }
    public static readonly DependencyProperty YAxisCursorLabelFormatterProperty = DependencyProperty.Register(
        "YAxisCursorLabelFormatter",
        typeof(Func<object, string>),
        typeof(CartesianChart),
        new PropertyMetadata(null));

    public Func<object, bool> YAxisLabelIdentifier
    {
        get => (Func<object, bool>)this.GetValue(YAxisLabelIdentifierProperty);
        set => this.SetValue(YAxisLabelIdentifierProperty, value);
    }
    public static readonly DependencyProperty YAxisLabelIdentifierProperty = DependencyProperty.Register(
        "YAxisLabelIdentifier",
        typeof(Func<object, bool>),
        typeof(CartesianChart),
        new PropertyMetadata(null));

    public Func<object, bool> XAxisLabelIdentifier
    {
        get => (Func<object, bool>)this.GetValue(XAxisLabelIdentifierProperty);
        set => this.SetValue(XAxisLabelIdentifierProperty, value);
    }
    public static readonly DependencyProperty XAxisLabelIdentifierProperty = DependencyProperty.Register(
        "XAxisLabelIdentifier",
        typeof(Func<object, bool>),
        typeof(CartesianChart),
        new PropertyMetadata(null));

    public bool ShowXSeparatorLines
    {
        get => (bool)this.GetValue(ShowXSeparatorLinesProperty);
        set => this.SetValue(ShowXSeparatorLinesProperty, value);
    }
    public static readonly DependencyProperty ShowXSeparatorLinesProperty = DependencyProperty.Register(
        "ShowXSeparatorLines",
        typeof(bool),
        typeof(CartesianChart),
        new PropertyMetadata(true));

    public bool ShowYSeparatorLines
    {
        get => (bool)this.GetValue(ShowYSeparatorLinesProperty);
        set => this.SetValue(ShowYSeparatorLinesProperty, value);
    }
    public static readonly DependencyProperty ShowYSeparatorLinesProperty = DependencyProperty.Register(
        "ShowYSeparatorLines",
        typeof(bool),
        typeof(CartesianChart),
        new PropertyMetadata(true));

    public bool IsZoomed
    {
        get => (bool)this.GetValue(IsZoomedProperty);
        private set => this.SetValue(IsZoomedProperty, value);
    }
    public static readonly DependencyProperty IsZoomedProperty = DependencyProperty.Register(
        "IsZoomed",
        typeof(bool),
        typeof(CartesianChart),
        new PropertyMetadata(false));

    public DataTemplate TooltipTemplate
    {
        get => (DataTemplate)this.GetValue(TooltipTemplateProperty);
        set => this.SetValue(TooltipTemplateProperty, value);
    }
    public static readonly DependencyProperty TooltipTemplateProperty = DependencyProperty.Register(
        "TooltipTemplate",
        typeof(DataTemplate),
        typeof(CartesianChart),
        new PropertyMetadata(null));

    public DataTemplate LegendTemplate
    {
        get => (DataTemplate)this.GetValue(LegendTemplateProperty);
        set => this.SetValue(LegendTemplateProperty, value);
    }
    public static readonly DependencyProperty LegendTemplateProperty = DependencyProperty.Register(
        "LegendTemplate",
        typeof(DataTemplate),
        typeof(CartesianChart),
        new PropertyMetadata(null));

    public LegendLocation LegendLocation
    {
        get => (LegendLocation)this.GetValue(LegendLocationProperty);
        set => this.SetValue(LegendLocationProperty, value);
    }
    public static readonly DependencyProperty LegendLocationProperty = DependencyProperty.Register(
        "LegendLocation",
        typeof(LegendLocation),
        typeof(CartesianChart),
        new UIPropertyMetadata(LegendLocation.None, OnLegendLocationSet));

    public TooltipFindingStrategy TooltipFindingStrategy
    {
        get => (TooltipFindingStrategy)this.GetValue(TooltipFindingStrategyProperty);
        set => this.SetValue(TooltipFindingStrategyProperty, value);
    }
    public static readonly DependencyProperty TooltipFindingStrategyProperty = DependencyProperty.Register(
        "TooltipFindingStrategy",
        typeof(TooltipFindingStrategy),
        typeof(CartesianChart),
        new PropertyMetadata(TooltipFindingStrategy.NearestXAllY));

    public TooltipLocation TooltipLocation
    {
        get => (TooltipLocation)this.GetValue(TooltipLocationProperty);
        set => this.SetValue(TooltipLocationProperty, value);
    }
    public static readonly DependencyProperty TooltipLocationProperty = DependencyProperty.Register(
        "TooltipLocation",
        typeof(TooltipLocation),
        typeof(CartesianChart),
        new FrameworkPropertyMetadata(TooltipLocation.Cursor));

    public double TooltipLocationThreshold
    {
        get => (double)this.GetValue(TooltipLocationThresholdProperty);
        set => this.SetValue(TooltipLocationThresholdProperty, value);
    }
    public static readonly DependencyProperty TooltipLocationThresholdProperty = DependencyProperty.Register(
        "TooltipLocationThreshold",
        typeof(double),
        typeof(CartesianChart),
        new PropertyMetadata(5d));

    public double TooltipOpacity
    {
        get => (double)this.GetValue(TooltipOpacityProperty);
        set => this.SetValue(TooltipOpacityProperty, value);
    }
    public static readonly DependencyProperty TooltipOpacityProperty = DependencyProperty.Register(
        "TooltipOpacity",
        typeof(double),
        typeof(CartesianChart),
        new PropertyMetadata(1d));

    public double YPaddingFrac
    {
        get => (double)this.GetValue(YPaddingFracProperty);
        set => this.SetValue(YPaddingFracProperty, value);
    }
    public static readonly DependencyProperty YPaddingFracProperty = DependencyProperty.Register(
        "YPaddingFrac",
        typeof(double),
        typeof(CartesianChart),
        new PropertyMetadata(0.1d));

    public Func<IEnumerable<IChartEntity>, IChartEntity, object> TooltipContentGetter
    {
        get => (Func<IEnumerable<IChartEntity>, IChartEntity, object>)this.GetValue(TooltipContentGetterProperty);
        set => this.SetValue(TooltipContentGetterProperty, value);
    }
    public static readonly DependencyProperty TooltipContentGetterProperty = DependencyProperty.Register(
        "TooltipContentGetter",
        typeof(Func<IEnumerable<IChartEntity>, IChartEntity, object>),
        typeof(CartesianChart),
        new PropertyMetadata(null));

    #endregion

    #region Private properties

    private ObservableCollection<InternalPathSeriesViewModel> InternalSeries
    {
        get => (ObservableCollection<InternalPathSeriesViewModel>)this.GetValue(InternalSeriesProperty);
        set => this.SetValue(InternalSeriesProperty, value);
    }
    public static readonly DependencyProperty InternalSeriesProperty = DependencyProperty.Register(
        "InternalSeries",
        typeof(ObservableCollection<InternalPathSeriesViewModel>),
        typeof(CartesianChart),
        new PropertyMetadata(new ObservableCollection<InternalPathSeriesViewModel>()));

    private ObservableCollection<AxisLabel> XAxisLabels
    {
        get => (ObservableCollection<AxisLabel>)this.GetValue(XAxisLabelsProperty);
        set => this.SetValue(XAxisLabelsProperty, value);
    }
    public static readonly DependencyProperty XAxisLabelsProperty = DependencyProperty.Register(
        "XAxisLabels",
        typeof(ObservableCollection<AxisLabel>),
        typeof(CartesianChart),
        new PropertyMetadata(new ObservableCollection<AxisLabel>()));

    private ObservableCollection<AxisLabel> YAxisLabels
    {
        get => (ObservableCollection<AxisLabel>)this.GetValue(YAxisLabelsProperty);
        set => this.SetValue(YAxisLabelsProperty, value);
    }
    public static readonly DependencyProperty YAxisLabelsProperty = DependencyProperty.Register(
        "YAxisLabels",
        typeof(ObservableCollection<AxisLabel>),
        typeof(CartesianChart),
        new PropertyMetadata(new ObservableCollection<AxisLabel>()));
    #endregion
}
