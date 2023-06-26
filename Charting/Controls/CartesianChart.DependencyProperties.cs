using ModernThemables.Charting.Interfaces;
using ModernThemables.Charting.Models;
using ModernThemables.Charting.ViewModels;
using ModernThemables.Charting.ViewModels.CartesianChart;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace ModernThemables.Charting.Controls
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
			private set => SetValue(IsZoomedProperty, value);
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
			new FrameworkPropertyMetadata(TooltipLocation.Cursor));

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

		//public double Min
		//{
		//	get => (double)GetValue(MinProperty);
		//	set => SetValue(MinProperty, value);
		//}
		//public static readonly DependencyProperty MinProperty = DependencyProperty.Register(
		//	"Min",
		//	typeof(double),
		//	typeof(CartesianChart),
		//	new UIPropertyMetadata(-1d, OnSetMinMax));

		//public double Max
		//{
		//	get => (double)GetValue(MaxProperty);
		//	set => SetValue(MaxProperty, value);
		//}
		//public static readonly DependencyProperty MaxProperty = DependencyProperty.Register(
		//	"Max",
		//	typeof(double),
		//	typeof(CartesianChart),
		//	new UIPropertyMetadata(-1d, OnSetMinMax));

		public Func<IEnumerable<IChartEntity>, IChartEntity, object> TooltipContentGetter
		{
			get => (Func<IEnumerable<IChartEntity>, IChartEntity, object>)GetValue(TooltipContentGetterProperty);
			set => SetValue(TooltipContentGetterProperty, value);
		}
		public static readonly DependencyProperty TooltipContentGetterProperty = DependencyProperty.Register(
			"TooltipContentGetter",
			typeof(Func<IEnumerable<IChartEntity>, IChartEntity, object>),
			typeof(CartesianChart),
			new PropertyMetadata(null));

		#endregion

		#region Private properties

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

		private Func<Point, IEnumerable<TooltipViewModel>> TooltipGetterFunc
		{
			get => (Func<Point, IEnumerable<TooltipViewModel>>)GetValue(TooltipGetterFuncProperty);
			set => SetValue(TooltipGetterFuncProperty, value);
		}
		public static readonly DependencyProperty TooltipGetterFuncProperty = DependencyProperty.Register(
			"TooltipGetterFunc",
			typeof(Func<Point, IEnumerable<TooltipViewModel>>),
			typeof(CartesianChart),
			new PropertyMetadata(null));

		#endregion
	}
}
