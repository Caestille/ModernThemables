using ControlzEx;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace ModernThemables.Controls
{
	public class ThemableWindow : Window
	{
		#region Dll imports and structs

		[DllImport("user32.dll")]
		private static extern IntPtr MonitorFromWindow(IntPtr handle, uint flags);

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

		[Serializable]
		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;

			public RECT(int left, int top, int right, int bottom)
			{
				Left = left;
				Top = top;
				Right = right;
				Bottom = bottom;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct MONITORINFO
		{
			public int cbSize;
			public RECT rcMonitor;
			public RECT rcWork;
			public uint dwFlags;
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int X;
			public int Y;

			public POINT(int x, int y)
			{
				X = x;
				Y = y;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct MINMAXINFO
		{
			public POINT ptReserved;
			public POINT ptMaxSize;
			public POINT ptMaxPosition;
			public POINT ptMinTrackSize;
			public POINT ptMaxTrackSize;
		}

		private struct CurrentMonitorInfo
		{
			public int WorkingAreaWidth;
			public int WorkingAreaHeight;
		}


		internal class NativeMethods
		{
			public const int SC_RESTORE = 0xF120;
			public const int SC_MINIMIZE = 0xF020;
			public const int SC_CHANGESTATE = 0xF220;
			public const int SC_CLOSE = 0xF060;
			public const int WM_SYSCOMMAND = 0x0112;
			public const int WS_SYSMENU = 0x80000;
			public const int WS_MINIMIZEBOX = 0x20000;
			public const int CS_DBLCLKS = 0x8;
			public const int WM_GETMINMAXINFO = 0x0024;
			public const uint MONITOR_DEFAULTTONEAREST = 0x00000002;
			NativeMethods() { }
		}

		#endregion

		#region Members and dependency properties

		private bool isThemingGridOpen;
		private bool isWindowMaximised;
		private bool isDockedOrMaximised;

		private const string PART_themingControl = "PART_ThemingControl";
		private const string PART_settingsGrid = "PART_SettingsGrid";
		private const string PART_contentGrid = "PART_ContentGrid";
		private const string PART_settingsDropShadow = "PART_SettingsDropShadow";
		private const string PART_splitter = "PART_Splitter";
		private const string PART_closeButton = "PART_CloseButton";
		private const string PART_settingsCloseButton = "PART_SettingsCloseButton";
		private const string PART_minimiseButton = "PART_MinimiseButton";
		private const string PART_changeStateButton = "PART_ChangeStateButton";
		private const string PART_themeSetButton = "PART_ThemeSetButton";
		private const string PART_blackoutGrid = "PART_BlackoutGrid";
		private const string PART_settingsBorder = "PART_SettingsBorder";

		private ThemingControl themingControl;
		private Grid settingsGrid;
		private Grid contentGrid;
		private DropShadowEffect settingsDropShadow;
		private Border splitter;
		private ExtendedButton closeButton;
		private ExtendedButton settingsCloseButton;
		private ExtendedButton minimiseButton;
		private ExtendedButton changeStateButton;
		private ExtendedButton themeSetButton;
		private ExtendedButton blackoutGrid;
		private Border settingsBorder;

		static ThemableWindow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ThemableWindow), new FrameworkPropertyMetadata(typeof(ThemableWindow)));
		}

		public bool IsToolWindow
		{
			get { return (bool)GetValue(IsToolWindowProperty); }
			set { SetValue(IsToolWindowProperty, value); }
		}
		public static readonly DependencyProperty IsToolWindowProperty = DependencyProperty.Register(
		  "IsToolWindow", typeof(bool), typeof(ThemableWindow), new PropertyMetadata(false));

		#endregion

		public ThemableWindow()
		{
			this.Loaded += ThemableWindow_Loaded;
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (themingControl != null)
			{
				themingControl.InternalRequestClose -= themingControl_InternalRequestClose;
			}
			themingControl = GetTemplateChild(PART_themingControl) as ThemingControl;
			themingControl.InternalRequestClose += themingControl_InternalRequestClose;

			settingsGrid = GetTemplateChild(PART_settingsGrid) as Grid;
			settingsGrid.ClipToBounds = true;

			contentGrid = GetTemplateChild(PART_contentGrid) as Grid;
			settingsDropShadow = GetTemplateChild(PART_settingsDropShadow) as DropShadowEffect;
			settingsDropShadow.Opacity = 0;

			splitter = GetTemplateChild(PART_splitter) as Border;

			if (closeButton != null)
			{
				closeButton.Click -= CloseButton_Click;
			}
			closeButton = GetTemplateChild(PART_closeButton) as ExtendedButton;
			closeButton.Click += CloseButton_Click;

			if (settingsCloseButton != null)
			{
				settingsCloseButton.Click -= SettingsCloseButton_Click;
			}
			settingsCloseButton = GetTemplateChild(PART_settingsCloseButton) as ExtendedButton;
			settingsCloseButton.Click += SettingsCloseButton_Click;

			if (minimiseButton != null)
			{
				minimiseButton.Click -= MinimiseButton_Click;
			}
			minimiseButton = GetTemplateChild(PART_minimiseButton) as ExtendedButton;
			minimiseButton.Click += MinimiseButton_Click;

			if (changeStateButton != null)
			{
				changeStateButton.Click -= ChangeStateButton_Click;
			}
			changeStateButton = GetTemplateChild(PART_changeStateButton) as ExtendedButton;
			changeStateButton.Click += ChangeStateButton_Click;

			if (themeSetButton != null)
			{
				themeSetButton.Click -= ThemeSetButton_Click;
			}
			themeSetButton = GetTemplateChild(PART_themeSetButton) as ExtendedButton;
			themeSetButton.Click += ThemeSetButton_Click;

			if (blackoutGrid != null)
			{
				blackoutGrid.Click -= SettingsCloseButton_Click;
			}
			blackoutGrid = GetTemplateChild(PART_blackoutGrid) as ExtendedButton;
			blackoutGrid.Click += SettingsCloseButton_Click;

			settingsBorder = GetTemplateChild(PART_settingsBorder) as Border;
		}

		private async void OpenThemingMenu(bool open)
		{
			if (open == isThemingGridOpen)
				return;

			settingsGrid.IsHitTestVisible = open;

			var menuEnd = open ? 6 : -settingsBorder.ActualHeight - 5;
			var blackoutGridOpacityEnd = open ? 0.3 : 0;
			var blackoutGridHitTest = open ? true : false;
			var buttonRotate = open ? 180 : 0;

			var marginEnd = new Thickness(0, menuEnd, 6, 0);

			themeSetButton.RenderTransform = new RotateTransform(buttonRotate) { CenterX = themeSetButton.ActualWidth / 2, CenterY = themeSetButton.ActualHeight / 2 };
			blackoutGrid.Opacity = blackoutGridOpacityEnd;
			blackoutGrid.IsHitTestVisible = blackoutGridHitTest;
			settingsBorder.BeginAnimation(MarginProperty, new ThicknessAnimation(marginEnd, new Duration(TimeSpan.FromSeconds(0.1))));

			isThemingGridOpen = open;

			if (!open)
				await Task.Delay(100);

			settingsDropShadow.Opacity = open ? 0.7 : 0;

			settingsGrid.IsEnabled = open;

			if (open)
			{
				await Task.Delay(100);
				themingControl.FocusOnOpen();
			}
		}

		private IntPtr HookProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == NativeMethods.WM_GETMINMAXINFO)
			{
				var mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

				var monitor = MonitorFromWindow(hwnd, NativeMethods.MONITOR_DEFAULTTONEAREST);

				if (monitor != IntPtr.Zero)
				{
					var monitorInfo = new MONITORINFO();
					monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
					GetMonitorInfo(monitor, ref monitorInfo);
					var rcWorkArea = monitorInfo.rcWork;
					var rcMonitorArea = monitorInfo.rcMonitor;
					mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
					mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
					mmi.ptMaxSize.X = Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
					mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top);
				}

				Marshal.StructureToPtr(mmi, lParam, true);
			}

			if (msg == NativeMethods.WM_SYSCOMMAND)
			{
				if (wParam.ToInt32() == NativeMethods.SC_MINIMIZE)
				{
					WindowStyle = WindowStyle.SingleBorderWindow;
					WindowState = WindowState.Minimized;

					handled = true;
				}
				else if (wParam.ToInt32() == NativeMethods.SC_RESTORE)
				{
					WindowState = WindowState.Normal;
					WindowStyle = WindowStyle.None;

					handled = true;
				}
				else if (wParam.ToInt32() == NativeMethods.SC_CHANGESTATE)
				{
					WindowStyle = WindowStyle.SingleBorderWindow;

					isWindowMaximised = !isWindowMaximised;
					contentGrid.Margin = isWindowMaximised ? new Thickness(0, 31, 0, 0) : new Thickness(1, 31, 1, 1);
					SetChangeStateButtonAppearance(isWindowMaximised);
					if (isWindowMaximised) ResizeMode = ResizeMode.NoResize;
					if (isWindowMaximised && WindowState == WindowState.Maximized) WindowState = WindowState.Normal;
					WindowState = isWindowMaximised
						? WindowState.Maximized
						: WindowState.Normal;
					if (!isWindowMaximised) ResizeMode = ResizeMode.CanResize;

					WindowStyle = WindowStyle.None;

					handled = true;
				}
				else if (wParam.ToInt32() == NativeMethods.SC_CLOSE)
				{
					WindowStyle = WindowStyle.SingleBorderWindow;
					Close();

					handled = true;
				}
			}

			return IntPtr.Zero;
		}

		private void SetChangeStateButtonAppearance(bool isWindowMaximised)
		{
			//var logicalElements = new List<FrameworkElement>();
			//logicalElements = (this as FrameworkElement).GetLogicalElements();
			//var grid = (RestoreIcon)logicalElements.First(x => x.Tag != null && x.Tag.ToString() == "RestoreDownGrid");
			//grid.Visibility = !isWindowMaximised ? Visibility.Collapsed : Visibility.Visible;
			//var border = (MaximiseIcon)logicalElements.First(x => x.Tag != null && x.Tag.ToString() == "MaximiseBorder");
			//border.Visibility = !isWindowMaximised ? Visibility.Visible : Visibility.Collapsed;
		}

		private async void ThemableWindow_Loaded(object sender, RoutedEventArgs e)
		{
			this.Loaded -= ThemableWindow_Loaded;
			Application.Current.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;

			if (IsToolWindow)
			{
				changeStateButton.Visibility = Visibility.Collapsed;
				minimiseButton.Visibility = Visibility.Collapsed;
				splitter.Visibility = Visibility.Collapsed;
				themeSetButton.Visibility = Visibility.Collapsed;
			}
			await Task.Run(() => Thread.Sleep(300));
			if (IsToolWindow)
			{
				SizeToContent = SizeToContent.Manual;
				Width = this.Width;
				Height = this.Height;
				SizeToContent = SizeToContent.WidthAndHeight;
			}

			((HwndSource)PresentationSource.FromVisual(this)).AddHook(HookProc);

			SizeChanged += MainWindow_SizeChanged;

			//this.SetBinding(IconProperty, new Binding("Icon") { Source = mainWindow });
			//this.SetBinding(TitleProperty, new Binding("Title") { Source = mainWindow });
			//this.SetBinding(IsMainWindowFocusedProperty, new Binding("IsActive") { Source = mainWindow });
		}

		private void themingControl_InternalRequestClose(object? sender, EventArgs e)
		{
			OpenThemingMenu(false);
		}

		private void MinimiseButton_Click(object sender, RoutedEventArgs e)
		{
			var m_hWnd = new WindowInteropHelper(this).Handle;
			SendMessage(m_hWnd, NativeMethods.WM_SYSCOMMAND, new IntPtr(NativeMethods.SC_MINIMIZE), IntPtr.Zero);
		}

		private void ChangeStateButton_Click(object sender, RoutedEventArgs e)
		{
			var m_hWnd = new WindowInteropHelper(this).Handle;
			SendMessage(m_hWnd, NativeMethods.WM_SYSCOMMAND, new IntPtr(NativeMethods.SC_CHANGESTATE), IntPtr.Zero);
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			var m_hWnd = new WindowInteropHelper(this).Handle;
			SendMessage(m_hWnd, NativeMethods.WM_SYSCOMMAND, new IntPtr(NativeMethods.SC_CLOSE), IntPtr.Zero);
		}

		private void ThemeSetButton_Click(object sender, RoutedEventArgs e)
		{
			OpenThemingMenu(!isThemingGridOpen);
		}

		private void SettingsCloseButton_Click(object sender, RoutedEventArgs e)
		{
			OpenThemingMenu(false);
		}

		private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var isDockedTop = !isWindowMaximised && (WindowState == WindowState.Maximized);
			var isDockedSide = WindowState == WindowState.Normal
				&& Width != RestoreBounds.Width
				&& Height != RestoreBounds.Height;
			var wasDocked = isDockedOrMaximised
				&& isWindowMaximised
				&& WindowState == WindowState.Normal;

			if (isDockedTop || wasDocked)
			{
				var m_hWnd = new WindowInteropHelper(this).Handle;
				SendMessage(m_hWnd, NativeMethods.WM_SYSCOMMAND, new IntPtr(NativeMethods.SC_CHANGESTATE), IntPtr.Zero);
			}
			isDockedOrMaximised = isDockedTop || isDockedSide || (WindowState == WindowState.Maximized);
		}

		private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
		{
			SizeChanged -= MainWindow_SizeChanged;
			themingControl.InternalRequestClose -= themingControl_InternalRequestClose;
		}
	}
}
