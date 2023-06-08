using ModernThemables.Charting.Models;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ModernThemables.Charting.Interfaces;
using System;
using ModernThemables.Charting.ViewModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

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

		private bool tooltipLeft;
		private bool tooltipTop;

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

		public TooltipControl()
		{
			InitializeComponent();

			NameScope.SetNameScope(ContextMenu, NameScope.GetNameScope(this));
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
			if (ShowCrosshairs)
			{
				// Move crosshairs
				XCrosshair.Margin = new Thickness(0, mouseLoc.Y, 0, 0);
				YCrosshair.Margin = new Thickness(mouseLoc.X, 0, 0, 0);
			}
			#endregion

			#region Selected range
			if (AllowSelection && IsUserSelectingRange && lowerSelection != null)
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

			#region Tooltip
			if (ShowTooltip)
			{
				TooltipPoints = new ObservableCollection<TooltipViewModel>(TooltipGetterFunc(mouseLoc));
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
			isUserPanning = false;
			userCouldBePanning = false;
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
				return;
			}

			e.Handled = true;
		}
	}
}
