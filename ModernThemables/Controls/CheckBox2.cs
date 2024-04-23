using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernThemables.Controls
{
	public class CheckBox2 : CheckBox
    {
        public Brush DisabledBorderBrush
        {
            get => (Brush)GetValue(DisabledBorderBrushProperty);
            set => SetValue(DisabledBorderBrushProperty, value);
        }
        public static readonly DependencyProperty DisabledBorderBrushProperty = DependencyProperty.Register(
            nameof(DisabledBorderBrush),
            typeof(Brush),
            typeof(CheckBox2),
            new UIPropertyMetadata(null));

        public Brush CheckedBackground
        {
            get => (Brush)GetValue(CheckedBackgroundProperty);
            set => SetValue(CheckedBackgroundProperty, value);
        }
        public static readonly DependencyProperty CheckedBackgroundProperty = DependencyProperty.Register(
            nameof(CheckedBackground),
            typeof(Brush),
            typeof(CheckBox2),
            new UIPropertyMetadata(null));

        public Brush CheckedForeground
        {
            get => (Brush)GetValue(CheckedForegroundProperty);
            set => SetValue(CheckedForegroundProperty, value);
        }
        public static readonly DependencyProperty CheckedForegroundProperty = DependencyProperty.Register(
            nameof(CheckedForeground),
            typeof(Brush),
            typeof(CheckBox2),
            new UIPropertyMetadata(null));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(CheckBox2),
            new PropertyMetadata(new CornerRadius(0)));
        
        static CheckBox2()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckBox2), new FrameworkPropertyMetadata(typeof(CheckBox2)));
        }
    }
}
