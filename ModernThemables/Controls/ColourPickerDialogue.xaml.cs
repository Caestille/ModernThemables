using System;
using System.Windows;
using System.Windows.Media;

namespace ModernThemables.Controls
{
	/// <summary>
	/// Interaction logic for ColourPickerDialogue.xaml
	/// </summary>
	public partial class ColourPickerDialogue : ThemableWindow2
	{
		private Color initialColour;
		private readonly Action<Color> colourChangedCallback;

		public Color Colour
		{
			get => (Color)GetValue(ColourProperty);
			set => SetValue(ColourProperty, value);
		}
		public static readonly DependencyProperty ColourProperty = DependencyProperty.Register(
			"Colour",
			typeof(Color),
			typeof(ColourPickerDialogue),
			new FrameworkPropertyMetadata(Colors.Black, OnColourSet));

		public ColourPickerDialogue(Color inputColour, Action<Color>? colourChangedCallback)
		{
			InitializeComponent();
			this.initialColour = inputColour;
			this.colourChangedCallback = colourChangedCallback;
			this.ColourPickerControl.colourChangedCallback = colourChangedCallback;
			this.Colour = inputColour;
		}

		private static void OnColourSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not ColourPickerDialogue dialog) return;

			dialog.ColourPickerControl.Colour = (Color)e.NewValue;
			if (dialog.colourChangedCallback != null) dialog.colourChangedCallback((Color)e.NewValue);
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			Colour = ColourPickerControl.Colour;
			DialogResult = true;
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			if (colourChangedCallback != null) colourChangedCallback(initialColour);
			DialogResult = false;
		}
	}
}
