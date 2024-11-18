using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernThemables.Controls
{
	public class Button2 : Button
    {
        readonly static SolidColorBrush DefaultMouseOverProperty = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFBEE6FD")!;

        public Brush MouseOverBrush
        {
            get => (Brush)GetValue(MouseOverBrushProperty);
            set => SetValue(MouseOverBrushProperty, value);
        }
        public static readonly DependencyProperty MouseOverBrushProperty = DependencyProperty.Register(
            nameof(MouseOverBrush),
            typeof(Brush),
            typeof(Button2),
            new FrameworkPropertyMetadata(DefaultMouseOverProperty));

        public Brush MouseDownBrush
        {
            get => (Brush)GetValue(MouseDownBrushProperty);
            set => SetValue(MouseDownBrushProperty, value);
        }
        public static readonly DependencyProperty MouseDownBrushProperty = DependencyProperty.Register(
            nameof(MouseDownBrush),
            typeof(Brush),
            typeof(Button2));

        public Brush DisabledBackground
        {
            get => (Brush)GetValue(DisabledBackgroundProperty);
            set => SetValue(DisabledBackgroundProperty, value);
        }
        public static readonly DependencyProperty DisabledBackgroundProperty = DependencyProperty.Register(
            nameof(DisabledBackground),
            typeof(Brush),
            typeof(Button2));

        public Brush DisabledForeground
        {
            get => (Brush)GetValue(DisabledForegroundProperty);
            set => SetValue(DisabledForegroundProperty, value);
        }
        public static readonly DependencyProperty DisabledForegroundProperty = DependencyProperty.Register(
            nameof(DisabledForeground),
            typeof(Brush),
            typeof(Button2));

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
