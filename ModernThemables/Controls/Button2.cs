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

        public Brush MouseOverBrush
        {
            get => (Brush)GetValue(MouseOverBrushProperty);
            set => SetValue(MouseOverBrushProperty, value);
        }
        public static readonly DependencyProperty MouseOverBrushProperty = DependencyProperty.Register(
            nameof(MouseOverBrush),
            typeof(Brush),
            typeof(Button2),
            new PropertyMetadata(null));

        public Brush MouseDownBrush
        {
            get => (Brush)GetValue(MouseDownBrushProperty);
            set => SetValue(MouseDownBrushProperty, value);
        }
        public static readonly DependencyProperty MouseDownBrushProperty = DependencyProperty.Register(
            nameof(MouseDownBrush),
            typeof(Brush),
            typeof(Button2),
            new PropertyMetadata(null));

        public Brush DisabledBrush
        {
            get => (Brush)GetValue(DisabledBrushProperty);
            set => SetValue(DisabledBrushProperty, value);
        }
        public static readonly DependencyProperty DisabledBrushProperty = DependencyProperty.Register(
            nameof(DisabledBrush),
            typeof(Brush),
            typeof(Button2),
            new PropertyMetadata(null));

        static Button2()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Button2), new FrameworkPropertyMetadata(typeof(Button2)));
        }
    }
}
