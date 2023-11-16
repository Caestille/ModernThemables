using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System;

namespace ModernThemables.Charting.Controls.ChartComponents
{
	/// <summary>
	/// Interaction logic for MouseCoordinator.xaml
	/// </summary>
	public partial class MouseCoordinator : UserControl
	{
		public event EventHandler<Point>? PointClicked;
		public event EventHandler<(Point lowerValue, Point upperValue)>? PointRangeSelected;
		public new event EventHandler<(bool isUserDragging, bool isUserPanning, Point? lowerSelection, Point lastMousePoint, MouseEventArgs args)>? MouseMove;

		private MouseButton? mouseDown;

		private DateTime timeLastUpdated;
		private TimeSpan? updateLimit;

		private Point? mouseDownPoint;
		private Point? mouseUpPoint;

		private Point? lastMouseMovePoint;

		private MouseEventArgs lastArgs;

		private bool isRunning;

		public double? MouseMoveThrottleMs
		{
			get => (double?)GetValue(MouseMoveThrottleMsProperty);
			set => SetValue(MouseMoveThrottleMsProperty, value);
		}
		public static readonly DependencyProperty MouseMoveThrottleMsProperty = DependencyProperty.Register(
			"MouseMoveThrottleMs",
			typeof(double?),
			typeof(MouseCoordinator),
			new PropertyMetadata(null, OnSetThrottle));

		public MouseCoordinator()
		{
			InitializeComponent();
		}

		private static void OnSetThrottle(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not MouseCoordinator control) return;
			control.updateLimit = control.MouseMoveThrottleMs != null
				? TimeSpan.FromMilliseconds(control.MouseMoveThrottleMs.Value)
				: null;
		}

		private void MouseCaptureGrid_MouseMove(object sender, MouseEventArgs e)
		{
			lastArgs = e;

			if (e.RightButton != MouseButtonState.Pressed)
			{
				MouseCaptureGrid.ReleaseMouseCapture();
			}

			if (isRunning || (updateLimit != null && DateTime.Now - timeLastUpdated < updateLimit)) return;

			isRunning = true;
			var mouseLoc = e.GetPosition(MouseCaptureGrid);
			timeLastUpdated = DateTime.Now;
			MouseMove?.Invoke(this, (mouseDown == MouseButton.Left, mouseDown == MouseButton.Right, mouseDownPoint, lastMouseMovePoint ?? mouseLoc, e));
			lastMouseMovePoint = mouseLoc;
			isRunning = false;
		}

		private void MouseCaptureGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			mouseDown = e.ChangedButton;
			mouseDownPoint = e.GetPosition(MouseCaptureGrid);
		}

		private void MouseCaptureGrid_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			mouseUpPoint = e.GetPosition(MouseCaptureGrid);

			if (mouseDown == MouseButton.Left)
			{
				if (mouseUpPoint == mouseDownPoint)
				{
					PointClicked?.Invoke(this, mouseDownPoint.Value);
					e.Handled = true;
				}
				else
				{
					PointRangeSelected?.Invoke(this, (mouseDownPoint.Value, mouseUpPoint.Value));
					e.Handled = true;
				}
			}

			mouseDown = null;
			mouseDownPoint = null;
			mouseUpPoint = null;
		}

		private void MouseCaptureGrid_MouseDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = false;
		}

		private void MouseCaptureGrid_MouseLeave(object sender, MouseEventArgs e)
		{
			
		}
	}
}
