using ModernThemables.HelperClasses.WpfChart;
using ModernThemables.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModernThemables.Controls
{
	public partial class WpfChart
	{
		public ObservableCollection<ISeries> Series
		{
			get { return (ObservableCollection<ISeries>)GetValue(SeriesProperty); }
			set { SetValue(SeriesProperty, value); }
		}
		public static readonly DependencyProperty SeriesProperty = DependencyProperty.Register(
		  "Series", typeof(ObservableCollection<ISeries>), typeof(WpfChart), new FrameworkPropertyMetadata(null, OnSeriesSet));

		public Func<object, string> XAxisFormatter
		{
			get { return (Func<object, string>)GetValue(XAxisFormatterProperty); }
			set { SetValue(XAxisFormatterProperty, value); }
		}
		public static readonly DependencyProperty XAxisFormatterProperty = DependencyProperty.Register(
		  "XAxisFormatter", typeof(Func<object, string>), typeof(WpfChart), new PropertyMetadata(null));

		public Func<object, string> XAxisCursorLabelFormatter
		{
			get { return (Func<object, string>)GetValue(XAxisCursorLabelFormatterProperty); }
			set { SetValue(XAxisCursorLabelFormatterProperty, value); }
		}
		public static readonly DependencyProperty XAxisCursorLabelFormatterProperty = DependencyProperty.Register(
		  "XAxisCursorLabelFormatter", typeof(Func<object, string>), typeof(WpfChart), new PropertyMetadata(null));

		public Func<object, string> YAxisFormatter
		{
			get { return (Func<object, string>)GetValue(YAxisFormatterProperty); }
			set { SetValue(YAxisFormatterProperty, value); }
		}
		public static readonly DependencyProperty YAxisFormatterProperty = DependencyProperty.Register(
		  "YAxisFormatter", typeof(Func<object, string>), typeof(WpfChart), new PropertyMetadata(null));

		public Func<object, string> YAxisCursorLabelFormatter
		{
			get { return (Func<object, string>)GetValue(YAxisCursorLabelFormatterProperty); }
			set { SetValue(YAxisCursorLabelFormatterProperty, value); }
		}
		public static readonly DependencyProperty YAxisCursorLabelFormatterProperty = DependencyProperty.Register(
		  "YAxisCursorLabelFormatter", typeof(Func<object, string>), typeof(WpfChart), new PropertyMetadata(null));

		public Func<object, bool> YAxisLabelIdentifier
		{
			get { return (Func<object, bool>)GetValue(YAxisLabelIdentifierProperty); }
			set { SetValue(YAxisLabelIdentifierProperty, value); }
		}
		public static readonly DependencyProperty YAxisLabelIdentifierProperty = DependencyProperty.Register(
		  "YAxisLabelIdentifier", typeof(Func<object, bool>), typeof(WpfChart), new PropertyMetadata(null));

		public Func<object, bool> XAxisLabelIdentifier
		{
			get { return (Func<object, bool>)GetValue(XAxisLabelIdentifierProperty); }
			set { SetValue(XAxisLabelIdentifierProperty, value); }
		}
		public static readonly DependencyProperty XAxisLabelIdentifierProperty = DependencyProperty.Register(
		  "XAxisLabelIdentifier", typeof(Func<object, bool>), typeof(WpfChart), new PropertyMetadata(null));

		public bool ShowXSeparatorLines
		{
			get { return (bool)GetValue(ShowXSeparatorLinesProperty); }
			set { SetValue(ShowXSeparatorLinesProperty, value); }
		}
		public static readonly DependencyProperty ShowXSeparatorLinesProperty = DependencyProperty.Register(
		  "ShowXSeparatorLines", typeof(bool), typeof(WpfChart), new PropertyMetadata(true));

		public bool ShowYSeparatorLines
		{
			get { return (bool)GetValue(ShowYSeparatorLinesProperty); }
			set { SetValue(ShowYSeparatorLinesProperty, value); }
		}
		public static readonly DependencyProperty ShowYSeparatorLinesProperty = DependencyProperty.Register(
		  "ShowYSeparatorLines", typeof(bool), typeof(WpfChart), new PropertyMetadata(true));

		public bool IsZoomed
		{
			get { return (bool)GetValue(IsZoomedProperty); }
			set { SetValue(IsZoomedProperty, value); }
		}
		public static readonly DependencyProperty IsZoomedProperty = DependencyProperty.Register(
		  "IsZoomed", typeof(bool), typeof(WpfChart), new PropertyMetadata(false));

		public DataTemplate TooltipTemplate
		{
			get { return (DataTemplate)GetValue(TooltipTemplateProperty); }
			set { SetValue(TooltipTemplateProperty, value); }
		}
		public static readonly DependencyProperty TooltipTemplateProperty = DependencyProperty.Register(
		  "TooltipTemplate", typeof(DataTemplate), typeof(WpfChart), new PropertyMetadata(null));

		private string TooltipString
		{
			get { return (string)GetValue(TooltipStringProperty); }
			set { SetValue(TooltipStringProperty, value); }
		}
		public static readonly DependencyProperty TooltipStringProperty = DependencyProperty.Register(
		  "TooltipString", typeof(string), typeof(WpfChart));

		private InternalChartPointRepresentation HoveredPoint
		{
			get { return (InternalChartPointRepresentation)GetValue(HoveredPointProperty); }
			set { SetValue(HoveredPointProperty, value); }
		}
		public static readonly DependencyProperty HoveredPointProperty = DependencyProperty.Register(
		  "HoveredPoint", typeof(InternalChartPointRepresentation), typeof(WpfChart), new PropertyMetadata(null));

		private ObservableCollection<WpfChartSeriesViewModel> ConvertedSeries
		{
			get { return (ObservableCollection<WpfChartSeriesViewModel>)GetValue(ConvertedSeriesProperty); }
			set { SetValue(ConvertedSeriesProperty, value); }
		}
		public static readonly DependencyProperty ConvertedSeriesProperty = DependencyProperty.Register(
		  "ConvertedSeries", typeof(ObservableCollection<WpfChartSeriesViewModel>), typeof(WpfChart), new PropertyMetadata(new ObservableCollection<WpfChartSeriesViewModel>()));

		private ObservableCollection<ValueWithHeight> XAxisLabels
		{
			get { return (ObservableCollection<ValueWithHeight>)GetValue(XAxisLabelsProperty); }
			set { SetValue(XAxisLabelsProperty, value); }
		}
		public static readonly DependencyProperty XAxisLabelsProperty = DependencyProperty.Register(
		  "XAxisLabels", typeof(ObservableCollection<ValueWithHeight>), typeof(WpfChart), new PropertyMetadata(new ObservableCollection<ValueWithHeight>()));

		private ObservableCollection<ValueWithHeight> YAxisLabels
		{
			get { return (ObservableCollection<ValueWithHeight>)GetValue(YAxisLabelsProperty); }
			set { SetValue(YAxisLabelsProperty, value); }
		}
		public static readonly DependencyProperty YAxisLabelsProperty = DependencyProperty.Register(
		  "YAxisLabels", typeof(ObservableCollection<ValueWithHeight>), typeof(WpfChart), new PropertyMetadata(new ObservableCollection<ValueWithHeight>()));

		private bool IsCrosshairVisible
		{
			get { return (bool)GetValue(IsCrosshairVisibleProperty); }
			set { SetValue(IsCrosshairVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsCrosshairVisibleProperty = DependencyProperty.Register(
		  "IsCrosshairVisible", typeof(bool), typeof(WpfChart), new PropertyMetadata(true));

		private bool IsTooltipVisible
		{
			get { return (bool)GetValue(IsTooltipVisibleProperty); }
			set { SetValue(IsTooltipVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsTooltipVisibleProperty = DependencyProperty.Register(
		  "IsTooltipVisible", typeof(bool), typeof(WpfChart), new PropertyMetadata(true));

		private bool IsAxisIndicatorsVisible
		{
			get { return (bool)GetValue(IsAxisIndicatorsVisibleProperty); }
			set { SetValue(IsAxisIndicatorsVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsAxisIndicatorsVisibleProperty = DependencyProperty.Register(
		  "IsAxisIndicatorsVisible", typeof(bool), typeof(WpfChart), new PropertyMetadata(true));

		private bool IsPointIndicatorsVisible
		{
			get { return (bool)GetValue(IsPointIndicatorsVisibleProperty); }
			set { SetValue(IsPointIndicatorsVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsPointIndicatorsVisibleProperty = DependencyProperty.Register(
		  "IsPointIndicatorsVisible", typeof(bool), typeof(WpfChart), new PropertyMetadata(true));

		private bool IsUserSelectingRange
		{
			get { return (bool)GetValue(IsUserSelectingRangeProperty); }
			set { SetValue(IsUserSelectingRangeProperty, value); }
		}
		public static readonly DependencyProperty IsUserSelectingRangeProperty = DependencyProperty.Register(
		  "IsUserSelectingRange", typeof(bool), typeof(WpfChart), new PropertyMetadata(false));

		private ZoomState CurrentZoomState
		{
			get { return (ZoomState)GetValue(CurrentZoomStateProperty); }
			set { SetValue(CurrentZoomStateProperty, value); }
		}
		public static readonly DependencyProperty CurrentZoomStateProperty = DependencyProperty.Register(
		  "CurrentZoomState", typeof(ZoomState), typeof(WpfChart), new PropertyMetadata(new ZoomState(0, 0, 0, 0, 0, 0.1)));
		
		private bool HasData
		{
			get { return (bool)GetValue(HasDataProperty); }
			set { SetValue(HasDataProperty, value); }
		}
		public static readonly DependencyProperty HasDataProperty = DependencyProperty.Register(
		  "HasData", typeof(bool), typeof(WpfChart), new PropertyMetadata(false));
	}
}
