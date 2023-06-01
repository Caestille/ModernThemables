using ModernThemables.Charting.Models;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ModernThemables.Charting.Interfaces;
using System;

namespace ModernThemables.Charting.Controls.ChartComponents
{
	/// <summary>
	/// Interaction logic for TooltipControl.xaml
	/// </summary>
	public partial class TooltipControl : UserControl
	{
		private bool ignoreNextMouseMove;
		private bool isMouseDown;
		private bool isUserDragging;
		private bool userCouldBePanning;
		private bool isUserPanning;
		private Point? lastMouseMovePoint;

		private DateTime timeLastUpdated;
		private TimeSpan updateLimit = TimeSpan.FromMilliseconds(1000 / 60d);

		private Point? lowerSelection;
		private Point? upperSelection;

		public bool ShowCrosshair
		{
			get => (bool)GetValue(ShowCrosshairProperty);
			set => SetValue(ShowCrosshairProperty, value);
		}
		public static readonly DependencyProperty ShowCrosshairProperty = DependencyProperty.Register(
			"ShowCrosshair",
			typeof(bool),
			typeof(TooltipControl),
			new UIPropertyMetadata(false));

		private bool IsUserSelectingRange
		{
			get => (bool)GetValue(IsUserSelectingRangeProperty);
			set => SetValue(IsUserSelectingRangeProperty, value);
		}
		public static readonly DependencyProperty IsUserSelectingRangeProperty = DependencyProperty.Register(
			"IsUserSelectingRange",
			typeof(bool),
			typeof(TooltipControl),
			new PropertyMetadata(false));

		public TooltipControl()
		{
			InitializeComponent();
		}

		private void MouseCaptureGrid_MouseMove(object sender, MouseEventArgs e)
		{
			if (ignoreNextMouseMove)
			{
				ignoreNextMouseMove = false;
				return;
			}

			var mouseLoc = e.GetPosition(Grid);

			if (isMouseDown)
			{
				isUserDragging = true;
				IsUserSelectingRange = !(userCouldBePanning || isUserPanning);
			}

			#region Chart panning
			if (userCouldBePanning)
			{
				isUserPanning = true;
				if (lastMouseMovePoint != null)
				{
					// Do we want to do anything here?
				}
			}
			lastMouseMovePoint = mouseLoc;
			#endregion

			if (DateTime.Now - timeLastUpdated < updateLimit || isUserPanning) return;

			timeLastUpdated = DateTime.Now;

			#region Crosshairs
			if (ShowCrosshair)
			{
				// Move crosshairs
				XCrosshair.Margin = new Thickness(0, mouseLoc.Y, 0, 0);
				YCrosshair.Margin = new Thickness(mouseLoc.X, 0, 0, 0);
			}
			#endregion

			#region Selected range
			if (IsUserSelectingRange && lowerSelection != null)
			{
				var negative = mouseLoc.X < lowerSelection.Value.X;
				var margin = SelectionRangeBorder.Margin;
				margin.Left = negative ? mouseLoc.X : lowerSelection.Value.X;
				SelectionRangeBorder.Margin = margin;
				SelectionRangeBorder.Width = negative
					? Math.Max(lowerSelection.Value.X - mouseLoc.X, 0)
					: Math.Max(mouseLoc.X - lowerSelection.Value.X, 0);
			}
			#endregion
		}

		private void MouseCaptureGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			isMouseDown = true;
			if (e.ChangedButton == MouseButton.Left)
			{
				e.Handled = true;
				lowerSelection = e.GetPosition(Grid);
				SelectionRangeBorder.Margin = new Thickness(lowerSelection.Value.X, 0, 0, 0);
			}
			else if (e.ChangedButton == MouseButton.Right)
			{
				userCouldBePanning = true;
			}
		}

		private void MouseCaptureGrid_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			isMouseDown = false;
			IsUserSelectingRange = false;

			if (e.ChangedButton != MouseButton.Left)
			{
				isUserDragging = false;
				return;
			}

			if (isUserDragging && lowerSelection != null)
			{
				upperSelection = e.GetPosition(Grid);
				isUserDragging = false;
				isUserPanning = false;
				userCouldBePanning = false;
				return;
			}

			e.Handled = true;
		}
	}
}
