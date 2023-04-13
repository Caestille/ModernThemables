using CoreUtilities.HelperClasses;
using ModernThemables.Charting.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using CoreUtilities.Converters;
using System.Collections.ObjectModel;

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

		private double LabelHeight
		{
			get => (double)GetValue(LabelHeightProperty);
			set => SetValue(LabelHeightProperty, value);
		}
		public static readonly DependencyProperty LabelHeightProperty = DependencyProperty.Register(
			"LabelHeight",
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

		private string MarginString
		{
			get => (string)GetValue(MarginStringProperty);
			set => SetValue(MarginStringProperty, value);
		}
		public static readonly DependencyProperty MarginStringProperty = DependencyProperty.Register(
			"MarginString",
			typeof(string),
			typeof(AxisControl),
			new UIPropertyMetadata("1-0-0-0"));

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

		public AxisControl()
		{
			InitializeComponent();
		}

		private static async void OnSetAxisOrientation(DependencyObject sender, DependencyPropertyChangedEventArgs e)
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
		}

		private static async void OnSetLabelRotation(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not AxisControl _this || _this.Labels == null) return;

			var value = _this.Labels.Any()
				? _this.Labels.Max(x => (double)(new StringWidthGetterConverter().Convert(
					new object[]
					{
						x.Value,
						_this.FontSize,
						_this.FontFamily,
						_this.FontStyle,
						_this.FontWeight,
						_this.FontStretch
					},
					null,
					null,
					null))) * Math.Sin(_this.LabelRotation * Math.PI / 180) + 10
				: 0;

			switch (_this.Orientation)
			{
				case Orientation.Horizontal:
					_this.MainItemsControl.Height = value;
					break;
				case Orientation.Vertical:
					_this.MainItemsControl.Width = value;
					break;
			}
		}
	}
}
