using CoreUtilities.Interfaces;
using CoreUtilities.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace Win10Themables.ViewModels
{
	public class ThemingControlViewModel : ObservableObject
	{
		[DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
		public static extern bool ShouldSystemUseDarkMode();

		private const string lightModeKey = "Light";
		private const string darkModeKey = "Dark";

		private const string ColourModeSettingName = "ColourMode";
		private const string OsSyncSettingName = "ThemeOsSync";
		private const string ThemeSettingName = "Theme";

		private bool? wasDarkBeforeSync;
		private byte[] themeBeforeSync;

		private System.Timers.Timer osThemePollTimer = new System.Timers.Timer(1000);

		private byte r;
		public byte R
		{
			get => r;
			set
			{
				this.SetProperty(ref r, value);
				SetAccentColour(255, R, G, B, false);
			}
		}

		private byte g;
		public byte G
		{
			get => g;
			set
			{
				this.SetProperty(ref g, value);
				SetAccentColour(255, R, G, B, false);
			}
		}

		private byte b;
		public byte B
		{
			get => b;
			set
			{
				this.SetProperty(ref b, value);
				SetAccentColour(255, R, G, B, false);
			}
		}

		private bool isSyncingWithOs;
		public bool IsSyncingWithOs
		{
			get => isSyncingWithOs;
			set
			{
				SetProperty(ref isSyncingWithOs, value);
				SyncThemeWithOs(value);
			}
		}

		private IRegistryService registryService;

		private bool isDarkMode;
		public bool IsDarkMode
		{
			get => isDarkMode;
			set
			{
				SetProperty(ref isDarkMode, value);
				if (isDarkMode)
					SetDarkModeColours();
				else
					SetLightModeColours();
			}
		}

		#region Colour stores
		// Background
		private static readonly Color MainBackgroundColourLight = Colors.White;
		private static readonly Color MainBackgroundColourDark = Color.FromArgb(255, 20, 20, 20);

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
		private static readonly Color StatusTextLightColourLight = MonoColour(225);
		private static readonly Color StatusTextLightColourDark = MonoColour(30);

		// Control clickable part colour
		private static readonly Color ControlClickablePartColourLight = MonoColour(150);
		private static readonly Color ControlClickablePartColourDark = MonoColour(140);
		// Mouse over
		private static readonly Color ControlClickablePartMouseOverColourLight = MonoColour(140);
		private static readonly Color ControlClickablePartMouseOverColourDark = MonoColour(150);
		// Mouse down
		private static readonly Color ControlClickablePartMouseDownColourLight = MonoColour(190);
		private static readonly Color ControlClickablePartMouseDownColourDark = MonoColour(100);

		// Control non clickable part colour
		private static readonly Color ControlNonClickablePartColourLight = MonoColour(200);
		private static readonly Color ControlNonClickablePartColourDark = MonoColour(80);

		// Disabled control clickable part colour
		private static readonly Color DisabledControlClickablePartColourLight = MonoColour(230);
		private static readonly Color DisabledControlClickablePartColourDark = MonoColour(40);

		// Disabled control non clickable part colour
		private static readonly Color DisabledControlNonClickablePartColourLight = MonoColour(210);
		private static readonly Color DisabledControlNonClickablePartColourDark = MonoColour(60);

		// Datagrid header
		private static readonly Color DatagridHeaderColourLight = MonoColour(237);
		private static readonly Color DatagridHeaderColourDark = MonoColour(85);

		// Datagrid row
		private static readonly Color DatagridRowColourLight = Colors.White;
		private static readonly Color DatagridRowColourDark = MonoColour(30);

		// Theme colour
		private static Color ThemeColour = Color.FromArgb(255, 47, 47, 74);

		// Theme mouse over colour
		private static Color ThemeMouseOverColour = Color.FromArgb(255, 30, 134, 204);

		// Theme click colour
		private static Color ThemeClickColour = Color.FromArgb(255, 0, 103, 173);

		// Theme click colour
		private static Color ThemeBackgroundColour = Color.FromArgb(255, 129, 172, 202);

		// Theme text colour
		private static Color ThemeTextColour = Colors.White;

		// Theme status colour
		private static Color ThemeStatusColour = Colors.Gray;

		// Theme disabled text colour
		private static readonly Color ThemeDisabledTextColourLight = Colors.White;
		private static readonly Color ThemeDisabledTextColourDark = Colors.Gray;

		#endregion

		public ThemingControlViewModel()
		{
			this.registryService = new RegistryService();

			registryService.TryGetSetting(ColourModeSettingName, lightModeKey, out string? mode);
			IsDarkMode = mode == darkModeKey;

			registryService.TryGetSetting(ThemeSettingName, $"{ThemeColour.A}-{ThemeColour.R}-{ThemeColour.G}-{ThemeColour.B}", out string? theme);
			var accent = theme?.Split('-').Select(byte.Parse).ToList();
			SetAccentColour(accent[0], accent[1], accent[2], accent[3]);

			registryService.TryGetSetting(OsSyncSettingName, false, out bool sync);
			IsSyncingWithOs = sync;

			osThemePollTimer.Elapsed += OsThemePollTimer_Elapsed;
			osThemePollTimer.AutoReset = true;

			Application.Current.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
		}

		private void SetLightModeColours()
		{
			registryService.SetSetting(ColourModeSettingName, lightModeKey);
			SetThemeBackground();

			Application.Current.Resources["MainBackgroundBrush"] = new SolidColorBrush(MainBackgroundColourLight);
			Application.Current.Resources["TextBrush"] = new SolidColorBrush(TextColourLight);
			Application.Current.Resources["TextColour"] = Color.FromArgb(TextColourLight.A, TextColourLight.R, TextColourLight.G, TextColourLight.B);
			Application.Current.Resources["StatusTextBrush"] = new SolidColorBrush(StatusTextColourLight);
			Application.Current.Resources["StatusTextLightBrush"] = new SolidColorBrush(StatusTextLightColourLight);
			Application.Current.Resources["InvertedTextBrush"] = new SolidColorBrush(InvertedTextColourLight);
			Application.Current.Resources["ThemeDisabledTextBrush"] = new SolidColorBrush(ThemeDisabledTextColourLight);
			Application.Current.Resources["DatagridHeaderBrush"] = new SolidColorBrush(DatagridHeaderColourLight);
			Application.Current.Resources["DatagridRowBrush"] = new SolidColorBrush(DatagridRowColourLight);
			Application.Current.Resources["DisabledControlClickablePartBrush"] = new SolidColorBrush(DisabledControlClickablePartColourLight);
			Application.Current.Resources["DisabledControlNonClickablePartBrush"] = new SolidColorBrush(DisabledControlNonClickablePartColourLight);
			Application.Current.Resources["ControlClickablePartBrush"] = new SolidColorBrush(ControlClickablePartColourLight);
			Application.Current.Resources["ControlClickablePartMouseOverBrush"] = new SolidColorBrush(ControlClickablePartMouseOverColourLight);
			Application.Current.Resources["ControlClickablePartMouseDownBrush"] = new SolidColorBrush(ControlClickablePartMouseDownColourLight);
			Application.Current.Resources["ControlNonClickablePartBrush"] = new SolidColorBrush(ControlNonClickablePartColourLight);
		}

		private void SetDarkModeColours()
		{
			registryService.SetSetting(ColourModeSettingName, darkModeKey);
			SetThemeBackground();

			Application.Current.Resources["MainBackgroundBrush"] = new SolidColorBrush(MainBackgroundColourDark);
			Application.Current.Resources["TextBrush"] = new SolidColorBrush(TextColourDark);
			Application.Current.Resources["TextColour"] = Color.FromArgb(TextColourDark.A, TextColourDark.R, TextColourDark.G, TextColourDark.B);
			Application.Current.Resources["StatusTextBrush"] = new SolidColorBrush(StatusTextColourDark);
			Application.Current.Resources["StatusTextLightBrush"] = new SolidColorBrush(StatusTextLightColourDark);
			Application.Current.Resources["InvertedTextBrush"] = new SolidColorBrush(InvertedTextColourDark);
			Application.Current.Resources["ThemeDisabledTextBrush"] = new SolidColorBrush(ThemeDisabledTextColourDark);
			Application.Current.Resources["DatagridHeaderBrush"] = new SolidColorBrush(DatagridHeaderColourDark);
			Application.Current.Resources["DatagridRowBrush"] = new SolidColorBrush(DatagridRowColourDark);
			Application.Current.Resources["DisabledControlClickablePartBrush"] = new SolidColorBrush(DisabledControlClickablePartColourDark);
			Application.Current.Resources["DisabledControlNonClickablePartBrush"] = new SolidColorBrush(DisabledControlNonClickablePartColourDark);
			Application.Current.Resources["ControlClickablePartBrush"] = new SolidColorBrush(ControlClickablePartColourDark);
			Application.Current.Resources["ControlClickablePartMouseOverBrush"] = new SolidColorBrush(ControlClickablePartMouseOverColourDark);
			Application.Current.Resources["ControlClickablePartMouseDownBrush"] = new SolidColorBrush(ControlClickablePartMouseDownColourDark);
			Application.Current.Resources["ControlNonClickablePartBrush"] = new SolidColorBrush(ControlNonClickablePartColourDark);
		}

		private void SyncThemeWithOs(bool doSync)
		{
			registryService.SetSetting(OsSyncSettingName, doSync.ToString());
			if (doSync)
			{
				if (!osThemePollTimer.Enabled)
				{
					wasDarkBeforeSync = isDarkMode;
					themeBeforeSync = new byte[] { 255, R, G, B };
					osThemePollTimer.Start();
				}
				IsDarkMode = ShouldSystemUseDarkMode();
				var colour = (SystemParameters.WindowGlassBrush as SolidColorBrush).Color;
				SetAccentColour(255, colour.R, colour.G, colour.B);
			}
			else
			{
				if (wasDarkBeforeSync != null)
					IsDarkMode = wasDarkBeforeSync.Value;
				if (themeBeforeSync != null)
					SetAccentColour(255, themeBeforeSync[1], themeBeforeSync[2], themeBeforeSync[3]);
				if (osThemePollTimer.Enabled)
					osThemePollTimer.Stop();
			}
		}

		private void SetAccentColour(byte A, byte R, byte G, byte B, bool setComponents = true)
		{
			if (setComponents)
			{
				this.R = R;
				this.G = G;
				this.B = B;
			}

			registryService.SetSetting(ThemeSettingName, $"{A}-{R}-{G}-{B}");

			ThemeColour = Color.FromArgb(A, R, G, B);
			ThemeMouseOverColour = ChangeColorBrightness(ThemeColour, 0.1f);
			ThemeClickColour = ChangeColorBrightness(ThemeColour, -0.1f);
			SetThemeBackground();

			double perceivedBrightness = Math.Sqrt(0.299 * Math.Pow(R, 2) + 0.587 * Math.Pow(G, 2) + 0.114 * Math.Pow(B, 2));
			bool isThemeDark = perceivedBrightness < 255 * 0.5;

			ThemeTextColour = isThemeDark ? Colors.White : Colors.Black;
			ThemeStatusColour = isThemeDark ? Colors.LightGray : Colors.DimGray;

			Application.Current.Resources["ThemeBrush"] = new SolidColorBrush(ThemeColour);
			Application.Current.Resources["ThemeMouseOverBrush"] = new SolidColorBrush(ThemeMouseOverColour);
			Application.Current.Resources["ThemeClickBrush"] = new SolidColorBrush(ThemeClickColour);
			Application.Current.Resources["ThemeBackgroundBrush"] = new SolidColorBrush(ThemeBackgroundColour);
			Application.Current.Resources["ThemeTextBrush"] = new SolidColorBrush(ThemeTextColour);
			Application.Current.Resources["ThemeStatusBrush"] = new SolidColorBrush(ThemeStatusColour);
		}

		private void SetThemeBackground()
		{
			float modifier = isDarkMode ? 0.2f : -0.2f;
			ThemeBackgroundColour = ChangeColorBrightness(ThemeColour, modifier);
			Application.Current.Resources["ThemeBackgroundBrush"] = new SolidColorBrush(ThemeBackgroundColour);
		}

		public Color ChangeColorBrightness(Color color, float correctionFactor)
		{
			float red = (float)color.R;
			float green = (float)color.G;
			float blue = (float)color.B;

			if (correctionFactor < 0)
			{
				correctionFactor = 1 + correctionFactor;
				red *= correctionFactor;
				green *= correctionFactor;
				blue *= correctionFactor;
			}
			else
			{
				red = (255 - red) * correctionFactor + red;
				green = (255 - green) * correctionFactor + green;
				blue = (255 - blue) * correctionFactor + blue;
			}

			return Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
		}

		private static Color MonoColour(byte value)
		{
			return Color.FromArgb(255, value, value, value);
		}

		private void OsThemePollTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
		{
			if (isSyncingWithOs)
				SyncThemeWithOs(true);
		}

		private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
		{
			osThemePollTimer.Elapsed -= OsThemePollTimer_Elapsed;
			osThemePollTimer.Stop();
		}
	}
}
