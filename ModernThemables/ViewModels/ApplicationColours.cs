namespace ModernThemables.ViewModels;

#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable SA1306 // Member names should begin with lower-case letter
#pragma warning disable SA1214 // Readonly field should appear before non-readonly field

using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class ThemingControlViewModel : ObservableObject
{
    public static readonly Color PrimaryBackgroundColourLight = MonoColour(255);
    public static readonly Color PrimaryBackgroundColourDark = MonoColour(10);

    public static readonly Color SecondaryBackgroundColourLight = MonoColour(240);
    public static readonly Color SecondaryBackgroundColourDark = MonoColour(15);

    public static readonly Color PrimaryTextColourLight = Colors.Black;
    public static readonly Color PrimaryTextColourDark = Colors.White;

    public static readonly Color SecondaryTextColourLight = Colors.Gray;
    public static readonly Color SecondaryTextColourDark = Colors.DarkGray;

    public static readonly Color TertiaryTextColourLight = MonoColour(215);
    public static readonly Color TertiaryTextColourDark = MonoColour(40);

    public static readonly Color PrimaryControlColourLight = MonoColour(195);
    public static readonly Color PrimaryControlColourDark = MonoColour(60);

    public static readonly Color SecondaryControlColourLight = MonoColour(235);
    public static readonly Color SecondaryControlColourDark = MonoColour(20);

    public static Color ThemeColour = Color.FromArgb(255, 47, 47, 74);

    public static Color ThemeTextColour = Colors.White;

    // Mouse over
    public static readonly Color PrimaryControlMouseOverColourLight = MonoColour(215);
    public static readonly Color PrimaryControlMouseOverColourDark = MonoColour(80);

    public static readonly Color SecondaryControlMouseOverColourLight = MonoColour(215);
    public static readonly Color SecondaryControlMouseOverColourDark = MonoColour(80);

    public static Color ThemeMouseOverColour = Color.FromArgb(255, 30, 134, 204);

    // Mouse down
    public static readonly Color PrimaryControlMouseDownColourLight = MonoColour(185);
    public static readonly Color PrimaryControlMouseDownColourDark = MonoColour(50);

    public static readonly Color SecondaryControlMouseDownColourLight = MonoColour(185);
    public static readonly Color SecondaryControlMouseDownColourDark = MonoColour(50);

    public static Color ThemeMouseDownColour = Color.FromArgb(255, 0, 103, 173);

    // Borders
    public static readonly Color PrimaryControlBorderColourLight = MonoColour(185);
    public static readonly Color PrimaryControlBorderColourDark = MonoColour(50);

    public static readonly Color SecondaryControlBorderColourLight = MonoColour(185);
    public static readonly Color SecondaryControlBorderColourDark = MonoColour(50);

    public static Color ThemeBorderColour = Color.FromArgb(255, 0, 103, 173);

    // Disabled
    public static readonly Color PrimaryControlDisabledColourLight = MonoColour(215);
    public static readonly Color PrimaryControlDisabledColourDark = MonoColour(40);

    public static readonly Color SecondaryControlDisabledColourLight = MonoColour(245);
    public static readonly Color SecondaryControlDisabledColourDark = MonoColour(10);

    private static Color ThemeDisabledColour = Color.FromArgb(255, 0, 103, 173);

    // ============= Resolved =============
    public static Color PrimaryBackgroundColour => DarkMode ? PrimaryBackgroundColourDark : PrimaryBackgroundColourLight;

    public static Color SecondaryBackgroundColour => DarkMode ? SecondaryBackgroundColourDark : SecondaryBackgroundColourLight;

    public static Color PrimaryTextColour => DarkMode ? PrimaryTextColourDark : PrimaryTextColourLight;

    public static Color SecondaryTextColour => DarkMode ? SecondaryTextColourDark : SecondaryTextColourLight;

    public static Color TertiaryTextColor => DarkMode ? TertiaryTextColourDark : TertiaryTextColourLight;

    public static Color PrimaryControlColour => DarkMode ? PrimaryControlColourDark : PrimaryControlColourLight;

    public static Color SecondaryControlColour => DarkMode ? SecondaryControlColourDark : SecondaryControlColourLight;

    // Mouse over
    public static Color PrimaryControlMouseOverColour => DarkMode ? PrimaryControlMouseOverColourDark : PrimaryControlMouseOverColourLight;

    public static Color SecondaryControlMouseOverColour => DarkMode ? SecondaryControlMouseOverColourDark : SecondaryControlMouseOverColourLight;

    // Mouse down
    public static Color PrimaryControlMouseDownColour => DarkMode ? PrimaryControlMouseDownColourDark : PrimaryControlMouseDownColourLight;

    public static Color SecondaryControlMouseDownColour => DarkMode ? SecondaryControlMouseDownColourDark : SecondaryControlMouseDownColourLight;

    // Borders
    public static Color PrimaryControlBorderColour => DarkMode ? PrimaryControlBorderColourDark : PrimaryControlBorderColourLight;

    public static Color SecondaryControlBorderColour => DarkMode ? SecondaryControlBorderColourDark : SecondaryControlBorderColourLight;

    // Disabled
    public static Color PrimaryControlDisabledColour => DarkMode ? PrimaryControlDisabledColourDark : PrimaryControlDisabledColourLight;

    public static Color SecondaryControlDisabledColour => DarkMode ? SecondaryControlDisabledColourDark : SecondaryControlDisabledColourLight;

    private static Color MonoColour(byte value) => Color.FromArgb(255, value, value, value);
}
