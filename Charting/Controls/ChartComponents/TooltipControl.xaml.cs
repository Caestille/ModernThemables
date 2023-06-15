using ModernThemables.Charting.Models;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using ModernThemables.Charting.ViewModels;
using System.Collections.Generic;
using System.Linq;
using ModernThemables.Charting.Services;

namespace ModernThemables.Charting.Controls.ChartComponents
{
	/// <summary>
	/// Interaction logic for TooltipControl.xaml
	/// </summary>
	public partial class TooltipControl : UserControl
	{
		private bool tooltipLeft;
		private bool tooltipTop;

		private bool isUserPanning;

		private bool ShowPointIndicators
		{
			get => (bool)GetValue(ShowPointIndicatorsProperty);
			set => SetValue(ShowPointIndicatorsProperty, value);
		}
		public static readonly DependencyProperty ShowPointIndicatorsProperty = DependencyProperty.Register(
			"ShowPointIndicators",
			typeof(bool),
			typeof(TooltipControl),
			new UIPropertyMetadata(true));

		private bool ShowTooltip
		{
			get => (bool)GetValue(ShowTooltipProperty);
			set => SetValue(ShowTooltipProperty, value);
		}
		public static readonly DependencyProperty ShowTooltipProperty = DependencyProperty.Register(
			"ShowTooltip",
			typeof(bool),
			typeof(TooltipControl),
			new UIPropertyMetadata(true));

		public bool? ForceCrosshairs
		{
			get => (bool?)GetValue(ForceCrosshairsProperty);
			set => SetValue(ForceCrosshairsProperty, value);
		}
		public static readonly DependencyProperty ForceCrosshairsProperty = DependencyProperty.Register(
			"ForceCrosshairs",
			typeof(bool?),
			typeof(TooltipControl),
			new UIPropertyMetadata(null, OnSetForceCrosshairs));

		public bool? ForceTooltip
		{
			get => (bool?)GetValue(ForceTooltipProperty);
			set => SetValue(ForceTooltipProperty, value);
		}
		public static readonly DependencyProperty ForceTooltipProperty = DependencyProperty.Register(
			"ForceTooltip",
			typeof(bool?),
			typeof(TooltipControl),
			new UIPropertyMetadata(null, OnSetForceTooltip));

		public bool IsMouseOverThis
		{
			get => (bool)GetValue(IsMouseOverThisProperty);
			private set => SetValue(IsMouseOverThisProperty, value);
		}
		public static readonly DependencyProperty IsMouseOverThisProperty = DependencyProperty.Register(
			"IsMouseOverThis",
			typeof(bool),
			typeof(TooltipControl),
			new UIPropertyMetadata(false));

		public bool? ForcePointIndicators
		{
			get => (bool?)GetValue(PointIndicatorsProperty);
			set => SetValue(PointIndicatorsProperty, value);
		}
		public static readonly DependencyProperty PointIndicatorsProperty = DependencyProperty.Register(
			"PointIndicators",
			typeof(bool?),
			typeof(TooltipControl),
			new UIPropertyMetadata(null, OnSetForcePointIndicators));

		private bool ShowCrosshairs
		{
			get => (bool)GetValue(ShowCrosshairsProperty);
			set => SetValue(ShowCrosshairsProperty, value);
		}
		public static readonly DependencyProperty ShowCrosshairsProperty = DependencyProperty.Register(
			"ShowCrosshairs",
			typeof(bool),
			typeof(TooltipControl),
			new UIPropertyMetadata(true));

		public bool AllowSelection
		{
			get => (bool)GetValue(AllowSelectionProperty);
			set => SetValue(AllowSelectionProperty, value);
		}
		public static readonly DependencyProperty AllowSelectionProperty = DependencyProperty.Register(
			"AllowSelection",
			typeof(bool),
			typeof(TooltipControl),
			new UIPropertyMetadata(true));

		public bool ForceTooltipTop
		{
			get => (bool)GetValue(ForceTooltipTopProperty);
			set => SetValue(ForceTooltipTopProperty, value);
		}
		public static readonly DependencyProperty ForceTooltipTopProperty = DependencyProperty.Register(
			"ForceTooltipTop",
			typeof(bool),
			typeof(TooltipControl),
			new UIPropertyMetadata(false));

		public double TooltipOffsetX
		{
			get => (double)GetValue(TooltipOffsetXProperty);
			set => SetValue(TooltipOffsetXProperty, value);
		}
		public static readonly DependencyProperty TooltipOffsetXProperty = DependencyProperty.Register(
			"TooltipOffsetX",
			typeof(double),
			typeof(TooltipControl),
			new UIPropertyMetadata(0d));

		public double TooltipOffsetY
		{
			get => (double)GetValue(TooltipOffsetYProperty);
			set => SetValue(TooltipOffsetYProperty, value);
		}
		public static readonly DependencyProperty TooltipOffsetYProperty = DependencyProperty.Register(
			"TooltipOffsetY",
			typeof(double),
			typeof(TooltipControl),
			new UIPropertyMetadata(0d));

		public double TooltipOpacity
		{
			get => (double)GetValue(TooltipOpacityProperty);
			set => SetValue(TooltipOpacityProperty, value);
		}
		public static readonly DependencyProperty TooltipOpacityProperty = DependencyProperty.Register(
			"TooltipOpacity",
			typeof(double),
			typeof(TooltipControl),
			new PropertyMetadata(1d));

		public bool InvertY
		{
			get => (bool)GetValue(InvertYProperty);
			set => SetValue(InvertYProperty, value);
		}
		public static readonly DependencyProperty InvertYProperty = DependencyProperty.Register(
			"InvertY",
			typeof(bool),
			typeof(TooltipControl),
			new UIPropertyMetadata(false));

		private ObservableCollection<TooltipViewModel> TooltipPoints
		{
			get => (ObservableCollection<TooltipViewModel>)GetValue(TooltipPointsProperty);
			set => SetValue(TooltipPointsProperty, value);
		}
		public static readonly DependencyProperty TooltipPointsProperty = DependencyProperty.Register(
			"TooltipPoints",
			typeof(ObservableCollection<TooltipViewModel>),
			typeof(TooltipControl),
			new PropertyMetadata(new ObservableCollection<TooltipViewModel>()));

		public TooltipLocation TooltipLocation
		{
			get => (TooltipLocation)GetValue(TooltipLocationProperty);
			set => SetValue(TooltipLocationProperty, value);
		}
		public static readonly DependencyProperty TooltipLocationProperty = DependencyProperty.Register(
			"TooltipLocation",
			typeof(TooltipLocation),
			typeof(TooltipControl),
			new UIPropertyMetadata(TooltipLocation.Cursor, OnTooltipLocationSet));

		public Func<Point, IEnumerable<TooltipViewModel>> TooltipGetterFunc
		{
			get => (Func<Point, IEnumerable<TooltipViewModel>>)GetValue(TooltipGetterFuncProperty);
			set => SetValue(TooltipGetterFuncProperty, value);
		}
		public static readonly DependencyProperty TooltipGetterFuncProperty = DependencyProperty.Register(
			"TooltipGetterFunc",
			typeof(Func<Point, IEnumerable<TooltipViewModel>>),
			typeof(TooltipControl),
			new PropertyMetadata(null));

		private bool IsTooltipByCursor
		{
			get => (bool)GetValue(IsTooltipByCursorProperty);
			set => SetValue(IsTooltipByCursorProperty, value);
		}
		public static readonly DependencyProperty IsTooltipByCursorProperty = DependencyProperty.Register(
			"IsTooltipByCursor",
			typeof(bool),
			typeof(TooltipControl),
			new PropertyMetadata(true));

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

		public MouseCoordinator Coordinator
		{
			get => (MouseCoordinator)GetValue(MouseCoordinatorProperty);
			set => SetValue(MouseCoordinatorProperty, value);
		}
		public static readonly DependencyProperty MouseCoordinatorProperty = DependencyProperty.Register(
			"MouseCoordinator",
			typeof(MouseCoordinator),
			typeof(TooltipControl),
			new PropertyMetadata(null, OnSetMouseCoordinator));

		public TooltipControl()
		{
			InitializeComponent();

			this.Loaded += TooltipControl_Loaded;

			NameScope.SetNameScope(ContextMenu, NameScope.GetNameScope(this));
		}

		private void TooltipControl_Loaded(object sender, RoutedEventArgs e)
		{
			this.Loaded -= TooltipControl_Loaded;
			if (ChartHelper.FindMouseCoordinatorFromVisualTree(this, out var coordinator))
			{
				Coordinator = coordinator;
			}
			else
			{
				throw new InvalidOperationException("Please add a MouseCoordinator to your chart");
			}
		}

		private static async void OnSetMouseCoordinator(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not TooltipControl _this) return;

			_this.Coordinator.MouseMove += _this.Coordinator_MouseMove;
			_this.Coordinator.MouseLeave += _this.Coordinator_MouseLeave;
		}

		private void Coordinator_MouseLeave(object sender, MouseEventArgs e)
		{
			IsMouseOverThis = false;
		}

		private static void OnTooltipLocationSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not TooltipControl control) return;

			control.IsTooltipByCursor = control.TooltipLocation == TooltipLocation.Cursor;
		}

		private static void OnSetForceTooltip(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not TooltipControl control) return;

			if (control.ForceTooltip.HasValue && !control.ForceTooltip.Value)
			{
				control.ShowTooltip = false;
			}
		}

		private static void OnSetForcePointIndicators(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not TooltipControl control) return;

			if (control.ForcePointIndicators.HasValue && !control.ForcePointIndicators.Value)
			{
				control.ShowPointIndicators = false;
			}
		}

		private static void OnSetForceCrosshairs(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not TooltipControl control) return;

			if (control.ForceCrosshairs.HasValue && !control.ForceCrosshairs.Value)
			{
				control.ShowCrosshairs = false;
			}
		}

		private void Coordinator_MouseMove(object? sender, (bool isUserDragging, bool isUserPanning, Point? lowerSelection, MouseEventArgs args) e)
		{
			IsMouseOverThis = true;
			isUserPanning = e.isUserPanning;
			var mouseLoc = e.args.GetPosition(Grid);

			#region Crosshairs
			if (ShowCrosshairs)
			{
				// Move crosshairs
				XCrosshair.Margin = new Thickness(0, mouseLoc.Y, 0, 0);
				YCrosshair.Margin = new Thickness(mouseLoc.X, 0, 0, 0);
			}
			#endregion

			#region Selected range
			if (AllowSelection && e.isUserDragging)
			{
				IsUserSelectingRange = true;
				var negative = mouseLoc.X < e.lowerSelection.Value.X;
				var margin = SelectionRangeBorder.Margin;
				margin.Left = negative ? mouseLoc.X : e.lowerSelection.Value.X;
				SelectionRangeBorder.Margin = margin;
				SelectionRangeBorder.Width = negative
					? Math.Max(e.lowerSelection.Value.X - mouseLoc.X, 0)
					: Math.Max(mouseLoc.X - e.lowerSelection.Value.X, 0);
			}
			else
			{
				IsUserSelectingRange = false;
			}
			#endregion

			#region Tooltip
			if (ShowTooltip && TooltipGetterFunc != null)
			{
				TooltipPoints = new ObservableCollection<TooltipViewModel>(TooltipGetterFunc(mouseLoc));

				if (TooltipPoints.Any())
				{
					TooltipPoints.First(x => x.LocationY - mouseLoc.Y == TooltipPoints.Min(y => y.LocationY - mouseLoc.Y)).IsNearest = true;

					if (TooltipLocation == TooltipLocation.Cursor)
					{
						// Get tooltip position variables
						if (!tooltipLeft && (ActualWidth - mouseLoc.X) < (TooltipsByCursor.ActualWidth + 10))
							tooltipLeft = true;
						if (tooltipLeft && (mouseLoc.X) < (TooltipsByCursor.ActualWidth + 5))
							tooltipLeft = false;
						if (!tooltipTop && (ActualHeight - mouseLoc.Y) < (TooltipsByCursor.ActualHeight + 10))
							tooltipTop = true;
						if (tooltipTop && (mouseLoc.Y) < (TooltipsByCursor.ActualHeight + 5))
							tooltipTop = false;

						TooltipsByCursor.Margin = new Thickness(
							!tooltipLeft ? mouseLoc.X + 5 : mouseLoc.X - TooltipsByCursor.ActualWidth - 5,
							!tooltipTop ? mouseLoc.Y + 5 : mouseLoc.Y - TooltipsByCursor.ActualHeight - 5,
							0, 0);
					}
				}
			}
			else
			{
				TooltipPoints.Clear();
			}
			#endregion
		}

		private void Grid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			if (isUserPanning)
			{
				e.Handled = true;
			}
		}
	}
}
