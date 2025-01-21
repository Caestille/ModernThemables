namespace ModernThemables.Charting.Models.PieChart;

using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

public class PieArc : Shape
{
    public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
        nameof(Radius),
        typeof(double),
        typeof(PieArc),
        new FrameworkPropertyMetadata(
            0.0,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register(
        nameof(IsIndeterminate),
        typeof(bool),
        typeof(PieArc),
        new FrameworkPropertyMetadata(
            true,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty PushOutProperty = DependencyProperty.Register(
        nameof(PushOut),
        typeof(double),
        typeof(PieArc),
        new FrameworkPropertyMetadata(
            0.0,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty InnerRadiusProperty = DependencyProperty.Register(
        nameof(InnerRadiusFraction),
        typeof(double),
        typeof(PieArc),
        new FrameworkPropertyMetadata(
            0.0,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty RotationAngleProperty = DependencyProperty.Register(
        nameof(RotationAngle),
        typeof(double),
        typeof(PieArc),
        new FrameworkPropertyMetadata(
            0.0,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty CentreXProperty = DependencyProperty.Register(
        nameof(CentreX),
        typeof(double),
        typeof(PieArc),
        new FrameworkPropertyMetadata(
            0.0,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty CentreYProperty = DependencyProperty.Register(
        nameof(CentreY),
        typeof(double),
        typeof(PieArc),
        new FrameworkPropertyMetadata(
            0.0,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty PercentageProperty = DependencyProperty.Register(
        nameof(Percentage),
        typeof(double),
        typeof(PieArc),
        new FrameworkPropertyMetadata(
            0.0,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty PieceValueProperty = DependencyProperty.Register(
        nameof(PieceValue),
        typeof(double),
        typeof(PieArc),
        new FrameworkPropertyMetadata(0.0));

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

    public double PushOut
    {
        get => (double)this.GetValue(PushOutProperty);
        set => this.SetValue(PushOutProperty, value);
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

    public double PieceValue
    {
        get => (double)this.GetValue(PieceValueProperty);
        set => this.SetValue(PieceValueProperty, value);
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
        Point startPoint = new Point(this.CentreX, this.CentreY);

        if (this.Percentage == 100)
        {
            this.Percentage = 99.9999;
        }

        var innerRadius = this.InnerRadiusFraction * this.Radius;

        Point innerArcStartPoint = this.ComputeCartesianCoordinate(this.RotationAngle, innerRadius);
        innerArcStartPoint.Offset(this.CentreX, this.CentreY);

        Point innerArcEndPoint = this.ComputeCartesianCoordinate(this.RotationAngle + ((this.Percentage * 360) / 100), innerRadius);
        innerArcEndPoint.Offset(this.CentreX, this.CentreY);

        Point outerArcStartPoint = this.ComputeCartesianCoordinate(this.RotationAngle, this.Radius);
        outerArcStartPoint.Offset(this.CentreX, this.CentreY);

        Point outerArcEndPoint = this.ComputeCartesianCoordinate(this.RotationAngle + ((this.Percentage * 360) / 100), this.Radius);
        outerArcEndPoint.Offset(this.CentreX, this.CentreY);

        bool largeArc = this.Percentage > 50;

        if (this.PushOut > 0)
        {
            Point offset = this.ComputeCartesianCoordinate(this.RotationAngle + (((this.Percentage * 360) / 100) / 2), this.PushOut);
            innerArcStartPoint.Offset(offset.X, offset.Y);
            innerArcEndPoint.Offset(offset.X, offset.Y);
            outerArcStartPoint.Offset(offset.X, offset.Y);
            outerArcEndPoint.Offset(offset.X, offset.Y);
        }

        Size outerArcSize = new Size(this.Radius, this.Radius);
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
