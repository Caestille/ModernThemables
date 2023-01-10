using ModernThemables.HelperClasses.Charting;
using ModernThemables.HelperClasses.Charting.PieChart;
using ModernThemables.Interfaces;
using ModernThemables.ViewModels.Charting.CartesianChart;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace ModernThemables.Controls
{
    public partial class CartesianChart // .DependencyProperties
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
			typeof(CartesianChart),
			new FrameworkPropertyMetadata(null, OnSeriesSet));

		public Func<object, string> XAxisFormatter
		{
			get => (Func<object, string>)GetValue(XAxisFormatterProperty);
			set => SetValue(XAxisFormatterProperty, value);
		}
		public static readonly DependencyProperty XAxisFormatterProperty = DependencyProperty.Register(
			"XAxisFormatter",
			typeof(Func<object, string>),
			typeof(CartesianChart),
			new PropertyMetadata(null));

		public Func<object, string> XAxisCursorLabelFormatter
		{
			get => (Func<object, string>)GetValue(XAxisCursorLabelFormatterProperty);
			set => SetValue(XAxisCursorLabelFormatterProperty, value);
		}
		public static readonly DependencyProperty XAxisCursorLabelFormatterProperty = DependencyProperty.Register(
			"XAxisCursorLabelFormatter",
			typeof(Func<object, string>),
			typeof(CartesianChart),
			new PropertyMetadata(null));

		public Func<object, string> YAxisFormatter
		{
			get => (Func<object, string>)GetValue(YAxisFormatterProperty);
			set => SetValue(YAxisFormatterProperty, value);
		}
		public static readonly DependencyProperty YAxisFormatterProperty = DependencyProperty.Register(
			"YAxisFormatter",
			typeof(Func<object, string>),
			typeof(CartesianChart),
			new PropertyMetadata(null));

		public Func<object, string> YAxisCursorLabelFormatter
		{
			get => (Func<object, string>)GetValue(YAxisCursorLabelFormatterProperty);
			set => SetValue(YAxisCursorLabelFormatterProperty, value);
		}
		public static readonly DependencyProperty YAxisCursorLabelFormatterProperty = DependencyProperty.Register(
			"YAxisCursorLabelFormatter",
			typeof(Func<object, string>),
			typeof(CartesianChart),
			new PropertyMetadata(null));

		public Func<object, bool> YAxisLabelIdentifier
		{
			get => (Func<object, bool>)GetValue(YAxisLabelIdentifierProperty);
			set => SetValue(YAxisLabelIdentifierProperty, value);
		}
		public static readonly DependencyProperty YAxisLabelIdentifierProperty = DependencyProperty.Register(
			"YAxisLabelIdentifier",
			typeof(Func<object, bool>),
			typeof(CartesianChart),
			new PropertyMetadata(null));

		public Func<object, bool> XAxisLabelIdentifier
		{
			get => (Func<object, bool>)GetValue(XAxisLabelIdentifierProperty);
			set => SetValue(XAxisLabelIdentifierProperty, value);
		}
		public static readonly DependencyProperty XAxisLabelIdentifierProperty = DependencyProperty.Register(
			"XAxisLabelIdentifier",
			typeof(Func<object, bool>),
			typeof(CartesianChart),
			new PropertyMetadata(null));

		public bool ShowXSeparatorLines
		{
			get => (bool)GetValue(ShowXSeparatorLinesProperty);
			set => SetValue(ShowXSeparatorLinesProperty, value);
		}
		public static readonly DependencyProperty ShowXSeparatorLinesProperty = DependencyProperty.Register(
			"ShowXSeparatorLines",
			typeof(bool),
			typeof(CartesianChart),
			new PropertyMetadata(true));

		public bool ShowYSeparatorLines
		{
			get => (bool)GetValue(ShowYSeparatorLinesProperty);
			set => SetValue(ShowYSeparatorLinesProperty, value);
		}
		public static readonly DependencyProperty ShowYSeparatorLinesProperty = DependencyProperty.Register(
			"ShowYSeparatorLines",
			typeof(bool),
			typeof(CartesianChart),
			new PropertyMetadata(true));

		public bool IsZoomed
		{
			get => (bool)GetValue(IsZoomedProperty);
			set => SetValue(IsZoomedProperty, value);
		}
		public static readonly DependencyProperty IsZoomedProperty = DependencyProperty.Register(
			"IsZoomed",
			typeof(bool),
			typeof(CartesianChart),
			new PropertyMetadata(false));

		public DataTemplate TooltipTemplate
		{
			get => (DataTemplate)GetValue(TooltipTemplateProperty);
			set => SetValue(TooltipTemplateProperty, value);
		}
		public static readonly DependencyProperty TooltipTemplateProperty = DependencyProperty.Register(
			"TooltipTemplate",
			typeof(DataTemplate),
			typeof(CartesianChart),
			new PropertyMetadata(null));

		public DataTemplate LegendTemplate
		{
			get => (DataTemplate)GetValue(LegendTemplateProperty);
			set => SetValue(LegendTemplateProperty, value);
		}
		public static readonly DependencyProperty LegendTemplateProperty = DependencyProperty.Register(
			"LegendTemplate",
			typeof(DataTemplate),
			typeof(CartesianChart),
			new PropertyMetadata(null));

		public LegendLocation LegendLocation
		{
			get => (LegendLocation)GetValue(LegendLocationProperty);
			set => SetValue(LegendLocationProperty, value);
		}
		public static readonly DependencyProperty LegendLocationProperty = DependencyProperty.Register(
			"LegendLocation",
			typeof(LegendLocation),
			typeof(CartesianChart),
			new UIPropertyMetadata(LegendLocation.None, OnLegendLocationSet));

		public TooltipFindingStrategy TooltipFindingStrategy
		{
			get => (TooltipFindingStrategy)GetValue(TooltipFindingStrategyProperty);
			set => SetValue(TooltipFindingStrategyProperty, value);
		}
		public static readonly DependencyProperty TooltipFindingStrategyProperty = DependencyProperty.Register(
			"TooltipFindingStrategy",
			typeof(TooltipFindingStrategy),
			typeof(CartesianChart),
			new PropertyMetadata(TooltipFindingStrategy.NearestXAllY));

		public TooltipLocation TooltipLocation
		{
			get => (TooltipLocation)GetValue(TooltipLocationProperty);
			set => SetValue(TooltipLocationProperty, value);
		}
		public static readonly DependencyProperty TooltipLocationProperty = DependencyProperty.Register(
			"TooltipLocation",
			typeof(TooltipLocation),
			typeof(CartesianChart),
			new FrameworkPropertyMetadata(TooltipLocation.Cursor, OnTooltipLocationSet));

		public double TooltipLocationThreshold
		{
			get => (double)GetValue(TooltipLocationThresholdProperty);
			set => SetValue(TooltipLocationThresholdProperty, value);
		}
		public static readonly DependencyProperty TooltipLocationThresholdProperty = DependencyProperty.Register(
			"TooltipLocationThreshold",
			typeof(double),
			typeof(CartesianChart),
			new PropertyMetadata(5d));

		public double TooltipOpacity
		{
			get => (double)GetValue(TooltipOpacityProperty);
			set => SetValue(TooltipOpacityProperty, value);
		}
		public static readonly DependencyProperty TooltipOpacityProperty = DependencyProperty.Register(
			"TooltipOpacity",
			typeof(double),
			typeof(CartesianChart),
			new PropertyMetadata(1d));

		public new double FontSize
		{
			get => (double)GetValue(FontSizeProperty);
			set => SetValue(FontSizeProperty, value);
		}
		public static readonly new DependencyProperty FontSizeProperty = DependencyProperty.Register(
			"FontSize",
			typeof(double),
			typeof(CartesianChart),
			new PropertyMetadata(12d));

		public double Min
		{
			get => (double)GetValue(MinProperty);
			set => SetValue(MinProperty, value);
		}
		public static readonly DependencyProperty MinProperty = DependencyProperty.Register(
			"Min",
			typeof(double),
			typeof(CartesianChart),
			new UIPropertyMetadata(-1d, OnSetMinMax));

		public double Max
		{
			get => (double)GetValue(MaxProperty);
			set => SetValue(MaxProperty, value);
		}
		public static readonly DependencyProperty MaxProperty = DependencyProperty.Register(
			"Max",
			typeof(double),
			typeof(CartesianChart),
			new UIPropertyMetadata(-1d, OnSetMinMax));

		#endregion

		#region Private properties

		private ObservableCollection<TooltipPointViewModel> TooltipPoints
		{
			get => (ObservableCollection<TooltipPointViewModel>)GetValue(TooltipPointsProperty);
			set => SetValue(TooltipPointsProperty, value);
		}
		public static readonly DependencyProperty TooltipPointsProperty = DependencyProperty.Register(
			"TooltipPoints",
			typeof(ObservableCollection<TooltipPointViewModel>),
			typeof(CartesianChart),
			new PropertyMetadata(new ObservableCollection<TooltipPointViewModel>()));

		private bool IsTooltipByCursor
		{
			get => (bool)GetValue(IsTooltipByCursorProperty);
			set => SetValue(IsTooltipByCursorProperty, value);
		}
		public static readonly DependencyProperty IsTooltipByCursorProperty = DependencyProperty.Register(
			"IsTooltipByCursor",
			typeof(bool),
			typeof(CartesianChart),
			new PropertyMetadata(true));

		private InternalChartPoint? MouseOverPoint
		{
			get => (InternalChartPoint)GetValue(MouseOverPointProperty);
			set => SetValue(MouseOverPointProperty, value);
		}
		public static readonly DependencyProperty MouseOverPointProperty = DependencyProperty.Register(
			"MouseOverPoint",
			typeof(InternalChartPoint),
			typeof(CartesianChart),
			new PropertyMetadata(null));

		private ObservableCollection<InternalPathSeriesViewModel> InternalSeries
		{
			get => (ObservableCollection<InternalPathSeriesViewModel>)GetValue(InternalSeriesProperty);
			set => SetValue(InternalSeriesProperty, value);
		}
		public static readonly DependencyProperty InternalSeriesProperty = DependencyProperty.Register(
			"InternalSeries",
			typeof(ObservableCollection<InternalPathSeriesViewModel>),
			typeof(CartesianChart),
			new PropertyMetadata(new ObservableCollection<InternalPathSeriesViewModel>()));

		private ObservableCollection<AxisLabel> XAxisLabels
		{
			get => (ObservableCollection<AxisLabel>)GetValue(XAxisLabelsProperty);
			set => SetValue(XAxisLabelsProperty, value);
		}
		public static readonly DependencyProperty XAxisLabelsProperty = DependencyProperty.Register(
			"XAxisLabels",
			typeof(ObservableCollection<AxisLabel>),
			typeof(CartesianChart), 
			new PropertyMetadata(new ObservableCollection<AxisLabel>()));

		private ObservableCollection<AxisLabel> YAxisLabels
		{
			get => (ObservableCollection<AxisLabel>)GetValue(YAxisLabelsProperty);
			set => SetValue(YAxisLabelsProperty, value);
		}
		public static readonly DependencyProperty YAxisLabelsProperty = DependencyProperty.Register(
			"YAxisLabels",
			typeof(ObservableCollection<AxisLabel>),
			typeof(CartesianChart),
			new PropertyMetadata(new ObservableCollection<AxisLabel>()));

		private bool IsCrosshairVisible
		{
			get => (bool)GetValue(IsCrosshairVisibleProperty);
			set => SetValue(IsCrosshairVisibleProperty, value);
		}
		public static readonly DependencyProperty IsCrosshairVisibleProperty = DependencyProperty.Register(
			"IsCrosshairVisible",
			typeof(bool),
			typeof(CartesianChart),
			new PropertyMetadata(true));

		private bool IsTooltipVisible
		{
			get => (bool)GetValue(IsTooltipVisibleProperty);
			set => SetValue(IsTooltipVisibleProperty, value);
		}
		public static readonly DependencyProperty IsTooltipVisibleProperty = DependencyProperty.Register(
			"IsTooltipVisible",
			typeof(bool),
			typeof(CartesianChart),
			new PropertyMetadata(true));

		private bool IsAxisIndicatorsVisible
		{
			get => (bool)GetValue(IsAxisIndicatorsVisibleProperty);
			set => SetValue(IsAxisIndicatorsVisibleProperty, value);
		}
		public static readonly DependencyProperty IsAxisIndicatorsVisibleProperty = DependencyProperty.Register(
			"IsAxisIndicatorsVisible",
			typeof(bool),
			typeof(CartesianChart),
			new PropertyMetadata(true));

		private bool IsPointIndicatorsVisible
		{
			get => (bool)GetValue(IsPointIndicatorsVisibleProperty);
			set => SetValue(IsPointIndicatorsVisibleProperty, value);
		}
		public static readonly DependencyProperty IsPointIndicatorsVisibleProperty = DependencyProperty.Register(
			"IsPointIndicatorsVisible",
			typeof(bool),
			typeof(CartesianChart),
			new PropertyMetadata(true));

		private bool IsUserSelectingRange
		{
			get => (bool)GetValue(IsUserSelectingRangeProperty);
			set => SetValue(IsUserSelectingRangeProperty, value);
		}
		public static readonly DependencyProperty IsUserSelectingRangeProperty = DependencyProperty.Register(
			"IsUserSelectingRange",
			typeof(bool),
			typeof(CartesianChart),
			new PropertyMetadata(false));

		private ZoomState CurrentZoomState
		{
			get => (ZoomState)GetValue(CurrentZoomStateProperty);
			set => SetValue(CurrentZoomStateProperty, value);
		}
		public static readonly DependencyProperty CurrentZoomStateProperty = DependencyProperty.Register(
			"CurrentZoomState",
			typeof(ZoomState),
			typeof(CartesianChart),
			new UIPropertyMetadata(new ZoomState(0, 0, 0, 0, 0, yBuffer), OnSetZoomState));
		
		private bool HasData
		{
			get => (bool)GetValue(HasDataProperty);
			set => SetValue(HasDataProperty, value);
		}
		public static readonly DependencyProperty HasDataProperty = DependencyProperty.Register(
			"HasData",
			typeof(bool),
			typeof(CartesianChart),
			new PropertyMetadata(false));

		#endregion
	}
}
