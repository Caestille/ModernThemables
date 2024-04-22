using System.Windows;
using System.Windows.Media;

namespace ModernThemables.TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            ThemeManager.SetDarkMode(true);
            ThemeManager.SetTheme(Colors.Red);

            InitializeComponent();
        }
    }
}