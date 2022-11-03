using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModernThemables.Controls
{
	/// <summary>
	/// Interaction logic for ColourPicker.xaml
	/// </summary>
	public partial class ColourPicker : UserControl
	{
		public Color Colour
		{
			get => (Color)GetValue(ColourProperty);
			set => SetValue(ColourProperty, value);
		}
		public static readonly DependencyProperty ColourProperty = DependencyProperty.Register(
			"Colour",
			typeof(Color),
			typeof(ColourPicker),
			new UIPropertyMetadata(null));

		private bool isMouseDown;

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr GetDesktopWindow();
		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr GetWindowDC(IntPtr window);
		[DllImport("gdi32.dll", SetLastError = true)]
		public static extern uint GetPixel(IntPtr dc, int x, int y);
		[DllImport("user32.dll", SetLastError = true)]
		public static extern int ReleaseDC(IntPtr window, IntPtr dc);

		public ColourPicker()
		{
			InitializeComponent();
		}

		private void Border_MouseMove(object sender, MouseEventArgs e)
		{
			if (!isMouseDown) return;

			var cursor = this.PointToScreen(e.GetPosition(this));
			var c = GetColorAt((int)cursor.X, (int)cursor.Y);
			Colour = c;

			var borderCursor = e.GetPosition(ColourSelectionBorder);
			SelectedColourBorder.Margin = new Thickness(borderCursor.X - 5, borderCursor.Y - 5, 0, 0);
		}

		public static Color GetColorAt(int x, int y)
		{
			IntPtr desk = GetDesktopWindow();
			IntPtr dc = GetWindowDC(desk);
			int a = (int)GetPixel(dc, x, y);
			ReleaseDC(desk, dc);
			return Color.FromArgb(
				255,
				(byte)((a >> 0) & 0xff),
				(byte)((a >> 8) & 0xff),
				(byte)((a >> 16) & 0xff));
		}

		private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			isMouseDown = true;
		}

		private void Border_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			isMouseDown = false;
		}

		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			
		}
	}
}
