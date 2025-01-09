namespace ModernThemables.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class DataGrid2 : DataGrid
    {
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)this.GetValue(CornerRadiusProperty);
            set => this.SetValue(CornerRadiusProperty, value);
        }
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(DataGrid2),
            new PropertyMetadata(new CornerRadius(0)));

        public Brush ColumnHeaderForeground
        {
            get => (Brush)this.GetValue(ColumnHeaderForegroundProperty);
            set => this.SetValue(ColumnHeaderForegroundProperty, value);
        }
        public static readonly DependencyProperty ColumnHeaderForegroundProperty = DependencyProperty.Register(
            nameof(ColumnHeaderForeground),
            typeof(Brush),
            typeof(DataGrid2),
            new PropertyMetadata(new SolidColorBrush(Colors.White)));
    }
}
