namespace ModernThemables.Charting.Controls.ChartComponents;

using ModernThemables.Charting.Interfaces;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

/// <summary>
/// Interaction logic for LegendControl.xaml.
/// </summary>
public partial class LegendControl : UserControl
{
    public ObservableCollection<ISeries> Items
    {
        get => (ObservableCollection<ISeries>)this.GetValue(ItemsProperty);
        set => this.SetValue(ItemsProperty, value);
    }
    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
        "Items",
        typeof(ObservableCollection<ISeries>),
        typeof(LegendControl),
        new UIPropertyMetadata(null));

    public DataTemplate LegendTemplate
    {
        get => (DataTemplate)this.GetValue(LegendTemplateProperty);
        set => this.SetValue(LegendTemplateProperty, value);
    }
    public static readonly DependencyProperty LegendTemplateProperty = DependencyProperty.Register(
        "LegendTemplate",
        typeof(DataTemplate),
        typeof(LegendControl),
        new PropertyMetadata(null));

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)this.GetValue(CornerRadiusProperty);
        set => this.SetValue(CornerRadiusProperty, value);
    }
    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        "CornerRadius",
        typeof(CornerRadius),
        typeof(LegendControl),
        new PropertyMetadata(null));

    public object TemplatedDataContext
    {
        get => (DataTemplate)this.GetValue(TemplatedDataContextProperty);
        set => this.SetValue(TemplatedDataContextProperty, value);
    }
    public static readonly DependencyProperty TemplatedDataContextProperty = DependencyProperty.Register(
        "TemplatedDataContext",
        typeof(object),
        typeof(LegendControl),
        new PropertyMetadata(null));

    public Orientation Orientation
    {
        get => (Orientation)this.GetValue(OrientationProperty);
        set => this.SetValue(OrientationProperty, value);
    }
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        "Orientation",
        typeof(Orientation),
        typeof(LegendControl),
        new UIPropertyMetadata(Orientation.Vertical, OnSetLegendOrientation));

    public new Brush Background
    {
        get => (Brush)this.GetValue(BackgroundProperty);
        set => this.SetValue(BackgroundProperty, value);
    }
    public static readonly new DependencyProperty BackgroundProperty = DependencyProperty.Register(
        "Background",
        typeof(Brush),
        typeof(LegendControl),
        new PropertyMetadata(null));

    public new Brush BorderBrush
    {
        get => (Brush)this.GetValue(BorderBrushProperty);
        set => this.SetValue(BorderBrushProperty, value);
    }
    public static readonly new DependencyProperty BorderBrushProperty = DependencyProperty.Register(
        "BorderBrush",
        typeof(Brush),
        typeof(LegendControl),
        new PropertyMetadata(null));

    public new Thickness BorderThickness
    {
        get => (Thickness)this.GetValue(BorderThicknessProperty);
        set => this.SetValue(BorderThicknessProperty, value);
    }
    public static readonly new DependencyProperty BorderThicknessProperty = DependencyProperty.Register(
        "BorderThickness",
        typeof(Thickness),
        typeof(LegendControl),
        new PropertyMetadata(null));

    public LegendControl()
    {
        this.InitializeComponent();
    }

    private static void OnSetLegendOrientation(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not LegendControl _this)
        {
            return;
        }

        switch (_this.Orientation)
        {
            case Orientation.Vertical:
                _this.LegendItemsControl.ItemsPanel = (ItemsPanelTemplate)_this.Resources["StackTemplate"];
                break;
            case Orientation.Horizontal:
                _this.LegendItemsControl.ItemsPanel = (ItemsPanelTemplate)_this.Resources["WrapTemplate"];
                break;
        }
    }
}
