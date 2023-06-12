using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace ModernThemables.Controls
{
	public class ExtendedToggleButton : ToggleButton
    {
        readonly static SolidColorBrush DefaultMouseOverProperty = new BrushConverter().ConvertFromString("#FFBEE6FD") as SolidColorBrush;
        private Border border;

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
        }

        private void ExtendedButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (border != null)
            {
                if (this.IsEnabled)
                {
                    border.Background = Background;
                    RecursivelySetContentBrushes(Content as DependencyObject, Foreground, Background);
                }
                else
                {
                    border.Background = DisabledBackgroundColour;
                    RecursivelySetContentBrushes(Content as DependencyObject, DisabledForegroundColour, DisabledBackgroundColour);
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.border = GetTemplateChild("PART_border") as Border;
            ExtendedButton_IsEnabledChanged(null, new DependencyPropertyChangedEventArgs());
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (border != null && IsEnabled)
            {
                border.Background = MouseDownColour;
                RecursivelySetContentBrushes(Content as DependencyObject, Foreground, MouseDownColour);
            }
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (border != null && IsEnabled)
            {
                border.Background = MouseOverColour;
                RecursivelySetContentBrushes(Content as DependencyObject, Foreground, MouseOverColour);
            }
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (border != null && IsEnabled)
            {
                border.Background = MouseOverColour;
                RecursivelySetContentBrushes(Content as DependencyObject, Foreground, MouseOverColour);
            }
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (border != null && IsEnabled && !IsChecked.Value)
            {
                border.Background = Background;
            }
            if (border != null && IsEnabled && IsChecked.Value)
            {
                border.Background = CheckedColour;
                RecursivelySetContentBrushes(Content as DependencyObject, Foreground, CheckedColour);
            }
            base.OnMouseLeave(e);
        }

        protected override void OnChecked(RoutedEventArgs e)
        {
            if (border != null && IsEnabled)
            {
                border.Background = CheckedColour;
                RecursivelySetContentBrushes(Content as DependencyObject, Foreground, CheckedColour);
            }
            base.OnChecked(e);
        }

        protected override void OnUnchecked(RoutedEventArgs e)
        {
            if (border != null && IsEnabled)
            {
                border.Background = MouseOverColour;
                RecursivelySetContentBrushes(Content as DependencyObject, Foreground, MouseOverColour);
            }
            base.OnUnchecked(e);
        }

        private void RecursivelySetContentBrushes(DependencyObject item, Brush foregroundBrush, Brush backgroundBrush)
        {
            if (item is Control fe)
            {
                fe.Foreground = foregroundBrush;
                fe.Background = backgroundBrush;
            }

            if (item != null && VisualTreeHelper.GetChildrenCount(item) > 0)
            {
                var child = VisualTreeHelper.GetChild(item, 0);
                RecursivelySetContentBrushes(child, foregroundBrush, backgroundBrush);
            }
        }

        private static void OnBindingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not ExtendedToggleButton button) return;

            button.ExtendedButton_IsEnabledChanged(null, new DependencyPropertyChangedEventArgs());
        }
    }
}
