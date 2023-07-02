using ModernThemables.Charting.Controls.ChartComponents;
using ModernThemables.Charting.Services;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ModernThemables.Charting.Controls.ChartComponents
{
	public class ZoomHost : ContentControl
	{
		private double currentZoomLevel = 1;
		private double currentOffset = 0;

		private double xMin = 0;
		private double xMax = 0;

		private MouseCoordinator currentCoordinator;

		static ZoomHost()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomHost), new FrameworkPropertyMetadata(typeof(ZoomHost)));
		}

		public ZoomHost()
		{
			Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			Loaded -= OnLoaded;
			if (ChartHelper.FindMouseCoordinatorFromVisualTree(this, out var coordinator))
			{
				currentCoordinator = coordinator;
				coordinator.MouseWheel += Coordinator_MouseWheel;
				coordinator.MouseMove += Coordinator_MouseMove;
			}
			else
			{
				throw new InvalidOperationException("Please add a MouseCoordinator to your chart");
			}
		}

		private void Coordinator_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
		{
			if (xMax == 0)
			{
				xMax = ActualWidth;
			}

			var zoomStep = e.Delta > 0 ? 0.9d : 1d / 0.9d;
			currentZoomLevel /= zoomStep;
			if (Math.Round(currentZoomLevel, 1) == 1)
			{
				currentZoomLevel = 1;
				currentOffset = 0;
				xMin = 0;
				xMax = currentCoordinator.ActualWidth;
				Margin = new Thickness(0);
			}
			else
			{
				var zoomCentre = e.GetPosition(currentCoordinator).X / currentCoordinator.ActualWidth;

				var currXRange = xMax - xMin;
				var newXRange = currXRange * zoomStep;
				var xDiff = currXRange - newXRange;
				xMin = xMin + xDiff * zoomCentre;
				xMax = xMax - xDiff * (1 - zoomCentre);

				var fracLeft = xMin / currentCoordinator.ActualWidth;
				var fracRight = (currentCoordinator.ActualWidth - xMax) / currentCoordinator.ActualWidth;

				var leftDiff = (currentCoordinator.ActualWidth * currentZoomLevel) * fracLeft;
				var rightDiff = (currentCoordinator.ActualWidth * currentZoomLevel) * fracRight;

				Margin = new Thickness(-leftDiff, 0, -rightDiff, 0);
			}

			//IsZoomed = SeriesItemsControl.Margin.Left != 0 || SeriesItemsControl.Margin.Right != 0;
		}

		private void Coordinator_MouseMove(
			object? sender, 
			(bool isUserDragging,
			bool isUserPanning,
			Point? lowerSelection,
			Point lastMousePoint,
			System.Windows.Input.MouseEventArgs args) e)
		{
			if (e.isUserPanning)
			{
				var prevOffset = currentOffset;
				currentOffset = currentOffset + e.lastMousePoint.X - e.args.GetPosition(currentCoordinator).X;
				Margin = new Thickness(Margin.Left + prevOffset - currentOffset, 0, Margin.Right - prevOffset + currentOffset, 0);
			}
		}
	}
}
