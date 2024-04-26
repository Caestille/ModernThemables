using Microsoft.Toolkit.Mvvm.ComponentModel;
using ModernThemables.ViewModels;
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
            InitializeComponent();

            this.DataContext = new VM();
        }

        public class VM : ObservableObject
        {
            private bool enabled = true;
            public bool IsEnabled
            {
                get => enabled;
                set => SetProperty(ref enabled, value);
            }

            public ThemingControlViewModel ThemeVm { get; } = new ThemingControlViewModel();

            public VM()
            {
                ThemeVm.IsDarkMode = false;
                ThemeVm.ThemeColourProperty = Colors.Red;
            }
        }
    }
}