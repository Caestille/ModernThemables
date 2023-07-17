using System.Windows;
using System.Windows.Controls;

namespace ModernThemables.Icons.AnimatedIcons
{
    public class AnimatedMenuOpenCloseIcon : Control
    {
        static AnimatedMenuOpenCloseIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedMenuOpenCloseIcon), new FrameworkPropertyMetadata(typeof(AnimatedMenuOpenCloseIcon)));
        }

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
            nameof(IsOpen),
            typeof(bool),
            typeof(AnimatedMenuOpenCloseIcon),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }
    }
}