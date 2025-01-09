namespace ModernThemables.Icons
{
    using System.Windows;

    public class Icon : BaseIcon
    {
        /// <summary>
        /// Gets or sets the icon to display.
        /// </summary>
        public IconType Kind
        {
            get => (IconType)this.GetValue(KindProperty);
            set => this.SetValue(KindProperty, value);
        }

        public static readonly DependencyProperty KindProperty = DependencyProperty.Register(
            nameof(Kind),
            typeof(IconType),
            typeof(Icon),
            new PropertyMetadata(default(IconType), KindPropertyChangedCallback));

        static Icon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Icon), new FrameworkPropertyMetadata(typeof(Icon)));
        }

        public Icon() { }

        internal override void SetKind<TKind>(TKind iconKind) => this.SetCurrentValue(KindProperty, iconKind);

        internal override void UpdateData()
        {
            if (this.Kind != default)
            {
                (string, bool) data = (string.Empty, false);
                IconDataFactory.DataIndex.Value?.TryGetValue(this.Kind, out data);
                this.Data = data.Item1!;
                this.YScale = data.Item2 ? -1 : 1;
            }
            else
            {
                this.Data = string.Empty;
                this.YScale = 1;
            }
        }

        private static void KindPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                ((Icon)dependencyObject).UpdateData();
            }
        }
    }
}