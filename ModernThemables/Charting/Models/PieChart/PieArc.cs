using System;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;

namespace ModernThemables.Charting.Models.PieChart
{
    public class PieArc : Shape
    {
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
            "Radius",
            typeof(double),
            typeof(PieArc),
            new FrameworkPropertyMetadata(
                0.0,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

		public bool IsIndeterminate
		{
			get { return (bool)GetValue(IsIndeterminateProperty); }
			set { SetValue(IsIndeterminateProperty, value); }
		}
		public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register(
			"IsIndeterminate",
			typeof(bool),
			typeof(PieArc),
			new FrameworkPropertyMetadata(
				true,
				FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

		public double PushOut
        {
            get { return (double)GetValue(PushOutProperty); }
            set { SetValue(PushOutProperty, value); }
        }
        public static readonly DependencyProperty PushOutProperty = DependencyProperty.Register(
            "PushOut",
            typeof(double),
            typeof(PieArc),
            new FrameworkPropertyMetadata(
                0.0,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double InnerRadiusFraction
        {
            get { return (double)GetValue(InnerRadiusProperty); }
            set { SetValue(InnerRadiusProperty, value); }
        }
        public static readonly DependencyProperty InnerRadiusProperty = DependencyProperty.Register(
            "InnerRadiusFraction",
            typeof(double),
            typeof(PieArc),
            new FrameworkPropertyMetadata(
                0.0,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double RotationAngle
        {
            get { return (double)GetValue(RotationAngleProperty); }
            set { SetValue(RotationAngleProperty, value); }
        }
        public static readonly DependencyProperty RotationAngleProperty = DependencyProperty.Register(
            "RotationAngle",
            typeof(double),
            typeof(PieArc),
            new FrameworkPropertyMetadata(
                0.0,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double CentreX
        {
            get { return (double)GetValue(CentreXProperty); }
            set { SetValue(CentreXProperty, value); }
        }
        public static readonly DependencyProperty CentreXProperty = DependencyProperty.Register(
            "CentreX",
            typeof(double),
            typeof(PieArc),
            new FrameworkPropertyMetadata(
                0.0,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double CentreY
        {
            get { return (double)GetValue(CentreYProperty); }
            set { SetValue(CentreYProperty, value); }
        }
        public static readonly DependencyProperty CentreYProperty = DependencyProperty.Register(
            "CentreY",
            typeof(double),
            typeof(PieArc),
            new FrameworkPropertyMetadata(
                0.0,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double Percentage
        {
            get { return (double)GetValue(PercentageProperty); }
            set { SetValue(PercentageProperty, value); }
        }
        public static readonly DependencyProperty PercentageProperty = DependencyProperty.Register(
            "Percentage",
            typeof(double),
            typeof(PieArc),
            new FrameworkPropertyMetadata(0.0,
				FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double PieceValue
        {
            get { return (double)GetValue(PieceValueProperty); }
            set { SetValue(PieceValueProperty, value); }
        }
        public static readonly DependencyProperty PieceValueProperty = DependencyProperty.Register(
            "PieceValue",
            typeof(double),
            typeof(PieArc),
            new FrameworkPropertyMetadata(0.0));

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
            Point startPoint = new Point(CentreX, CentreY);

            if (Percentage == 100) Percentage = 99.9999;

            var innerRadius = InnerRadiusFraction * Radius;

            Point innerArcStartPoint = ComputeCartesianCoordinate(RotationAngle, innerRadius);
            innerArcStartPoint.Offset(CentreX, CentreY);

            Point innerArcEndPoint = ComputeCartesianCoordinate(RotationAngle + Percentage * 360 / 100, innerRadius);
            innerArcEndPoint.Offset(CentreX, CentreY);

            Point outerArcStartPoint = ComputeCartesianCoordinate(RotationAngle, Radius);
            outerArcStartPoint.Offset(CentreX, CentreY);

            Point outerArcEndPoint = ComputeCartesianCoordinate(RotationAngle + Percentage * 360 / 100, Radius);
            outerArcEndPoint.Offset(CentreX, CentreY);

            bool largeArc = Percentage > 50;

            if (PushOut > 0)
            {
                Point offset = ComputeCartesianCoordinate(RotationAngle + Percentage * 360 / 100 / 2, PushOut);
                innerArcStartPoint.Offset(offset.X, offset.Y);
                innerArcEndPoint.Offset(offset.X, offset.Y);
                outerArcStartPoint.Offset(offset.X, offset.Y);
                outerArcEndPoint.Offset(offset.X, offset.Y);
            }

            Size outerArcSize = new Size(Radius, Radius);
            Size innerArcSize = new Size(innerRadius, innerRadius);

            context.BeginFigure(innerArcStartPoint, true, true);
            context.LineTo(outerArcStartPoint, true, true);
            context.ArcTo(outerArcEndPoint, outerArcSize, 0, largeArc, SweepDirection.Clockwise, true, true);
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
