using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernThemables.Controls
{
	public class Button2 : Button
    {
        public new Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }
        public new static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            nameof(Background),
            typeof(Brush),
            typeof(Button2),
            new UIPropertyMetadata(null));

        public new Brush Foreground
        {
            get => (Brush)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }
        public new static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
            nameof(Foreground),
            typeof(Brush),
            typeof(Button2),
            new UIPropertyMetadata(null));

        public Brush MouseOverBrush
        {
            get => (Brush)GetValue(MouseOverBrushProperty);
            set => SetValue(MouseOverBrushProperty, value);
        }
        public static readonly DependencyProperty MouseOverBrushProperty = DependencyProperty.Register(
            nameof(MouseOverBrush),
            typeof(Brush),
            typeof(Button2),
            new UIPropertyMetadata(null));

        public Brush MouseDownBrush
        {
            get => (Brush)GetValue(MouseDownBrushProperty);
            set => SetValue(MouseDownBrushProperty, value);
        }
        public static readonly DependencyProperty MouseDownBrushProperty = DependencyProperty.Register(
            nameof(MouseDownBrush),
            typeof(Brush),
            typeof(Button2),
            new UIPropertyMetadata(null));

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
