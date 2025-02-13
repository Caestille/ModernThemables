namespace ModernThemables.Controls;

using System;
using System.Windows;
using System.Windows.Media;

/// <summary>
/// Interaction logic for ColourPickerDialogue.xaml.
/// </summary>
public partial class ColourPickerDialogue : Window2
{
    public static readonly DependencyProperty ColourProperty = DependencyProperty.Register(
        nameof(Colour),
        typeof(Color),
        typeof(ColourPickerDialogue),
        new FrameworkPropertyMetadata(Colors.Black, OnColourSet));

    private readonly Action<Color>? colourChangedCallback;
    private Color initialColour;

    public ColourPickerDialogue(Color inputColour, Action<Color>? colourChangedCallback)
    {
        this.InitializeComponent();
        this.initialColour = inputColour;
        this.colourChangedCallback = colourChangedCallback;
        this.ColourPickerControl.ColourChangedCallback = colourChangedCallback;
        this.Colour = inputColour;
    }

    public Color Colour
    {
        get => (Color)this.GetValue(ColourProperty);
        set => this.SetValue(ColourProperty, value);
    }

    private static void OnColourSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not ColourPickerDialogue dialog)
        {
            return;
        }

        dialog.ColourPickerControl.Colour = (Color)e.NewValue;
        if (dialog.colourChangedCallback != null)
        {
            dialog.colourChangedCallback((Color)e.NewValue);
        }
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        this.Colour = this.ColourPickerControl.Colour;
        this.DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (this.colourChangedCallback != null)
        {
            this.colourChangedCallback(this.initialColour);
        }

        this.DialogResult = false;
    }
}
