using ModernThemables.Controls;
using ModernThemables.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window2
    {
        private class DgDataVm
        {
            public DgDataVm(string name, double value)
            {
                Name = name;
                Value = value;
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
            InitializeComponent();

            this.DataGrid.ItemsSource = new List<DgDataVm>() { new DgDataVm("Hello World", 42), new DgDataVm("I am test data", 69) };

            this.Menu.Items = new List<MenuItemVm>() { new MenuItemVm() };
        }
    }
}