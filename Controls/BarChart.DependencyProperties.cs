using ModernThemables.HelperClasses.Charting;
using ModernThemables.HelperClasses.Charting.CartesianChart;
using ModernThemables.Interfaces;
using ModernThemables.ViewModels.Charting;
using ModernThemables.ViewModels.Charting.CartesianChart;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace ModernThemables.Controls
{
	public partial class BarChart // .DependencyProperties
	{
		#region Public properties

		public ObservableCollection<ISeries> Series
		{
			get => (ObservableCollection<ISeries>)GetValue(SeriesProperty);
			set => SetValue(SeriesProperty, value);
		}
		public static readonly DependencyProperty SeriesProperty = DependencyProperty.Register(
			"Series",
			typeof(ObservableCollection<ISeries>),
			typeof(BarChart),
			new FrameworkPropertyMetadata(null, OnSeriesSet));

		public Func<object, string> YAxisFormatter
		{
			get => (Func<object, string>)GetValue(YAxisFormatterProperty);
			set => SetValue(YAxisFormatterProperty, value);
		}
		public static readonly DependencyProperty YAxisFormatterProperty = DependencyProperty.Register(
			"YAxisFormatter",
			typeof(Func<object, string>),
			typeof(BarChart),
			new PropertyMetadata(null));

		public Func<object, bool> YAxisLabelIdentifier
		{
			get => (Func<object, bool>)GetValue(YAxisLabelIdentifierProperty);
			set => SetValue(YAxisLabelIdentifierProperty, value);
		}
		public static readonly DependencyProperty YAxisLabelIdentifierProperty = DependencyProperty.Register(
			"YAxisLabelIdentifier",
			typeof(Func<object, bool>),
			typeof(BarChart),
			new PropertyMetadata(null));

		public bool ShowXSeparatorLines
		{
			get => (bool)GetValue(ShowXSeparatorLinesProperty);
			set => SetValue(ShowXSeparatorLinesProperty, value);
		}
		public static readonly DependencyProperty ShowXSeparatorLinesProperty = DependencyProperty.Register(
			"ShowXSeparatorLines",
			typeof(bool),
			typeof(BarChart),
			new PropertyMetadata(true));

		public bool ShowYSeparatorLines
		{
			get => (bool)GetValue(ShowYSeparatorLinesProperty);
			set => SetValue(ShowYSeparatorLinesProperty, value);
		}
		public static readonly DependencyProperty ShowYSeparatorLinesProperty = DependencyProperty.Register(
			"ShowYSeparatorLines",
			typeof(bool),
			typeof(BarChart),
			new PropertyMetadata(true));

		public bool IsZoomed
		{
			get => (bool)GetValue(IsZoomedProperty);
			set => SetValue(IsZoomedProperty, value);
		}
		public static readonly DependencyProperty IsZoomedProperty = DependencyProperty.Register(
			"IsZoomed",
			typeof(bool),
			typeof(BarChart),
			new PropertyMetadata(false));

		public DataTemplate TooltipTemplate
		{
			get => (DataTemplate)GetValue(TooltipTemplateProperty);
			set => SetValue(TooltipTemplateProperty, value);
		}
		public static readonly DependencyProperty TooltipTemplateProperty = DependencyProperty.Register(
			"TooltipTemplate",
			typeof(DataTemplate),
			typeof(BarChart),
			new PropertyMetadata(null));

		public DataTemplate LegendTemplate
		{
			get => (DataTemplate)GetValue(LegendTemplateProperty);
			set => SetValue(LegendTemplateProperty, value);
		}
		public static readonly DependencyProperty LegendTemplateProperty = DependencyProperty.Register(
			"LegendTemplate",
			typeof(DataTemplate),
			typeof(BarChart),
			new PropertyMetadata(null));

		public LegendLocation LegendLocation
		{
			get => (LegendLocation)GetValue(LegendLocationProperty);
			set => SetValue(LegendLocationProperty, value);
		}
		public static readonly DependencyProperty LegendLocationProperty = DependencyProperty.Register(
			"LegendLocation",
			typeof(LegendLocation),
			typeof(BarChart),
			new UIPropertyMetadata(LegendLocation.None, OnLegendLocationSet));

		public TooltipLocation TooltipLocation
		{
			get => (TooltipLocation)GetValue(TooltipLocationProperty);
			set => SetValue(TooltipLocationProperty, value);
		}
		public static readonly DependencyProperty TooltipLocationProperty = DependencyProperty.Register(
			"TooltipLocation",
			typeof(TooltipLocation),
			typeof(BarChart),
			new FrameworkPropertyMetadata(TooltipLocation.Cursor, OnTooltipLocationSet));

		public double TooltipOpacity
		{
			get => (double)GetValue(TooltipOpacityProperty);
			set => SetValue(TooltipOpacityProperty, value);
		}
		public static readonly DependencyProperty TooltipOpacityProperty = DependencyProperty.Register(
			"TooltipOpacity",
			typeof(double),
			typeof(BarChart),
			new PropertyMetadata(1d));

		public double BarCornerRadiusFraction
		{
			get => (double)GetValue(BarCornerRadiusFractionProperty);
			set => SetValue(BarCornerRadiusFractionProperty, value);
		}
		public static readonly DependencyProperty BarCornerRadiusFractionProperty = DependencyProperty.Register(
			"BarCornerRadiusFraction",
			typeof(double),
			typeof(BarChart),
			new UIPropertyMetadata(0d, TriggerReRender));

		public double BarGroupSeparationPixels
		{
			get => (double)GetValue(BarGroupSeparationPixelsProperty);
			set => SetValue(BarGroupSeparationPixelsProperty, value);
		}
		public static readonly DependencyProperty BarGroupSeparationPixelsProperty = DependencyProperty.Register(
			"BarGroupSeparationPixels",
			typeof(double),
			typeof(BarChart),
			new UIPropertyMetadata(0d, TriggerReRender));

		public double BarSeparationPixels
		{
			get => (double)GetValue(BarSeparationPixelsProperty);
			set => SetValue(BarSeparationPixelsProperty, value);
		}
		public static readonly DependencyProperty BarSeparationPixelsProperty = DependencyProperty.Register(
			"BarSeparationPixels",
			typeof(double),
			typeof(BarChart),
			new UIPropertyMetadata(0d, TriggerReRender));

		public double XAxisLabelRotation
		{
			get => (double)GetValue(XAxisLabelRotationProperty);
			set => SetValue(XAxisLabelRotationProperty, value);
		}
		public static readonly DependencyProperty XAxisLabelRotationProperty = DependencyProperty.Register(
			"XAxisLabelRotation",
			typeof(double),
			typeof(BarChart),
			new UIPropertyMetadata(0d, TriggerReRender));

		#endregion

		#region Private properties

		private bool IsTooltipByCursor
		{
			get => (bool)GetValue(IsTooltipByCursorProperty);
			set => SetValue(IsTooltipByCursorProperty, value);
		}
		public static readonly DependencyProperty IsTooltipByCursorProperty = DependencyProperty.Register(
			"IsTooltipByCursor",
			typeof(bool),
			typeof(BarChart),
			new PropertyMetadata(true));

		private ObservableCollection<InternalChartEntity> InternalSeries
		{
			get => (ObservableCollection<InternalChartEntity>)GetValue(InternalSeriesProperty);
			set => SetValue(InternalSeriesProperty, value);
		}
		public static readonly DependencyProperty InternalSeriesProperty = DependencyProperty.Register(
			"InternalSeries",
			typeof(ObservableCollection<InternalChartEntity>),
			typeof(BarChart),
			new PropertyMetadata(new ObservableCollection<InternalChartEntity>()));

		private ObservableCollection<AxisLabel> XAxisLabels
		{
			get => (ObservableCollection<AxisLabel>)GetValue(XAxisLabelsProperty);
			set => SetValue(XAxisLabelsProperty, value);
		}
		public static readonly DependencyProperty XAxisLabelsProperty = DependencyProperty.Register(
			"XAxisLabels",
			typeof(ObservableCollection<AxisLabel>),
			typeof(BarChart), 
			new PropertyMetadata(new ObservableCollection<AxisLabel>()));

		private ObservableCollection<AxisLabel> YAxisLabels
		{
			get => (ObservableCollection<AxisLabel>)GetValue(YAxisLabelsProperty);
			set => SetValue(YAxisLabelsProperty, value);
		}
		public static readonly DependencyProperty YAxisLabelsProperty = DependencyProperty.Register(
			"YAxisLabels",
			typeof(ObservableCollection<AxisLabel>),
			typeof(BarChart),
			new PropertyMetadata(new ObservableCollection<AxisLabel>()));

		private bool IsTooltipVisible
		{
			get => (bool)GetValue(IsTooltipVisibleProperty);
			set => SetValue(IsTooltipVisibleProperty, value);
		}
		public static readonly DependencyProperty IsTooltipVisibleProperty = DependencyProperty.Register(
			"IsTooltipVisible",
			typeof(bool),
			typeof(BarChart),
			new PropertyMetadata(true));
		
		private bool HasData
		{
			get => (bool)GetValue(HasDataProperty);
			set => SetValue(HasDataProperty, value);
		}
		public static readonly DependencyProperty HasDataProperty = DependencyProperty.Register(
			"HasData",
			typeof(bool),
			typeof(BarChart),
			new PropertyMetadata(false));

		private double BarWidth
		{
			get => (double)GetValue(BarWidthProperty);
			set => SetValue(BarWidthProperty, value);
		}
		public static readonly DependencyProperty BarWidthProperty = DependencyProperty.Register(
			"BarWidth",
			typeof(double),
			typeof(BarChart),
			new PropertyMetadata(0d));

		private CornerRadius BarCornerRadius
		{
			get => (CornerRadius)GetValue(BarCornerRadiusProperty);
			set => SetValue(BarCornerRadiusProperty, value);
		}
		public static readonly DependencyProperty BarCornerRadiusProperty = DependencyProperty.Register(
			"BarCornerRadius",
			typeof(CornerRadius),
			typeof(BarChart),
			new PropertyMetadata(new CornerRadius(0)));

		private double XAxisLabelHeight
		{
			get => (double)GetValue(XAxisLabelHeightProperty);
			set => SetValue(XAxisLabelHeightProperty, value);
		}
		public static readonly DependencyProperty XAxisLabelHeightProperty = DependencyProperty.Register(
			"XAxisLabelHeight",
			typeof(double),
			typeof(BarChart),
			new PropertyMetadata(0d));

		#endregion
	}
}
