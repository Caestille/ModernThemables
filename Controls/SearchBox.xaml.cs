using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Win10Themables.Controls
{
    /// <summary>
    /// Interaction logic for SearchBox.xaml
    /// </summary>
    public partial class SearchBox : UserControl
    {
        private const string DefaultSearchText = "Search...";

        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }
        public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register(
          "SearchText", typeof(string), typeof(SearchBox), new PropertyMetadata(DefaultSearchText));

        public SearchBox()
        {
            InitializeComponent();
            SearchTextbox.Foreground = Application.Current.Resources["StatusTextBrush"] as SolidColorBrush;
        }

        private void TextBox_PreviewGotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if (SearchTextbox.Text == DefaultSearchText)
            {
                SearchTextbox.Foreground = Application.Current.Resources["TextBrush"] as SolidColorBrush;
                SearchTextbox.Text = string.Empty;
            }
        }

        private void TextBox_PreviewLostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if (SearchTextbox.Text == string.Empty)
            {
                SearchTextbox.Foreground = Application.Current.Resources["StatusTextBrush"] as SolidColorBrush;
                SearchTextbox.Text = DefaultSearchText;
            }
        }
    }
}
