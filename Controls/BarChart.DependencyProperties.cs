using ModernThemables.HelperClasses.Charting;
using ModernThemables.HelperClasses.Charting.CartesianChart;
using ModernThemables.Interfaces;
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

		public Func<object, string> XAxisFormatter
		{
			get => (Func<object, string>)GetValue(XAxisFormatterProperty);
			set => SetValue(XAxisFormatterProperty, value);
		}
		public static readonly DependencyProperty XAxisFormatterProperty = DependencyProperty.Register(
			"XAxisFormatter",
			typeof(Func<object, string>),
			typeof(BarChart),
			new PropertyMetadata(null));

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

		public new double FontSize
		{
			get => (double)GetValue(FontSizeProperty);
			set => SetValue(FontSizeProperty, value);
		}
		public static readonly new DependencyProperty FontSizeProperty = DependencyProperty.Register(
			"FontSize",
			typeof(double),
			typeof(BarChart),
			new PropertyMetadata(12d));

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

		private ObservableCollection<InternalPathSeriesViewModel> InternalSeries
		{
			get => (ObservableCollection<InternalPathSeriesViewModel>)GetValue(InternalSeriesProperty);
			set => SetValue(InternalSeriesProperty, value);
		}
		public static readonly DependencyProperty InternalSeriesProperty = DependencyProperty.Register(
			"InternalSeries",
			typeof(ObservableCollection<InternalPathSeriesViewModel>),
			typeof(BarChart),
			new PropertyMetadata(new ObservableCollection<InternalPathSeriesViewModel>()));

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

		#endregion
	}
}
