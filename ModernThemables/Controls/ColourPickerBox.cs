namespace ModernThemables.Controls;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ModernThemables.Services;

public class ColourPickerBox : Control
{
    private const string PART_button = "PART_button";

    private Button2? button;

    public Color TemporaryColour
    {
        get => (Color)this.GetValue(TemporaryColourProperty);
        set => this.SetValue(TemporaryColourProperty, value);
    }

    public static readonly DependencyProperty TemporaryColourProperty =
        DependencyProperty.Register(
            nameof(TemporaryColour),
            typeof(Color),
            typeof(ColourPickerBox),
            new FrameworkPropertyMetadata(Colors.Black));

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)this.GetValue(CornerRadiusProperty);
        set => this.SetValue(CornerRadiusProperty, value);
    }

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius),
        typeof(CornerRadius),
        typeof(ColourPickerBox),
        new PropertyMetadata(new CornerRadius(0)));

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

    private void Button_Click(object sender, RoutedEventArgs e) => this.Background = new SolidColorBrush(
            new DialogueService().ShowColourPickerDialogue(
                (this.Background as SolidColorBrush)!.Color,
                colour => this.TemporaryColour = colour));
}
