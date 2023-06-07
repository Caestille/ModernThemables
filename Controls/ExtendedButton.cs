using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace ModernThemables.Controls
{
	public class ExtendedButton : Button
    {
        readonly static SolidColorBrush DefaultMouseOverProperty = new BrushConverter().ConvertFromString("#FFBEE6FD") as SolidColorBrush;
        private Brush backGround;
        private Brush foreGround;

        public SolidColorBrush MouseOverColour
		{
			get { return (SolidColorBrush)GetValue(MouseOverColourProperty); }
			set { SetValue(MouseOverColourProperty, value); }
		}
		public static readonly DependencyProperty MouseOverColourProperty = DependencyProperty.Register(
		  "MouseOverColour", typeof(SolidColorBrush), typeof(ExtendedButton), new PropertyMetadata(DefaultMouseOverProperty));

		public SolidColorBrush MouseDownColour
		{
			get { return (SolidColorBrush)GetValue(MouseDownColourProperty); }
			set { SetValue(MouseDownColourProperty, value); }
		}
		public static readonly DependencyProperty MouseDownColourProperty = DependencyProperty.Register(
		  "MouseDownColour", typeof(SolidColorBrush), typeof(ExtendedButton), new PropertyMetadata(DefaultMouseOverProperty));

		public SolidColorBrush DisabledBackgroundColour
		{
			get { return (SolidColorBrush)GetValue(DisabledBackgroundColourProperty); }
			set { SetValue(DisabledBackgroundColourProperty, value); }
		}
		public static readonly DependencyProperty DisabledBackgroundColourProperty = DependencyProperty.Register(
		  "DisabledBackgroundColour", typeof(SolidColorBrush), typeof(ExtendedButton), new PropertyMetadata(DefaultMouseOverProperty));

		public SolidColorBrush DisabledForegroundColour
		{
			get { return (SolidColorBrush)GetValue(DisabledForegroundColourProperty); }
			set { SetValue(DisabledForegroundColourProperty, value); }
		}
		public static readonly DependencyProperty DisabledForegroundColourProperty = DependencyProperty.Register(
		  "DisabledForegroundColour", typeof(SolidColorBrush), typeof(ExtendedButton), new PropertyMetadata(DefaultMouseOverProperty));

		public CornerRadius CornerRadius
		{
			get { return (CornerRadius)GetValue(CornerRadiusProperty); }
			set { SetValue(CornerRadiusProperty, value); }
		}
		public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
		  "CornerRadius", typeof(CornerRadius), typeof(ExtendedButton), new PropertyMetadata(new CornerRadius(0)));

        public ExtendedButton()
        {
            this.IsEnabledChanged += ExtendedButton_IsEnabledChanged;
        }

        private void ExtendedButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SetDefaults();
            if (backGround != null)
            {
                if (this.IsEnabled)
                {
                    Background = backGround;
                    Foreground = foreGround;
                }
                else
                {
                    Background = DisabledBackgroundColour;
                    Foreground = DisabledForegroundColour;
                }
            }
        }

        static ExtendedButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedButton), new FrameworkPropertyMetadata(typeof(ExtendedButton)));
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            SetDefaults();
            if (backGround != null && IsEnabled)
            {
				Background = MouseDownColour;
			}
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            SetDefaults();
            if (backGround != null && IsEnabled)
            {
				Background = MouseOverColour;
			}
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            SetDefaults();
            if (backGround != null && IsEnabled)
            {
				Background = MouseOverColour;
			}
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            SetDefaults();
            if (backGround != null && IsEnabled)
			{
                Background = backGround;
            }
            base.OnMouseLeave(e);
        }

        private void SetDefaults()
        {
            if (backGround == null)
            {
                backGround = Background;
            }

            if (foreGround == null)
            {
                foreGround = Foreground;
            }
        }
    }
}
