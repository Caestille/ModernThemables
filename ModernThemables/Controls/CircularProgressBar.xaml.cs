namespace ModernThemables.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    public partial class CircularProgressBar : UserControl
	{
		public double Percentage
		{
			get => (double)this.GetValue(PercentageProperty);
			set => this.SetValue(PercentageProperty, value);
		}

		public static readonly DependencyProperty PercentageProperty = DependencyProperty.Register(
			nameof(Percentage),
			typeof(double),
			typeof(CircularProgressBar),
			new PropertyMetadata(0d, OnSetPercentage));
		public double StrokeWidthFraction
		{
			get => (double)this.GetValue(StrokeWidthFractionProperty);
			set => this.SetValue(StrokeWidthFractionProperty, value);
		}

		public static readonly DependencyProperty StrokeWidthFractionProperty = DependencyProperty.Register(
			nameof(StrokeWidthFraction),
			typeof(double),
			typeof(CircularProgressBar),
			new PropertyMetadata(0d, OnSetStrokeFraction));

		public bool RoundedEnd
        {
            get => (bool)this.GetValue(RoundedEndProperty);
            set => this.SetValue(RoundedEndProperty, value);
        }
        public static readonly DependencyProperty RoundedEndProperty = DependencyProperty.Register(
			nameof(RoundedEnd),
			typeof(bool),
			typeof(CircularProgressBar),
			new FrameworkPropertyMetadata(
				true,
				FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

		public bool IsIndeterminate
		{
			get => (bool)this.GetValue(IsIndeterminateProperty);
			set => this.SetValue(IsIndeterminateProperty, value);
		}

		public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register(
			nameof(IsIndeterminate),
			typeof(bool),
			typeof(CircularProgressBar),
			new PropertyMetadata(false));

		public CircularProgressBar()
		{
            this.InitializeComponent();
		}

		private static void OnSetPercentage(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not CircularProgressBar _this) return;

			if (!_this.IsIndeterminate)
			{
				_this.Arc.RotationAngle = 0;
				_this.Arc.Percentage = Math.Max(0, Math.Min(100, _this.Percentage));
			}
		}

		private static void OnSetStrokeFraction(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not CircularProgressBar _this) return;

			_this.Arc.InnerRadiusFraction =  1 - Math.Max(0, Math.Min(1, _this.StrokeWidthFraction));
		}
	}
}