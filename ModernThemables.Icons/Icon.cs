using System.Windows;

namespace ModernThemables.Icons
{
    public class Icon : BaseIcon
    {
        /// <summary>
        /// Gets or sets the icon to display.
        /// </summary>
        public IconType Kind
        {
            get => (IconType)GetValue(KindProperty);
            set => SetValue(KindProperty, value);
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

        internal override void SetKind<TKind>(TKind iconKind) => SetCurrentValue(KindProperty, iconKind);

        internal override void UpdateData()
        {
            if (Kind != default)
            {
                (string, bool) data = (null, false);
                IconDataFactory.DataIndex.Value?.TryGetValue(Kind, out data);
                Data = data.Item1!;
                YScale = data.Item2 ? -1 : 1;
            }
            else
            {
                Data = null;
                YScale = 1;
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