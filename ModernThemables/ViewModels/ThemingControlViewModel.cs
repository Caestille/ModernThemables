namespace ModernThemables.ViewModels
{
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

		public ICommand ChangeColourCommand => new RelayCommand(this.ChangeColour);

		private void ChangeColour()
		{
            this.ThemeColourProperty = this.dialogueService.ShowColourPickerDialogue(this.ThemeColourProperty, (colour) => this.ThemeColourProperty = colour);
		}

		/// <summary>
		/// Gets or sets the current Theme colour.
		/// </summary>
		public Color ThemeColourProperty
		{
			get => ThemeColour;
			set => this.SetThemeColour(value);
		}

		private bool isSyncingWithOs;
		/// <summary>
		/// Gets or sets whether the theme is being synchronised with the OS.
		/// </summary>
		public bool IsSyncingWithOs
		{
			get => this.isSyncingWithOs;
			set
			{
                this.SetProperty(ref this.isSyncingWithOs, value);
                this.SyncThemeWithOs(value);
				SyncWithOsChanged?.Invoke(this, value);
			}
		}

		private bool isDarkMode;
		/// <summary>
		/// Gets or sets whether the theme is Dark or Light.
		/// </summary>
		public bool IsDarkMode
		{
			get => this.isDarkMode;
			set
			{
                this.SetProperty(ref this.isDarkMode, value);
                this.SetBrightnessMode();
				IsDarkChanged?.Invoke(this, value);
			}
		}

		private bool isTransparentHeader;
		/// <summary>
		/// Gets or sets whether the theme is being synchronised with the OS.
		/// </summary>
		public bool IsTransparentHeader
		{
			get => this.isTransparentHeader;
			set
			{
                this.SetProperty(ref this.isTransparentHeader, value);
				TransparentHeaderChanged?.Invoke(this, value);
                this.registryService.SetSetting(TransparentHeaderSettingName, value.ToString());
			}
		}

		/// <summary>
		/// Initialises a new <see cref="ThemingControlViewModel"/>.
		/// </summary>
		public ThemingControlViewModel()
		{
            this.registryService = new RegistryService(@"SOFTWARE\ThemableApps", true);
            this.dialogueService = new DialogueService();

            this.registryService.TryGetSetting(ColourModeSettingName, lightModeKey, out string? mode);
            this.IsDarkMode = mode == darkModeKey;

			var tempSetting = $"{ThemeColour.A}-{ThemeColour.R}-{ThemeColour.G}-{ThemeColour.B}";
            this.registryService.TryGetSetting(ThemeSettingName, tempSetting, out string? theme);
			if (!string.IsNullOrEmpty(theme))
			{
				var accent = theme.Split('-').Select(byte.Parse).ToList();
                this.SetThemeColour(Color.FromArgb(accent[0], accent[1], accent[2], accent[3]));
			}

            this.registryService.TryGetSetting(TransparentHeaderSettingName, "false", out string? transparent);
			if (!string.IsNullOrEmpty(transparent))
			{
                this.IsTransparentHeader = bool.Parse(transparent);
			}

            this.registryService.TryGetSetting(OsSyncSettingName, false, out bool sync);
            this.IsSyncingWithOs = sync;

            this.osThemePollTimer.Elapsed += this.OsThemePollTimer_Elapsed;
            this.osThemePollTimer.AutoReset = true;
            this.osThemePollTimer.Start();

			Application.Current.Dispatcher.ShutdownStarted += this.Dispatcher_ShutdownStarted;
		}

		public void Dispose()
		{
            this.osThemePollTimer.Elapsed -= this.OsThemePollTimer_Elapsed;
            this.osThemePollTimer.Stop();
        }

        private void SetBrightnessMode()
		{
            this.registryService.SetSetting(ColourModeSettingName, this.isDarkMode ? darkModeKey : lightModeKey);

			Application.Current.Resources[nameof(PrimaryBackgroundColourLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(this.isDarkMode ? PrimaryBackgroundColourDark : PrimaryBackgroundColourLight);
			Application.Current.Resources[nameof(SecondaryBackgroundColourLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(this.isDarkMode ? SecondaryBackgroundColourDark : SecondaryBackgroundColourLight);
			Application.Current.Resources[nameof(PrimaryTextColourLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(this.isDarkMode ? PrimaryTextColourDark : PrimaryTextColourLight);
			Application.Current.Resources[nameof(SecondaryTextColourLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(this.isDarkMode ? SecondaryTextColourDark : SecondaryTextColourLight);
			Application.Current.Resources[nameof(TertiaryTextColorLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(this.isDarkMode ? TertiaryTextColourDark : TertiaryTextColorLight);
			Application.Current.Resources[nameof(PrimaryControlColourLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(this.isDarkMode ? PrimaryControlColourDark : PrimaryControlColourLight);
			Application.Current.Resources[nameof(SecondaryControlColourLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(this.isDarkMode ? SecondaryControlColourDark : SecondaryControlColourLight);
			Application.Current.Resources[nameof(PrimaryControlMouseOverBrushLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(this.isDarkMode ? PrimaryControlMouseOverBrushDark : PrimaryControlMouseOverBrushLight);
			Application.Current.Resources[nameof(SecondaryControlMouseOverBrushLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(this.isDarkMode ? SecondaryControlMouseOverBrushDark : SecondaryControlMouseOverBrushLight);
			Application.Current.Resources[nameof(PrimaryControlMouseDownBrushLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(this.isDarkMode ? PrimaryControlMouseDownBrushDark : PrimaryControlMouseDownBrushLight);
			Application.Current.Resources[nameof(SecondaryControlMouseDownBrushLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(this.isDarkMode ? SecondaryControlMouseDownBrushDark : SecondaryControlMouseDownBrushLight);
			Application.Current.Resources[nameof(PrimaryControlBorderColourLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(this.isDarkMode ? PrimaryControlBorderColourDark : PrimaryControlBorderColourLight);
			Application.Current.Resources[nameof(SecondaryControlBorderColourLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(this.isDarkMode ? SecondaryControlBorderColourDark : SecondaryControlBorderColourLight);
			Application.Current.Resources[nameof(PrimaryControlDisabledColourLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(this.isDarkMode ? PrimaryControlDisabledColourDark : PrimaryControlDisabledColourLight);
			Application.Current.Resources[nameof(SecondaryControlDisabledColourLight).Replace("Colour", "Brush").Replace("Light", "")]
				= new SolidColorBrush(this.isDarkMode ? SecondaryControlDisabledColourDark : SecondaryControlDisabledColourLight);
		}

		private async void SyncThemeWithOs(bool doSync)
		{
			await Task.Run(() =>
			{
                this.registryService.SetSetting(OsSyncSettingName, doSync.ToString());
				if (doSync)
				{
					if (!this.osThemePollTimer.Enabled)
					{
                        this.wasDarkBeforeSync = this.isDarkMode;
                        this.themeBeforeSync = ThemeColour;
					}
					var shouldBeDark = ShouldSystemUseDarkMode();
					if (shouldBeDark != this.isDarkMode)
                        this.IsDarkMode = shouldBeDark;

					var colour = (SystemParameters.WindowGlassBrush as SolidColorBrush)?.Color;
					if (colour.HasValue && ThemeColour != colour.Value)
                        this.SetThemeColour(colour.Value);
				}
				else
				{
					if (this.wasDarkBeforeSync != null)
                        this.IsDarkMode = this.wasDarkBeforeSync.Value;
					if (this.themeBeforeSync != null)
                        this.SetThemeColour(this.themeBeforeSync.Value);
				}
			});
		}

		private void SetThemeColour(Color colour)
		{
            this.registryService.SetSetting(ThemeSettingName, $"{colour.A}-{colour.R}-{colour.G}-{colour.B}");

			ThemeColour = colour;
            this.OnPropertyChanged(nameof(this.ThemeColourProperty));

			var isThemeDark = ThemeColour.PerceivedBrightness() < 0.5;
			ThemeTextColour = isThemeDark ? Colors.White : Colors.Black;

			ThemeMouseOverBrush = ThemeColour.ChangeColourBrightness(0.3f);

			ThemeMouseDownBrush = ThemeColour.ChangeColourBrightness(-0.2f);

			ThemeBorderColour = ThemeColour.ChangeColourBrightness(this.isDarkMode ? -0.2f : 0.3f);

			ThemeDisabledColour = ThemeColour
				.ChangeColourBrightness(this.isDarkMode ? -0.2f : 0.3f)
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

			if (this.isSyncingWithOs)
                this.SyncThemeWithOs(true);
		}

		private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
		{
            this.Dispose();
		}
	}
}
