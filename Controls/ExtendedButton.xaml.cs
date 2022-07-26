using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Win10Themables.Controls
{
	/// <summary>
	/// Interaction logic for ExtendedButton2.xaml
	/// </summary>
	public partial class ExtendedButton : Button
	{
        readonly static SolidColorBrush DefaultMouseOverProperty = new BrushConverter().ConvertFromString("#FFBEE6FD") as SolidColorBrush;

        public ExtendedButton()
        {
            InitializeComponent();
        }

        public SolidColorBrush MouseOverColour
        {
            get { return (SolidColorBrush)GetValue(MouseOverProperty); }
            set { SetValue(MouseOverProperty, value); }
        }
        public static readonly DependencyProperty MouseOverProperty = DependencyProperty.Register(
          "MouseOverColour", typeof(SolidColorBrush), typeof(ExtendedButton), new PropertyMetadata(DefaultMouseOverProperty));

        public SolidColorBrush MouseDownColour
        {
            get { return (SolidColorBrush)GetValue(MouseDownProperty); }
            set { SetValue(MouseDownProperty, value); }
        }
        public static readonly DependencyProperty MouseDownProperty = DependencyProperty.Register(
          "MouseDownColour", typeof(SolidColorBrush), typeof(ExtendedButton), new PropertyMetadata(DefaultMouseOverProperty));

        public SolidColorBrush DisabledBackgroundColour
        {
            get { return (SolidColorBrush)GetValue(DisabledBackgroundColourProperty); }
            set { SetValue(DisabledBackgroundColourProperty, value); }
        }
        public static readonly DependencyProperty DisabledBackgroundColourProperty = DependencyProperty.Register(
          "DisabledBackgroundColour", typeof(SolidColorBrush), typeof(ExtendedButton), new PropertyMetadata(DefaultMouseOverProperty));

        public SolidColorBrush DisabledForegroundColour
        {
            get { return (SolidColorBrush)GetValue(DisabledForegroundColourProperty); }
            set { SetValue(DisabledForegroundColourProperty, value); }
        }
        public static readonly DependencyProperty DisabledForegroundColourProperty = DependencyProperty.Register(
          "DisabledForegroundColour", typeof(SolidColorBrush), typeof(ExtendedButton), new PropertyMetadata(DefaultMouseOverProperty));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
          "CornerRadius", typeof(CornerRadius), typeof(ExtendedButton), new PropertyMetadata(new CornerRadius(0)));
    }
}
