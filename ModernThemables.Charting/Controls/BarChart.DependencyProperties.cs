namespace ModernThemables.Charting.Controls;

using System.Collections.ObjectModel;
using System.Windows;
using ModernThemables.Charting.Interfaces;
using ModernThemables.Charting.Models;
using ModernThemables.Charting.ViewModels;

public partial class BarChart // .DependencyProperties
{
    public static readonly DependencyProperty SeriesProperty = DependencyProperty.Register(
        "Series",
        typeof(ObservableCollection<ISeries>),
        typeof(BarChart),
        new FrameworkPropertyMetadata(null, OnSeriesSet));

    public static readonly DependencyProperty YAxisFormatterProperty = DependencyProperty.Register(
        "YAxisFormatter",
        typeof(Func<object, string>),
        typeof(BarChart),
        new PropertyMetadata(null));

    public static readonly DependencyProperty YAxisLabelIdentifierProperty = DependencyProperty.Register(
        "YAxisLabelIdentifier",
        typeof(Func<object, bool>),
        typeof(BarChart),
        new PropertyMetadata(null));

    public static readonly DependencyProperty ShowXSeparatorLinesProperty = DependencyProperty.Register(
        "ShowXSeparatorLines",
        typeof(bool),
        typeof(BarChart),
        new PropertyMetadata(true));

    public static readonly DependencyProperty ShowYSeparatorLinesProperty = DependencyProperty.Register(
        "ShowYSeparatorLines",
        typeof(bool),
        typeof(BarChart),
        new PropertyMetadata(true));

    public static readonly DependencyProperty TooltipTemplateProperty = DependencyProperty.Register(
        "TooltipTemplate",
        typeof(DataTemplate),
        typeof(BarChart),
        new PropertyMetadata(null));

    public static readonly DependencyProperty LegendTemplateProperty = DependencyProperty.Register(
        "LegendTemplate",
        typeof(DataTemplate),
        typeof(BarChart),
        new PropertyMetadata(null));

    public static readonly DependencyProperty LegendLocationProperty = DependencyProperty.Register(
        "LegendLocation",
        typeof(LegendLocation),
        typeof(BarChart),
        new UIPropertyMetadata(LegendLocation.None, OnLegendLocationSet));

    public static readonly DependencyProperty TooltipLocationProperty = DependencyProperty.Register(
        "TooltipLocation",
        typeof(TooltipLocation),
        typeof(BarChart),
        new FrameworkPropertyMetadata(TooltipLocation.Cursor));

    public static readonly DependencyProperty TooltipOpacityProperty = DependencyProperty.Register(
        "TooltipOpacity",
        typeof(double),
        typeof(BarChart),
        new PropertyMetadata(1d));

    public static readonly DependencyProperty BarCornerRadiusFractionProperty = DependencyProperty.Register(
        "BarCornerRadiusFraction",
        typeof(double),
        typeof(BarChart),
        new UIPropertyMetadata(0d, TriggerReRender));

    public static readonly DependencyProperty BarGroupSeparationPixelsProperty = DependencyProperty.Register(
        "BarGroupSeparationPixels",
        typeof(double),
        typeof(BarChart),
        new UIPropertyMetadata(0d, TriggerReRender));

    public static readonly DependencyProperty BarSeparationPixelsProperty = DependencyProperty.Register(
        "BarSeparationPixels",
        typeof(double),
        typeof(BarChart),
        new UIPropertyMetadata(0d, TriggerReRender));

    public static readonly DependencyProperty XAxisLabelRotationProperty = DependencyProperty.Register(
        "XAxisLabelRotation",
        typeof(double),
        typeof(BarChart),
        new UIPropertyMetadata(0d, TriggerReRender));

    public static readonly DependencyProperty InternalSeriesProperty = DependencyProperty.Register(
        "InternalSeries",
        typeof(ObservableCollection<InternalChartEntity>),
        typeof(BarChart),
        new PropertyMetadata(new ObservableCollection<InternalChartEntity>()));

    public static readonly DependencyProperty XAxisLabelsProperty = DependencyProperty.Register(
        "XAxisLabels",
        typeof(ObservableCollection<AxisLabel>),
        typeof(BarChart),
        new PropertyMetadata(new ObservableCollection<AxisLabel>()));

    public static readonly DependencyProperty YAxisLabelsProperty = DependencyProperty.Register(
        "YAxisLabels",
        typeof(ObservableCollection<AxisLabel>),
        typeof(BarChart),
        new PropertyMetadata(new ObservableCollection<AxisLabel>()));

    public static readonly DependencyProperty BarWidthProperty = DependencyProperty.Register(
        "BarWidth",
        typeof(double),
        typeof(BarChart),
        new PropertyMetadata(0d));

    public static readonly DependencyProperty GroupWidthProperty = DependencyProperty.Register(
        "GroupWidth",
        typeof(double),
        typeof(BarChart),
        new PropertyMetadata(0d));

    public static readonly DependencyProperty BarCornerRadiusProperty = DependencyProperty.Register(
        "BarCornerRadius",
        typeof(CornerRadius),
        typeof(BarChart),
        new PropertyMetadata(new CornerRadius(0)));

    public static readonly DependencyProperty TooltipGetterFuncProperty = DependencyProperty.Register(
        "TooltipGetterFunc",
        typeof(Func<Point, IEnumerable<TooltipViewModel>>),
        typeof(BarChart),
        new PropertyMetadata(null));

    public ObservableCollection<ISeries> Series
    {
        get => (ObservableCollection<ISeries>)this.GetValue(SeriesProperty);
        set => this.SetValue(SeriesProperty, value);
    }

    public Func<object, string> YAxisFormatter
    {
        get => (Func<object, string>)this.GetValue(YAxisFormatterProperty);
        set => this.SetValue(YAxisFormatterProperty, value);
    }

    public Func<object, bool> YAxisLabelIdentifier
    {
        get => (Func<object, bool>)this.GetValue(YAxisLabelIdentifierProperty);
        set => this.SetValue(YAxisLabelIdentifierProperty, value);
    }

    public bool ShowXSeparatorLines
    {
        get => (bool)this.GetValue(ShowXSeparatorLinesProperty);
        set => this.SetValue(ShowXSeparatorLinesProperty, value);
    }

    public bool ShowYSeparatorLines
    {
        get => (bool)this.GetValue(ShowYSeparatorLinesProperty);
        set => this.SetValue(ShowYSeparatorLinesProperty, value);
    }

    public DataTemplate TooltipTemplate
    {
        get => (DataTemplate)this.GetValue(TooltipTemplateProperty);
        set => this.SetValue(TooltipTemplateProperty, value);
    }

    public DataTemplate LegendTemplate
    {
        get => (DataTemplate)this.GetValue(LegendTemplateProperty);
        set => this.SetValue(LegendTemplateProperty, value);
    }

    public LegendLocation LegendLocation
    {
        get => (LegendLocation)this.GetValue(LegendLocationProperty);
        set => this.SetValue(LegendLocationProperty, value);
    }

    public TooltipLocation TooltipLocation
    {
        get => (TooltipLocation)this.GetValue(TooltipLocationProperty);
        set => this.SetValue(TooltipLocationProperty, value);
    }

    public double TooltipOpacity
    {
        get => (double)this.GetValue(TooltipOpacityProperty);
        set => this.SetValue(TooltipOpacityProperty, value);
    }

    public double BarCornerRadiusFraction
    {
        get => (double)this.GetValue(BarCornerRadiusFractionProperty);
        set => this.SetValue(BarCornerRadiusFractionProperty, value);
    }

    public double BarGroupSeparationPixels
    {
        get => (double)this.GetValue(BarGroupSeparationPixelsProperty);
        set => this.SetValue(BarGroupSeparationPixelsProperty, value);
    }

    public double BarSeparationPixels
    {
        get => (double)this.GetValue(BarSeparationPixelsProperty);
        set => this.SetValue(BarSeparationPixelsProperty, value);
    }

    public double XAxisLabelRotation
    {
        get => (double)this.GetValue(XAxisLabelRotationProperty);
        set => this.SetValue(XAxisLabelRotationProperty, value);
    }

    private ObservableCollection<InternalChartEntity> InternalSeries
    {
        get => (ObservableCollection<InternalChartEntity>)this.GetValue(InternalSeriesProperty);
        set => this.SetValue(InternalSeriesProperty, value);
    }

    private ObservableCollection<AxisLabel> XAxisLabels
    {
        get => (ObservableCollection<AxisLabel>)this.GetValue(XAxisLabelsProperty);
        set => this.SetValue(XAxisLabelsProperty, value);
    }

    private ObservableCollection<AxisLabel> YAxisLabels
    {
        get => (ObservableCollection<AxisLabel>)this.GetValue(YAxisLabelsProperty);
        set => this.SetValue(YAxisLabelsProperty, value);
    }

    private double BarWidth
    {
        get => (double)this.GetValue(BarWidthProperty);
        set => this.SetValue(BarWidthProperty, value);
    }

    private double GroupWidth
    {
        get => (double)this.GetValue(GroupWidthProperty);
        set => this.SetValue(GroupWidthProperty, value);
    }

    private CornerRadius BarCornerRadius
    {
        get => (CornerRadius)this.GetValue(BarCornerRadiusProperty);
        set => this.SetValue(BarCornerRadiusProperty, value);
    }

    private Func<Point, IEnumerable<TooltipViewModel>> TooltipGetterFunc
    {
        get => (Func<Point, IEnumerable<TooltipViewModel>>)this.GetValue(TooltipGetterFuncProperty);
        set => this.SetValue(TooltipGetterFuncProperty, value);
    }
}
