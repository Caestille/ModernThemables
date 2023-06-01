using ModernThemables.Charting.Models;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace ModernThemables.Charting.Controls.ChartComponents
{
	/// <summary>
	/// Interaction logic for GridControl.xaml
	/// </summary>
	public partial class GridControl : UserControl
	{
		public ObservableCollection<AxisLabel> XLabels
		{
			get => (ObservableCollection<AxisLabel>)GetValue(XLabelsProperty);
			set => SetValue(XLabelsProperty, value);
		}
		public static readonly DependencyProperty XLabelsProperty = DependencyProperty.Register(
			"XLabels",
			typeof(ObservableCollection<AxisLabel>),
			typeof(GridControl),
			new UIPropertyMetadata(null));

		public ObservableCollection<AxisLabel> YLabels
		{
			get => (ObservableCollection<AxisLabel>)GetValue(YLabelsProperty);
			set => SetValue(YLabelsProperty, value);
		}
		public static readonly DependencyProperty YLabelsProperty = DependencyProperty.Register(
			"YLabels",
			typeof(ObservableCollection<AxisLabel>),
			typeof(GridControl),
			new UIPropertyMetadata(null));

		public GridControl()
		{
			InitializeComponent();
		}
	}
}
