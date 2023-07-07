using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernThemables.ScalableIcons
{
	public class BaseIcon : Control
	{
		public static readonly DependencyProperty StrokeStyleProperty = DependencyProperty.Register("StrokeStyle", typeof(PenLineCap), typeof(BaseIcon), new UIPropertyMetadata(PenLineCap.Flat));

		public PenLineCap StrokeStyle
		{
			get { return (PenLineCap)GetValue(StrokeStyleProperty); }
			set { SetValue(StrokeStyleProperty, value); }
		}

		public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(BaseIcon), new UIPropertyMetadata(1d));

		public double StrokeThickness
		{
			get { return (double)GetValue(StrokeThicknessProperty); }
			set { SetValue(StrokeThicknessProperty, value); }
		}

		public static readonly DependencyProperty IsVisuallyDisabledProperty = DependencyProperty.Register("IsVisuallyDisabled", typeof(bool), typeof(BaseIcon), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

		public bool IsVisuallyDisabled
		{
			get { return (bool)GetValue(IsVisuallyDisabledProperty); }
			set 
			{ 
				SetValue(IsVisuallyDisabledProperty, value); 
			}
		}
	}
}
