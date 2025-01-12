namespace ModernThemables.Icons.AnimatedIcons;

using System.Windows;
using System.Windows.Controls;

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
        get => (bool)this.GetValue(IsOpenProperty);
        set => this.SetValue(IsOpenProperty, value);
    }
}