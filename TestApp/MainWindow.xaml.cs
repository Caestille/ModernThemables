namespace TestApp;

using ModernThemables.Controls;
using ModernThemables.ViewModels;

/// <summary>
/// Interaction logic for MainWindow.xaml.
/// </summary>
public partial class MainWindow : Window2
{
    private class DgDataVm
    {
        public DgDataVm(string name, double value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; set; }

        public double Value { get; set; }
    }

    private class MenuItemVm : IHamburgerMenuItem
    {
        public string Name => "A Menu Item";

        public List<object> GetChildren(bool recurse = false) => new List<object>();
    }

    public MainWindow()
    {
        this.InitializeComponent();

        this.DataGrid.ItemsSource = new List<DgDataVm>() { new DgDataVm("Hello World", 42), new DgDataVm("I am test data", 69) };

        this.Menu.Items = new List<MenuItemVm>() { new MenuItemVm() };
    }
}