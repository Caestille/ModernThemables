namespace ModernThemables.Icons;

using System;
using System.Windows.Markup;

[MarkupExtensionReturnType(typeof(Icon))]
public class IconExtension : MarkupExtension, IIconExtension
{
    private double width = 16d;
    private double height = 16d;
    private double rotationAngle = 0d;

    public IconExtension()
    {
    }

    public IconExtension(IconType kind)
    {
        this.Kind = kind;
    }

    [ConstructorArgument("kind")]
    public IconType Kind { get; set; }

    public double Width
    {
        get => this.width;
        set
        {
            if (Equals(this.width, value))
            {
                return;
            }

            this.width = value;
            this.WriteFieldChangedFlag(ChangedFieldFlags.Width, true);
        }
    }

    public double Height
    {
        get => this.height;
        set
        {
            if (Equals(this.height, value))
            {
                return;
            }

            this.height = value;
            this.WriteFieldChangedFlag(ChangedFieldFlags.Height, true);
        }
    }

    public double RotationAngle
    {
        get => this.rotationAngle;
        set
        {
            if (Equals(this.rotationAngle, value))
            {
                return;
            }

            this.rotationAngle = value;
            this.WriteFieldChangedFlag(ChangedFieldFlags.RotationAngle, true);
        }
    }

    internal ChangedFieldFlags ChangedField { get; set; } // Cache changed field bits

    public override object ProvideValue(IServiceProvider serviceProvider)
        => this.GetPackIcon<Icon, IconType>(this.Kind);

    internal bool IsFieldChanged(ChangedFieldFlags reqFlag) => (this.ChangedField & reqFlag) != 0;

    internal void WriteFieldChangedFlag(ChangedFieldFlags reqFlag, bool set)
    {
        if (set)
        {
            this.ChangedField |= reqFlag;
        }
        else
        {
            this.ChangedField &= ~reqFlag;
        }
    }
}
