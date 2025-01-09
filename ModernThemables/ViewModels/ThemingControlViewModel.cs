using CommunityToolkit.Mvvm.ComponentModel;
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
using CommunityToolkit.Mvvm.Input;
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
				SetBrightnessMode();
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

        private void SetBrightnessMode()
		{
			registryService.SetSetting(ColourModeSettingName, isDarkMode ? darkModeKey : lightModeKey);

			Application.Current.Resources[nameof(PrimaryBackgroundColourLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(isDarkMode ? PrimaryBackgroundColourDark : PrimaryBackgroundColourLight);
			Application.Current.Resources[nameof(SecondaryBackgroundColourLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(isDarkMode ? SecondaryBackgroundColourDark : SecondaryBackgroundColourLight);
			Application.Current.Resources[nameof(PrimaryTextColourLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(isDarkMode ? PrimaryTextColourDark : PrimaryTextColourLight);
			Application.Current.Resources[nameof(SecondaryTextColourLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(isDarkMode ? SecondaryTextColourDark : SecondaryTextColourLight);
			Application.Current.Resources[nameof(TertiaryTextColorLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(isDarkMode ? TertiaryTextColourDark : TertiaryTextColorLight);
			Application.Current.Resources[nameof(PrimaryControlColourLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(isDarkMode ? PrimaryControlColourDark : PrimaryControlColourLight);
			Application.Current.Resources[nameof(SecondaryControlColourLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(isDarkMode ? SecondaryControlColourDark : SecondaryControlColourLight);
			Application.Current.Resources[nameof(PrimaryControlMouseOverBrushLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(isDarkMode ? PrimaryControlMouseOverBrushDark : PrimaryControlMouseOverBrushLight);
			Application.Current.Resources[nameof(SecondaryControlMouseOverBrushLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(isDarkMode ? SecondaryControlMouseOverBrushDark : SecondaryControlMouseOverBrushLight);
			Application.Current.Resources[nameof(PrimaryControlMouseDownBrushLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(isDarkMode ? PrimaryControlMouseDownBrushDark : PrimaryControlMouseDownBrushLight);
			Application.Current.Resources[nameof(SecondaryControlMouseDownBrushLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(isDarkMode ? SecondaryControlMouseDownBrushDark : SecondaryControlMouseDownBrushLight);
			Application.Current.Resources[nameof(PrimaryControlBorderColourLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(isDarkMode ? PrimaryControlBorderColourDark : PrimaryControlBorderColourLight);
			Application.Current.Resources[nameof(SecondaryControlBorderColourLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(isDarkMode ? SecondaryControlBorderColourDark : SecondaryControlBorderColourLight);
			Application.Current.Resources[nameof(PrimaryControlDisabledColourLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(isDarkMode ? PrimaryControlDisabledColourDark : PrimaryControlDisabledColourLight);
			Application.Current.Resources[nameof(SecondaryControlDisabledColourLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(isDarkMode ? SecondaryControlDisabledColourDark : SecondaryControlDisabledColourLight);
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

			var isThemeDark = ThemeColour.PerceivedBrightness() < 0.5;
			ThemeTextColour = isThemeDark ? Colors.White : Colors.Black;

			ThemeMouseOverBrush = ThemeColour.ChangeColourBrightness(0.3f);

			ThemeMouseDownBrush = ThemeColour.ChangeColourBrightness(-0.2f);

			ThemeBorderColour = ThemeColour.ChangeColourBrightness(isDarkMode ? -0.2f : 0.3f);

			ThemeDisabledColour = ThemeColour
				.ChangeColourBrightness(isDarkMode ? -0.2f : 0.3f)
				.Combine(Colors.Gray, 0.4);

			Application.Current.Resources["ThemeBrush"] = new SolidColorBrush(ThemeColour);
			Application.Current.Resources["ThemeTextBrush"] = new SolidColorBrush(ThemeTextColour);
			Application.Current.Resources["ThemeMouseOverBrush"] = new SolidColorBrush(ThemeMouseOverBrush);
			Application.Current.Resources["ThemeMouseDownBrush"] = new SolidColorBrush(ThemeMouseDownBrush);
			Application.Current.Resources["ThemeBorderBrush"] = new SolidColorBrush(ThemeMouseOverBrush);
			Application.Current.Resources["ThemeDisabledBrush"] = new SolidColorBrush(ThemeMouseDownBrush);
		}

		private void OsThemePollTimer_Elapsed(object? sender, ElapsedEventArgs e)
		{
			if (Application.Current == null) return;

			if (isSyncingWithOs)
				SyncThemeWithOs(true);
		}

		private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
		{
			Dispose();
		}
	}
}
