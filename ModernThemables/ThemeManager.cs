using CoreUtilities.HelperClasses.Extensions;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Resources;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media;

namespace ModernThemables
{
    public static class ThemeManager
    {
        [DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
        public static extern bool ShouldSystemUseDarkMode();

        private static readonly Timer osThemePollTimer = new Timer(1000);

        private static bool isDarkMode;
        private static bool isSyncingWithOs;

        public static Color BackgroundColour => isDarkMode ? MonoColour(10) : MonoColour(255);
        public static Color SecondaryBackgroundColour => isDarkMode ? MonoColour(30) : MonoColour(235);
        public static Color TertiaryBackgroundColour => isDarkMode ? MonoColour(50) : MonoColour(215);
        public static Color PrimaryTextColour => isDarkMode ? Colors.White : Colors.Black;
        public static Color SecondaryTextColour => isDarkMode ? Colors.DarkGray : Colors.Gray;
        public static Color TertiaryTextColour => isDarkMode ? MonoColour(40) : MonoColour(215);
        public static Color PrimaryControlColour => isDarkMode ? MonoColour(60) : MonoColour(195);
        public static Color SecondaryControlColour => isDarkMode ? MonoColour(20) : MonoColour(235);

        public static Color ThemeColour = Color.FromArgb(255, 47, 47, 74);
        public static Color ThemeTextColour = Color.FromArgb(255, 47, 47, 74);

        public static float DisabledModifier => isDarkMode ? -0.7f : 0.7f;
        public static float MouseOverModifier => isDarkMode ? 0.1f : 0.2f;
        public static float MouseDownModifier => isDarkMode ? -0.1f : -0.1f;
        public static float BorderModifier => isDarkMode ? 0.3f : -0.3f;

        static ThemeManager()
        {
            Load();
            Application.Current.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;

            osThemePollTimer.Elapsed += OsThemePollTimer_Elapsed;
            osThemePollTimer.AutoReset = true;
            osThemePollTimer.Start();
        }

        private static void Load()
        {

        }

        public static void SetDarkMode(bool isDarkMode)
        {
            ThemeManager.isDarkMode = isDarkMode;

            Application.Current.Resources[nameof(BackgroundColour).Replace("Colour", "Brush")] = new SolidColorBrush(BackgroundColour);
            Application.Current.Resources[nameof(SecondaryBackgroundColour).Replace("Colour", "Brush")] = new SolidColorBrush(SecondaryBackgroundColour);
            Application.Current.Resources[nameof(TertiaryBackgroundColour).Replace("Colour", "Brush")] = new SolidColorBrush(TertiaryBackgroundColour);
            Application.Current.Resources[nameof(PrimaryTextColour).Replace("Colour", "Brush")] = new SolidColorBrush(PrimaryTextColour);
            Application.Current.Resources[nameof(SecondaryTextColour).Replace("Colour", "Brush")] = new SolidColorBrush(SecondaryTextColour);
            Application.Current.Resources[nameof(TertiaryTextColour).Replace("Colour", "Brush")] = new SolidColorBrush(TertiaryTextColour);
            Application.Current.Resources[nameof(PrimaryControlColour).Replace("Colour", "Brush")] = new SolidColorBrush(PrimaryControlColour);
            Application.Current.Resources[nameof(SecondaryControlColour).Replace("Colour", "Brush")] = new SolidColorBrush(SecondaryControlColour);
            Application.Current.Resources[nameof(PrimaryControlColour).Replace("Colour", "BorderBrush")] = new SolidColorBrush(PrimaryControlColour.ChangeColourBrightness(BorderModifier));
            Application.Current.Resources[nameof(SecondaryControlColour).Replace("Colour", "BorderBrush")] = new SolidColorBrush(SecondaryControlColour.ChangeColourBrightness(BorderModifier));

            Application.Current.Resources[nameof(PrimaryControlColour).Replace("Colour", "MouseOverBrush")] = new SolidColorBrush(PrimaryControlColour.ChangeColourBrightness(MouseOverModifier));
            Application.Current.Resources[nameof(SecondaryControlColour).Replace("Colour", "MouseOverBrush")] = new SolidColorBrush(SecondaryControlColour.ChangeColourBrightness(MouseOverModifier));

            Application.Current.Resources[nameof(PrimaryControlColour).Replace("Colour", "MouseDownBrush")] = new SolidColorBrush(PrimaryControlColour.ChangeColourBrightness(MouseDownModifier));
            Application.Current.Resources[nameof(SecondaryControlColour).Replace("Colour", "MouseDownBrush")] = new SolidColorBrush(SecondaryControlColour.ChangeColourBrightness(MouseDownModifier));

            Application.Current.Resources[nameof(PrimaryControlColour).Replace("Colour", "DisabledBrush")] = new SolidColorBrush(PrimaryControlColour.ChangeColourBrightness(DisabledModifier));
            Application.Current.Resources[nameof(SecondaryControlColour).Replace("Colour", "DisabledBrush")] = new SolidColorBrush(SecondaryControlColour.ChangeColourBrightness(DisabledModifier));
        }

        public static void SetTheme(Color newThemeColour)
        {
            ThemeColour = newThemeColour;

            var isThemeDark = ThemeColour.PerceivedBrightness() < 0.5;

            ThemeTextColour = isThemeDark ? Colors.White : Colors.Black;

            Application.Current.Resources[nameof(ThemeColour).Replace("Colour", "Brush")] = new SolidColorBrush(ThemeColour);
            Application.Current.Resources[nameof(ThemeTextColour).Replace("Colour", "Brush")] = new SolidColorBrush(ThemeTextColour);

            Application.Current.Resources[nameof(ThemeColour).Replace("Colour", "BorderBrush")] = new SolidColorBrush(ThemeColour.ChangeColourBrightness(BorderModifier));

            Application.Current.Resources[nameof(ThemeColour).Replace("Colour", "MouseOverBrush")] = new SolidColorBrush(ThemeColour.ChangeColourBrightness(MouseOverModifier));

            Application.Current.Resources[nameof(ThemeColour).Replace("Colour", "MouseDownBrush")] = new SolidColorBrush(ThemeColour.ChangeColourBrightness(MouseDownModifier));

            Application.Current.Resources[nameof(ThemeColour).Replace("Colour", "DisabledBrush")] = new SolidColorBrush(ThemeColour.ChangeColourBrightness(DisabledModifier));
        }

        public static void SyncThemeWithOs(bool doSync)
        {
            isSyncingWithOs = doSync;
        }

        public static void Dispose()
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.ShutdownStarted -= Dispatcher_ShutdownStarted;
            }

            osThemePollTimer.Elapsed -= OsThemePollTimer_Elapsed;
            osThemePollTimer.Stop();
        }

        private static Color MonoColour(byte value)
        {
            return Color.FromArgb(255, value, value, value);
        }

        private static async void OsThemePollTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (!isSyncingWithOs) return;

            await Task.Run(() =>
            {
                var shouldBeDark = ShouldSystemUseDarkMode();
                if (shouldBeDark != isDarkMode)
                    isDarkMode = shouldBeDark;

                var colour = (SystemParameters.WindowGlassBrush as SolidColorBrush)?.Color;
                if (colour.HasValue && ThemeColour != colour.Value)
                    SetTheme(colour.Value);
            });
        }

        private static void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
        {
            Dispose();
        }
    }
}
