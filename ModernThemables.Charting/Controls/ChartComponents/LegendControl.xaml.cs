namespace ModernThemables.Charting.Controls.ChartComponents;

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ModernThemables.Charting.Interfaces;

/// <summary>
/// Interaction logic for LegendControl.xaml.
/// </summary>
public partial class LegendControl : UserControl
{
    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
        nameof(Items),
        typeof(ObservableCollection<ISeries>),
        typeof(LegendControl),
        new UIPropertyMetadata(null));

    public static readonly DependencyProperty LegendTemplateProperty = DependencyProperty.Register(
        nameof(LegendTemplate),
        typeof(DataTemplate),
        typeof(LegendControl),
        new PropertyMetadata(null));

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius),
        typeof(CornerRadius),
        typeof(LegendControl),
        new PropertyMetadata(null));

    public static readonly DependencyProperty TemplatedDataContextProperty = DependencyProperty.Register(
        nameof(TemplatedDataContext),
        typeof(object),
        typeof(LegendControl),
        new PropertyMetadata(null));

    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation),
        typeof(Orientation),
        typeof(LegendControl),
        new UIPropertyMetadata(Orientation.Vertical, OnSetLegendOrientation));

    public static readonly new DependencyProperty BackgroundProperty = DependencyProperty.Register(
        nameof(Background),
        typeof(Brush),
        typeof(LegendControl),
        new PropertyMetadata(null));

    public static readonly new DependencyProperty BorderBrushProperty = DependencyProperty.Register(
        nameof(BorderBrush),
        typeof(Brush),
        typeof(LegendControl),
        new PropertyMetadata(null));

    public static readonly new DependencyProperty BorderThicknessProperty = DependencyProperty.Register(
        nameof(BorderThickness),
        typeof(Thickness),
        typeof(LegendControl),
        new PropertyMetadata(null));

    public LegendControl()
    {
        this.InitializeComponent();
    }

    public ObservableCollection<ISeries> Items
    {
        get => (ObservableCollection<ISeries>)this.GetValue(ItemsProperty);
        set => this.SetValue(ItemsProperty, value);
    }

    public DataTemplate LegendTemplate
    {
        get => (DataTemplate)this.GetValue(LegendTemplateProperty);
        set => this.SetValue(LegendTemplateProperty, value);
    }

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)this.GetValue(CornerRadiusProperty);
        set => this.SetValue(CornerRadiusProperty, value);
    }

    public object TemplatedDataContext
    {
        get => (DataTemplate)this.GetValue(TemplatedDataContextProperty);
        set => this.SetValue(TemplatedDataContextProperty, value);
    }

    public Orientation Orientation
    {
        get => (Orientation)this.GetValue(OrientationProperty);
        set => this.SetValue(OrientationProperty, value);
    }

    public new Brush Background
    {
        get => (Brush)this.GetValue(BackgroundProperty);
        set => this.SetValue(BackgroundProperty, value);
    }

    public new Brush BorderBrush
    {
        get => (Brush)this.GetValue(BorderBrushProperty);
        set => this.SetValue(BorderBrushProperty, value);
    }

    public new Thickness BorderThickness
    {
        get => (Thickness)this.GetValue(BorderThicknessProperty);
        set => this.SetValue(BorderThicknessProperty, value);
    }

    private static void OnSetLegendOrientation(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not LegendControl control)
        {
            return;
        }

        switch (control.Orientation)
        {
            case Orientation.Vertical:
                control.LegendItemsControl.ItemsPanel = (ItemsPanelTemplate)control.Resources["StackTemplate"];
                break;
            case Orientation.Horizontal:
                control.LegendItemsControl.ItemsPanel = (ItemsPanelTemplate)control.Resources["WrapTemplate"];
                break;
        }
    }
}
