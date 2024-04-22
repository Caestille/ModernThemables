using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernThemables.Controls
{
	public class CircularProgressBar : UserControl
    {
        private const string PART_ProgressArc = "PART_ProgressArc";
        private ProgressArc progressArc;
        public double Percentage
        {
            get => (double)GetValue(PercentageProperty);
            set => SetValue(PercentageProperty, value);
        }

        public static readonly DependencyProperty PercentageProperty = DependencyProperty.Register(
            nameof(Percentage),
            typeof(double),
            typeof(CircularProgressBar),
            new PropertyMetadata(0d, OnSetPercentage));
        public double StrokeWidthFraction
        {
            get => (double)GetValue(StrokeWidthFractionProperty);
            set => SetValue(StrokeWidthFractionProperty, value);
        }

        public static readonly DependencyProperty StrokeWidthFractionProperty = DependencyProperty.Register(
            nameof(StrokeWidthFraction),
            typeof(double),
            typeof(CircularProgressBar),
            new PropertyMetadata(0d, OnSetStrokeFraction));

        public bool RoundedEnd
        {
            get => (bool)GetValue(RoundedEndProperty);
            set => SetValue(RoundedEndProperty, value);
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
            get => (bool)GetValue(IsIndeterminateProperty);
            set => SetValue(IsIndeterminateProperty, value);
        }

        public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register(
            nameof(IsIndeterminate),
            typeof(bool),
            typeof(CircularProgressBar),
            new PropertyMetadata(false));

        static CircularProgressBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircularProgressBar), new FrameworkPropertyMetadata(typeof(CircularProgressBar)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Template.FindName(PART_ProgressArc, this) is ProgressArc arc) progressArc = arc;

            if (progressArc != null)
            {
                OnSetPercentage(this, new DependencyPropertyChangedEventArgs());
                OnSetStrokeFraction(this, new DependencyPropertyChangedEventArgs());
            }
        }

        private static void OnSetPercentage(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not CircularProgressBar _this || _this.IsIndeterminate || _this.progressArc == null ) return;

            _this.progressArc.RotationAngle = 0;
            _this.progressArc.Percentage = Math.Max(0, Math.Min(100, _this.Percentage));
        }

        private static void OnSetStrokeFraction(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not CircularProgressBar _this || _this.progressArc == null) return;

            _this.progressArc.InnerRadiusFraction = 1 - Math.Max(0, Math.Min(1, _this.StrokeWidthFraction));
        }
    }
}
