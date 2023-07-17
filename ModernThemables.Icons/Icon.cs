using System.Windows;

namespace ModernThemables.Icons
{
    public class Icon : BaseIcon
    {
        /// <summary>
        /// Gets or sets the icon to display.
        /// </summary>
        public IconEnum Kind
        {
            get => (IconEnum)GetValue(KindProperty);
            set => SetValue(KindProperty, value);
        }

        public static readonly DependencyProperty KindProperty = DependencyProperty.Register(
            nameof(Kind),
            typeof(IconEnum),
            typeof(Icon),
            new PropertyMetadata(default(IconEnum), KindPropertyChangedCallback));

        static Icon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Icon), new FrameworkPropertyMetadata(typeof(Icon)));
        }

        public Icon() { }

        internal override void SetKind<TKind>(TKind iconKind) => SetCurrentValue(KindProperty, iconKind);

        internal override void UpdateData()
        {
            if (Kind != default)
            {
                string data = null;
                IconDataFactory.DataIndex.Value?.TryGetValue(Kind, out data);
                Data = data;
            }
            else
            {
                Data = null;
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