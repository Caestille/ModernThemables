using System.Windows;
using System.Windows.Media;

namespace ModernThemables.Controls
{
	/// <summary>
	/// Interaction logic for ColourPickerDialogue.xaml
	/// </summary>
	public partial class ColourPickerDialogue : Window
	{
		public Color ColourResult
		{
			get => (Color)GetValue(ColourResultProperty);
			set => SetValue(ColourResultProperty, value);
		}
		public static readonly DependencyProperty ColourResultProperty = DependencyProperty.Register(
			"ColourResult",
			typeof(Color),
			typeof(ColourPicker),
			new UIPropertyMetadata(null));

		public ColourPickerDialogue()
		{
			InitializeComponent();
		}

		private void ExtendedButton_Click(object sender, RoutedEventArgs e)
		{
			ColourResult = ColourPicker.Colour;
			DialogResult = true;
		}
	}
}
