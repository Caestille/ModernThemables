using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ModernThemables.Controls
{
	public class ExtendedButton : Button
    {
        readonly static SolidColorBrush DefaultMouseOverProperty = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFBEE6FD")!;

        public Brush CurrentForeground
        {
            get => (Brush)GetValue(CurrentForegroundProperty);
            set => SetValue(CurrentForegroundProperty, value);
        }
        public static readonly DependencyProperty CurrentForegroundProperty = DependencyProperty.Register(
            "CurrentForeground",
            typeof(Brush),
            typeof(ExtendedButton),
            new UIPropertyMetadata(DefaultMouseOverProperty));

        public Brush CurrentBackground
        {
            get => (Brush)GetValue(CurrentBackgroundProperty);
            set => SetValue(CurrentBackgroundProperty, value);
        }
        public static readonly DependencyProperty CurrentBackgroundProperty = DependencyProperty.Register(
            "CurrentBackground",
            typeof(Brush),
            typeof(ExtendedButton),
            new UIPropertyMetadata(DefaultMouseOverProperty));

        public Brush MouseOverColour
        {
            get => (Brush)GetValue(MouseOverColourProperty);
            set => SetValue(MouseOverColourProperty, value);
        }
        public static readonly DependencyProperty MouseOverColourProperty = DependencyProperty.Register(
            "MouseOverColour",
            typeof(Brush),
            typeof(ExtendedButton),
            new UIPropertyMetadata(DefaultMouseOverProperty, OnBindingChanged));

        public Brush MouseDownColour
        {
            get => (Brush)GetValue(MouseDownColourProperty);
            set => SetValue(MouseDownColourProperty, value);
        }
        public static readonly DependencyProperty MouseDownColourProperty = DependencyProperty.Register(
            "MouseDownColour",
            typeof(Brush),
            typeof(ExtendedButton),
            new UIPropertyMetadata(DefaultMouseOverProperty, OnBindingChanged));

        public Brush DisabledBackgroundColour
        {
            get => (Brush)GetValue(DisabledBackgroundColourProperty);
            set => SetValue(DisabledBackgroundColourProperty, value);
        }
        public static readonly DependencyProperty DisabledBackgroundColourProperty = DependencyProperty.Register(
            "DisabledBackgroundColour",
            typeof(Brush),
            typeof(ExtendedButton),
            new UIPropertyMetadata(DefaultMouseOverProperty, OnBindingChanged));

        public Brush DisabledForegroundColour
        {
            get => (Brush)GetValue(DisabledForegroundColourProperty);
            set => SetValue(DisabledForegroundColourProperty, value);
        }
        public static readonly DependencyProperty DisabledForegroundColourProperty = DependencyProperty.Register(
            "DisabledForegroundColour",
            typeof(Brush),
            typeof(ExtendedButton),
            new UIPropertyMetadata(DefaultMouseOverProperty, OnBindingChanged));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius",
            typeof(CornerRadius),
            typeof(ExtendedButton),
            new PropertyMetadata(new CornerRadius(0)));

        public ExtendedButton()
        {
            this.Loaded += ExtendedButton_Loaded;
        }

        private void ExtendedButton_Loaded(object sender, RoutedEventArgs e)
        {
            this.IsEnabledChanged += ExtendedButton_IsEnabledChanged;
            this.Loaded -= ExtendedButton_Loaded;
            ExtendedButton_IsEnabledChanged(this, new DependencyPropertyChangedEventArgs());
        }

        private void ExtendedButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsEnabled)
            {
                CurrentBackground = Background;
                CurrentForeground = Foreground;
            }
            else
            {
                CurrentBackground = DisabledBackgroundColour;
                CurrentForeground = DisabledForegroundColour;
            }
        }

        static ExtendedButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedButton), new FrameworkPropertyMetadata(typeof(ExtendedButton)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ExtendedButton_IsEnabledChanged(this, new DependencyPropertyChangedEventArgs());
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (this.IsEnabled)
            {
                CurrentBackground = MouseDownColour;
                CurrentForeground = Foreground;
            }
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (this.IsEnabled)
            {
                CurrentBackground = MouseOverColour;
                CurrentForeground = Foreground;
            }
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (this.IsEnabled)
            {
                CurrentBackground = MouseOverColour;
                CurrentForeground = Foreground;
            }
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (this.IsEnabled)
            {
                CurrentBackground = Background;
                CurrentForeground = Foreground;
            }
            base.OnMouseLeave(e);
        }

        private static void OnBindingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not ExtendedButton button) return;

            button.ExtendedButton_IsEnabledChanged(button, new DependencyPropertyChangedEventArgs());
        }
    }
}
