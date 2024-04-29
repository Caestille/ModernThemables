using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernThemables.Controls
{
	public class Button2 : Button
    {
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(Button2),
            new PropertyMetadata(new CornerRadius(0)));
        
        static Button2()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Button2), new FrameworkPropertyMetadata(typeof(Button2)));
        }
    }
}
