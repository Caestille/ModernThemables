namespace ModernThemables.Controls;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

public class SearchBox : TextBox
{
    private const string PART_button = "PART_button";

    private Button2? button;

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)this.GetValue(CornerRadiusProperty);
        set => this.SetValue(CornerRadiusProperty, value);
    }
    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius),
        typeof(CornerRadius),
        typeof(SearchBox),
        new PropertyMetadata(new CornerRadius(0)));

    public Brush WatermarkForeground
    {
        get => (Brush)this.GetValue(WatermarkForegroundProperty);
        set => this.SetValue(WatermarkForegroundProperty, value);
    }
    public static readonly DependencyProperty WatermarkForegroundProperty = DependencyProperty.Register(
        nameof(WatermarkForeground),
        typeof(Brush),
        typeof(SearchBox));

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (this.button != null)
        {
            this.button.Click -= this.Button_Click;
        }

        if (this.Template.FindName(PART_button, this) is Button2 bt)
        {
            this.button = bt;
        }

        if (this.button != null)
        {
            this.button.Click += this.Button_Click;
        }
        else
        {
            throw new InvalidOperationException("Template missing rquired UI elements");
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        this.Text = string.Empty;
    }
}