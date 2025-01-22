namespace ModernThemables.Controls;

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

public class ProgressArc : Shape
{
    public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
        nameof(Radius),
        typeof(double),
        typeof(ProgressArc),
        new FrameworkPropertyMetadata(
            0.0,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register(
        nameof(IsIndeterminate),
        typeof(bool),
        typeof(ProgressArc),
        new FrameworkPropertyMetadata(
            false,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty RoundedEndProperty = DependencyProperty.Register(
        nameof(RoundedEnd),
        typeof(bool),
        typeof(ProgressArc),
        new FrameworkPropertyMetadata(
            false,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty InnerRadiusProperty = DependencyProperty.Register(
        nameof(InnerRadiusFraction),
        typeof(double),
        typeof(ProgressArc),
        new FrameworkPropertyMetadata(
            0.0,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty RotationAngleProperty = DependencyProperty.Register(
        nameof(RotationAngle),
        typeof(double),
        typeof(ProgressArc),
        new FrameworkPropertyMetadata(
            0.0,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty CentreXProperty = DependencyProperty.Register(
        nameof(CentreX),
        typeof(double),
        typeof(ProgressArc),
        new FrameworkPropertyMetadata(
            0.0,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty CentreYProperty = DependencyProperty.Register(
        nameof(CentreY),
        typeof(double),
        typeof(ProgressArc),
        new FrameworkPropertyMetadata(
            0.0,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty PercentageProperty = DependencyProperty.Register(
        nameof(Percentage),
        typeof(double),
        typeof(ProgressArc),
        new FrameworkPropertyMetadata(
            0.0,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    static ProgressArc()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressArc), new FrameworkPropertyMetadata(typeof(ProgressArc)));
    }

    public ProgressArc() { }

    public double Radius
    {
        get => (double)this.GetValue(RadiusProperty);
        set => this.SetValue(RadiusProperty, value);
    }

    public bool IsIndeterminate
    {
        get => (bool)this.GetValue(IsIndeterminateProperty);
        set => this.SetValue(IsIndeterminateProperty, value);
    }

    public bool RoundedEnd
    {
        get => (bool)this.GetValue(RoundedEndProperty);
        set => this.SetValue(RoundedEndProperty, value);
    }

    public double InnerRadiusFraction
    {
        get => (double)this.GetValue(InnerRadiusProperty);
        set => this.SetValue(InnerRadiusProperty, value);
    }

    public double RotationAngle
    {
        get => (double)this.GetValue(RotationAngleProperty);
        set => this.SetValue(RotationAngleProperty, value);
    }

    public double CentreX
    {
        get => (double)this.GetValue(CentreXProperty);
        set => this.SetValue(CentreXProperty, value);
    }

    public double CentreY
    {
        get => (double)this.GetValue(CentreYProperty);
        set => this.SetValue(CentreYProperty, value);
    }

    public double Percentage
    {
        get => (double)this.GetValue(PercentageProperty);
        set => this.SetValue(PercentageProperty, value);
    }

    protected override Geometry DefiningGeometry
    {
        get
        {
            // Create a StreamGeometry for describing the shape
            StreamGeometry geometry = new StreamGeometry();
            geometry.FillRule = FillRule.EvenOdd;

            using (StreamGeometryContext context = geometry.Open())
            {
                this.DrawGeometry(context);
            }

            // Freeze the geometry for performance benefits
            geometry.Freeze();

            return geometry;
        }
    }

    private void DrawGeometry(StreamGeometryContext context)
    {
        if (this.Percentage == 100)
        {
            this.Percentage = 99.9999;
        }

        var innerRadius = this.InnerRadiusFraction * this.Radius;
        var endArcRadius = (this.Radius - innerRadius) / 2;

        var allowRound = (this.IsIndeterminate || (this.Percentage < 99.9999 && !this.IsIndeterminate)) && this.InnerRadiusFraction >= 0.4;

        // Prevents circular ends overlapping and drawing weirdly
        // Not accurate as this is trig rather than circular arc but I cba, sozzles
        var overlapArcAngle = allowRound ? Math.Atan(endArcRadius / (this.Radius - endArcRadius)) * 180 / Math.PI : 0;

        Point innerArcStartPoint = this.ComputeCartesianCoordinate(this.RotationAngle + overlapArcAngle, innerRadius);
        innerArcStartPoint.Offset(this.CentreX, this.CentreY);

        Point innerArcEndPoint = this.ComputeCartesianCoordinate(this.RotationAngle + (this.Percentage * 360 / 100) - overlapArcAngle, innerRadius);
        innerArcEndPoint.Offset(this.CentreX, this.CentreY);

        Point outerArcStartPoint = this.ComputeCartesianCoordinate(this.RotationAngle + overlapArcAngle, this.Radius);
        outerArcStartPoint.Offset(this.CentreX, this.CentreY);

        Point outerArcEndPoint = this.ComputeCartesianCoordinate(this.RotationAngle + (this.Percentage * 360 / 100) - overlapArcAngle, this.Radius);
        outerArcEndPoint.Offset(this.CentreX, this.CentreY);

        bool largeArc = (this.Percentage * 360 / 100) - (allowRound ? 2 * overlapArcAngle : 0) > 180;

        Size outerArcSize = new Size(this.Radius, this.Radius);
        Size innerArcSize = new Size(innerRadius, innerRadius);

        Size startArcSize = new Size(endArcRadius, endArcRadius);
        Size endArcSize = new Size(endArcRadius, endArcRadius);

        context.BeginFigure(innerArcStartPoint, true, true);
        if (this.RoundedEnd && allowRound)
        {
            context.ArcTo(outerArcStartPoint, startArcSize, 0, true, SweepDirection.Clockwise, true, true);
        }
        else
        {
            context.LineTo(outerArcStartPoint, true, true);
        }

        context.ArcTo(outerArcEndPoint, outerArcSize, 0, largeArc, SweepDirection.Clockwise, true, true);
        if (this.RoundedEnd && allowRound)
        {
            context.ArcTo(innerArcEndPoint, endArcSize, 0, true, SweepDirection.Clockwise, true, true);
        }
        else
        {
            context.LineTo(innerArcEndPoint, true, true);
        }

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
