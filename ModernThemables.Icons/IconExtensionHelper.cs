namespace ModernThemables.Icons;

public static class IconExtensionHelper
{
    public static Icon GetPackIcon<TPack, TKind>(this IIconExtension packIconExtension, TKind kind)
        where TPack : Icon, new()
    {
        var packIcon = new TPack();
        packIcon.SetKind(kind);

        if (((IconExtension)packIconExtension).IsFieldChanged(ChangedFieldFlags.Width))
        {
            packIcon.Width = packIconExtension.Width;
        }

        if (((IconExtension)packIconExtension).IsFieldChanged(ChangedFieldFlags.Height))
        {
            packIcon.Height = packIconExtension.Height;
        }

        if (((IconExtension)packIconExtension).IsFieldChanged(ChangedFieldFlags.RotationAngle))
        {
            packIcon.RotationAngle = packIconExtension.RotationAngle;
        }

        return packIcon;
    }
}
