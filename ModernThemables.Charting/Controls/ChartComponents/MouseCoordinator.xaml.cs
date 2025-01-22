namespace ModernThemables.Charting.Controls.ChartComponents;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

/// <summary>
/// Interaction logic for MouseCoordinator.xaml.
/// </summary>
public partial class MouseCoordinator : UserControl
{
    public static readonly DependencyProperty MouseMoveThrottleMsProperty = DependencyProperty.Register(
        nameof(MouseMoveThrottleMs),
        typeof(double?),
        typeof(MouseCoordinator),
        new PropertyMetadata(null, OnSetThrottle));

    private MouseButton? mouseDown;

    private DateTime timeLastUpdated;
    private TimeSpan? updateLimit;

    private Point? mouseDownPoint;
    private Point? mouseUpPoint;

    private Point? lastMouseMovePoint;

    private MouseEventArgs? lastArgs;

    private bool isRunning;

    public MouseCoordinator()
    {
        this.InitializeComponent();
    }

    public event EventHandler<Point>? PointClicked;

    public event EventHandler<(Point LowerValue, Point UpperValue)>? PointRangeSelected;

    public new event EventHandler<MouseCoordinatorMouseMoveEventArgs>? MouseMove;

    public double? MouseMoveThrottleMs
    {
        get => (double?)this.GetValue(MouseMoveThrottleMsProperty);
        set => this.SetValue(MouseMoveThrottleMsProperty, value);
    }

    private static void OnSetThrottle(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not MouseCoordinator control)
        {
            return;
        }

        control.updateLimit = control.MouseMoveThrottleMs != null
            ? TimeSpan.FromMilliseconds(control.MouseMoveThrottleMs.Value)
            : null;
    }

    private void MouseCaptureGrid_MouseMove(object sender, MouseEventArgs e)
    {
        this.lastArgs = e;

        if (e.RightButton != MouseButtonState.Pressed)
        {
            this.MouseCaptureGrid.ReleaseMouseCapture();
        }

        if (this.isRunning || (this.updateLimit != null && DateTime.Now - this.timeLastUpdated < this.updateLimit))
        {
            return;
        }

        this.isRunning = true;
        var mouseLoc = e.GetPosition(this.MouseCaptureGrid);
        this.timeLastUpdated = DateTime.Now;
        this.MouseMove?.Invoke(
            this,
            new MouseCoordinatorMouseMoveEventArgs(
                this.mouseDown == MouseButton.Left,
                this.mouseDown == MouseButton.Right,
                this.mouseDownPoint,
                this.lastMouseMovePoint ?? mouseLoc,
                e));
        this.lastMouseMovePoint = mouseLoc;
        this.isRunning = false;
    }

    private void MouseCaptureGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        this.mouseDown = e.ChangedButton;
        this.mouseDownPoint = e.GetPosition(this.MouseCaptureGrid);
    }

    private void MouseCaptureGrid_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        this.mouseUpPoint = e.GetPosition(this.MouseCaptureGrid);

        if (this.mouseDown == MouseButton.Left)
        {
            if (this.mouseUpPoint == this.mouseDownPoint)
            {
                this.PointClicked?.Invoke(this, this.mouseDownPoint.Value);
                e.Handled = true;
            }
            else if (this.mouseDownPoint != null && this.mouseUpPoint != null)
            {
                this.PointRangeSelected?.Invoke(this, (this.mouseDownPoint.Value, this.mouseUpPoint.Value));
                e.Handled = true;
            }
        }

        this.mouseDown = null;
        this.mouseDownPoint = null;
        this.mouseUpPoint = null;
    }

    private void MouseCaptureGrid_MouseDown(object sender, MouseButtonEventArgs e) => e.Handled = false;

    private void MouseCaptureGrid_MouseLeave(object sender, MouseEventArgs e) { }
}
