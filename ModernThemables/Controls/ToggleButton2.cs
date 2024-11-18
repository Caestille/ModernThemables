using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ModernThemables.Controls
{
	public class ToggleButton2 : ToggleButton
    {
        public SolidColorBrush MouseOverBrush
        {
            get => (SolidColorBrush)GetValue(MouseOverBrushProperty);
            set => SetValue(MouseOverBrushProperty, value);
        }
        public static readonly DependencyProperty MouseOverBrushProperty = DependencyProperty.Register(
            nameof(MouseOverBrush),
            typeof(SolidColorBrush),
            typeof(ToggleButton2));

        public SolidColorBrush MouseDownBrush
        {
            get => (SolidColorBrush)GetValue(MouseDownBrushProperty);
            set => SetValue(MouseDownBrushProperty, value);
        }
        public static readonly DependencyProperty MouseDownBrushProperty = DependencyProperty.Register(
            nameof(MouseDownBrush),
            typeof(SolidColorBrush),
            typeof(ToggleButton2));

        public SolidColorBrush CheckedBrush
        {
            get => (SolidColorBrush)GetValue(CheckedBrushProperty);
            set => SetValue(CheckedBrushProperty, value);
        }
        public static readonly DependencyProperty CheckedBrushProperty = DependencyProperty.Register(
            nameof(CheckedBrush),
            typeof(SolidColorBrush),
            typeof(ToggleButton2));

        public SolidColorBrush DisabledBackground
        {
            get => (SolidColorBrush)GetValue(DisabledBackgroundProperty);
            set => SetValue(DisabledBackgroundProperty, value);
        }
        public static readonly DependencyProperty DisabledBackgroundProperty = DependencyProperty.Register(
            nameof(DisabledBackground),
            typeof(SolidColorBrush),
            typeof(ToggleButton2));

        public SolidColorBrush DisabledForeground
        {
            get => (SolidColorBrush)GetValue(DisabledForegroundProperty);
            set => SetValue(DisabledForegroundProperty, value);
        }
        public static readonly DependencyProperty DisabledForegroundProperty = DependencyProperty.Register(
            nameof(DisabledForeground),
            typeof(SolidColorBrush),
            typeof(ToggleButton2));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(ToggleButton2));

        static ToggleButton2()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleButton2), new FrameworkPropertyMetadata(typeof(ToggleButton2)));
        }
    }
}
