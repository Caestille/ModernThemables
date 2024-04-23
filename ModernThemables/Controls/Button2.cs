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

        public new Brush DisabledBackground
        {
            get => (Brush)GetValue(DisabledBackgroundProperty);
            set => SetValue(DisabledBackgroundProperty, value);
        }
        public new static readonly DependencyProperty DisabledBackgroundProperty = DependencyProperty.Register(
            nameof(DisabledBackground),
            typeof(Brush),
            typeof(Button2),
            new UIPropertyMetadata(null));

        public new Brush DisabledForeground
        {
            get => (Brush)GetValue(DisabledForegroundProperty);
            set => SetValue(DisabledForegroundProperty, value);
        }
        public new static readonly DependencyProperty DisabledForegroundProperty = DependencyProperty.Register(
            nameof(DisabledForeground),
            typeof(Brush),
            typeof(Button2),
            new UIPropertyMetadata(null));

        public new Brush DisabledBorderBrush
        {
            get => (Brush)GetValue(DisabledBorderBrushProperty);
            set => SetValue(DisabledBorderBrushProperty, value);
        }
        public new static readonly DependencyProperty DisabledBorderBrushProperty = DependencyProperty.Register(
            nameof(DisabledBorderBrush),
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
