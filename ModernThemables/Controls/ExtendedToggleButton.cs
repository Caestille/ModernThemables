using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace ModernThemables.Controls
{
	public class ExtendedToggleButton : ToggleButton
    {
        readonly static SolidColorBrush DefaultMouseOverProperty = new BrushConverter().ConvertFromString("#FFBEE6FD") as SolidColorBrush;
        public Brush CurrentForeground
        {
            get => (Brush)GetValue(CurrentForegroundProperty);
            set => SetValue(CurrentForegroundProperty, value);
        }
        public static readonly DependencyProperty CurrentForegroundProperty = DependencyProperty.Register(
            "CurrentForeground",
            typeof(Brush),
            typeof(ExtendedToggleButton),
            new UIPropertyMetadata(DefaultMouseOverProperty));

        public Brush CurrentBackground
        {
            get => (Brush)GetValue(CurrentBackgroundProperty);
            set => SetValue(CurrentBackgroundProperty, value);
        }
        public static readonly DependencyProperty CurrentBackgroundProperty = DependencyProperty.Register(
            "CurrentBackground",
            typeof(Brush),
            typeof(ExtendedToggleButton),
            new UIPropertyMetadata(DefaultMouseOverProperty));

        public SolidColorBrush MouseOverColour
        {
            get { return (SolidColorBrush)GetValue(MouseOverColourProperty); }
            set { SetValue(MouseOverColourProperty, value); }
        }
        public static readonly DependencyProperty MouseOverColourProperty = DependencyProperty.Register(
            "MouseOverColour",
            typeof(SolidColorBrush),
            typeof(ExtendedToggleButton),
            new UIPropertyMetadata(DefaultMouseOverProperty, OnBindingChanged));

        public SolidColorBrush MouseDownColour
        {
            get { return (SolidColorBrush)GetValue(MouseDownColourProperty); }
            set { SetValue(MouseDownColourProperty, value); }
        }
        public static readonly DependencyProperty MouseDownColourProperty = DependencyProperty.Register(
            "MouseDownColour",
            typeof(SolidColorBrush),
            typeof(ExtendedToggleButton),
            new UIPropertyMetadata(DefaultMouseOverProperty, OnBindingChanged));

        public SolidColorBrush CheckedColour
        {
            get { return (SolidColorBrush)GetValue(CheckedColourProperty); }
            set { SetValue(CheckedColourProperty, value); }
        }
        public static readonly DependencyProperty CheckedColourProperty = DependencyProperty.Register(
            "CheckedColour",
            typeof(SolidColorBrush),
            typeof(ExtendedToggleButton),
            new UIPropertyMetadata(DefaultMouseOverProperty, OnBindingChanged));

        public SolidColorBrush DisabledBackgroundColour
        {
            get { return (SolidColorBrush)GetValue(DisabledBackgroundColourProperty); }
            set { SetValue(DisabledBackgroundColourProperty, value); }
        }
        public static readonly DependencyProperty DisabledBackgroundColourProperty = DependencyProperty.Register(
            "DisabledBackgroundColour",
            typeof(SolidColorBrush),
            typeof(ExtendedToggleButton),
            new UIPropertyMetadata(DefaultMouseOverProperty, OnBindingChanged));

        public SolidColorBrush DisabledForegroundColour
        {
            get { return (SolidColorBrush)GetValue(DisabledForegroundColourProperty); }
            set { SetValue(DisabledForegroundColourProperty, value); }
        }
        public static readonly DependencyProperty DisabledForegroundColourProperty = DependencyProperty.Register(
            "DisabledForegroundColour",
            typeof(SolidColorBrush),
            typeof(ExtendedToggleButton),
            new UIPropertyMetadata(DefaultMouseOverProperty, OnBindingChanged));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius",
            typeof(CornerRadius),
            typeof(ExtendedToggleButton),
            new PropertyMetadata(new CornerRadius(0)));

        static ExtendedToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedToggleButton), new FrameworkPropertyMetadata(typeof(ExtendedToggleButton)));
        }

        public ExtendedToggleButton()
        {
            this.Loaded += ExtendedButton_Loaded;
        }

        private void ExtendedButton_Loaded(object sender, RoutedEventArgs e)
        {
            this.IsEnabledChanged += ExtendedButton_IsEnabledChanged;
            this.Loaded -= ExtendedButton_Loaded;
            ExtendedButton_IsEnabledChanged(null, new DependencyPropertyChangedEventArgs());
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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ExtendedButton_IsEnabledChanged(null, new DependencyPropertyChangedEventArgs());
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (IsEnabled)
            {
                CurrentBackground = MouseDownColour;
                CurrentForeground = Foreground;
            }
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (IsEnabled)
            {
                CurrentBackground = MouseOverColour;
                CurrentForeground = Foreground;
            }
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (IsEnabled)
            {
                CurrentBackground = MouseOverColour;
                CurrentForeground = Foreground;
            }
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (IsEnabled && !IsChecked.Value)
            {
                CurrentBackground = Background;
                CurrentForeground = Foreground;
            }
            if (IsEnabled && IsChecked.Value)
            {
                CurrentBackground = CheckedColour;
                CurrentForeground = Foreground;
            }
            base.OnMouseLeave(e);
        }

        protected override void OnChecked(RoutedEventArgs e)
        {
            if (IsEnabled)
            {
                CurrentBackground = CheckedColour;
                CurrentForeground = Foreground;
            }
            base.OnChecked(e);
        }

        protected override void OnUnchecked(RoutedEventArgs e)
        {
            if (IsEnabled)
            {
                CurrentBackground = MouseOverColour;
                CurrentForeground = Foreground;
            }
            base.OnUnchecked(e);
        }

        private static void OnBindingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not ExtendedToggleButton button) return;

            button.ExtendedButton_IsEnabledChanged(null, new DependencyPropertyChangedEventArgs());
        }
    }
}
