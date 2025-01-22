namespace ModernThemables.Icons;

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

public class Icon : Control
{
    public static readonly DependencyProperty KindProperty = DependencyProperty.Register(
        nameof(Kind),
        typeof(IconType),
        typeof(Icon),
        new PropertyMetadata(default(IconType), KindPropertyChangedCallback));

    /// <summary>
    /// Identifies the RotationAngle dependency property.
    /// </summary>
    public static readonly DependencyProperty RotationAngleProperty
        = DependencyProperty.Register(
            nameof(RotationAngle),
            typeof(double),
            typeof(Icon),
            new PropertyMetadata(0d, null, (dependencyObject, value) =>
            {
                var val = (double)value;
                return val < 0 ? 0d : (val > 360 ? 360d : value);
            }));

    private static readonly DependencyPropertyKey DataPropertyKey
        = DependencyProperty.RegisterReadOnly(
            nameof(Data),
            typeof(string),
            typeof(Icon),
            new PropertyMetadata(string.Empty));

    private static readonly DependencyPropertyKey YScalePropertyKey
        = DependencyProperty.RegisterReadOnly(
            nameof(YScale),
            typeof(int),
            typeof(Icon),
            new PropertyMetadata(1));

#pragma warning disable SA1202 // Elements should be ordered by access
    public static readonly DependencyProperty DataProperty = DataPropertyKey.DependencyProperty;

    public static readonly DependencyProperty YScaleProperty = YScalePropertyKey.DependencyProperty;
#pragma warning restore SA1202 // Elements should be ordered by access

    static Icon()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Icon), new FrameworkPropertyMetadata(typeof(Icon)));
    }

    public Icon() { }

    /// <summary>
    /// Gets or sets the icon to display.
    /// </summary>
    public IconType Kind
    {
        get => (IconType)this.GetValue(KindProperty);
        set => this.SetValue(KindProperty, value);
    }

    /// <summary>
    /// Gets the YScale property for the current icon kind.
    /// </summary>
    public int YScale
    {
        get => (int)this.GetValue(YScaleProperty);
        protected set => this.SetValue(YScalePropertyKey, value);
    }

    /// <summary>
    /// Gets or sets the rotation (angle).
    /// </summary>
    /// <value>The rotation.</value>
    public double RotationAngle
    {
        get => (double)this.GetValue(RotationAngleProperty);
        set => this.SetValue(RotationAngleProperty, value);
    }

    /// <summary>
    /// Gets the path data for the current icon kind.
    /// </summary>
    [TypeConverter(typeof(GeometryConverter))]
    public string Data
    {
        get => (string)this.GetValue(DataProperty);
        protected set => this.SetValue(DataPropertyKey, value);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        this.UpdateData();
    }

    internal void SetKind<TKind>(TKind iconKind) => this.SetCurrentValue(KindProperty, iconKind);

    internal void UpdateData()
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
