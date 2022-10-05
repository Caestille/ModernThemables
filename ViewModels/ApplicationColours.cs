using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Windows.Media;
using ModernThemables.Extensions;

namespace ModernThemables.ViewModels
{
	public partial class ThemingControlViewModel : ObservableObject
	{
		// Background
		private static readonly Color MainBackgroundColourLight = ColourHelpers.MonoColour(255);
		private static readonly Color MainBackgroundColourDark = ColourHelpers.MonoColour(10);

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
		private static readonly Color StatusTextLightColourLight = ColourHelpers.MonoColour(215);
		private static readonly Color StatusTextLightColourDark = ColourHelpers.MonoColour(40);

		// Menu colour
		private static readonly Color MenuColourLight = ColourHelpers.MonoColour(225);
		private static readonly Color MenuColourDark = ColourHelpers.MonoColour(22);

		// Menu outline
		private static readonly Color MenuBorderColourLight = ColourHelpers.MonoColour(218);
		private static readonly Color MenuBorderColourDark = ColourHelpers.MonoColour(37);

		// Menu colour
		private static readonly Color MenuMouseOverColourLight = ColourHelpers.MonoColour(195);
		private static readonly Color MenuMouseOverColourDark = ColourHelpers.MonoColour(60);

		// Menu colour
		private static readonly Color MenuMouseDownColourLight = ColourHelpers.MonoColour(215);
		private static readonly Color MenuMouseDownColourDark = ColourHelpers.MonoColour(40);

		// Control clickable part colour
		private static readonly Color ControlClickablePartColourLight = ColourHelpers.MonoColour(195);
		private static readonly Color ControlClickablePartColourDark = ColourHelpers.MonoColour(60);
		// Mouse over
		private static readonly Color ControlClickablePartMouseOverColourLight = ColourHelpers.MonoColour(215);
		private static readonly Color ControlClickablePartMouseOverColourDark = ColourHelpers.MonoColour(80);
		// Mouse down
		private static readonly Color ControlClickablePartMouseDownColourLight = ColourHelpers.MonoColour(185);
		private static readonly Color ControlClickablePartMouseDownColourDark = ColourHelpers.MonoColour(50);

		// Control non clickable part colour
		private static readonly Color ControlNonClickablePartColourLight = ColourHelpers.MonoColour(235);
		private static readonly Color ControlNonClickablePartColourDark = ColourHelpers.MonoColour(20);

		// Disabled control clickable part colour
		private static readonly Color DisabledControlClickablePartColourLight = ColourHelpers.MonoColour(215);
		private static readonly Color DisabledControlClickablePartColourDark = ColourHelpers.MonoColour(40);

		// Disabled control non clickable part colour
		private static readonly Color DisabledControlNonClickablePartColourLight = ColourHelpers.MonoColour(245);
		private static readonly Color DisabledControlNonClickablePartColourDark = ColourHelpers.MonoColour(10);

		// Button foreground
		private static readonly Color ButtonForegroundColourLight = ColourHelpers.MonoColour(130);
		private static readonly Color ButtonForegroundColourDark = ColourHelpers.MonoColour(125);

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
