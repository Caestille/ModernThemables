namespace ModernThemables.Charting.Controls.ChartComponents;

using System.Windows;
using System.Windows.Controls;
using ModernThemables.Charting.Services;

public class ZoomHost : ContentControl
{
    public static readonly DependencyProperty PanOffsetFractionProperty = DependencyProperty.Register(
        nameof(PanOffsetFraction),
        typeof(double),
        typeof(ZoomHost),
        new PropertyMetadata(0d));

    public static readonly DependencyProperty LeftFractionProperty = DependencyProperty.Register(
        nameof(LeftFraction),
        typeof(double),
        typeof(ZoomHost),
        new PropertyMetadata(0d));

    public static readonly DependencyProperty RightFractionProperty = DependencyProperty.Register(
        nameof(RightFraction),
        typeof(double),
        typeof(ZoomHost),
        new PropertyMetadata(0d));

    public static readonly DependencyProperty BottomFractionProperty = DependencyProperty.Register(
        nameof(BottomFraction),
        typeof(double),
        typeof(ZoomHost),
        new PropertyMetadata(0d));

    public static readonly DependencyProperty TopFractionProperty = DependencyProperty.Register(
        nameof(TopFraction),
        typeof(double),
        typeof(ZoomHost),
        new PropertyMetadata(0d));

    public static readonly DependencyProperty IsZoomedProperty = DependencyProperty.Register(
        nameof(IsZoomed),
        typeof(bool),
        typeof(ZoomHost),
        new PropertyMetadata(false));

    public static readonly DependencyProperty GetDataHeightPixelsInBoundsProperty = DependencyProperty.Register(
        nameof(GetDataHeightPixelsInBounds),
        typeof(Func<(double MinFrac, double MaxFrac)>),
        typeof(ZoomHost),
        new PropertyMetadata(null));

    public static readonly DependencyProperty YPaddingFracProperty = DependencyProperty.Register(
        nameof(YPaddingFrac),
        typeof(double),
        typeof(ZoomHost),
        new UIPropertyMetadata(0d, (s, e) => { (s as ZoomHost)!.Coordinator_MouseWheel(s, (s as ZoomHost)!.lastArgs); }));

    private double currentZoomLevel = 1;

    private double xMin = 0;
    private double xMax = 0;

    private MouseCoordinator? currentCoordinator;
    private System.Windows.Input.MouseWheelEventArgs? lastArgs;

    public ZoomHost()
    {
        this.Loaded += this.OnLoaded;
    }

    public event EventHandler? ZoomChanged;

    public double PanOffsetFraction
    {
        get => (double)this.GetValue(PanOffsetFractionProperty);
        private set => this.SetValue(PanOffsetFractionProperty, value);
    }

    public double LeftFraction
    {
        get => (double)this.GetValue(LeftFractionProperty);
        private set => this.SetValue(LeftFractionProperty, value);
    }

    public double RightFraction
    {
        get => (double)this.GetValue(RightFractionProperty);
        private set => this.SetValue(RightFractionProperty, value);
    }

    public double BottomFraction
    {
        get => (double)this.GetValue(BottomFractionProperty);
        private set => this.SetValue(BottomFractionProperty, value);
    }

    public double TopFraction
    {
        get => (double)this.GetValue(TopFractionProperty);
        private set => this.SetValue(TopFractionProperty, value);
    }

    public bool IsZoomed
    {
        get => (bool)this.GetValue(IsZoomedProperty);
        set => this.SetValue(IsZoomedProperty, value);
    }

    public Func<(double MinFrac, double MaxFrac)> GetDataHeightPixelsInBounds
    {
        get => (Func<(double MinFrac, double MaxFrac)>)this.GetValue(GetDataHeightPixelsInBoundsProperty);
        set => this.SetValue(GetDataHeightPixelsInBoundsProperty, value);
    }

    public double YPaddingFrac
    {
        get => (double)this.GetValue(YPaddingFracProperty);
        set => this.SetValue(YPaddingFracProperty, value);
    }

    public void ResetZoom()
    {
        if (this.currentCoordinator == null)
        {
            return;
        }

        this.currentZoomLevel = 1;
        this.PanOffsetFraction = 0;
        this.xMin = 0;
        this.xMax = this.currentCoordinator.ActualWidth;
        this.LeftFraction = 0;
        this.RightFraction = 0;
        this.IsZoomed = false;

        var diffs = this.GetTopBottomDiff();

        this.Margin = new Thickness(
            0,
            double.IsNaN(diffs.Top) ? 0 : -diffs.Top,
            0,
            double.IsNaN(diffs.Bottom) ? 0 : -diffs.Bottom);

        this.ZoomChanged?.Invoke(this, EventArgs.Empty);
    }

    public new void InvalidateArrange()
    {
        if (this.currentCoordinator == null)
        {
            return;
        }

        this.DoZoom(1, 0.5, this.PanOffsetFraction * this.currentCoordinator.ActualWidth);
        base.InvalidateArrange();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        this.Loaded -= this.OnLoaded;
        if (ChartHelper.FindMouseCoordinatorFromVisualTree(this, out var coordinator) && coordinator != null)
        {
            this.currentCoordinator = coordinator;
            coordinator.MouseWheel += this.Coordinator_MouseWheel;
            coordinator.MouseMove += this.Coordinator_MouseMove;
        }
        else
        {
#if !DEBUG
            throw new InvalidOperationException("Please add a MouseCoordinator to your chart");
#endif
        }

        this.ResetZoom();
    }

    private void Coordinator_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs? e)
    {
        if (this.currentCoordinator == null)
        {
            return;
        }

        this.lastArgs = e;

        var zoomStep = e == null ? 1d : e.Delta > 0 ? 0.9d : 1d / 0.9d;
        var panOffset = this.PanOffsetFraction * this.currentCoordinator.ActualWidth;
        var zoomCentre = e == null
            ? 0.5
            : (e.GetPosition(this.currentCoordinator).X + panOffset) / this.currentCoordinator.ActualWidth;

        this.DoZoom(zoomStep, zoomCentre, panOffset);
    }

    private void DoZoom(double zoomStep, double zoomCentre, double panOffset)
    {
        if (this.currentCoordinator == null)
        {
            return;
        }

        if (this.xMax == 0)
        {
            this.xMax = this.ActualWidth;
        }

        this.currentZoomLevel /= zoomStep;
        if (Math.Round(this.currentZoomLevel, 1) == 1)
        {
            this.ResetZoom();
        }
        else
        {
            var currXRange = this.xMax - this.xMin;
            var newXRange = currXRange * zoomStep;
            var xDiff = currXRange - newXRange;

            this.xMin = this.xMin + (xDiff * zoomCentre);
            this.xMax = this.xMax - (xDiff * (1 - zoomCentre));

            this.LeftFraction = this.xMin / this.currentCoordinator.ActualWidth;
            this.RightFraction = (this.currentCoordinator.ActualWidth - this.xMax) / this.currentCoordinator.ActualWidth;

            var leftDiff = (this.currentCoordinator.ActualWidth * this.currentZoomLevel) * this.LeftFraction;
            var rightDiff = (this.currentCoordinator.ActualWidth * this.currentZoomLevel) * this.RightFraction;

            var diffs = this.GetTopBottomDiff();

            this.Margin = new Thickness(
                -leftDiff - panOffset,
                -diffs.Top,
                -rightDiff + panOffset,
                -diffs.Bottom);
        }

        this.IsZoomed = this.currentZoomLevel != 1 || this.PanOffsetFraction != 0;
        this.ZoomChanged?.Invoke(this, EventArgs.Empty);
    }

    private (double Top, double Bottom) GetTopBottomDiff()
    {
        if (this.currentCoordinator == null)
        {
            return (0, 0);
        }

        var dataHeightPx = this.GetDataHeightPixelsInBounds != null
            ? this.GetDataHeightPixelsInBounds()
            : (0, this.currentCoordinator.ActualHeight);
        var dataRange = dataHeightPx.Item2 - dataHeightPx.Item1;
        var buffer = dataRange * this.YPaddingFrac;
        this.TopFraction = (dataHeightPx.Item1 - buffer) / this.currentCoordinator.ActualHeight;
        this.BottomFraction = 1 - ((dataHeightPx.Item2 + buffer) / this.currentCoordinator.ActualHeight);

        var newHeight = this.currentCoordinator.ActualHeight / (1 - (this.TopFraction + this.BottomFraction));

        var topDiff = newHeight * this.TopFraction;
        var bottomDiff = newHeight * this.BottomFraction;

        return (topDiff, bottomDiff);
    }

    private void Coordinator_MouseMove(object? sender, MouseCoordinatorMouseMoveEventArgs e)
    {
        if (this.currentCoordinator == null)
        {
            return;
        }

        if (e.IsUserPanning)
        {
            var prevOffset = this.PanOffsetFraction * this.currentCoordinator.ActualWidth;
            this.PanOffsetFraction = this.PanOffsetFraction + ((e.LastMousePoint.X - e.Args.GetPosition(this.currentCoordinator).X) / this.currentCoordinator.ActualWidth);
            var panOffset = this.PanOffsetFraction * this.currentCoordinator.ActualWidth;

            var diffs = this.GetTopBottomDiff();

            this.Margin = new Thickness(
                this.Margin.Left + prevOffset - panOffset,
                -diffs.Top,
                this.Margin.Right - prevOffset + panOffset,
                -diffs.Bottom);

            this.IsZoomed = this.currentZoomLevel != 1 || this.PanOffsetFraction != 0;
            this.ZoomChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
