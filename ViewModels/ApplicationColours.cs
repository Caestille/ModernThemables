using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Windows.Media;

namespace ModernThemables.ViewModels
{
	public partial class ThemingControlViewModel : ObservableObject
	{
		// Background
		private static readonly Color MainBackgroundColourLight = MonoColour(255);
		private static readonly Color MainBackgroundColourDark = MonoColour(10);

		// Main text colour
		private static readonly Color TextColourLight = Colors.Black;
		private static readonly Color TextColourDark = Colors.White;

		// Inverted text colour
		private static readonly Color InvertedTextColourLight = Colors.White;
		private static readonly Color InvertedTextColourDark = Colors.Black;

		// Status text colour
		private static readonly Color StatusTextColourLight = Colors.Gray;
		private static readonly Color StatusTextColourDark = Colors.DarkGray;

		// Status text light colour
		private static readonly Color StatusTextLightColourLight = MonoColour(215);
		private static readonly Color StatusTextLightColourDark = MonoColour(40);

		// Menu colour
		private static readonly Color MenuColourLight = MonoColour(225);
		private static readonly Color MenuColourDark = MonoColour(22);

		// Menu outline
		private static readonly Color MenuBorderColourLight = MonoColour(218);
		private static readonly Color MenuBorderColourDark = MonoColour(37);

		// Menu colour
		private static readonly Color MenuMouseOverColourLight = MonoColour(195);
		private static readonly Color MenuMouseOverColourDark = MonoColour(60);

		// Menu colour
		private static readonly Color MenuMouseDownColourLight = MonoColour(215);
		private static readonly Color MenuMouseDownColourDark = MonoColour(40);

		// Control clickable part colour
		private static readonly Color ControlClickablePartColourLight = MonoColour(195);
		private static readonly Color ControlClickablePartColourDark = MonoColour(60);
		// Mouse over
		private static readonly Color ControlClickablePartMouseOverColourLight = MonoColour(215);
		private static readonly Color ControlClickablePartMouseOverColourDark = MonoColour(80);
		// Mouse down
		private static readonly Color ControlClickablePartMouseDownColourLight = MonoColour(185);
		private static readonly Color ControlClickablePartMouseDownColourDark = MonoColour(50);

		// Control non clickable part colour
		private static readonly Color ControlNonClickablePartColourLight = MonoColour(235);
		private static readonly Color ControlNonClickablePartColourDark = MonoColour(20);

		// Disabled control clickable part colour
		private static readonly Color DisabledControlClickablePartColourLight = MonoColour(215);
		private static readonly Color DisabledControlClickablePartColourDark = MonoColour(40);

		// Disabled control non clickable part colour
		private static readonly Color DisabledControlNonClickablePartColourLight = MonoColour(245);
		private static readonly Color DisabledControlNonClickablePartColourDark = MonoColour(10);

		// Button foreground
		private static readonly Color ButtonForegroundColourLight = MonoColour(130);
		private static readonly Color ButtonForegroundColourDark = MonoColour(125);

		// Theme colour
		private static Color ThemeColour = Color.FromArgb(255, 47, 47, 74);

		// Theme mouse over colour
		private static Color ThemeMouseOverColour = Color.FromArgb(255, 30, 134, 204);

		// Theme click colour
		private static Color ThemeMouseDownColour = Color.FromArgb(255, 0, 103, 173);

		// Theme click colour
		private static Color ThemeBackgroundColour = Color.FromArgb(255, 129, 172, 202);

		// Theme text colour
		private static Color ThemeTextColour = Colors.White;

		// Theme status colour
		private static Color ThemeStatusColour = Colors.Gray;

		// Theme disabled text colour
		private static Color ThemeDisabledTextColour = Colors.Gray;

		// Theme near background colour
		private static Color ThemeNearBackgroundColour = Colors.Gray;
		private static Color ThemeBackgroundNearBackgroundColour = Colors.Gray;
		private static Color ThemeMouseOverNearBackgroundColour = Colors.Gray;
		private static Color ThemeMouseDownNearBackgroundColour = Colors.Gray;
	}
}
