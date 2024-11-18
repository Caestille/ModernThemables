using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernThemables.Controls
{
	public class ComboBox2 : ComboBox
    {
        public Brush MouseOverBrush
        {
            get => (Brush)GetValue(MouseOverBrushProperty);
            set => SetValue(MouseOverBrushProperty, value);
        }
        public static readonly DependencyProperty MouseOverBrushProperty = DependencyProperty.Register(
            nameof(MouseOverBrush),
            typeof(Brush),
            typeof(ComboBox2));

        public Brush MouseDownBrush
        {
            get => (Brush)GetValue(MouseDownBrushProperty);
            set => SetValue(MouseDownBrushProperty, value);
        }
        public static readonly DependencyProperty MouseDownBrushProperty = DependencyProperty.Register(
            nameof(MouseDownBrush),
            typeof(Brush),
            typeof(ComboBox2));

        public Brush DisabledBackground
        {
            get => (Brush)GetValue(DisabledBackgroundProperty);
            set => SetValue(DisabledBackgroundProperty, value);
        }
        public static readonly DependencyProperty DisabledBackgroundProperty = DependencyProperty.Register(
            nameof(DisabledBackground),
            typeof(Brush),
            typeof(ComboBox2));

        public Brush DisabledForeground
        {
            get => (Brush)GetValue(DisabledForegroundProperty);
            set => SetValue(DisabledForegroundProperty, value);
        }
        public static readonly DependencyProperty DisabledForegroundProperty = DependencyProperty.Register(
            nameof(DisabledForeground),
            typeof(Brush),
            typeof(ComboBox2));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(ComboBox2),
            new PropertyMetadata(new CornerRadius(0)));

        public Brush ContentBackground
        {
            get => (Brush)GetValue(ContentBackgroundProperty);
            set => SetValue(ContentBackgroundProperty, value);
        }
        public static readonly DependencyProperty ContentBackgroundProperty = DependencyProperty.Register(
            nameof(ContentBackground),
            typeof(Brush),
            typeof(ComboBox2));

        static ComboBox2()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ComboBox2), new FrameworkPropertyMetadata(typeof(ComboBox2)));
        }
    }
}
