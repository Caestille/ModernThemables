namespace ModernThemables.ViewModels;

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CoreUtilities.Helpers.Extensions;
using CoreUtilities.Interfaces.Dialogues;
using ModernThemables.Services;

/// <summary>
/// A view model for a theming control to interact with the theme status of an application with.
/// </summary>
public partial class ThemingControlViewModel : ObservableObject, IDisposable
{
    private const string ThemePath = "Theme.json";

    private readonly IDialogueService dialogueService;
    private readonly Timer osThemePollTimer = new Timer(1000);

    private bool isTransparentHeader;
    private bool isDarkMode;
    private bool isSyncingWithOs;

    private bool? wasDarkBeforeSync;
    private Color? themeBeforeSync;

    /// <summary>
    /// Initialises a new <see cref="ThemingControlViewModel"/>.
    /// </summary>
    public ThemingControlViewModel()
    {
        this.dialogueService = new DialogueService();

        var theme = File.Exists(ThemePath)
            ? JsonSerializer.Deserialize<Theme>(File.ReadAllText(ThemePath)) ?? new Theme()
            : new Theme();

        this.IsDarkMode = theme.IsDarkMode;
        this.ThemeColourProperty = theme.ThemeColour;
        this.IsTransparentHeader = theme.IsTransparentHeader;
        this.IsSyncingWithOs = theme.IsSyncingWithOs;

        this.osThemePollTimer.Elapsed += this.OsThemePollTimer_Elapsed;
        this.osThemePollTimer.AutoReset = true;
        this.osThemePollTimer.Start();

        Application.Current.Dispatcher.ShutdownStarted += this.Dispatcher_ShutdownStarted;
    }

    public static event EventHandler<bool>? DarkModeChanged;

    public static bool DarkMode { get; private set; }

    public event EventHandler<bool>? TransparentHeaderChanged;

    public event EventHandler<bool>? IsDarkChanged;

    public event EventHandler<bool>? SyncWithOsChanged;

    public ICommand ChangeColourCommand => new RelayCommand(this.ChangeColour);

    /// <summary>
    /// Gets or sets the current Theme colour.
    /// </summary>
    public Color ThemeColourProperty
    {
        get => ThemeColour;
        set => this.SetThemeColour(value);
    }

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
            this.SyncWithOsChanged?.Invoke(this, value);
        }
    }

    /// <summary>
    /// Gets or sets whether the theme is Dark or Light.
    /// </summary>
    public bool IsDarkMode
    {
        get => this.isDarkMode;
        set
        {
            DarkMode = value;
            DarkModeChanged?.Invoke(this, value);
            this.SetProperty(ref this.isDarkMode, value);
            this.SetBrightnessMode();
            this.IsDarkChanged?.Invoke(this, value);
        }
    }

    /// <summary>
    /// Gets or sets whether the theme is being synchronised with the OS.
    /// </summary>
    public bool IsTransparentHeader
    {
        get => this.isTransparentHeader;
        set
        {
            this.SetProperty(ref this.isTransparentHeader, value);
            this.TransparentHeaderChanged?.Invoke(this, value);
        }
    }

    [DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
    public static extern bool ShouldSystemUseDarkMode();

    public void Dispose()
    {
        this.osThemePollTimer.Elapsed -= this.OsThemePollTimer_Elapsed;
        this.osThemePollTimer.Stop();
    }

    private void ChangeColour()
        => this.ThemeColourProperty = this.dialogueService.ShowColourPickerDialogue(
            this.ThemeColourProperty,
            (colour) => this.ThemeColourProperty = colour);

    private void SetBrightnessMode()
    {
        Application.Current.Resources[nameof(PrimaryBackgroundColourLight).Replace("Colour", "Brush").Replace("Light", string.Empty)]
            = new SolidColorBrush(this.isDarkMode ? PrimaryBackgroundColourDark : PrimaryBackgroundColourLight);
        Application.Current.Resources[nameof(SecondaryBackgroundColourLight).Replace("Colour", "Brush").Replace("Light", string.Empty)]
            = new SolidColorBrush(this.isDarkMode ? SecondaryBackgroundColourDark : SecondaryBackgroundColourLight);
        Application.Current.Resources[nameof(PrimaryTextColourLight).Replace("Colour", "Brush").Replace("Light", string.Empty)]
            = new SolidColorBrush(this.isDarkMode ? PrimaryTextColourDark : PrimaryTextColourLight);
        Application.Current.Resources[nameof(SecondaryTextColourLight).Replace("Colour", "Brush").Replace("Light", string.Empty)]
            = new SolidColorBrush(this.isDarkMode ? SecondaryTextColourDark : SecondaryTextColourLight);
        Application.Current.Resources[nameof(TertiaryTextColourLight).Replace("Colour", "Brush").Replace("Light", string.Empty)]
            = new SolidColorBrush(this.isDarkMode ? TertiaryTextColourDark : TertiaryTextColourLight);
        Application.Current.Resources[nameof(PrimaryControlColourLight).Replace("Colour", "Brush").Replace("Light", string.Empty)]
            = new SolidColorBrush(this.isDarkMode ? PrimaryControlColourDark : PrimaryControlColourLight);
        Application.Current.Resources[nameof(SecondaryControlColourLight).Replace("Colour", "Brush").Replace("Light", string.Empty)]
            = new SolidColorBrush(this.isDarkMode ? SecondaryControlColourDark : SecondaryControlColourLight);
        Application.Current.Resources[nameof(PrimaryControlMouseOverColourLight).Replace("Colour", "Brush").Replace("Light", string.Empty)]
            = new SolidColorBrush(this.isDarkMode ? PrimaryControlMouseOverColourDark : PrimaryControlMouseOverColourLight);
        Application.Current.Resources[nameof(SecondaryControlMouseOverColourLight).Replace("Colour", "Brush").Replace("Light", string.Empty)]
            = new SolidColorBrush(this.isDarkMode ? SecondaryControlMouseOverColourDark : SecondaryControlMouseOverColourLight);
        Application.Current.Resources[nameof(PrimaryControlMouseDownColourLight).Replace("Colour", "Brush").Replace("Light", string.Empty)]
            = new SolidColorBrush(this.isDarkMode ? PrimaryControlMouseDownColourDark : PrimaryControlMouseDownColourLight);
        Application.Current.Resources[nameof(SecondaryControlMouseDownColourLight).Replace("Colour", "Brush").Replace("Light", string.Empty)]
            = new SolidColorBrush(this.isDarkMode ? SecondaryControlMouseDownColourDark : SecondaryControlMouseDownColourLight);
        Application.Current.Resources[nameof(PrimaryControlBorderColourLight).Replace("Colour", "Brush").Replace("Light", string.Empty)]
            = new SolidColorBrush(this.isDarkMode ? PrimaryControlBorderColourDark : PrimaryControlBorderColourLight);
        Application.Current.Resources[nameof(SecondaryControlBorderColourLight).Replace("Colour", "Brush").Replace("Light", string.Empty)]
            = new SolidColorBrush(this.isDarkMode ? SecondaryControlBorderColourDark : SecondaryControlBorderColourLight);
        Application.Current.Resources[nameof(PrimaryControlDisabledColourLight).Replace("Colour", "Brush").Replace("Light", string.Empty)]
            = new SolidColorBrush(this.isDarkMode ? PrimaryControlDisabledColourDark : PrimaryControlDisabledColourLight);
        Application.Current.Resources[nameof(SecondaryControlDisabledColourLight).Replace("Colour", "Brush").Replace("Light", string.Empty)]
            = new SolidColorBrush(this.isDarkMode ? SecondaryControlDisabledColourDark : SecondaryControlDisabledColourLight);
    }

    private async void SyncThemeWithOs(bool doSync)
    {
        await Task.Run(() =>
        {
            if (doSync)
            {
                if (!this.osThemePollTimer.Enabled)
                {
                    this.wasDarkBeforeSync = this.isDarkMode;
                    this.themeBeforeSync = ThemeColour;
                }

                var shouldBeDark = ShouldSystemUseDarkMode();
                if (shouldBeDark != this.isDarkMode)
                {
                    this.IsDarkMode = shouldBeDark;
                }

                var colour = (SystemParameters.WindowGlassBrush as SolidColorBrush)?.Color;
                if (colour.HasValue && ThemeColour != colour.Value)
                {
                    this.SetThemeColour(colour.Value);
                }
            }
            else
            {
                if (this.wasDarkBeforeSync != null)
                {
                    this.IsDarkMode = this.wasDarkBeforeSync.Value;
                }

                if (this.themeBeforeSync != null)
                {
                    this.SetThemeColour(this.themeBeforeSync.Value);
                }
            }
        });
    }

    private void SetThemeColour(Color colour)
    {
        ThemeColour = colour;
        this.OnPropertyChanged(nameof(this.ThemeColourProperty));

        var isThemeDark = ThemeColour.PerceivedBrightness() < 0.5;
        ThemeTextColour = isThemeDark ? Colors.White : Colors.Black;

        ThemeMouseOverColour = ThemeColour.ChangeColourBrightness(0.3f);

        ThemeMouseDownColour = ThemeColour.ChangeColourBrightness(-0.2f);

        ThemeBorderColour = ThemeColour.ChangeColourBrightness(this.isDarkMode ? -0.2f : 0.3f);

        ThemeDisabledColour = ThemeColour
            .ChangeColourBrightness(this.isDarkMode ? -0.2f : 0.3f)
            .Combine(Colors.Gray, 0.4);

        Application.Current.Resources["ThemeBrush"] = new SolidColorBrush(ThemeColour);
        Application.Current.Resources["ThemeTextBrush"] = new SolidColorBrush(ThemeTextColour);
        Application.Current.Resources["ThemeMouseOverBrush"] = new SolidColorBrush(ThemeMouseOverColour);
        Application.Current.Resources["ThemeMouseDownBrush"] = new SolidColorBrush(ThemeMouseDownColour);
        Application.Current.Resources["ThemeBorderBrush"] = new SolidColorBrush(ThemeMouseOverColour);
        Application.Current.Resources["ThemeDisabledBrush"] = new SolidColorBrush(ThemeMouseDownColour);
    }

    private void OsThemePollTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        if (Application.Current == null)
        {
            return;
        }

        if (this.isSyncingWithOs)
        {
            this.SyncThemeWithOs(true);
        }
    }

    private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
    {
        File.WriteAllText(ThemePath, JsonSerializer.Serialize(new Theme()
        {
            IsDarkMode = this.IsDarkMode,
            ThemeColour = this.ThemeColourProperty,
            IsTransparentHeader = this.IsTransparentHeader,
            IsSyncingWithOs = this.IsSyncingWithOs,
        }));

        this.Dispose();
    }
}
