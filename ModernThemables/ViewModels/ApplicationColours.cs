namespace ModernThemables.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using System.Windows.Media;

    public partial class ThemingControlViewModel : ObservableObject
    {
        private static readonly Color PrimaryBackgroundColourLight = MonoColour(255);
		private static readonly Color PrimaryBackgroundColourDark = MonoColour(10);

		private static readonly Color SecondaryBackgroundColourLight = MonoColour(225);
		private static readonly Color SecondaryBackgroundColourDark = MonoColour(22);

		private static readonly Color PrimaryTextColourLight = Colors.Black;
		private static readonly Color PrimaryTextColourDark = Colors.White;

		private static readonly Color SecondaryTextColourLight = Colors.Gray;
		private static readonly Color SecondaryTextColourDark = Colors.DarkGray;

		private static readonly Color TertiaryTextColorLight = MonoColour(215);
		private static readonly Color TertiaryTextColourDark = MonoColour(40);

		private static readonly Color PrimaryControlColourLight = MonoColour(195);
		private static readonly Color PrimaryControlColourDark = MonoColour(60);

		private static readonly Color SecondaryControlColourLight = MonoColour(235);
		private static readonly Color SecondaryControlColourDark = MonoColour(20);

		private static Color ThemeColour = Color.FromArgb(255, 47, 47, 74);

		private static Color ThemeTextColour = Colors.White;

		// Mouse over
		private static readonly Color PrimaryControlMouseOverBrushLight = MonoColour(215);
		private static readonly Color PrimaryControlMouseOverBrushDark = MonoColour(80);

		private static readonly Color SecondaryControlMouseOverBrushLight = MonoColour(215);
		private static readonly Color SecondaryControlMouseOverBrushDark = MonoColour(80);

		private static Color ThemeMouseOverBrush = Color.FromArgb(255, 30, 134, 204);

		// Mouse down
		private static readonly Color PrimaryControlMouseDownBrushLight = MonoColour(185);
		private static readonly Color PrimaryControlMouseDownBrushDark = MonoColour(50);

		private static readonly Color SecondaryControlMouseDownBrushLight = MonoColour(185);
		private static readonly Color SecondaryControlMouseDownBrushDark = MonoColour(50);

		private static Color ThemeMouseDownBrush = Color.FromArgb(255, 0, 103, 173);

		// Borders
		private static readonly Color PrimaryControlBorderColourLight = MonoColour(185);
		private static readonly Color PrimaryControlBorderColourDark = MonoColour(50);

		private static readonly Color SecondaryControlBorderColourLight = MonoColour(185);
		private static readonly Color SecondaryControlBorderColourDark = MonoColour(50);

		private static Color ThemeBorderColour = Color.FromArgb(255, 0, 103, 173);

		// Disabled
		private static readonly Color PrimaryControlDisabledColourLight = MonoColour(215);
		private static readonly Color PrimaryControlDisabledColourDark = MonoColour(40);

		private static readonly Color SecondaryControlDisabledColourLight = MonoColour(245);
		private static readonly Color SecondaryControlDisabledColourDark = MonoColour(10);

		private static Color ThemeDisabledColour = Color.FromArgb(255, 0, 103, 173);

        private static Color MonoColour(byte value)
        {
            return Color.FromArgb(255, value, value, value);
        }
    }
}
