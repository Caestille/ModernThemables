namespace ModernThemables.Icons;

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

/// <summary>
/// Class PackIconControlBase which is the base class for any PackIcon control.
/// </summary>
public abstract class BaseIcon : Control
{
    static BaseIcon() { }

    private static readonly DependencyPropertyKey DataPropertyKey
        = DependencyProperty.RegisterReadOnly(nameof(Data), typeof(string), typeof(BaseIcon), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty DataProperty = DataPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the path data for the current icon kind.
    /// </summary>
    [TypeConverter(typeof(GeometryConverter))]
    public string Data
    {
        get => (string)this.GetValue(DataProperty);
        protected set => this.SetValue(DataPropertyKey, value);
    }

    private static readonly DependencyPropertyKey YScalePropertyKey
        = DependencyProperty.RegisterReadOnly(nameof(YScale), typeof(int), typeof(BaseIcon), new PropertyMetadata(1));

    public static readonly DependencyProperty YScaleProperty = YScalePropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the YScale property for the current icon kind.
    /// </summary>
    public int YScale
    {
        get => (int)this.GetValue(YScaleProperty);
        protected set => this.SetValue(YScalePropertyKey, value);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        this.UpdateData();
    }

    internal abstract void UpdateData();

    internal abstract void SetKind<TKind>(TKind iconKind);

    /// <summary>
    /// Identifies the RotationAngle dependency property.
    /// </summary>
    public static readonly DependencyProperty RotationAngleProperty
        = DependencyProperty.Register(
            nameof(RotationAngle),
            typeof(double),
            typeof(BaseIcon),
            new PropertyMetadata(0d, null, (dependencyObject, value) =>
            {
                var val = (double)value;
                return val < 0 ? 0d : (val > 360 ? 360d : value);
            }));

    /// <summary>
    /// Gets or sets the rotation (angle).
    /// </summary>
    /// <value>The rotation.</value>
    public double RotationAngle
    {
        get => (double)this.GetValue(RotationAngleProperty);
        set => this.SetValue(RotationAngleProperty, value);
    }
}
