namespace ModernThemables.Controls;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

public class Slider2 : Slider
{
    public static readonly DependencyProperty ThumbBrushProperty = DependencyProperty.Register(
        nameof(ThumbBrush),
        typeof(Brush),
        typeof(Slider2),
        new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Red)));

    public static readonly DependencyProperty ThumbMouseOverBrushProperty = DependencyProperty.Register(
        nameof(ThumbMouseOverBrush),
        typeof(Brush),
        typeof(Slider2),
        new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Red)));

    public static readonly DependencyProperty ThumbMouseDownBrushProperty = DependencyProperty.Register(
        nameof(ThumbMouseDownBrush),
        typeof(Brush),
        typeof(Slider2),
        new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Red)));

    public static readonly DependencyProperty ThumbBorderBrushProperty = DependencyProperty.Register(
        nameof(ThumbBorderBrush),
        typeof(Brush),
        typeof(Slider2),
        new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Red)));

    public static readonly DependencyProperty ThumbBorderThicknessProperty = DependencyProperty.Register(
        nameof(ThumbBorderThickness),
        typeof(int),
        typeof(Slider2),
        new FrameworkPropertyMetadata(1));

    public static readonly DependencyProperty ActiveBarBrushProperty = DependencyProperty.Register(
        nameof(ActiveBarBrush),
        typeof(Brush),
        typeof(Slider2),
        new FrameworkPropertyMetadata(new SolidColorBrush(Colors.PaleVioletRed)));

    public static readonly DependencyProperty InactiveBarBrushProperty = DependencyProperty.Register(
        nameof(InactiveBarBrush),
        typeof(Brush),
        typeof(Slider2),
        new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Gray)));

    public static readonly DependencyProperty ThumbCornerRadiusProperty = DependencyProperty.Register(
        nameof(ThumbCornerRadius),
        typeof(CornerRadius),
        typeof(Slider2),
        new PropertyMetadata(new CornerRadius(0)));

    public static readonly DependencyProperty BarCornerRadiusProperty = DependencyProperty.Register(
        nameof(BarCornerRadius),
        typeof(CornerRadius),
        typeof(Slider2),
        new PropertyMetadata(new CornerRadius(0)));

    static Slider2()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Slider2), new FrameworkPropertyMetadata(typeof(Slider2)));
    }

    public Slider2() { }

    public Brush ThumbBrush
    {
        get => (Brush)this.GetValue(ThumbBrushProperty);
        set => this.SetValue(ThumbBrushProperty, value);
    }

    public Brush ThumbMouseOverBrush
    {
        get => (Brush)this.GetValue(ThumbMouseOverBrushProperty);
        set => this.SetValue(ThumbMouseOverBrushProperty, value);
    }

    public Brush ThumbMouseDownBrush
    {
        get => (Brush)this.GetValue(ThumbMouseDownBrushProperty);
        set => this.SetValue(ThumbMouseDownBrushProperty, value);
    }

    public Brush ThumbBorderBrush
    {
        get => (Brush)this.GetValue(ThumbBorderBrushProperty);
        set => this.SetValue(ThumbBorderBrushProperty, value);
    }

    public int ThumbBorderThickness
    {
        get => (int)this.GetValue(ThumbBorderThicknessProperty);
        set => this.SetValue(ThumbBorderThicknessProperty, value);
    }

    public Brush ActiveBarBrush
    {
        get => (Brush)this.GetValue(ActiveBarBrushProperty);
        set => this.SetValue(ActiveBarBrushProperty, value);
    }

    public Brush InactiveBarBrush
    {
        get => (Brush)this.GetValue(InactiveBarBrushProperty);
        set => this.SetValue(InactiveBarBrushProperty, value);
    }

    public CornerRadius ThumbCornerRadius
    {
        get => (CornerRadius)this.GetValue(ThumbCornerRadiusProperty);
        set => this.SetValue(ThumbCornerRadiusProperty, value);
    }

    public CornerRadius BarCornerRadius
    {
        get => (CornerRadius)this.GetValue(BarCornerRadiusProperty);
        set => this.SetValue(BarCornerRadiusProperty, value);
    }
}
