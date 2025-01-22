namespace ModernThemables.Icons;

using System;

[Flags]
internal enum ChangedFieldFlags : ushort
{
    Width = 0x0001,
    Height = 0x0002,
    RotationAngle = 0x0008,
}
