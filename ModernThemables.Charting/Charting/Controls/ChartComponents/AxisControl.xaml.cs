using ModernThemables.Charting.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Collections.ObjectModel;
using ModernThemables.Charting.Services;
using System.Windows.Input;
using CoreUtilities.Converters;
using System.Diagnostics;

namespace ModernThemables.Charting.Controls.ChartComponents
{
	/// <summary>
	/// Interaction logic for AxisControl.xaml
	/// </summary>
	public partial class AxisControl : UserControl
	{
		public Orientation Orientation
		{
			get => (Orientation)GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}
		public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
			"Orientation",
			typeof(Orientation),
			typeof(AxisControl),
			new UIPropertyMetadata(Orientation.Vertical, OnSetAxisOrientation));

		public double LabelRotation
		{
			get => (double)GetValue(LabelRotationProperty);
			set => SetValue(LabelRotationProperty, value);
		}
		public static readonly DependencyProperty LabelRotationProperty = DependencyProperty.Register(
			"LabelRotation",
			typeof(double),
			typeof(AxisControl),
			new UIPropertyMetadata(0d, OnSetLabelRotation));

		public bool ShowDividers
		{
			get => (bool)GetValue(ShowDividersProperty);
			set => SetValue(ShowDividersProperty, value);
		}
		public static readonly DependencyProperty ShowDividersProperty = DependencyProperty.Register(
			"ShowDividers",
			typeof(bool),
			typeof(AxisControl),
			new UIPropertyMetadata(true));

		public bool ShowIndicators
		{
			get => (bool)GetValue(ShowIndicatorsProperty);
			set => SetValue(ShowIndicatorsProperty, value);
		}
		public static readonly DependencyProperty ShowIndicatorsProperty = DependencyProperty.Register(
			"ShowIndicators",
			typeof(bool),
			typeof(AxisControl),
			new UIPropertyMetadata(true));
		public static readonly DependencyProperty LabelHeightProperty = DependencyProperty.Register(
			"LabelHeight",
			typeof(double),
			typeof(AxisControl),
			new UIPropertyMetadata(0d));

		private double DividerWidth
		{
			get => (double)GetValue(DividerWidthtProperty);
			set => SetValue(DividerWidthtProperty, value);
		}
		public static readonly DependencyProperty DividerWidthtProperty = DependencyProperty.Register(
			"DividerWidth",
			typeof(double),
			typeof(AxisControl),
			new UIPropertyMetadata(0d));

		private double DividerHeight
		{
			get => (double)GetValue(DividerHeightProperty);
			set => SetValue(DividerHeightProperty, value);
		}
		public static readonly DependencyProperty DividerHeightProperty = DependencyProperty.Register(
			"DividerHeight",
			typeof(double),
			typeof(AxisControl),
			new UIPropertyMetadata(0d));

		public double DividerOffset
		{
			get => (double)GetValue(DividerOffsetProperty);
			set => SetValue(DividerOffsetProperty, value);
		}
		public static readonly DependencyProperty DividerOffsetProperty = DependencyProperty.Register(
			"DividerOffset",
			typeof(double),
			typeof(AxisControl),
			new UIPropertyMetadata(0d));
		public static readonly DependencyProperty LabelAlignmentProperty = DependencyProperty.Register(
			"LabelAlignment",
			typeof(HorizontalAlignment),
			typeof(AxisControl),
			new UIPropertyMetadata(HorizontalAlignment.Center));

		private Thickness DividerBorderThickness
		{
			get => (Thickness)GetValue(DividerBorderThicknessProperty);
			set => SetValue(DividerBorderThicknessProperty, value);
		}
		public static readonly DependencyProperty DividerBorderThicknessProperty = DependencyProperty.Register(
			"DividerBorderThickness",
			typeof(Thickness),
			typeof(AxisControl),
			new UIPropertyMetadata(new Thickness(0)));

		private string MarginString
		{
			get => (string)GetValue(MarginStringProperty);
			set => SetValue(MarginStringProperty, value);
		}
		public static readonly DependencyProperty MarginStringProperty = DependencyProperty.Register(
			"MarginString",
			typeof(string),
			typeof(AxisControl),
			new UIPropertyMetadata("0-0-0-1"));

		private HorizontalAlignment DividerAlignment
		{
			get => (HorizontalAlignment)GetValue(DividerAlignmentProperty);
			set => SetValue(DividerAlignmentProperty, value);
		}
		public static readonly DependencyProperty DividerAlignmentProperty = DependencyProperty.Register(
			"DividerAlignment",
			typeof(HorizontalAlignment),
			typeof(AxisControl),
			new UIPropertyMetadata(HorizontalAlignment.Right));

		private VerticalAlignment Alignment
		{
			get => (VerticalAlignment)GetValue(AlignmentProperty);
			set => SetValue(AlignmentProperty, value);
		}
		public static readonly DependencyProperty AlignmentProperty = DependencyProperty.Register(
			"Alignment",
			typeof(VerticalAlignment),
			typeof(AxisControl),
			new UIPropertyMetadata(VerticalAlignment.Bottom));

		public ObservableCollection<AxisLabel> Labels
		{
			get => (ObservableCollection<AxisLabel>)GetValue(LabelsProperty);
			set => SetValue(LabelsProperty, value);
		}
		public static readonly DependencyProperty LabelsProperty = DependencyProperty.Register(
			"Labels",
			typeof(ObservableCollection<AxisLabel>),
			typeof(AxisControl),
			new UIPropertyMetadata(null, OnSetLabelRotation));

		public MouseCoordinator Coordinator
		{
			get => (MouseCoordinator)GetValue(MouseCoordinatorProperty);
			set => SetValue(MouseCoordinatorProperty, value);
		}
		public static readonly DependencyProperty MouseCoordinatorProperty = DependencyProperty.Register(
			"MouseCoordinator",
			typeof(MouseCoordinator),
			typeof(AxisControl),
			new PropertyMetadata(null, OnSetMouseCoordinator));

		public AxisControl()
		{
			InitializeComponent();
			MainItemsControl.SizeChanged += MainItemsControl_SizeChanged;
			Loaded += AxisControl_Loaded;
		}

		private void AxisControl_Loaded(object sender, RoutedEventArgs e)
		{
			if (ChartHelper.FindMouseCoordinatorFromVisualTree(this, out var coordinator))
			{
				Coordinator = coordinator;
			}
			Loaded -= AxisControl_Loaded;
		}

		private void MainItemsControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			OnSetLabelRotation(this, new DependencyPropertyChangedEventArgs());
		}

		private static void OnSetAxisOrientation(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not AxisControl _this) return;

			switch (_this.Orientation)
			{
				case Orientation.Vertical:
					_this.MarginString = "0-0-0-1";
					break;
				case Orientation.Horizontal:
					_this.MarginString = "1-0-0-0";
					break;
			}

			OnSetLabelRotation(sender, e);
		}

		private static void OnSetMouseCoordinator(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not AxisControl _this) return;

			_this.Coordinator.MouseMove += _this.Coordinator_MouseMove;
			_this.Coordinator.MouseLeave += _this.Coordinator_MouseLeave;
			_this.Coordinator.MouseEnter += _this.Coordinator_MouseEnter;
		}

		private static void OnSetLabelRotation(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not AxisControl _this || _this.Labels == null || !_this.Labels.Any()) return;

			var sizes = _this.Labels.Select(
				x => StringWidthGetterConverter.MeasureString(
					x.FormattedValue,
					_this.FontSize,
					_this.FontFamily,
					_this.FontStyle,
					_this.FontWeight,
					_this.FontStretch));

			var width = sizes.Max(x => x.Width);
			var height = sizes.Max(x => x.Height);

			var mult = _this.Orientation == Orientation.Vertical
				? Math.Cos(_this.LabelRotation * Math.PI / 180)
				: Math.Sin(_this.LabelRotation * Math.PI / 180);
			var value2 = width * mult + (_this.Orientation == Orientation.Horizontal ? 20 : 10);

			switch (_this.Orientation)
			{
				case Orientation.Horizontal:
					_this.MainItemsControl.Height = _this.DividerItemsControl.Height = value2;
					_this.MainItemsControl.Width = _this.DividerItemsControl.Width = Double.NaN;
					_this.MainItemsControl.Margin = _this.DividerItemsControl.Margin = new Thickness(-_this.BorderThickness.Left, 0, 0, 0);
					_this.Alignment = VerticalAlignment.Top;
					_this.DividerWidth = 2;
					_this.DividerHeight = 6;
					_this.DividerBorderThickness = new Thickness(1, 0, 0, 0);
					_this.DividerAlignment = HorizontalAlignment.Left;
					break;
				case Orientation.Vertical:
					_this.MainItemsControl.Width = _this.DividerItemsControl.Width = value2;
					_this.MainItemsControl.Height = _this.DividerItemsControl.Height = Double.NaN;
					_this.MainItemsControl.Margin = new Thickness(-_this.BorderThickness.Left, 0, 0, -height / 2);
					_this.DividerItemsControl.Margin = new Thickness(-_this.BorderThickness.Left, 0, 0, -1);
					_this.Alignment = VerticalAlignment.Bottom;
					_this.DividerWidth = 6;
					_this.DividerHeight = 2;
					_this.DividerBorderThickness = new Thickness(0, 0, 0, 1);
					_this.DividerAlignment = HorizontalAlignment.Right;
					break;
			}
		}

		private void Coordinator_MouseMove(object? sender, (bool isUserDragging, bool isUserPanning, Point? lowerSelection, Point lastMousePoint, MouseEventArgs args) e)
		{
			if (!ShowIndicators) return;

			var mouseLoc = e.args.GetPosition(Coordinator);
			var axisLength = Orientation == Orientation.Horizontal ? Grid.ActualWidth : Grid.ActualHeight;
			var axisFrac = Orientation == Orientation.Horizontal
				? mouseLoc.X / axisLength
				: 1 - (mouseLoc.Y / axisLength);
			AxisLabel? labelMin = Labels.FirstOrDefault(x => x.Location == Labels.Min(y => y.Location));
			var minFrac = (labelMin?.Location ?? 0) / axisLength;
			AxisLabel? labelMax = Labels.FirstOrDefault(x => x.Location == Labels.Max(y => y.Location));
			var maxFrac = (labelMax?.Location ?? 0) / axisLength;
			var fullRange = (labelMax?.Value - labelMin?.Value) / (maxFrac - minFrac);

			if (fullRange != null && double.IsNaN(fullRange.Value)) return;

			var min = labelMin?.Value - minFrac * fullRange;
			var max = labelMax?.Value + (1 - maxFrac) * fullRange;

			var value = min + axisFrac * (max - min);

			ValueLabel.Text = (Labels.First().IndicatorFormatter ?? Labels.First().ValueFormatter)(value ?? 0);
			ValueDisplay.Margin = Orientation == Orientation.Horizontal
				? new Thickness(axisFrac * Grid.ActualWidth - (ValueDisplay.ActualWidth / 2), 4, -100, -100)
				: new Thickness(-5, (1 - axisFrac) * Grid.ActualHeight - 9, -100, 0);
		}

		private void Coordinator_MouseLeave(object sender, MouseEventArgs e)
		{
			if (ValueDisplay.Visibility == Visibility.Visible) ValueDisplay.Visibility = Visibility.Collapsed;
		}

		private void Coordinator_MouseEnter(object sender, MouseEventArgs e)
		{
			if (ShowIndicators && ValueDisplay.Visibility == Visibility.Collapsed) ValueDisplay.Visibility = Visibility.Visible;
		}
	}
}
