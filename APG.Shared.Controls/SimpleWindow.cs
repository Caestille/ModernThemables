using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace APG.Shared
{
    public abstract class SimpleWindow : NullAutomationWindow
    {
        static SimpleWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SimpleWindow), new FrameworkPropertyMetadata(typeof(SimpleWindow)));
        }

        public SimpleWindow()
        {
            StateChanged += (s, e) => OnStateChanged();
        }

        private void OnStateChanged()
        {
            if (WindowState == WindowState.Maximized)
            {
                var dpiScale = VisualTreeHelper.GetDpi(this);
                var val = 10 - 2 * dpiScale.PixelsPerDip;
                BorderThickness = new Thickness(val);
            }
            else
            {
                BorderThickness = new Thickness(1);
            }

            Debug.WriteLine(BorderThickness);
        }

        public override void OnApplyTemplate()
        {
            if (GetTemplateChild("Icon") is Image iconImage) iconImage.Source = GetIcon();

            if (GetTemplateChild("CloseButton") is Button closeButton) closeButton.Click += (_, _) => Close();
            if (GetTemplateChild("MaximizeButton") is Button maximizeButton) maximizeButton.Click += (_, _) => WindowState = WindowState.Maximized;
            if (GetTemplateChild("MinimizeButton") is Button minimizeButton) minimizeButton.Click += (_, _) => WindowState = WindowState.Minimized;
            if (GetTemplateChild("RestoreButton") is Button restoreButton) restoreButton.Click += (_, _) => WindowState = WindowState.Normal;

            base.OnApplyTemplate();

            OnStateChanged();
        }

        private ImageSource? GetIcon()
        {
            // Window.Icon property
            if (Icon != null) return Icon;

            // Assembly Icon
            var hInstance = GetModuleHandle(null!);
            var iconPtr = LoadIcon(hInstance, (IntPtr)IDI_APPLICATION);
            if (iconPtr != IntPtr.Zero) return Imaging.CreateBitmapSourceFromHIcon(iconPtr, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            // System Icon
            iconPtr = LoadIcon(IntPtr.Zero, (IntPtr)IDI_APPLICATION);
            if (iconPtr != IntPtr.Zero) return Imaging.CreateBitmapSourceFromHIcon(iconPtr, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            return null;
        }

        #region P/Invoke

        private const int IDI_APPLICATION = 0x7F00;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIconName);

        #endregion
    }
}
