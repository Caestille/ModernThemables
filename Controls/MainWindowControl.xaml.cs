using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using CoreUtilities.HelperClasses;
using System.Threading.Tasks;
using System.Threading;
using ModernThemables.ScalableIcons;
using CoreUtilities.HelperClasses.Extensions;

namespace ModernThemables.Controls
{
	/// <summary>
	/// Interaction logic for MainWindowControl.xaml
	/// </summary>
	public partial class MainWindowControl : UserControl
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
		private bool preventTrigger;

		private Window mainWindow;
		private static CurrentMonitorInfo currentMonitorInfo;

		private bool IsMainWindowFocused
		{
			get { return (bool)GetValue(IsMainWindowFocusedProperty); }
			set { SetValue(IsMainWindowFocusedProperty, value); }
		}
		private static readonly DependencyProperty IsMainWindowFocusedProperty = DependencyProperty.Register(
		  "IsMainWindowFocused", typeof(bool), typeof(MainWindowControl), new PropertyMetadata(false));

		public bool IsToolWindow
		{
			get { return (bool)GetValue(IsToolWindowProperty); }
			set { SetValue(IsToolWindowProperty, value); }
		}
		public static readonly DependencyProperty IsToolWindowProperty = DependencyProperty.Register(
		  "IsToolWindow", typeof(bool), typeof(MainWindowControl), new PropertyMetadata(false));

		private string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}
		private static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
		  "Title", typeof(string), typeof(MainWindowControl), new PropertyMetadata(""));

		private ImageSource Icon
		{
			get { return (ImageSource)GetValue(IconProperty); }
			set { SetValue(IconProperty, value); }
		}
		private static readonly DependencyProperty IconProperty = DependencyProperty.Register(
		  "Icon", typeof(ImageSource), typeof(MainWindowControl), new PropertyMetadata(null));

		public ObservableObject VisibleViewModel
		{
			get { return (ObservableObject)GetValue(VisibleViewModelProperty); }
			set { SetValue(VisibleViewModelProperty, value); }
		}
		public static readonly DependencyProperty VisibleViewModelProperty = DependencyProperty.Register(
		  "VisibleViewModel", typeof(ObservableObject), typeof(MainWindowControl), new PropertyMetadata(null));

		#endregion

		public MainWindowControl()
		{
			InitializeComponent();
			SettingsClippingStackPanel.ClipToBounds = true;
			SettingsBorderBlur.Opacity = 0;
			this.Loaded += MainWindowControl_Loaded;
		}

		private async void OpenThemingMenu(bool open)
		{
			if (open == isThemingGridOpen)
				return;

			SettingsClippingStackPanel.IsHitTestVisible = open;

			var menuHiddenTopMargin = 5;
			var menuStart = open ? SettingsGrid.ActualHeight * -1 : menuHiddenTopMargin;
			var menuEnd = open ? menuHiddenTopMargin : SettingsGrid.ActualHeight * -1;
			var blackoutGridOpacityEnd = open ? 0.3 : 0;
			var buttonRotate = open ? 180 : 0;

			ThemeSetButton.RenderTransform = new RotateTransform(buttonRotate) { CenterX = ThemeSetButton.ActualWidth / 2, CenterY = ThemeSetButton.ActualHeight / 2 };
			BlackoutGrid.Opacity = blackoutGridOpacityEnd;
			SettingsGrid.BeginAnimation(Canvas.TopProperty, new DoubleAnimation(menuStart, menuEnd, new Duration(TimeSpan.FromSeconds(0.1))));

			isThemingGridOpen = open;

			if (!open)
				await Task.Delay(100);

			SettingsBorderBlur.Opacity = open ? 0.7 : 0;

			SettingsClippingStackPanel.IsEnabled = open;

			if (open)
			{
				await Task.Delay(100);
				ThemingControl.FocusOnOpen();
				Blurrer.DrawBlurredElementBackground();
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

					currentMonitorInfo = new CurrentMonitorInfo()
					{
						WorkingAreaWidth = mmi.ptMaxSize.X,
						WorkingAreaHeight = mmi.ptMaxSize.Y
					};
				}

				Marshal.StructureToPtr(mmi, lParam, true);
			}

			if (msg == NativeMethods.WM_SYSCOMMAND)
			{
				if (wParam.ToInt32() == NativeMethods.SC_MINIMIZE)
				{
					mainWindow.WindowStyle = WindowStyle.SingleBorderWindow;
					mainWindow.WindowState = WindowState.Minimized;

					handled = true;
				}
				else if (wParam.ToInt32() == NativeMethods.SC_RESTORE)
				{
					mainWindow.WindowState = WindowState.Normal;
					mainWindow.WindowStyle = WindowStyle.None;

					if (isWindowMaximised)
					{
						mainWindow.WindowState = WindowState.Minimized;
						mainWindow.WindowState = WindowState.Maximized;
					}

					handled = true;
				}
				else if (wParam.ToInt32() == NativeMethods.SC_CHANGESTATE)
				{
					mainWindow.WindowStyle = WindowStyle.SingleBorderWindow;

					isWindowMaximised = !isWindowMaximised;
					ContentGrid.Margin = isWindowMaximised ? new Thickness(0, 31, 0, 0) : new Thickness(1, 31, 1, 1);
					SetChangeStateButtonAppearance(isWindowMaximised);
					mainWindow.WindowState = isWindowMaximised
						? WindowState.Maximized
						: WindowState.Normal;

					mainWindow.WindowStyle = WindowStyle.None;
					if (isWindowMaximised)
					{
						mainWindow.WindowState = WindowState.Minimized;
						mainWindow.WindowState = WindowState.Maximized;
					}

					handled = true;
				}
				else if (wParam.ToInt32() == NativeMethods.SC_CLOSE)
				{
					mainWindow.WindowStyle = WindowStyle.SingleBorderWindow;
					mainWindow.Close();

					handled = true;
				}
			}

			return IntPtr.Zero;
		}

		private void SetChangeStateButtonAppearance(bool isWindowMaximised)
		{
			var logicalElements = new List<FrameworkElement>();
			logicalElements = (this as FrameworkElement).GetLogicalElements();
			var grid = (RestoreIcon)logicalElements.First(x => x.Tag != null && x.Tag.ToString() == "RestoreDownGrid");
			grid.Visibility = !isWindowMaximised ? Visibility.Collapsed : Visibility.Visible;
			var border = (MaximiseIcon)logicalElements.First(x => x.Tag != null && x.Tag.ToString() == "MaximiseBorder");
			border.Visibility = !isWindowMaximised ? Visibility.Visible : Visibility.Collapsed;
		}

		private async void MainWindowControl_Loaded(object sender, RoutedEventArgs e)
		{
			this.Loaded -= MainWindowControl_Loaded;
			Application.Current.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
			ThemingControl.InternalRequestClose += ThemingControl_InternalRequestClose;

			if (IsToolWindow)
			{
				ChangeStateButton.Visibility = Visibility.Collapsed;
				MinimiseButton.Visibility = Visibility.Collapsed;
				Splitter.Visibility = Visibility.Collapsed;
				ThemeSetButton.Visibility = Visibility.Collapsed;
			}
			await Task.Run(() => Thread.Sleep(300));
			mainWindow = Window.GetWindow(this);
			if (IsToolWindow)
			{
				mainWindow.SizeToContent = SizeToContent.Manual;
				mainWindow.Width = this.Width;
				mainWindow.Height = this.Height;
				mainWindow.SizeToContent = SizeToContent.WidthAndHeight;
			}


			if (mainWindow == null)
				return;

			((HwndSource)PresentationSource.FromVisual(this)).AddHook(HookProc);

			mainWindow.SizeChanged += MainWindow_SizeChanged;

			this.SetBinding(IconProperty, new Binding("Icon") { Source = mainWindow });
			this.SetBinding(TitleProperty, new Binding("Title") { Source = mainWindow });
			this.SetBinding(IsMainWindowFocusedProperty, new Binding("IsActive") { Source = mainWindow });
		}

		private void ThemingControl_InternalRequestClose(object? sender, EventArgs e)
		{
			OpenThemingMenu(false);
		}

		private void MinimiseButton_Click(object sender, RoutedEventArgs e)
		{
			var m_hWnd = new WindowInteropHelper(mainWindow).Handle;
			SendMessage(m_hWnd, NativeMethods.WM_SYSCOMMAND, new IntPtr(NativeMethods.SC_MINIMIZE), IntPtr.Zero);
		}

		private void ChangeStateButton_Click(object sender, RoutedEventArgs e)
		{
			var m_hWnd = new WindowInteropHelper(mainWindow).Handle;
			SendMessage(m_hWnd, NativeMethods.WM_SYSCOMMAND, new IntPtr(NativeMethods.SC_CHANGESTATE), IntPtr.Zero);
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			var m_hWnd = new WindowInteropHelper(mainWindow).Handle;
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
			if (preventTrigger) return;

			var isDockedTop = !isWindowMaximised && (mainWindow.WindowState == WindowState.Maximized);
			var isDockedSide = mainWindow.WindowState == WindowState.Normal
				&& mainWindow.Width != mainWindow.RestoreBounds.Width
				&& mainWindow.Height != mainWindow.RestoreBounds.Height;
			var wasDocked = isDockedOrMaximised
				&& isWindowMaximised
				&& mainWindow.WindowState == WindowState.Normal;

			if (isDockedTop || wasDocked)
			{
				var m_hWnd = new WindowInteropHelper(mainWindow).Handle;
				SendMessage(m_hWnd, NativeMethods.WM_SYSCOMMAND, new IntPtr(NativeMethods.SC_CHANGESTATE), IntPtr.Zero);
			}
			isDockedOrMaximised = isDockedTop || isDockedSide || (mainWindow.WindowState == WindowState.Maximized);
		}

		private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
		{
			mainWindow.SizeChanged -= MainWindow_SizeChanged;
			ThemingControl.InternalRequestClose -= ThemingControl_InternalRequestClose;
		}
	}
}