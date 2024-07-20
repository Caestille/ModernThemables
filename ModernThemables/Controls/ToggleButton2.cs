using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace ModernThemables.Controls
{
	public class ToggleButton2 : ToggleButton
    {
        public SolidColorBrush CheckedBrush
        {
            get => (SolidColorBrush)GetValue(CheckedBrushProperty);
            set => SetValue(CheckedBrushProperty, value);
        }
        public static readonly DependencyProperty CheckedBrushProperty = DependencyProperty.Register(
            nameof(CheckedBrush),
            typeof(SolidColorBrush),
            typeof(ToggleButton2),
            new UIPropertyMetadata(null));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(ToggleButton2),
            new PropertyMetadata(new CornerRadius(0)));

        static ToggleButton2()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleButton2), new FrameworkPropertyMetadata(typeof(ToggleButton2)));
        }
    }
}
