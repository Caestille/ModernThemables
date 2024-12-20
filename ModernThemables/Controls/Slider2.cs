using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernThemables.Controls
{
	public class Slider2 : Slider
    {
        public Brush ThumbBrush
        {
            get => (Brush)GetValue(ThumbBrushProperty);
            set => SetValue(ThumbBrushProperty, value);
        }
        public static readonly DependencyProperty ThumbBrushProperty = DependencyProperty.Register(
            nameof(ThumbBrush),
            typeof(Brush),
            typeof(Slider2),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Red)));

        public Brush ThumbMouseOverBrush
        {
            get => (Brush)GetValue(ThumbMouseOverBrushProperty);
            set => SetValue(ThumbMouseOverBrushProperty, value);
        }
        public static readonly DependencyProperty ThumbMouseOverBrushProperty = DependencyProperty.Register(
            nameof(ThumbMouseOverBrush),
            typeof(Brush),
            typeof(Slider2),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Red)));

        public Brush ThumbMouseDownBrush
        {
            get => (Brush)GetValue(ThumbMouseDownBrushProperty);
            set => SetValue(ThumbMouseDownBrushProperty, value);
        }
        public static readonly DependencyProperty ThumbMouseDownBrushProperty = DependencyProperty.Register(
            nameof(ThumbMouseDownBrush),
            typeof(Brush),
            typeof(Slider2),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Red)));

        public Brush ThumbBorderBrush
        {
            get => (Brush)GetValue(ThumbBorderBrushProperty);
            set => SetValue(ThumbBorderBrushProperty, value);
        }
        public static readonly DependencyProperty ThumbBorderBrushProperty = DependencyProperty.Register(
            nameof(ThumbBorderBrush),
            typeof(Brush),
            typeof(Slider2),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Red)));

        public int ThumbBorderThickness
        {
            get => (int)GetValue(ThumbBorderThicknessProperty);
            set => SetValue(ThumbBorderThicknessProperty, value);
        }
        public static readonly DependencyProperty ThumbBorderThicknessProperty = DependencyProperty.Register(
            nameof(ThumbBorderThickness),
            typeof(int),
            typeof(Slider2),
            new FrameworkPropertyMetadata(1));

        public Brush ActiveBarBrush
        {
            get => (Brush)GetValue(ActiveBarBrushProperty);
            set => SetValue(ActiveBarBrushProperty, value);
        }
        public static readonly DependencyProperty ActiveBarBrushProperty = DependencyProperty.Register(
            nameof(ActiveBarBrush),
            typeof(Brush),
            typeof(Slider2),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.PaleVioletRed)));

        public Brush InactiveBarBrush
        {
            get => (Brush)GetValue(InactiveBarBrushProperty);
            set => SetValue(InactiveBarBrushProperty, value);
        }
        public static readonly DependencyProperty InactiveBarBrushProperty = DependencyProperty.Register(
            nameof(InactiveBarBrush),
            typeof(Brush),
            typeof(Slider2),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Gray)));

        public CornerRadius ThumbCornerRadius
        {
            get => (CornerRadius)GetValue(ThumbCornerRadiusProperty);
            set => SetValue(ThumbCornerRadiusProperty, value);
        }
        public static readonly DependencyProperty ThumbCornerRadiusProperty = DependencyProperty.Register(
            nameof(ThumbCornerRadius),
            typeof(CornerRadius),
            typeof(Slider2),
            new PropertyMetadata(new CornerRadius(0)));

        public CornerRadius BarCornerRadius
        {
            get => (CornerRadius)GetValue(BarCornerRadiusProperty);
            set => SetValue(BarCornerRadiusProperty, value);
        }
        public static readonly DependencyProperty BarCornerRadiusProperty = DependencyProperty.Register(
            nameof(BarCornerRadius),
            typeof(CornerRadius),
            typeof(Slider2),
            new PropertyMetadata(new CornerRadius(0)));

        static Slider2()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Slider2), new FrameworkPropertyMetadata(typeof(Slider2)));
        }
    }
}
