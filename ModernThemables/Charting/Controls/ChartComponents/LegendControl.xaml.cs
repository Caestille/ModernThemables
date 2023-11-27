using ModernThemables.Charting.Interfaces;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ModernThemables.Charting.Controls.ChartComponents
{
	/// <summary>
	/// Interaction logic for LegendControl.xaml
	/// </summary>
	public partial class LegendControl : UserControl
	{
		public ObservableCollection<ISeries> Items
		{
			get => (ObservableCollection<ISeries>)GetValue(ItemsProperty);
			set => SetValue(ItemsProperty, value);
		}
		public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
			"Items",
			typeof(ObservableCollection<ISeries>),
			typeof(LegendControl),
			new UIPropertyMetadata(null));

		public DataTemplate LegendTemplate
		{
			get => (DataTemplate)GetValue(LegendTemplateProperty);
			set => SetValue(LegendTemplateProperty, value);
		}
		public static readonly DependencyProperty LegendTemplateProperty = DependencyProperty.Register(
			"LegendTemplate",
			typeof(DataTemplate),
			typeof(LegendControl),
			new PropertyMetadata(null));

		public CornerRadius CornerRadius
		{
			get => (CornerRadius)GetValue(CornerRadiusProperty);
			set => SetValue(CornerRadiusProperty, value);
		}
		public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
			"CornerRadius",
			typeof(CornerRadius),
			typeof(LegendControl),
			new PropertyMetadata(null));

		public object TemplatedDataContext
		{
			get => (DataTemplate)GetValue(TemplatedDataContextProperty);
			set => SetValue(TemplatedDataContextProperty, value);
		}
		public static readonly DependencyProperty TemplatedDataContextProperty = DependencyProperty.Register(
			"TemplatedDataContext",
			typeof(object),
			typeof(LegendControl),
			new PropertyMetadata(null));

		public Orientation Orientation
		{
			get => (Orientation)GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}
		public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
			"Orientation",
			typeof(Orientation),
			typeof(LegendControl),
			new UIPropertyMetadata(Orientation.Vertical, OnSetLegendOrientation));

		public LegendControl()
		{
			InitializeComponent();
		}

		private static void OnSetLegendOrientation(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not LegendControl _this) return;

			switch (_this.Orientation)
			{
				case Orientation.Vertical:
					_this.LegendItemsControl.ItemsPanel = (ItemsPanelTemplate)_this.Resources["StackTemplate"];
					break;
				case Orientation.Horizontal:
					_this.LegendItemsControl.ItemsPanel = (ItemsPanelTemplate)_this.Resources["WrapTemplate"];
					break;
			}
		}
	}
}
