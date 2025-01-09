namespace ModernThemables.Charting.Controls.ChartComponents
{
    using ModernThemables.Charting.Services;
    using System.Windows;
    using System.Windows.Controls;

    public class ZoomHost : ContentControl
	{
		private double currentZoomLevel = 1;

		private double xMin = 0;
		private double xMax = 0;

		private MouseCoordinator? currentCoordinator;
		private System.Windows.Input.MouseWheelEventArgs? lastArgs;

		public event EventHandler? ZoomChanged;

		public double PanOffsetFraction
		{
			get => (double)this.GetValue(PanOffsetFractionProperty);
			private set => this.SetValue(PanOffsetFractionProperty, value);
		}
		public static readonly DependencyProperty PanOffsetFractionProperty = DependencyProperty.Register(
			nameof(PanOffsetFraction),
			typeof(double),
			typeof(ZoomHost),
			new PropertyMetadata(0d));

		public double LeftFraction
		{
			get => (double)this.GetValue(LeftFractionProperty);
			private set => this.SetValue(LeftFractionProperty, value);
		}
		public static readonly DependencyProperty LeftFractionProperty = DependencyProperty.Register(
			nameof(LeftFraction),
			typeof(double),
			typeof(ZoomHost),
			new PropertyMetadata(0d));

		public double RightFraction
		{
			get => (double)this.GetValue(RightFractionProperty);
			private set => this.SetValue(RightFractionProperty, value);
		}
		public static readonly DependencyProperty RightFractionProperty = DependencyProperty.Register(
			nameof(RightFraction),
			typeof(double),
			typeof(ZoomHost),
			new PropertyMetadata(0d));

		public double BottomFraction
		{
			get => (double)this.GetValue(BottomFractionProperty);
			private set => this.SetValue(BottomFractionProperty, value);
		}
		public static readonly DependencyProperty BottomFractionProperty = DependencyProperty.Register(
			nameof(BottomFraction),
			typeof(double),
			typeof(ZoomHost),
			new PropertyMetadata(0d));

		public double TopFraction
		{
			get => (double)this.GetValue(TopFractionProperty);
			private set => this.SetValue(TopFractionProperty, value);
		}
		public static readonly DependencyProperty TopFractionProperty = DependencyProperty.Register(
			nameof(TopFraction),
			typeof(double),
			typeof(ZoomHost),
			new PropertyMetadata(0d));

		public bool IsZoomed
		{
			get => (bool)this.GetValue(IsZoomedProperty);
			set => this.SetValue(IsZoomedProperty, value);
		}
		public static readonly DependencyProperty IsZoomedProperty = DependencyProperty.Register(
			nameof(IsZoomed),
			typeof(bool),
			typeof(ZoomHost),
			new PropertyMetadata(false));

		public Func<(double minFrac, double maxFrac)> GetDataHeightPixelsInBounds
		{
			get => (Func<(double minFrac, double maxFrac)>)this.GetValue(GetDataHeightPixelsInBoundsProperty);
			set => this.SetValue(GetDataHeightPixelsInBoundsProperty, value);
		}
		public static readonly DependencyProperty GetDataHeightPixelsInBoundsProperty = DependencyProperty.Register(
			nameof(GetDataHeightPixelsInBounds),
			typeof(Func<(double minFrac, double maxFrac)>),
			typeof(ZoomHost),
			new PropertyMetadata(null));

		public double YPaddingFrac
		{
			get => (double)this.GetValue(YPaddingFracProperty);
			set => this.SetValue(YPaddingFracProperty, value);
		}
		public static readonly DependencyProperty YPaddingFracProperty = DependencyProperty.Register(
			"YPaddingFrac",
			typeof(double),
			typeof(ZoomHost),
			new UIPropertyMetadata(0d, (s, e) => { (s as ZoomHost)!.Coordinator_MouseWheel(s, (s as ZoomHost)!.lastArgs); }));

		public ZoomHost()
		{
			Loaded += this.OnLoaded;
		}

		public void ResetZoom()
		{
			if (this.currentCoordinator == null) return;

            this.currentZoomLevel = 1;
            this.PanOffsetFraction = 0;
            this.xMin = 0;
            this.xMax = this.currentCoordinator.ActualWidth;
            this.LeftFraction = 0;
            this.RightFraction = 0;
            this.IsZoomed = false;

			var diffs = this.GetTopBottomDiff();

            this.Margin = new Thickness(0, double.IsNaN(diffs.top) ? 0 : -diffs.top, 0, double.IsNaN(diffs.bottom) ? 0 : -diffs.bottom);

			ZoomChanged?.Invoke(this, EventArgs.Empty);
		}

		public new void InvalidateArrange()
		{
			if (this.currentCoordinator == null) return;

            this.DoZoom(1, 0.5, this.PanOffsetFraction * this.currentCoordinator.ActualWidth);
			base.InvalidateArrange();
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			Loaded -= this.OnLoaded;
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
			if (this.currentCoordinator == null) return;
            this.lastArgs = e;

			var zoomStep = e == null ? 1d : e.Delta > 0 ? 0.9d : 1d / 0.9d;
			var panOffset = this.PanOffsetFraction * this.currentCoordinator.ActualWidth;
			var zoomCentre = e == null ? 0.5 : (e.GetPosition(this.currentCoordinator).X + panOffset) / this.currentCoordinator.ActualWidth;

            this.DoZoom(zoomStep, zoomCentre, panOffset);
		}

		private void DoZoom(double zoomStep, double zoomCentre, double panOffset)
		{
			if (this.currentCoordinator == null) return;
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

                this.xMin = this.xMin + xDiff * zoomCentre;
                this.xMax = this.xMax - xDiff * (1 - zoomCentre);

                this.LeftFraction = this.xMin / this.currentCoordinator.ActualWidth;
                this.RightFraction = (this.currentCoordinator.ActualWidth - this.xMax) / this.currentCoordinator.ActualWidth;

				var leftDiff = (this.currentCoordinator.ActualWidth * this.currentZoomLevel) * this.LeftFraction;
				var rightDiff = (this.currentCoordinator.ActualWidth * this.currentZoomLevel) * this.RightFraction;

				var diffs = this.GetTopBottomDiff();

                this.Margin = new Thickness(-leftDiff - panOffset, -diffs.top, -rightDiff + panOffset, -diffs.bottom);
			}

            this.IsZoomed = this.currentZoomLevel != 1 || this.PanOffsetFraction != 0;
			ZoomChanged?.Invoke(this, EventArgs.Empty);
		}

		private (double top, double bottom) GetTopBottomDiff()
		{
			if (this.currentCoordinator == null) return (0, 0);
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

		private void Coordinator_MouseMove(
			object? sender, 
			(bool isUserDragging,
			bool isUserPanning,
			Point? lowerSelection,
			Point lastMousePoint,
			System.Windows.Input.MouseEventArgs args) e)
		{
			if (this.currentCoordinator == null) return;
			if (e.isUserPanning)
			{
				var prevOffset = this.PanOffsetFraction * this.currentCoordinator.ActualWidth;
                this.PanOffsetFraction = this.PanOffsetFraction + (e.lastMousePoint.X - e.args.GetPosition(this.currentCoordinator).X) / this.currentCoordinator.ActualWidth;
				var panOffset = this.PanOffsetFraction * this.currentCoordinator.ActualWidth;

				var diffs = this.GetTopBottomDiff();

                this.Margin = new Thickness(this.Margin.Left + prevOffset - panOffset, -diffs.top, this.Margin.Right - prevOffset + panOffset, -diffs.bottom);

                this.IsZoomed = this.currentZoomLevel != 1 || this.PanOffsetFraction != 0;
				ZoomChanged?.Invoke(this, EventArgs.Empty);
			}
		}
	}
}
