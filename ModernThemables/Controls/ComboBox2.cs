namespace ModernThemables.Controls;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

public class ComboBox2 : ComboBox
{
    public static readonly DependencyProperty MouseOverBrushProperty = DependencyProperty.Register(
        nameof(MouseOverBrush),
        typeof(Brush),
        typeof(ComboBox2));

    public static readonly DependencyProperty MouseDownBrushProperty = DependencyProperty.Register(
        nameof(MouseDownBrush),
        typeof(Brush),
        typeof(ComboBox2));

    public static readonly DependencyProperty DisabledBackgroundProperty = DependencyProperty.Register(
        nameof(DisabledBackground),
        typeof(Brush),
        typeof(ComboBox2));

    public static readonly DependencyProperty DisabledForegroundProperty = DependencyProperty.Register(
        nameof(DisabledForeground),
        typeof(Brush),
        typeof(ComboBox2));

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius),
        typeof(CornerRadius),
        typeof(ComboBox2),
        new PropertyMetadata(new CornerRadius(0)));

    public static readonly DependencyProperty ContentBackgroundProperty = DependencyProperty.Register(
        nameof(ContentBackground),
        typeof(Brush),
        typeof(ComboBox2));

    static ComboBox2()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ComboBox2), new FrameworkPropertyMetadata(typeof(ComboBox2)));
    }

    public ComboBox2() { }

    public Brush MouseOverBrush
    {
        get => (Brush)this.GetValue(MouseOverBrushProperty);
        set => this.SetValue(MouseOverBrushProperty, value);
    }

    public Brush MouseDownBrush
    {
        get => (Brush)this.GetValue(MouseDownBrushProperty);
        set => this.SetValue(MouseDownBrushProperty, value);
    }

    public Brush DisabledBackground
    {
        get => (Brush)this.GetValue(DisabledBackgroundProperty);
        set => this.SetValue(DisabledBackgroundProperty, value);
    }

    public Brush DisabledForeground
    {
        get => (Brush)this.GetValue(DisabledForegroundProperty);
        set => this.SetValue(DisabledForegroundProperty, value);
    }

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)this.GetValue(CornerRadiusProperty);
        set => this.SetValue(CornerRadiusProperty, value);
    }

    public Brush ContentBackground
    {
        get => (Brush)this.GetValue(ContentBackgroundProperty);
        set => this.SetValue(ContentBackgroundProperty, value);
    }
}
