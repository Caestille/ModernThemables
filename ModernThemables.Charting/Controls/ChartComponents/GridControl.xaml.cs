namespace ModernThemables.Charting.Controls.ChartComponents;

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ModernThemables.Charting.Models;

/// <summary>
/// Interaction logic for GridControl.xaml.
/// </summary>
public partial class GridControl : UserControl
{
    public static readonly DependencyProperty XLabelsProperty = DependencyProperty.Register(
        nameof(XLabels),
        typeof(ObservableCollection<AxisLabel>),
        typeof(GridControl),
        new UIPropertyMetadata(null));

    public static readonly DependencyProperty YLabelsProperty = DependencyProperty.Register(
        nameof(YLabels),
        typeof(ObservableCollection<AxisLabel>),
        typeof(GridControl),
        new UIPropertyMetadata(null));

    public GridControl()
    {
        this.InitializeComponent();
    }

    public ObservableCollection<AxisLabel> XLabels
    {
        get => (ObservableCollection<AxisLabel>)this.GetValue(XLabelsProperty);
        set => this.SetValue(XLabelsProperty, value);
    }

    public ObservableCollection<AxisLabel> YLabels
    {
        get => (ObservableCollection<AxisLabel>)this.GetValue(YLabelsProperty);
        set => this.SetValue(YLabelsProperty, value);
    }
}
