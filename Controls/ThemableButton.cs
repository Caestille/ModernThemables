using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernThemables.Controls
{
	public class ExtendedButton : Button
	{
		static ExtendedButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedButton), new FrameworkPropertyMetadata(typeof(ExtendedButton)));
		}

		readonly static SolidColorBrush DefaultMouseOverProperty = new BrushConverter().ConvertFromString("#FFBEE6FD") as SolidColorBrush;

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
	}
}
