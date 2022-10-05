using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ModernThemables.Controls
{
	public class ExtendedToggleButton : ToggleButton
	{
		static ExtendedToggleButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedToggleButton), new FrameworkPropertyMetadata(typeof(ExtendedToggleButton)));
		}

		protected override void OnClick()
		{
			base.OnClick();
			Debug.WriteLine(IsChecked);
		}

		readonly static SolidColorBrush DefaultMouseOverProperty = new BrushConverter().ConvertFromString("#FFBEE6FD") as SolidColorBrush;

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
	}
}
