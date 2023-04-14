using System.Windows;
using System.Windows.Controls;

namespace ModernThemables.Charting.Controls.ChartComponents
{
    /// <summary>
    /// Interaction logic for LegendControl.xaml
    /// </summary>
    public partial class LegendControl : UserControl
	{
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

		private static async void OnSetLegendOrientation(DependencyObject sender, DependencyPropertyChangedEventArgs e)
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
