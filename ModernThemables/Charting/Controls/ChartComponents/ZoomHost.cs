using ModernThemables.Charting.Services;
using System;
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
		private System.Windows.Input.MouseWheelEventArgs lastArgs;

		public event EventHandler ZoomChanged;

		public double PanOffsetFraction
		{
			get => (double)GetValue(PanOffsetFractionProperty);
			private set => SetValue(PanOffsetFractionProperty, value);
		}
		public static readonly DependencyProperty PanOffsetFractionProperty = DependencyProperty.Register(
			nameof(PanOffsetFraction),
			typeof(double),
			typeof(ZoomHost),
			new PropertyMetadata(0d));

		public double XZoom
		{
			get => (double)GetValue(XZoomProperty);
			private set => SetValue(XZoomProperty, value);
		}
		public static readonly DependencyProperty XZoomProperty = DependencyProperty.Register(
			nameof(XZoom),
			typeof(double),
			typeof(ZoomHost),
			new PropertyMetadata(1d));

		public double YZoom
		{
			get => (double)GetValue(YZoomProperty);
			private set => SetValue(YZoomProperty, value);
		}
		public static readonly DependencyProperty YZoomProperty = DependencyProperty.Register(
			nameof(YZoom),
			typeof(double),
			typeof(ZoomHost),
			new PropertyMetadata(1d));

		public double LeftFraction
		{
			get => (double)GetValue(LeftFractionProperty);
			private set => SetValue(LeftFractionProperty, value);
		}
		public static readonly DependencyProperty LeftFractionProperty = DependencyProperty.Register(
			nameof(LeftFraction),
			typeof(double),
			typeof(ZoomHost),
			new PropertyMetadata(0d));

		public double RightFraction
		{
			get => (double)GetValue(RightFractionProperty);
			private set => SetValue(RightFractionProperty, value);
		}
		public static readonly DependencyProperty RightFractionProperty = DependencyProperty.Register(
			nameof(RightFraction),
			typeof(double),
			typeof(ZoomHost),
			new PropertyMetadata(0d));

		public double BottomFraction
		{
			get => (double)GetValue(BottomFractionProperty);
			private set => SetValue(BottomFractionProperty, value);
		}
		public static readonly DependencyProperty BottomFractionProperty = DependencyProperty.Register(
			nameof(BottomFraction),
			typeof(double),
			typeof(ZoomHost),
			new PropertyMetadata(0d));

		public double TopFraction
		{
			get => (double)GetValue(TopFractionProperty);
			private set => SetValue(TopFractionProperty, value);
		}
		public static readonly DependencyProperty TopFractionProperty = DependencyProperty.Register(
			nameof(TopFraction),
			typeof(double),
			typeof(ZoomHost),
			new PropertyMetadata(0d));

		public bool IsZoomed
		{
			get => (bool)GetValue(IsZoomedProperty);
			set => SetValue(IsZoomedProperty, value);
		}
		public static readonly DependencyProperty IsZoomedProperty = DependencyProperty.Register(
			nameof(IsZoomed),
			typeof(bool),
			typeof(ZoomHost),
			new PropertyMetadata(false));

		public Func<(double minFrac, double maxFrac)> GetDataHeightPixelsInBounds
		{
			get => (Func<(double minFrac, double maxFrac)>)GetValue(GetDataHeightPixelsInBoundsProperty);
			set => SetValue(GetDataHeightPixelsInBoundsProperty, value);
		}
		public static readonly DependencyProperty GetDataHeightPixelsInBoundsProperty = DependencyProperty.Register(
			nameof(GetDataHeightPixelsInBounds),
			typeof(Func<(double minFrac, double maxFrac)>),
			typeof(ZoomHost),
			new PropertyMetadata(null));

		public double YPaddingFrac
		{
			get => (double)GetValue(YPaddingFracProperty);
			set => SetValue(YPaddingFracProperty, value);
		}
		public static readonly DependencyProperty YPaddingFracProperty = DependencyProperty.Register(
			"YPaddingFrac",
			typeof(double),
			typeof(ZoomHost),
			new UIPropertyMetadata(0d, (s, e) => { (s as ZoomHost).Coordinator_MouseWheel(s, (s as ZoomHost).lastArgs); }));

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
			PanOffsetFraction = 0;
			xMin = 0;
			xMax = currentCoordinator.ActualWidth;
			LeftFraction = 0;
			RightFraction = 0;
			IsZoomed = false;
			XZoom = 1;

			var diffs = GetTopBottomDiff(true);

			Margin = new Thickness(0, -diffs.top, 0, -diffs.bottom);

			ZoomChanged?.Invoke(this, EventArgs.Empty);
		}

		public void InvalidateArrange()
		{
			DoZoom(1, 0.5, PanOffsetFraction * currentCoordinator.ActualWidth);
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

			ResetZoom();
		}

		private void Coordinator_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
		{
			if (currentCoordinator == null) return;
			lastArgs = e;

			var zoomStep = e == null ? 1d : e.Delta > 0 ? 0.9d : 1d / 0.9d;
			var panOffset = PanOffsetFraction * currentCoordinator.ActualWidth;
			var zoomCentre = e == null ? 0.5 : (e.GetPosition(currentCoordinator).X + panOffset) / currentCoordinator.ActualWidth;

			DoZoom(zoomStep, zoomCentre, panOffset);
		}

		private void DoZoom(double zoomStep, double zoomCentre, double panOffset)
		{
			if (xMax == 0)
			{
				xMax = ActualWidth;
			}

			currentZoomLevel /= zoomStep;
			if (Math.Round(currentZoomLevel, 1) == 1)
			{
				ResetZoom();
			}
			else
			{
				var currXRange = xMax - xMin;
				var newXRange = currXRange * zoomStep;
				var xDiff = currXRange - newXRange;

				xMin = xMin + xDiff * zoomCentre;
				xMax = xMax - xDiff * (1 - zoomCentre);

				LeftFraction = xMin / currentCoordinator.ActualWidth;
				RightFraction = (currentCoordinator.ActualWidth - xMax) / currentCoordinator.ActualWidth;

				var leftDiff = (currentCoordinator.ActualWidth * currentZoomLevel) * LeftFraction;
				var rightDiff = (currentCoordinator.ActualWidth * currentZoomLevel) * RightFraction;

				var diffs = GetTopBottomDiff();

				XZoom = 1 / (1 - RightFraction - LeftFraction);

				Margin = new Thickness(-leftDiff - panOffset, -diffs.top, -rightDiff + panOffset, -diffs.bottom);
			}

			IsZoomed = currentZoomLevel != 1 || PanOffsetFraction != 0;
			ZoomChanged?.Invoke(this, EventArgs.Empty);
		}

		private (double top, double bottom) GetTopBottomDiff(bool isReset = false)
		{
			var dataHeightPx = GetDataHeightPixelsInBounds != null && !isReset
					? GetDataHeightPixelsInBounds() : (currentCoordinator.ActualHeight, 0);
			var dataRange = dataHeightPx.Item2 - dataHeightPx.Item1;
			var buffer = dataRange * YPaddingFrac;
			BottomFraction = 1 - (dataHeightPx.Item1 - buffer) / currentCoordinator.ActualHeight;
			TopFraction = (dataHeightPx.Item2 + buffer) / currentCoordinator.ActualHeight;

			var newHeight = currentCoordinator.ActualHeight / (1 - (TopFraction + BottomFraction));

			var topDiff = newHeight * TopFraction;
			var bottomDiff = newHeight * BottomFraction;

			YZoom = 1 / (1 - BottomFraction - TopFraction);

			return (topDiff, bottomDiff);
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
				var prevOffset = PanOffsetFraction * currentCoordinator.ActualWidth;
				PanOffsetFraction = PanOffsetFraction + (e.lastMousePoint.X - e.args.GetPosition(currentCoordinator).X) / currentCoordinator.ActualWidth;
				var panOffset = PanOffsetFraction * currentCoordinator.ActualWidth;

				var diffs = GetTopBottomDiff();

				Margin = new Thickness(Margin.Left + prevOffset - panOffset, -diffs.top, Margin.Right - prevOffset + panOffset, -diffs.bottom);

				IsZoomed = currentZoomLevel != 1 || PanOffsetFraction != 0;
				ZoomChanged?.Invoke(this, EventArgs.Empty);
			}
		}
	}
}
