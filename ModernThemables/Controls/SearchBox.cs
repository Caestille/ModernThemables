using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernThemables.Controls
{
    public class SearchBox : TextBox
    {
        private const string PART_button = "PART_button";

        private Button2? button;

        static SearchBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchBox), new FrameworkPropertyMetadata(typeof(SearchBox)));
        }
        
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(SearchBox),
            new PropertyMetadata(new CornerRadius(0)));

        public Brush WatermarkForeground
        {
            get => (Brush)GetValue(WatermarkForegroundProperty);
            set => SetValue(WatermarkForegroundProperty, value);
        }
        public static readonly DependencyProperty WatermarkForegroundProperty = DependencyProperty.Register(
            nameof(WatermarkForeground),
            typeof(Brush),
            typeof(SearchBox));

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
            Text = string.Empty;
        }
    }
}