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
		public event EventHandler<(bool isUserDragging, bool isUserPanning, Point? lowerSelection, MouseEventArgs args)>? MouseMove;

		private bool isMouseDown;
		private bool isUserDragging;
		private bool userCouldBePanning;
		private bool isUserPanning;
		private Point? lastMouseMovePoint;

		private DateTime timeLastUpdated;
		private TimeSpan? updateLimit;

		private Point? lowerSelection;
		private Point? upperSelection;

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
			var mouseLoc = e.GetPosition(MouseCaptureGrid);

			if (isMouseDown)
			{
				isUserDragging = true;
			}

			if (userCouldBePanning)
			{
				isUserPanning = true;
				if (lastMouseMovePoint != null)
				{
					// Do we want to do anything here?
				}
			}
			lastMouseMovePoint = mouseLoc;

			if (updateLimit != null && DateTime.Now - timeLastUpdated < updateLimit) return;

			timeLastUpdated = DateTime.Now;

			MouseMove?.Invoke(this, (isUserDragging, isUserPanning, lowerSelection, e));
		}

		private void MouseCaptureGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			isMouseDown = true;
			if (e.ChangedButton == MouseButton.Left)
			{
				e.Handled = true;
				lowerSelection = e.GetPosition(MouseCaptureGrid);
			}
			else if (e.ChangedButton == MouseButton.Right)
			{
				userCouldBePanning = true;
			}
		}

		private void MouseCaptureGrid_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			isMouseDown = false;

			if (e.ChangedButton != MouseButton.Left)
			{
				isUserDragging = false;
				PointClicked?.Invoke(this, lowerSelection.Value);
				return;
			}

			if (isUserDragging && lowerSelection != null)
			{
				upperSelection = e.GetPosition(MouseCaptureGrid);
				isUserDragging = false;
				isUserPanning = false;
				PointRangeSelected?.Invoke(this, (lowerSelection.Value, upperSelection.Value));
				return;
			}

			e.Handled = true;
		}
	}
}
