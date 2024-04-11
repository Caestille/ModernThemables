using System;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;

namespace ModernThemables.Controls
{
	public class ProgressArc : Shape
	{
		public double Radius
        {
            get => (double)GetValue(RadiusProperty);
            set => SetValue(RadiusProperty, value);
        }
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
			"Radius",
			typeof(double),
			typeof(ProgressArc),
			new FrameworkPropertyMetadata(
				0.0,
				FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

		public bool IsIndeterminate
        {
            get => (bool)GetValue(IsIndeterminateProperty);
            set => SetValue(IsIndeterminateProperty, value);
        }
        public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register(
			"IsIndeterminate",
			typeof(bool),
			typeof(ProgressArc),
			new FrameworkPropertyMetadata(
				false,
				FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

		public bool RoundedEnd
        {
            get => (bool)GetValue(RoundedEndProperty);
            set => SetValue(RoundedEndProperty, value);
        }
        public static readonly DependencyProperty RoundedEndProperty = DependencyProperty.Register(
			"RoundedEnd",
			typeof(bool),
			typeof(ProgressArc),
			new FrameworkPropertyMetadata(
				false,
				FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

		public double InnerRadiusFraction
        {
            get => (double)GetValue(InnerRadiusProperty);
            set => SetValue(InnerRadiusProperty, value);
        }
        public static readonly DependencyProperty InnerRadiusProperty = DependencyProperty.Register(
			"InnerRadiusFraction",
			typeof(double),
			typeof(ProgressArc),
			new FrameworkPropertyMetadata(
				0.0,
				FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

		public double RotationAngle
        {
            get => (double)GetValue(RotationAngleProperty);
            set => SetValue(RotationAngleProperty, value);
        }
        public static readonly DependencyProperty RotationAngleProperty = DependencyProperty.Register(
			"RotationAngle",
			typeof(double),
			typeof(ProgressArc),
			new FrameworkPropertyMetadata(
				0.0,
				FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

		public double CentreX
        {
            get => (double)GetValue(CentreXProperty);
            set => SetValue(CentreXProperty, value);
        }
        public static readonly DependencyProperty CentreXProperty = DependencyProperty.Register(
			"CentreX",
			typeof(double),
			typeof(ProgressArc),
			new FrameworkPropertyMetadata(
				0.0,
				FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

		public double CentreY
        {
            get => (double)GetValue(CentreYProperty);
            set => SetValue(CentreYProperty, value);
        }
        public static readonly DependencyProperty CentreYProperty = DependencyProperty.Register(
			"CentreY",
			typeof(double),
			typeof(ProgressArc),
			new FrameworkPropertyMetadata(
				0.0,
				FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

		public double Percentage
        {
            get => (double)GetValue(PercentageProperty);
            set => SetValue(PercentageProperty, value);
        }
        public static readonly DependencyProperty PercentageProperty = DependencyProperty.Register(
			"Percentage",
			typeof(double),
			typeof(ProgressArc),
			new FrameworkPropertyMetadata(0.0,
				FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

		protected override Geometry DefiningGeometry
		{
			get
			{
				// Create a StreamGeometry for describing the shape
				StreamGeometry geometry = new StreamGeometry();
				geometry.FillRule = FillRule.EvenOdd;

				using (StreamGeometryContext context = geometry.Open())
				{
					DrawGeometry(context);
				}

				// Freeze the geometry for performance benefits
				geometry.Freeze();

				return geometry;
			}
		}

		private void DrawGeometry(StreamGeometryContext context)
		{
			if (Percentage == 100) Percentage = 99.9999;

			var innerRadius = InnerRadiusFraction * Radius;
			var endArcRadius = (Radius - innerRadius) / 2;

			var allowRound = (IsIndeterminate || (Percentage < 99.9999 && !IsIndeterminate)) && InnerRadiusFraction >= 0.4;

			// Prevents circular ends overlapping and drawing weirdly
			// Not accurate as this is trig rather than circular arc but I cba, sozzles
			var overlapArcAngle = allowRound ? Math.Atan(endArcRadius / (Radius - endArcRadius)) * 180 / Math.PI : 0;

			Point innerArcStartPoint = ComputeCartesianCoordinate(RotationAngle + overlapArcAngle, innerRadius);
			innerArcStartPoint.Offset(CentreX, CentreY);

			Point innerArcEndPoint = ComputeCartesianCoordinate(RotationAngle + (Percentage * 360 / 100) - overlapArcAngle, innerRadius);
			innerArcEndPoint.Offset(CentreX, CentreY);

			Point outerArcStartPoint = ComputeCartesianCoordinate(RotationAngle + overlapArcAngle, Radius);
			outerArcStartPoint.Offset(CentreX, CentreY);

			Point outerArcEndPoint = ComputeCartesianCoordinate(RotationAngle + (Percentage * 360 / 100) - overlapArcAngle, Radius);
			outerArcEndPoint.Offset(CentreX, CentreY);

			bool largeArc = (Percentage * 360 / 100) - (allowRound ? 2 * overlapArcAngle : 0) > 180;

			Size outerArcSize = new Size(Radius, Radius);
			Size innerArcSize = new Size(innerRadius, innerRadius);

			Size startArcSize = new Size(endArcRadius, endArcRadius);
			Size endArcSize = new Size(endArcRadius, endArcRadius);

			context.BeginFigure(innerArcStartPoint, true, true);
			if (RoundedEnd && allowRound)
				context.ArcTo(outerArcStartPoint, startArcSize, 0, true, SweepDirection.Clockwise, true, true);
			else
				context.LineTo(outerArcStartPoint, true, true);
			context.ArcTo(outerArcEndPoint, outerArcSize, 0, largeArc, SweepDirection.Clockwise, true, true);
			if (RoundedEnd && allowRound)
				context.ArcTo(innerArcEndPoint, endArcSize, 0, true, SweepDirection.Clockwise, true, true);
			else
				context.LineTo(innerArcEndPoint, true, true);
			context.ArcTo(innerArcStartPoint, innerArcSize, 0, largeArc, SweepDirection.Counterclockwise, true, true);
		}

		private Point ComputeCartesianCoordinate(double angle, double radius)
		{
			double angleRad = Math.PI / 180.0 * (angle - 90);

			double x = radius * Math.Cos(angleRad);
			double y = radius * Math.Sin(angleRad);

			return new Point(x, y);
		}
	}
}
