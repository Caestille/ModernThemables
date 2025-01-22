namespace ModernThemables.Controls;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

public class Button2 : Button
{
    private readonly static SolidColorBrush DefaultMouseOverProperty = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFBEE6FD")!;

    public Brush MouseOverBrush
    {
        get => (Brush)this.GetValue(MouseOverBrushProperty);
        set => this.SetValue(MouseOverBrushProperty, value);
    }

    public static readonly DependencyProperty MouseOverBrushProperty = DependencyProperty.Register(
        nameof(MouseOverBrush),
        typeof(Brush),
        typeof(Button2),
        new FrameworkPropertyMetadata(DefaultMouseOverProperty));

    public Brush MouseDownBrush
    {
        get => (Brush)this.GetValue(MouseDownBrushProperty);
        set => this.SetValue(MouseDownBrushProperty, value);
    }

    public static readonly DependencyProperty MouseDownBrushProperty = DependencyProperty.Register(
        nameof(MouseDownBrush),
        typeof(Brush),
        typeof(Button2));

    public Brush DisabledBackground
    {
        get => (Brush)this.GetValue(DisabledBackgroundProperty);
        set => this.SetValue(DisabledBackgroundProperty, value);
    }

    public static readonly DependencyProperty DisabledBackgroundProperty = DependencyProperty.Register(
        nameof(DisabledBackground),
        typeof(Brush),
        typeof(Button2));

    public Brush DisabledForeground
    {
        get => (Brush)this.GetValue(DisabledForegroundProperty);
        set => this.SetValue(DisabledForegroundProperty, value);
    }

    public static readonly DependencyProperty DisabledForegroundProperty = DependencyProperty.Register(
        nameof(DisabledForeground),
        typeof(Brush),
        typeof(Button2));

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)this.GetValue(CornerRadiusProperty);
        set => this.SetValue(CornerRadiusProperty, value);
    }

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius),
        typeof(CornerRadius),
        typeof(Button2),
        new PropertyMetadata(new CornerRadius(0)));
}
