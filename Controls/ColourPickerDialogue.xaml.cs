using System.Windows;
using System.Windows.Media;

namespace ModernThemables.Controls
{
	/// <summary>
	/// Interaction logic for ColourPickerDialogue.xaml
	/// </summary>
	public partial class ColourPickerDialogue : Window
	{
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

		public ColourPickerDialogue(Color inputColour)
		{
			InitializeComponent();
			this.Colour = inputColour;
		}

		private static void OnColourSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not ColourPickerDialogue dialog) return;

			dialog.ColourPickerControl.Colour = (Color)e.NewValue;
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			Colour = ColourPickerControl.Colour;
			DialogResult = true;
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}
}
