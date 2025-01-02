using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernThemables.Controls
{
	public class DataGrid2 : DataGrid
    {
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(DataGrid2),
            new PropertyMetadata(new CornerRadius(0)));

        public Brush ColumnHeaderForeground
        {
            get => (Brush)GetValue(ColumnHeaderForegroundProperty);
            set => SetValue(ColumnHeaderForegroundProperty, value);
        }
        public static readonly DependencyProperty ColumnHeaderForegroundProperty = DependencyProperty.Register(
            nameof(ColumnHeaderForeground),
            typeof(Brush),
            typeof(DataGrid2),
            new PropertyMetadata(new SolidColorBrush(Colors.White)));
    }
}
