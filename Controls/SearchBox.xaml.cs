using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Win10Themables.Controls
{
    /// <summary>
    /// Interaction logic for SearchBox.xaml
    /// </summary>
    public partial class SearchBox
    {
        private const string DefaultSearchText = "Search...";

        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }
        public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register(
          "SearchText", typeof(string), typeof(SearchBox), new PropertyMetadata(DefaultSearchText));

        public double BackgroundOpacity
        {
            get { return (double)GetValue(BackgroundOpacityProperty); }
            set { SetValue(BackgroundOpacityProperty, value); }
        }
        public static readonly DependencyProperty BackgroundOpacityProperty = DependencyProperty.Register(
          "BackgroundOpacity", typeof(double), typeof(SearchBox), new PropertyMetadata(1d));

        public SolidColorBrush DefaultForegroundBrush
		{
            get { return (SolidColorBrush)GetValue(DefaultForegroundBrushProperty); }
            set { SetValue(DefaultForegroundBrushProperty, value); }
        }
        public static readonly DependencyProperty DefaultForegroundBrushProperty = DependencyProperty.Register(
          "DefaultForegroundBrush", typeof(SolidColorBrush), typeof(SearchBox), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        public SolidColorBrush SearchboxBackgroundBrush
        {
            get { return (SolidColorBrush)GetValue(SearchboxBackgroundBrushProperty); }
            set { SetValue(SearchboxBackgroundBrushProperty, value); }
        }
        public static readonly DependencyProperty SearchboxBackgroundBrushProperty = DependencyProperty.Register(
          "SearchboxBackgroundBrush", typeof(SolidColorBrush), typeof(SearchBox), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public SearchBox()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewGotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if (SearchTextbox.Text == DefaultSearchText)
            {
                SearchTextbox.Text = string.Empty;
            }
        }

        private void TextBox_PreviewLostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if (SearchTextbox.Text == string.Empty)
            {
                SearchTextbox.Text = DefaultSearchText;
            }
        }
    }
}
