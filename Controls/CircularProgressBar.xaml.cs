using System;
using System.Windows;
using System.Windows.Controls;

namespace ModernThemables.Controls
{
	public partial class CircularProgressBar : UserControl
	{
		public double Percentage
		{
			get => (double)GetValue(PercentageProperty);
			set => SetValue(PercentageProperty, value);
		}

		public static readonly DependencyProperty PercentageProperty = DependencyProperty.Register(
			"Percentage",
			typeof(double),
			typeof(CircularProgressBar),
			new PropertyMetadata(0d, OnSetPercentage));
		public double StrokeWidthFraction
		{
			get => (double)GetValue(StrokeWidthFractionProperty);
			set => SetValue(StrokeWidthFractionProperty, value);
		}

		public static readonly DependencyProperty StrokeWidthFractionProperty = DependencyProperty.Register(
			"StrokeWidthFraction",
			typeof(double),
			typeof(CircularProgressBar),
			new PropertyMetadata(0d, OnSetStrokeFraction));

		public bool IsIndeterminate
		{
			get => (bool)GetValue(IsIndeterminateProperty);
			set => SetValue(IsIndeterminateProperty, value);
		}

		public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register(
			"IsIndeterminate",
			typeof(bool),
			typeof(CircularProgressBar),
			new PropertyMetadata(false));

		public CircularProgressBar()
		{
			InitializeComponent();
		}

		private static async void OnSetPercentage(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not CircularProgressBar _this) return;

			if (!_this.IsIndeterminate)
			{
				_this.Arc.RotationAngle = 0;
				_this.Arc.Percentage = Math.Max(0, Math.Min(100, _this.Percentage));
			}
		}

		private static async void OnSetStrokeFraction(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not CircularProgressBar _this) return;

			_this.Arc.InnerRadiusFraction =  1 - Math.Max(0, Math.Min(1, _this.StrokeWidthFraction));
		}
	}
}