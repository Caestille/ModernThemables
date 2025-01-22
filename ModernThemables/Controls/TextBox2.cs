namespace ModernThemables.Controls;

using System.Windows;
using System.Windows.Controls;

public class TextBox2 : TextBox
{
    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius),
        typeof(CornerRadius),
        typeof(TextBox2),
        new PropertyMetadata(new CornerRadius(0)));

    static TextBox2()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBox2), new FrameworkPropertyMetadata(typeof(TextBox2)));
    }

    public TextBox2() { }

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)this.GetValue(CornerRadiusProperty);
        set => this.SetValue(CornerRadiusProperty, value);
    }
}
