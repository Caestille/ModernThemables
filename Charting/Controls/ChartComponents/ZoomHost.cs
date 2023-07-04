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

		private double xMin = 0;
		private double xMax = 0;

		private MouseCoordinator currentCoordinator;

		public event EventHandler ZoomChanged;

		public double PanOffset
		{
			get => (double)GetValue(PanOffsetProperty);
			private set => SetValue(PanOffsetProperty, value);
		}
		public static readonly DependencyProperty PanOffsetProperty = DependencyProperty.Register(
			"PanOffset",
			typeof(double),
			typeof(ZoomHost),
			new PropertyMetadata(0d));

		public double Min
		{
			get => (double)GetValue(MinProperty);
			private set => SetValue(MinProperty, value);
		}
		public static readonly DependencyProperty MinProperty = DependencyProperty.Register(
			"MinProperty",
			typeof(double),
			typeof(ZoomHost),
			new PropertyMetadata(0d));

		public double Max
		{
			get => (double)GetValue(MaxProperty);
			private set => SetValue(MaxProperty, value);
		}
		public static readonly DependencyProperty MaxProperty = DependencyProperty.Register(
			"MaxProperty",
			typeof(double),
			typeof(ZoomHost),
			new PropertyMetadata(1d));

		public bool IsZoomed
		{
			get => (bool)GetValue(IsZoomedProperty);
			set => SetValue(IsZoomedProperty, value);
		}
		public static readonly DependencyProperty IsZoomedProperty = DependencyProperty.Register(
			"IsZoomed",
			typeof(bool),
			typeof(ZoomHost),
			new PropertyMetadata(false));

		static ZoomHost()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomHost), new FrameworkPropertyMetadata(typeof(ZoomHost)));
		}

		public ZoomHost()
		{
			Loaded += OnLoaded;
		}

		public void ResetZoom()
		{
			currentZoomLevel = 1;
			PanOffset = 0;
			xMin = 0;
			xMax = currentCoordinator.ActualWidth;
			Margin = new Thickness(0);
			IsZoomed = false;
			ZoomChanged?.Invoke(this, EventArgs.Empty);
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
				ResetZoom();
			}
			else
			{
				var zoomCentre = (e.GetPosition(currentCoordinator).X + PanOffset) / currentCoordinator.ActualWidth;

				var currXRange = xMax - xMin;
				var newXRange = currXRange * zoomStep;
				var xDiff = currXRange - newXRange;

				xMin = xMin + xDiff * zoomCentre;
				xMax = xMax - xDiff * (1 - zoomCentre);

				Min = xMin / currentCoordinator.ActualWidth;
				Max = (currentCoordinator.ActualWidth - xMax) / currentCoordinator.ActualWidth;

				var leftDiff = (currentCoordinator.ActualWidth * currentZoomLevel) * Min;
				var rightDiff = (currentCoordinator.ActualWidth * currentZoomLevel) * Max;

				Margin = new Thickness(-leftDiff - PanOffset, 0, -rightDiff + PanOffset, 0);
			}

			IsZoomed = currentZoomLevel != 1 || PanOffset != 0;
			ZoomChanged?.Invoke(this, EventArgs.Empty);
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
				var prevOffset = PanOffset;
				PanOffset = PanOffset + e.lastMousePoint.X - e.args.GetPosition(currentCoordinator).X;
				Margin = new Thickness(Margin.Left + prevOffset - PanOffset, 0, Margin.Right - prevOffset + PanOffset, 0);

				IsZoomed = currentZoomLevel != 1 || PanOffset != 0;
				ZoomChanged?.Invoke(this, EventArgs.Empty);
			}
		}
	}
}
