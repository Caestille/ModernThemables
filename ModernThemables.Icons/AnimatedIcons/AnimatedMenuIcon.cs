using System.Windows;
using System.Windows.Controls;

namespace ModernThemables.Icons.AnimatedIcons
{
    public class AnimatedMenuIcon : Control
    {
        static AnimatedMenuIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedMenuIcon), new FrameworkPropertyMetadata(typeof(AnimatedMenuIcon)));
        }

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
            nameof(IsOpen),
            typeof(bool),
            typeof(AnimatedMenuIcon),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }
    }
}