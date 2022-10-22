using ModernThemables.HelperClasses.WpfChart;
using ModernThemables.Interfaces;
using ModernThemables.ViewModels.WpfChart;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace ModernThemables.Controls
{
	public partial class WpfChart // .DependencyProperties
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
			typeof(WpfChart),
			new FrameworkPropertyMetadata(null, OnSeriesSet));

		public Func<object, string> XAxisFormatter
		{
			get => (Func<object, string>)GetValue(XAxisFormatterProperty);
			set => SetValue(XAxisFormatterProperty, value);
		}
		public static readonly DependencyProperty XAxisFormatterProperty = DependencyProperty.Register(
			"XAxisFormatter",
			typeof(Func<object, string>),
			typeof(WpfChart),
			new PropertyMetadata(null));

		public Func<object, string> XAxisCursorLabelFormatter
		{
			get => (Func<object, string>)GetValue(XAxisCursorLabelFormatterProperty);
			set => SetValue(XAxisCursorLabelFormatterProperty, value);
		}
		public static readonly DependencyProperty XAxisCursorLabelFormatterProperty = DependencyProperty.Register(
			"XAxisCursorLabelFormatter",
			typeof(Func<object, string>),
			typeof(WpfChart),
			new PropertyMetadata(null));

		public Func<object, string> YAxisFormatter
		{
			get => (Func<object, string>)GetValue(YAxisFormatterProperty);
			set => SetValue(YAxisFormatterProperty, value);
		}
		public static readonly DependencyProperty YAxisFormatterProperty = DependencyProperty.Register(
			"YAxisFormatter",
			typeof(Func<object, string>),
			typeof(WpfChart),
			new PropertyMetadata(null));

		public Func<object, string> YAxisCursorLabelFormatter
		{
			get => (Func<object, string>)GetValue(YAxisCursorLabelFormatterProperty);
			set => SetValue(YAxisCursorLabelFormatterProperty, value);
		}
		public static readonly DependencyProperty YAxisCursorLabelFormatterProperty = DependencyProperty.Register(
			"YAxisCursorLabelFormatter",
			typeof(Func<object, string>),
			typeof(WpfChart),
			new PropertyMetadata(null));

		public Func<object, bool> YAxisLabelIdentifier
		{
			get => (Func<object, bool>)GetValue(YAxisLabelIdentifierProperty);
			set => SetValue(YAxisLabelIdentifierProperty, value);
		}
		public static readonly DependencyProperty YAxisLabelIdentifierProperty = DependencyProperty.Register(
			"YAxisLabelIdentifier",
			typeof(Func<object, bool>),
			typeof(WpfChart),
			new PropertyMetadata(null));

		public Func<object, bool> XAxisLabelIdentifier
		{
			get => (Func<object, bool>)GetValue(XAxisLabelIdentifierProperty);
			set => SetValue(XAxisLabelIdentifierProperty, value);
		}
		public static readonly DependencyProperty XAxisLabelIdentifierProperty = DependencyProperty.Register(
			"XAxisLabelIdentifier",
			typeof(Func<object, bool>),
			typeof(WpfChart),
			new PropertyMetadata(null));

		public bool ShowXSeparatorLines
		{
			get => (bool)GetValue(ShowXSeparatorLinesProperty);
			set => SetValue(ShowXSeparatorLinesProperty, value);
		}
		public static readonly DependencyProperty ShowXSeparatorLinesProperty = DependencyProperty.Register(
			"ShowXSeparatorLines",
			typeof(bool),
			typeof(WpfChart),
			new PropertyMetadata(true));

		public bool ShowYSeparatorLines
		{
			get => (bool)GetValue(ShowYSeparatorLinesProperty);
			set => SetValue(ShowYSeparatorLinesProperty, value);
		}
		public static readonly DependencyProperty ShowYSeparatorLinesProperty = DependencyProperty.Register(
			"ShowYSeparatorLines",
			typeof(bool),
			typeof(WpfChart),
			new PropertyMetadata(true));

		public bool IsZoomed
		{
			get => (bool)GetValue(IsZoomedProperty);
			set => SetValue(IsZoomedProperty, value);
		}
		public static readonly DependencyProperty IsZoomedProperty = DependencyProperty.Register(
			"IsZoomed",
			typeof(bool),
			typeof(WpfChart),
			new PropertyMetadata(false));

		public DataTemplate TooltipTemplate
		{
			get => (DataTemplate)GetValue(TooltipTemplateProperty);
			set => SetValue(TooltipTemplateProperty, value);
		}
		public static readonly DependencyProperty TooltipTemplateProperty = DependencyProperty.Register(
			"TooltipTemplate",
			typeof(DataTemplate),
			typeof(WpfChart),
			new PropertyMetadata(null));

		public DataTemplate LegendTemplate
		{
			get => (DataTemplate)GetValue(LegendTemplateProperty);
			set => SetValue(LegendTemplateProperty, value);
		}
		public static readonly DependencyProperty LegendTemplateProperty = DependencyProperty.Register(
			"LegendTemplate",
			typeof(DataTemplate),
			typeof(WpfChart),
			new PropertyMetadata(null));

		public LegendLocation LegendLocation
		{
			get => (LegendLocation)GetValue(LegendLocationProperty);
			set => SetValue(LegendLocationProperty, value);
		}
		public static readonly DependencyProperty LegendLocationProperty = DependencyProperty.Register(
			"LegendLocation",
			typeof(LegendLocation),
			typeof(WpfChart),
			new PropertyMetadata(LegendLocation.None));

		public TooltipFindingStrategy TooltipFindingStrategy
		{
			get => (TooltipFindingStrategy)GetValue(TooltipFindingStrategyProperty);
			set => SetValue(TooltipFindingStrategyProperty, value);
		}
		public static readonly DependencyProperty TooltipFindingStrategyProperty = DependencyProperty.Register(
			"TooltipFindingStrategy",
			typeof(TooltipFindingStrategy),
			typeof(WpfChart),
			new PropertyMetadata(TooltipFindingStrategy.NearestXAllY));

		public TooltipLocation TooltipLocation
		{
			get => (TooltipLocation)GetValue(TooltipLocationProperty);
			set => SetValue(TooltipLocationProperty, value);
		}
		public static readonly DependencyProperty TooltipLocationProperty = DependencyProperty.Register(
			"TooltipLocation",
			typeof(TooltipLocation),
			typeof(WpfChart),
			new FrameworkPropertyMetadata(TooltipLocation.Cursor, OnTooltipLocationSet));

		public double TooltipLocationThreshold
		{
			get => (double)GetValue(TooltipLocationThresholdProperty);
			set => SetValue(TooltipLocationThresholdProperty, value);
		}
		public static readonly DependencyProperty TooltipLocationThresholdProperty = DependencyProperty.Register(
			"TooltipLocationThreshold",
			typeof(double),
			typeof(WpfChart),
			new PropertyMetadata(5d));

		public double TooltipOpacity
		{
			get => (double)GetValue(TooltipOpacityProperty);
			set => SetValue(TooltipOpacityProperty, value);
		}
		public static readonly DependencyProperty TooltipOpacityProperty = DependencyProperty.Register(
			"TooltipOpacity",
			typeof(double),
			typeof(WpfChart),
			new PropertyMetadata(1d));

		#endregion

		#region Private properties

		private ObservableCollection<HoveredPointViewModel> TooltipPoints
		{
			get => (ObservableCollection<HoveredPointViewModel>)GetValue(TooltipPointsProperty);
			set => SetValue(TooltipPointsProperty, value);
		}
		public static readonly DependencyProperty TooltipPointsProperty = DependencyProperty.Register(
			"TooltipPoints",
			typeof(ObservableCollection<HoveredPointViewModel>),
			typeof(WpfChart),
			new PropertyMetadata(new ObservableCollection<HoveredPointViewModel>()));

		private bool IsTooltipByCursor
		{
			get => (bool)GetValue(IsTooltipByCursorProperty);
			set => SetValue(IsTooltipByCursorProperty, value);
		}
		public static readonly DependencyProperty IsTooltipByCursorProperty = DependencyProperty.Register(
			"IsTooltipByCursor",
			typeof(bool),
			typeof(WpfChart),
			new PropertyMetadata(true));

		private InternalChartPoint? HoveredPoint
		{
			get => (InternalChartPoint)GetValue(HoveredPointProperty);
			set => SetValue(HoveredPointProperty, value);
		}
		public static readonly DependencyProperty HoveredPointProperty = DependencyProperty.Register(
			"HoveredPoint",
			typeof(InternalChartPoint),
			typeof(WpfChart),
			new PropertyMetadata(null));

		private ObservableCollection<ConvertedSeriesViewModel> ConvertedSeries
		{
			get => (ObservableCollection<ConvertedSeriesViewModel>)GetValue(ConvertedSeriesProperty);
			set => SetValue(ConvertedSeriesProperty, value);
		}
		public static readonly DependencyProperty ConvertedSeriesProperty = DependencyProperty.Register(
			"ConvertedSeries",
			typeof(ObservableCollection<ConvertedSeriesViewModel>),
			typeof(WpfChart),
			new PropertyMetadata(new ObservableCollection<ConvertedSeriesViewModel>()));

		private ObservableCollection<ValueWithHeight> XAxisLabels
		{
			get => (ObservableCollection<ValueWithHeight>)GetValue(XAxisLabelsProperty);
			set => SetValue(XAxisLabelsProperty, value);
		}
		public static readonly DependencyProperty XAxisLabelsProperty = DependencyProperty.Register(
			"XAxisLabels",
			typeof(ObservableCollection<ValueWithHeight>),
			typeof(WpfChart), 
			new PropertyMetadata(new ObservableCollection<ValueWithHeight>()));

		private ObservableCollection<ValueWithHeight> YAxisLabels
		{
			get => (ObservableCollection<ValueWithHeight>)GetValue(YAxisLabelsProperty);
			set => SetValue(YAxisLabelsProperty, value);
		}
		public static readonly DependencyProperty YAxisLabelsProperty = DependencyProperty.Register(
			"YAxisLabels",
			typeof(ObservableCollection<ValueWithHeight>),
			typeof(WpfChart),
			new PropertyMetadata(new ObservableCollection<ValueWithHeight>()));

		private bool IsCrosshairVisible
		{
			get => (bool)GetValue(IsCrosshairVisibleProperty);
			set => SetValue(IsCrosshairVisibleProperty, value);
		}
		public static readonly DependencyProperty IsCrosshairVisibleProperty = DependencyProperty.Register(
			"IsCrosshairVisible",
			typeof(bool),
			typeof(WpfChart),
			new PropertyMetadata(true));

		private bool IsTooltipVisible
		{
			get => (bool)GetValue(IsTooltipVisibleProperty);
			set => SetValue(IsTooltipVisibleProperty, value);
		}
		public static readonly DependencyProperty IsTooltipVisibleProperty = DependencyProperty.Register(
			"IsTooltipVisible",
			typeof(bool),
			typeof(WpfChart),
			new PropertyMetadata(true));

		private bool IsAxisIndicatorsVisible
		{
			get => (bool)GetValue(IsAxisIndicatorsVisibleProperty);
			set => SetValue(IsAxisIndicatorsVisibleProperty, value);
		}
		public static readonly DependencyProperty IsAxisIndicatorsVisibleProperty = DependencyProperty.Register(
			"IsAxisIndicatorsVisible",
			typeof(bool),
			typeof(WpfChart),
			new PropertyMetadata(true));

		private bool IsPointIndicatorsVisible
		{
			get => (bool)GetValue(IsPointIndicatorsVisibleProperty);
			set => SetValue(IsPointIndicatorsVisibleProperty, value);
		}
		public static readonly DependencyProperty IsPointIndicatorsVisibleProperty = DependencyProperty.Register(
			"IsPointIndicatorsVisible",
			typeof(bool),
			typeof(WpfChart),
			new PropertyMetadata(true));

		private bool IsUserSelectingRange
		{
			get => (bool)GetValue(IsUserSelectingRangeProperty);
			set => SetValue(IsUserSelectingRangeProperty, value);
		}
		public static readonly DependencyProperty IsUserSelectingRangeProperty = DependencyProperty.Register(
			"IsUserSelectingRange",
			typeof(bool),
			typeof(WpfChart),
			new PropertyMetadata(false));

		private ZoomState CurrentZoomState
		{
			get => (ZoomState)GetValue(CurrentZoomStateProperty);
			set => SetValue(CurrentZoomStateProperty, value);
		}
		public static readonly DependencyProperty CurrentZoomStateProperty = DependencyProperty.Register(
			"CurrentZoomState",
			typeof(ZoomState),
			typeof(WpfChart),
			new PropertyMetadata(new ZoomState(0, 0, 0, 0, 0, yBuffer)));
		
		private bool HasData
		{
			get => (bool)GetValue(HasDataProperty);
			set => SetValue(HasDataProperty, value);
		}
		public static readonly DependencyProperty HasDataProperty = DependencyProperty.Register(
			"HasData",
			typeof(bool),
			typeof(WpfChart),
			new PropertyMetadata(false));

		#endregion
	}
}
