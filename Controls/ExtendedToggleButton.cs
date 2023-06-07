using System.Diagnostics;
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
        private Brush backGround;

        public SolidColorBrush MouseOverColour
		{
			get { return (SolidColorBrush)GetValue(MouseOverColourProperty); }
			set { SetValue(MouseOverColourProperty, value); }
		}
		public static readonly DependencyProperty MouseOverColourProperty = DependencyProperty.Register(
		  "MouseOverColour", typeof(SolidColorBrush), typeof(ExtendedToggleButton), new PropertyMetadata(DefaultMouseOverProperty));

		public SolidColorBrush MouseDownColour
		{
			get { return (SolidColorBrush)GetValue(MouseDownColourProperty); }
			set { SetValue(MouseDownColourProperty, value); }
		}
		public static readonly DependencyProperty MouseDownColourProperty = DependencyProperty.Register(
		  "MouseDownColour", typeof(SolidColorBrush), typeof(ExtendedToggleButton), new PropertyMetadata(DefaultMouseOverProperty));

		public SolidColorBrush CheckedColour
		{
			get { return (SolidColorBrush)GetValue(CheckedColourProperty); }
			set { SetValue(CheckedColourProperty, value); }
		}
		public static readonly DependencyProperty CheckedColourProperty = DependencyProperty.Register(
		  "CheckedColour", typeof(SolidColorBrush), typeof(ExtendedToggleButton), new PropertyMetadata(DefaultMouseOverProperty));

		public SolidColorBrush DisabledBackgroundColour
		{
			get { return (SolidColorBrush)GetValue(DisabledBackgroundColourProperty); }
			set { SetValue(DisabledBackgroundColourProperty, value); }
		}
		public static readonly DependencyProperty DisabledBackgroundColourProperty = DependencyProperty.Register(
		  "DisabledBackgroundColour", typeof(SolidColorBrush), typeof(ExtendedToggleButton), new PropertyMetadata(DefaultMouseOverProperty));

		public SolidColorBrush DisabledForegroundColour
		{
			get { return (SolidColorBrush)GetValue(DisabledForegroundColourProperty); }
			set { SetValue(DisabledForegroundColourProperty, value); }
		}
		public static readonly DependencyProperty DisabledForegroundColourProperty = DependencyProperty.Register(
		  "DisabledForegroundColour", typeof(SolidColorBrush), typeof(ExtendedToggleButton), new PropertyMetadata(DefaultMouseOverProperty));

		public CornerRadius CornerRadius
		{
			get { return (CornerRadius)GetValue(CornerRadiusProperty); }
			set { SetValue(CornerRadiusProperty, value); }
		}
		public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
		  "CornerRadius", typeof(CornerRadius), typeof(ExtendedToggleButton), new PropertyMetadata(new CornerRadius(0)));

        static ExtendedToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedToggleButton), new FrameworkPropertyMetadata(typeof(ExtendedToggleButton)));
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (backGround == null)
            {
                backGround = Background;
            }
            if (backGround != null)
            {
                Background = MouseDownColour;
            }
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (backGround == null)
            {
                backGround = Background;
            }
            if (backGround != null)
            {
                Background = MouseOverColour;
            }
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (backGround == null)
            {
                backGround = Background;
            }
            if (backGround != null)
            {
                Background = MouseOverColour;
            }
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (backGround == null)
            {
                backGround = Background;
            }
            if (backGround != null && !IsChecked.Value)
            {
                Background = backGround;
            }
            if (IsChecked.Value)
            {
                Background = CheckedColour;
            }
            base.OnMouseLeave(e);
        }

        protected override void OnChecked(RoutedEventArgs e)
        {
            if (backGround == null)
            {
                backGround = Background;
            }
            if (backGround != null)
            {
                Background = CheckedColour;
            }
            base.OnChecked(e);
        }

        protected override void OnUnchecked(RoutedEventArgs e)
        {
            if (backGround == null)
            {
                backGround = Background;
            }
            if (backGround != null)
            {
                Background = MouseOverColour;
            }
            base.OnUnchecked(e);
        }
    }
}
