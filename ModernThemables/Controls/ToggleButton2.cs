namespace ModernThemables.Controls
{
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;

    public class ToggleButton2 : ToggleButton
    {
        public SolidColorBrush MouseOverBrush
        {
            get => (SolidColorBrush)this.GetValue(MouseOverBrushProperty);
            set => this.SetValue(MouseOverBrushProperty, value);
        }
        public static readonly DependencyProperty MouseOverBrushProperty = DependencyProperty.Register(
            nameof(MouseOverBrush),
            typeof(SolidColorBrush),
            typeof(ToggleButton2));

        public SolidColorBrush MouseDownBrush
        {
            get => (SolidColorBrush)this.GetValue(MouseDownBrushProperty);
            set => this.SetValue(MouseDownBrushProperty, value);
        }
        public static readonly DependencyProperty MouseDownBrushProperty = DependencyProperty.Register(
            nameof(MouseDownBrush),
            typeof(SolidColorBrush),
            typeof(ToggleButton2));

        public SolidColorBrush CheckedBrush
        {
            get => (SolidColorBrush)this.GetValue(CheckedBrushProperty);
            set => this.SetValue(CheckedBrushProperty, value);
        }
        public static readonly DependencyProperty CheckedBrushProperty = DependencyProperty.Register(
            nameof(CheckedBrush),
            typeof(SolidColorBrush),
            typeof(ToggleButton2));

        public SolidColorBrush DisabledBackground
        {
            get => (SolidColorBrush)this.GetValue(DisabledBackgroundProperty);
            set => this.SetValue(DisabledBackgroundProperty, value);
        }
        public static readonly DependencyProperty DisabledBackgroundProperty = DependencyProperty.Register(
            nameof(DisabledBackground),
            typeof(SolidColorBrush),
            typeof(ToggleButton2));

        public SolidColorBrush DisabledForeground
        {
            get => (SolidColorBrush)this.GetValue(DisabledForegroundProperty);
            set => this.SetValue(DisabledForegroundProperty, value);
        }
        public static readonly DependencyProperty DisabledForegroundProperty = DependencyProperty.Register(
            nameof(DisabledForeground),
            typeof(SolidColorBrush),
            typeof(ToggleButton2));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)this.GetValue(CornerRadiusProperty);
            set => this.SetValue(CornerRadiusProperty, value);
        }
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(ToggleButton2));
    }
}
