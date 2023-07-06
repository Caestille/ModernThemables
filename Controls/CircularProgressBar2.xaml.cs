using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernThemables.Controls
{
	public partial class CircularProgressBar2 : UserControl
	{
		public CircularProgressBar2()
		{
			InitializeComponent();
		}

		public Brush SegmentColor
		{
			get => (Brush)GetValue(SegmentColorProperty);
			set => SetValue(SegmentColorProperty, value);
		}

		public double Percentage
		{
			get => (double)GetValue(PercentageProperty);
			set => SetValue(PercentageProperty, value);
		}

		public bool IsIndeterminate
		{
			get => (bool)GetValue(IsIndeterminateProperty);
			set => SetValue(IsIndeterminateProperty, value);
		}

		public static readonly DependencyProperty PercentageProperty =
			DependencyProperty.Register("Percentage", typeof(double), typeof(CircularProgressBar2), new PropertyMetadata(65d));

		public static readonly DependencyProperty SegmentColorProperty =
			DependencyProperty.Register("SegmentColor", typeof(Brush), typeof(CircularProgressBar2), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

		public static readonly DependencyProperty IsIndeterminateProperty =
			DependencyProperty.Register("IsIndeterminate", typeof(bool), typeof(CircularProgressBar2), new PropertyMetadata(false));
	}
}