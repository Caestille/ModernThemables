using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using CoreUtilities.Interfaces.RegistryInteraction;
using CoreUtilities.Services.RegistryInteraction;
using System.Timers;
using CoreUtilities.HelperClasses.Extensions;
using CoreUtilities.Interfaces.Dialogues;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows.Input;
using ModernThemables.Services;

namespace ModernThemables.ViewModels
{
	/// <summary>
	/// A view model for a theming control to interact with the theme status of an application with.
	/// </summary>
	public partial class ThemingControlViewModel : ObservableObject, IDisposable
	{
		[DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
		public static extern bool ShouldSystemUseDarkMode();

		private const string lightModeKey = "Light";
		private const string darkModeKey = "Dark";

		private const string ColourModeSettingName = "ColourMode";
		private const string OsSyncSettingName = "ThemeOsSync";
		private const string ThemeSettingName = "Theme";
        private const string TransparentHeaderSettingName = "TransparentHeader";

        private bool? wasDarkBeforeSync;
		private Color? themeBeforeSync;

		private readonly IRegistryService registryService;
		private readonly IDialogueService dialogueService;

		private readonly Timer osThemePollTimer = new Timer(1000);

        public event EventHandler<bool>? TransparentHeaderChanged;
        public event EventHandler<bool>? IsDarkChanged;
        public event EventHandler<bool>? SyncWithOsChanged;

        public ICommand ChangeColourCommand => new RelayCommand(ChangeColour);

		private void ChangeColour()
		{
			ThemeColourProperty = dialogueService.ShowColourPickerDialogue(ThemeColourProperty, (colour) => ThemeColourProperty = colour);
		}

		/// <summary>
		/// Gets or sets the current Theme colour.
		/// </summary>
		public Color ThemeColourProperty
		{
			get => ThemeColour;
			set => SetThemeColour(value);
		}

		private bool isSyncingWithOs;
		/// <summary>
		/// Gets or sets whether the theme is being synchronised with the OS.
		/// </summary>
		public bool IsSyncingWithOs
		{
			get => isSyncingWithOs;
			set
			{
				SetProperty(ref isSyncingWithOs, value);
				SyncThemeWithOs(value);
                SyncWithOsChanged?.Invoke(this, value);
            }
		}

		private bool isDarkMode;
		/// <summary>
		/// Gets or sets whether the theme is Dark or Light.
		/// </summary>
		public bool IsDarkMode
		{
			get => isDarkMode;
			set
			{
				SetProperty(ref isDarkMode, value);
				SetThemeAndBrightnessMode();
                IsDarkChanged?.Invoke(this, value);
            }
        }

        private bool isTransparentHeader;
        /// <summary>
        /// Gets or sets whether the theme is being synchronised with the OS.
        /// </summary>
        public bool IsTransparentHeader
        {
            get => isTransparentHeader;
            set
            {
                SetProperty(ref isTransparentHeader, value);
                TransparentHeaderChanged?.Invoke(this, value);
                registryService.SetSetting(TransparentHeaderSettingName, value.ToString());
            }
        }

        /// <summary>
        /// Initialises a new <see cref="ThemingControlViewModel"/>.
        /// </summary>
        public ThemingControlViewModel()
        {
            registryService = new RegistryService(@"SOFTWARE\ThemableApps", true);
			dialogueService = new DialogueService();

			registryService.TryGetSetting(ColourModeSettingName, lightModeKey, out string? mode);
			IsDarkMode = mode == darkModeKey;

			var tempSetting = $"{ThemeColour.A}-{ThemeColour.R}-{ThemeColour.G}-{ThemeColour.B}";
			registryService.TryGetSetting(ThemeSettingName, tempSetting, out string? theme);
			if (!string.IsNullOrEmpty(theme))
			{
				var accent = theme.Split('-').Select(byte.Parse).ToList();
				SetThemeColour(Color.FromArgb(accent[0], accent[1], accent[2], accent[3]));
            }

            registryService.TryGetSetting(TransparentHeaderSettingName, "false", out string? transparent);
            if (!string.IsNullOrEmpty(transparent))
            {
                IsTransparentHeader = bool.Parse(transparent);
            }

            registryService.TryGetSetting(OsSyncSettingName, false, out bool sync);
			IsSyncingWithOs = sync;

            osThemePollTimer.Elapsed += OsThemePollTimer_Elapsed;
			osThemePollTimer.AutoReset = true;
            osThemePollTimer.Start();

            Application.Current.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
		}

		public void Dispose()
		{
			osThemePollTimer.Elapsed -= OsThemePollTimer_Elapsed;
			osThemePollTimer.Stop();
		}

		private void SetThemeAndBrightnessMode()
		{
			registryService.SetSetting(ColourModeSettingName, isDarkMode ? darkModeKey : lightModeKey);
			SetThemeBackground();

			Application.Current.Resources["MainBackgroundBrush"] = isDarkMode 
				? new SolidColorBrush(MainBackgroundColourDark) 
				: new SolidColorBrush(MainBackgroundColourLight);
			Application.Current.Resources["TextBrush"] = isDarkMode 
				? new SolidColorBrush(TextColourDark) 
				: new SolidColorBrush(TextColourLight);
			Application.Current.Resources["TextColour"] = isDarkMode 
				? Color.FromArgb(TextColourDark.A, TextColourDark.R, TextColourDark.G, TextColourDark.B) 
				: Color.FromArgb(TextColourLight.A, TextColourLight.R, TextColourLight.G, TextColourLight.B);
			Application.Current.Resources["InvertedTextColour"] = !isDarkMode
				? Color.FromArgb(TextColourDark.A, TextColourDark.R, TextColourDark.G, TextColourDark.B)
				: Color.FromArgb(TextColourLight.A, TextColourLight.R, TextColourLight.G, TextColourLight.B);
			Application.Current.Resources["InvertedTextBrush"] = isDarkMode
				? new SolidColorBrush(InvertedTextColourDark)
				: new SolidColorBrush(InvertedTextColourLight);
			Application.Current.Resources["StatusTextBrush"] = isDarkMode 
				? new SolidColorBrush(StatusTextColourDark) 
				: new SolidColorBrush(StatusTextColourLight);
			Application.Current.Resources["StatusTextLightBrush"] = isDarkMode 
				? new SolidColorBrush(StatusTextLightColourDark) 
				: new SolidColorBrush(StatusTextLightColourLight);
			Application.Current.Resources["MenuBrush"] = isDarkMode
				? new SolidColorBrush(MenuColourDark)
				: new SolidColorBrush(MenuColourLight);
			Application.Current.Resources["MenuMouseOverBrush"] = isDarkMode
				? new SolidColorBrush(MenuMouseOverColourDark)
				: new SolidColorBrush(MenuMouseOverColourLight);
			Application.Current.Resources["MenuMouseDownBrush"] = isDarkMode
				? new SolidColorBrush(MenuMouseDownColourDark)
				: new SolidColorBrush(MenuMouseDownColourLight);
			Application.Current.Resources["MenuBorderBrush"] = isDarkMode 
				? new SolidColorBrush(MenuBorderColourDark) 
				: new SolidColorBrush(MenuBorderColourLight);
			Application.Current.Resources["DisabledControlClickablePartBrush"] = isDarkMode
				? new SolidColorBrush(DisabledControlClickablePartColourDark) 
				: new SolidColorBrush(DisabledControlClickablePartColourLight);
			Application.Current.Resources["DisabledControlClickablePartColour"] = isDarkMode
				? Color.FromArgb(DisabledControlClickablePartColourDark.A, DisabledControlClickablePartColourDark.R, DisabledControlClickablePartColourDark.G, DisabledControlClickablePartColourDark.B)
				: Color.FromArgb(DisabledControlClickablePartColourLight.A, DisabledControlClickablePartColourLight.R, DisabledControlClickablePartColourLight.G, DisabledControlClickablePartColourLight.B);
			Application.Current.Resources["DisabledControlNonClickablePartBrush"] = isDarkMode 
				? new SolidColorBrush(DisabledControlNonClickablePartColourDark) 
				: new SolidColorBrush(DisabledControlNonClickablePartColourLight);
			Application.Current.Resources["ControlClickablePartBrush"] = isDarkMode 
				? new SolidColorBrush(ControlClickablePartColourDark) 
				: new SolidColorBrush(ControlClickablePartColourLight);
			Application.Current.Resources["ControlClickablePartMouseOverBrush"] = isDarkMode 
				? new SolidColorBrush(ControlClickablePartMouseOverColourDark) 
				: new SolidColorBrush(ControlClickablePartMouseOverColourLight);
			Application.Current.Resources["ControlClickablePartMouseDownBrush"] = isDarkMode 
				? new SolidColorBrush(ControlClickablePartMouseDownColourDark)
				: new SolidColorBrush(ControlClickablePartMouseDownColourLight);
			Application.Current.Resources["ControlNonClickablePartBrush"] = isDarkMode 
				? new SolidColorBrush(ControlNonClickablePartColourDark) 
				: new SolidColorBrush(ControlNonClickablePartColourLight);
			Application.Current.Resources["ButtonForegroundBrush"] = isDarkMode
				? new SolidColorBrush(ButtonForegroundColourDark)
				: new SolidColorBrush(ButtonForegroundColourLight);
		}

		private async void SyncThemeWithOs(bool doSync)
		{
			await Task.Run(() =>
			{
				registryService.SetSetting(OsSyncSettingName, doSync.ToString());
				if (doSync)
				{
					if (!osThemePollTimer.Enabled)
					{
						wasDarkBeforeSync = isDarkMode;
						themeBeforeSync = ThemeColour;
					}
					var shouldBeDark = ShouldSystemUseDarkMode();
					if (shouldBeDark != isDarkMode)
						IsDarkMode = shouldBeDark;

					var colour = (SystemParameters.WindowGlassBrush as SolidColorBrush)?.Color;
					if (colour.HasValue && ThemeColour != colour.Value)
						SetThemeColour(colour.Value);
				}
				else
				{
					if (wasDarkBeforeSync != null)
						IsDarkMode = wasDarkBeforeSync.Value;
					if (themeBeforeSync != null)
						SetThemeColour(themeBeforeSync.Value);
				}
			});
		}

		private void SetThemeColour(Color colour)
		{
			registryService.SetSetting(ThemeSettingName, $"{colour.A}-{colour.R}-{colour.G}-{colour.B}");

			ThemeColour = colour;
			OnPropertyChanged(nameof(ThemeColourProperty));
			SetThemeBackground();

			var isThemeDark = ThemeColour.PerceivedBrightness() < 0.5;

			ThemeTextColour = isThemeDark ? Colors.White : Colors.Black;
			ThemeStatusColour = isThemeDark ? Colors.LightGray : Colors.DimGray;

			var desiredBrightness = Math.Min(
				Math.Max(
					ThemeBackgroundColour.PerceivedBrightness() + (isThemeDark ? 0.15 : -0.15),
					0),
				1);
			ThemeDisabledTextColour = Colors.Black;
			var themeDisabledTextPerceivedBrightness = ThemeDisabledTextColour.PerceivedBrightness();
			var count = 0;
			while (Math.Abs(themeDisabledTextPerceivedBrightness - desiredBrightness) > 0.01 && count < 200)
			{
				ThemeDisabledTextColour.ChangeThisColourBrightness(0.01f);
				themeDisabledTextPerceivedBrightness = ThemeDisabledTextColour.PerceivedBrightness();
				count++;
			}

			Application.Current.Resources["ThemeBrush"] = new SolidColorBrush(ThemeColour);
			Application.Current.Resources["ThemeBackgroundBrush"] = new SolidColorBrush(ThemeBackgroundColour);
			Application.Current.Resources["ThemeTextBrush"] = new SolidColorBrush(ThemeTextColour);
			Application.Current.Resources["ThemeStatusBrush"] = new SolidColorBrush(ThemeStatusColour);
			Application.Current.Resources["ThemeDisabledTextBrush"] = new SolidColorBrush(ThemeDisabledTextColour);
		}

		private void SetThemeBackground()
		{
			float modifier = isDarkMode ? -0.2f : 0.2f;
			float perceivedBrightnessFactor = (((float)ThemeColour.PerceivedBrightness() + 1f) / 1f) * 1f;

			ThemeBackgroundColour = ThemeColour.ChangeColourBrightness(modifier * perceivedBrightnessFactor);
			ThemeMouseOverColour = ThemeColour.ChangeColourBrightness(0.1f * perceivedBrightnessFactor);
			ThemeMouseDownColour = ThemeColour.ChangeColourBrightness(-0.1f * perceivedBrightnessFactor);
			var compare = isDarkMode ? ThemeBackgroundColour : ThemeMouseOverColour;
			var background = isDarkMode ? MainBackgroundColourDark : MainBackgroundColourLight;
			var threshold = isDarkMode ? 0.03 : 0.03;
			var diffFactor = isDarkMode ? -1 : 1;
			var modifierFactor = -modifier / (isDarkMode ? 1 : 2) * perceivedBrightnessFactor;
			ThemeNearBackgroundColour.AdjustBrightnessIfNearColour(
				compare, background, ThemeColour, diffFactor, threshold, modifierFactor);
			ThemeBackgroundNearBackgroundColour.AdjustBrightnessIfNearColour(
				compare, background, ThemeBackgroundColour, diffFactor, threshold, modifierFactor);
			ThemeMouseOverNearBackgroundColour.AdjustBrightnessIfNearColour(
				compare, background, ThemeMouseOverColour, diffFactor, threshold, modifierFactor);
			ThemeMouseDownNearBackgroundColour.AdjustBrightnessIfNearColour(
				compare, background, ThemeMouseDownColour, diffFactor, threshold, modifierFactor);

			Application.Current.Resources["ThemeBackgroundBrush"] = new SolidColorBrush(ThemeBackgroundColour);
			Application.Current.Resources["ThemeMouseOverBrush"] = new SolidColorBrush(ThemeMouseOverColour);
			Application.Current.Resources["ThemeMouseOverColour"] = ThemeMouseOverColour;
			Application.Current.Resources["ThemeMouseDownBrush"] = new SolidColorBrush(ThemeMouseDownColour);
			Application.Current.Resources["ThemeNearBackgroundBrush"] = new SolidColorBrush(ThemeNearBackgroundColour);
			Application.Current.Resources["ThemeBackgroundNearBackgroundBrush"]
				= new SolidColorBrush(ThemeBackgroundNearBackgroundColour);
			Application.Current.Resources["ThemeMouseOverNearBackgroundBrush"]
				= new SolidColorBrush(ThemeMouseOverNearBackgroundColour);
			Application.Current.Resources["ThemeMouseDownNearBackgroundBrush"]
				= new SolidColorBrush(ThemeMouseDownNearBackgroundColour);
		}

		private void OsThemePollTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
		{
			if (isSyncingWithOs)
				SyncThemeWithOs(true);
			if (Application.Current == null) return;
        }

		private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
		{
			Dispose();
		}

		private static Color MonoColour(byte value)
		{
			return Color.FromArgb(255, value, value, value);
		}
	}
}
