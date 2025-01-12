namespace ModernThemables.Icons;

using System;
using System.Windows.Markup;

public interface IIconExtension
{
    double Width { get; set; }
    double Height { get; set; }
    double RotationAngle { get; set; }
}

public static class IconExtensionHelper
{
    public static BaseIcon GetPackIcon<TPack, TKind>(this IIconExtension packIconExtension, TKind kind) where TPack : BaseIcon, new()
    {
        var packIcon = new TPack();
        packIcon.SetKind(kind);

        if (((BaseIconExtension)packIconExtension).IsFieldChanged(BaseIconExtension.ChangedFieldFlags.Width))
        {
            packIcon.Width = packIconExtension.Width;
        }

        if (((BaseIconExtension)packIconExtension).IsFieldChanged(BaseIconExtension.ChangedFieldFlags.Height))
        {
            packIcon.Height = packIconExtension.Height;
        }

        if (((BaseIconExtension)packIconExtension).IsFieldChanged(BaseIconExtension.ChangedFieldFlags.RotationAngle))
        {
            packIcon.RotationAngle = packIconExtension.RotationAngle;
        }

        return packIcon;
    }
}

[MarkupExtensionReturnType(typeof(BaseIcon))]
public abstract class BaseIconExtension : MarkupExtension, IIconExtension
{
    private double width = 16d;

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

    private double height = 16d;

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

    private double rotationAngle = 0d;

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

    internal ChangedFieldFlags changedField; // Cache changed field bits

    internal bool IsFieldChanged(ChangedFieldFlags reqFlag)
    {
        return (this.changedField & reqFlag) != 0;
    }

    internal void WriteFieldChangedFlag(ChangedFieldFlags reqFlag, bool set)
    {
        if (set)
        {
            this.changedField |= reqFlag;
        }
        else
        {
            this.changedField &= (~reqFlag);
        }
    }

    [Flags]
    internal enum ChangedFieldFlags : ushort
    {
        Width = 0x0001,
        Height = 0x0002,
        RotationAngle = 0x0008,
    }
}