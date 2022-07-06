using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Win10Themables.Views;

namespace Win10Themables.Controls
{
	/// <summary>
	/// Interaction logic for MainWindowControl.xaml
	/// </summary>
	public partial class MainWindowControl : UserControl
	{
		private struct CurrentMonitorInfo
		{
			public int WorkingAreaWidth;
			public int WorkingAreaHeight;
		}

		private bool isThemingGridOpen;
		private bool isWindowMaximised;
		private bool isAnimating;

		private Window mainWindow;

		private static CurrentMonitorInfo currentMonitorInfo;

		public ICommand WindowSizeChangedCommand => new RelayCommand(WindowSizeChanged);

		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}
		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
		  "Title", typeof(string), typeof(MainWindowControl), new PropertyMetadata(""));

		public WindowState WindowStateProperty
		{
			get { return (WindowState)GetValue(WindowStateDProperty); }
			set { SetValue(WindowStateDProperty, value); }
		}
		public static readonly DependencyProperty WindowStateDProperty = DependencyProperty.Register(
		  "WindowStateProperty", typeof(WindowState), typeof(MainWindowControl), new PropertyMetadata(WindowState.Normal));

		public ObservableObject VisibleViewModel
		{
			get { return (ObservableObject)GetValue(VisibleViewModelProperty); }
			set { SetValue(VisibleViewModelProperty, value); }
		}
		public static readonly DependencyProperty VisibleViewModelProperty = DependencyProperty.Register(
		  "VisibleViewModel", typeof(ObservableObject), typeof(MainWindowControl), new PropertyMetadata(null));

		public bool IsDockedOrMaximised
		{
			get { return (bool)GetValue(IsDockedProperty); }
			private set { SetValue(IsDockedProperty, value); }
		}
		public static readonly DependencyProperty IsDockedProperty = DependencyProperty.Register(
		  "IsDockedOrMaximised", typeof(bool), typeof(MainWindowControl), new PropertyMetadata(false));

		public MainWindowControl()
		{
			InitializeComponent();
			mainWindow = Application.Current.MainWindow;
			mainWindow.Loaded += MainWindow_Loaded;
			SettingsClippingStackPanel.ClipToBounds = true;
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			OnSourceInitialized();
			mainWindow.Loaded -= MainWindow_Loaded;
		}

		private void MinimiseButton_Click(object sender, RoutedEventArgs e)
		{
			WindowStateProperty = WindowState.Minimized;
		}

		public void ChangeStateButton_Click(object sender, RoutedEventArgs e)
		{
			ChangeWindowState();
		}

		public void WindowSizeChanged()
		{
			var isDockedTop = !isWindowMaximised && (mainWindow.WindowState == WindowState.Maximized);
			var isDockedSide = mainWindow.WindowState == WindowState.Normal
				&& mainWindow.Width != mainWindow.RestoreBounds.Width
				&& mainWindow.Height != mainWindow.RestoreBounds.Height;
			var wasDocked = IsDockedOrMaximised
				&& isWindowMaximised
				&& mainWindow.WindowState == WindowState.Normal;

			if (isDockedTop || wasDocked)
			{
				ChangeWindowState();
			}

			IsDockedOrMaximised = isDockedTop || isDockedSide || (mainWindow.WindowState == WindowState.Maximized);
		}

		public async void ChangeWindowState()
		{
			isWindowMaximised = !isWindowMaximised;

			WindowStateProperty = !isWindowMaximised ? WindowState.Normal : WindowState.Maximized;
			var logicalElements = new List<FrameworkElement>();
			GetLogicalElements(this, logicalElements);
			var grid = (Grid)logicalElements.First(x => x.Tag != null && x.Tag.ToString() == "RestoreDownGrid");
			grid.Visibility = !isWindowMaximised ? Visibility.Collapsed : Visibility.Visible;
			var border = (Border)logicalElements.First(x => x.Tag != null && x.Tag.ToString() == "MaximiseBorder");
			border.Visibility = !isWindowMaximised ? Visibility.Visible : Visibility.Collapsed;
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void GetLogicalElements(object parent, List<FrameworkElement> logicalElements)
		{
			if (parent == null) return;
			if (parent.GetType().IsSubclassOf(typeof(FrameworkElement)))
				logicalElements.Add((FrameworkElement)parent);
			var doParent = parent as DependencyObject;
			if (doParent == null) return;
			foreach (object child in LogicalTreeHelper.GetChildren(doParent))
			{
				GetLogicalElements(child, logicalElements);
			}
		}

		private void ThemeSetButton_Click(object sender, RoutedEventArgs e)
		{
			if (isThemingGridOpen)
				CloseThemingMenu();
			else
				OpenThemingMenu();
		}

		private void OpenThemingMenu()
		{
			if (isThemingGridOpen)
				return;

			SettingsClippingStackPanel.IsHitTestVisible = true;

			ThemeSetButton.RenderTransform = new RotateTransform(180) { CenterX = ThemeSetButton.ActualWidth / 2, CenterY = ThemeSetButton.ActualHeight / 2 };

			var start = SettingsGrid.ActualHeight * -1;
			var end = 5;
			SettingsGrid.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.3))));
			BlackoutGrid.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 0.3, new Duration(TimeSpan.FromSeconds(0.1))));
			SettingsGrid.BeginAnimation(Canvas.TopProperty, new DoubleAnimation(start, end, new Duration(TimeSpan.FromSeconds(0.1))));

			isThemingGridOpen = true;
		}

		private void CloseThemingMenu()
		{
			if (!isThemingGridOpen)
				return;

			SettingsClippingStackPanel.IsHitTestVisible = false;

			RotateTransform rotateTransform = new RotateTransform(0) { CenterX = ThemeSetButton.ActualWidth / 2, CenterY = ThemeSetButton.ActualHeight / 2 };
			ThemeSetButton.RenderTransform = rotateTransform;

			var start = 5;
			var end = SettingsGrid.ActualHeight * -1;
			SettingsGrid.BeginAnimation(OpacityProperty, new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.1))));
			BlackoutGrid.BeginAnimation(OpacityProperty, new DoubleAnimation(0.3, 0, new Duration(TimeSpan.FromSeconds(0.1))));
			SettingsGrid.BeginAnimation(Canvas.TopProperty, new DoubleAnimation(start, end, new Duration(TimeSpan.FromSeconds(0.1))));

			isThemingGridOpen = false;
		}

		private void SettingsCloseButton_Click(object sender, RoutedEventArgs e)
		{
			CloseThemingMenu();
		}

		protected void OnSourceInitialized()
		{
			((HwndSource)PresentationSource.FromVisual(this)).AddHook(HookProc);
		}

		public static IntPtr HookProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == WM_GETMINMAXINFO)
			{
				// We need to tell the system what our size should be when maximized. Otherwise it will cover the whole screen,
				// including the task bar.
				var mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

				// Adjust the maximized size and position to fit the work area of the correct monitor
				var monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

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

			return IntPtr.Zero;
		}

		private const int WM_GETMINMAXINFO = 0x0024;

		private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

		[DllImport("user32.dll")]
		private static extern IntPtr MonitorFromWindow(IntPtr handle, uint flags);

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
	}
}