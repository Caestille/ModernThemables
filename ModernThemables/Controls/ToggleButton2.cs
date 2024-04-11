using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace ModernThemables.Controls
{
	public class ToggleButton2 : ToggleButton
    {
        public new Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }
        public new static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            nameof(Background),
            typeof(Brush),
            typeof(ToggleButton2),
            new UIPropertyMetadata(null));

        public new Brush Foreground
        {
            get => (Brush)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }
        public new static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
            nameof(Foreground),
            typeof(Brush),
            typeof(ToggleButton2),
            new UIPropertyMetadata(null));

        public SolidColorBrush MouseOverBrush
        {
            get => (SolidColorBrush)GetValue(MouseOverBrushProperty);
            set => SetValue(MouseOverBrushProperty, value);
        }
        public static readonly DependencyProperty MouseOverBrushProperty = DependencyProperty.Register(
            nameof(MouseOverBrush),
            typeof(SolidColorBrush),
            typeof(ToggleButton2),
            new UIPropertyMetadata(null));

        public SolidColorBrush MouseDownBrush
        {
            get => (SolidColorBrush)GetValue(MouseDownBrushProperty);
            set => SetValue(MouseDownBrushProperty, value);
        }
        public static readonly DependencyProperty MouseDownBrushProperty = DependencyProperty.Register(
            nameof(MouseDownBrush),
            typeof(SolidColorBrush),
            typeof(ToggleButton2),
            new UIPropertyMetadata(null));

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
