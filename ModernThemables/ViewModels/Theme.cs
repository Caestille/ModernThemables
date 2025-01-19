namespace ModernThemables.ViewModels;

using System.Windows.Media;

public class Theme
{
    public bool IsSyncingWithOs { get; set; } = false;

    public bool IsDarkMode { get; set; } = false;

    public Color ThemeColour { get; set; } = Color.FromArgb(255, 47, 47, 74);

    public bool IsTransparentHeader { get; set; } = false;
}
