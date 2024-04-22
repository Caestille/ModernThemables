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
        public static Color PrimaryTextColour => isDarkMode ? Colors.White : Colors.Black;
        public static Color SecondaryTextColour => isDarkMode ? Colors.DarkGray : Colors.Gray;
        public static Color TertiaryTextColour => isDarkMode ? MonoColour(40) : MonoColour(215);
        public static Color PrimaryControlColour => isDarkMode ? MonoColour(60) : MonoColour(195);
        public static Color SecondaryControlColour => isDarkMode ? MonoColour(20) : MonoColour(235);

        public static Color ThemeColour = Color.FromArgb(255, 47, 47, 74);
        public static Color ThemeTextColour = Color.FromArgb(255, 47, 47, 74);

        public static double DisabledModifier => isDarkMode ? -0.7 : 0.7;
        public static double MouseOverModifier => isDarkMode ? 0.1 : -0.1;
        public static double MouseDownModifier => isDarkMode ? -0.1 : 0.1;
        public static double BorderModifier => isDarkMode ? 0.3 : -0.3;

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
            Application.Current.Resources[nameof(PrimaryTextColour).Replace("Colour", "Brush")] = new SolidColorBrush(PrimaryTextColour);
            Application.Current.Resources[nameof(SecondaryTextColour).Replace("Colour", "Brush")] = new SolidColorBrush(SecondaryTextColour);
            Application.Current.Resources[nameof(TertiaryTextColour).Replace("Colour", "Brush")] = new SolidColorBrush(TertiaryTextColour);
            Application.Current.Resources[nameof(PrimaryControlColour).Replace("Colour", "Brush")] = new SolidColorBrush(PrimaryControlColour);
            Application.Current.Resources[nameof(SecondaryControlColour).Replace("Colour", "Brush")] = new SolidColorBrush(SecondaryControlColour);
        }

        public static void SetTheme(Color newThemeColour)
        {
            ThemeColour = newThemeColour;

            var isThemeDark = ThemeColour.PerceivedBrightness() < 0.5;

            ThemeTextColour = isThemeDark ? Colors.White : Colors.Black;

            Application.Current.Resources[nameof(ThemeColour).Replace("Colour", "Brush")] = new SolidColorBrush(ThemeColour);
            Application.Current.Resources[nameof(ThemeTextColour).Replace("Colour", "Brush")] = new SolidColorBrush(ThemeTextColour);
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
