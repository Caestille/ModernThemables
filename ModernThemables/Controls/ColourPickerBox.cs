using ModernThemables.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernThemables.Controls
{
    public class ColourPickerBox : Control
	{
		private const string PART_button = "PART_button";

		private Button2? button;

		public event EventHandler<Color>? ColourChanged;

        public Color TemporaryColour
        {
            get => (Color)GetValue(TemporaryColourProperty);
            set => SetValue(TemporaryColourProperty, value);
        }

        public static readonly DependencyProperty TemporaryColourProperty =
            DependencyProperty.Register(
                nameof(TemporaryColour),
                typeof(Color),
                typeof(ColourPickerBox),
                new FrameworkPropertyMetadata(Colors.Black));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(ColourPickerBox),
            new PropertyMetadata(new CornerRadius(0)));

        public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (button != null)
			{
				button.Click -= Button_Click;
			}

			if (Template.FindName(PART_button, this) is Button2 bt)
			{
				button = bt;
			}

			if (button != null)
			{
				button.Click += Button_Click;
			}
			else
			{
				throw new InvalidOperationException("Template missing rquired UI elements");
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
            Background = new SolidColorBrush(
                new DialogueService().ShowColourPickerDialogue(
                    (Background as SolidColorBrush)!.Color,
                    colour => TemporaryColour = colour));
		}
    }
}