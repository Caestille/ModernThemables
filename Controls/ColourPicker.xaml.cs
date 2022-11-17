using CoreUtilities.HelperClasses.Extensions;
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
using System.Windows.Threading;

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
		private static List<KeyValuePair<Color, double>> horizontalColourStops = new List<KeyValuePair<Color, double>>()
		{
			new KeyValuePair<Color, double>(Colors.Red, 0),
			new KeyValuePair<Color, double>(Colors.Magenta, 1d/7),
			new KeyValuePair<Color, double>(Colors.Blue, 2d/7),
			new KeyValuePair<Color, double>(Colors.Turquoise, 3d/7),
			new KeyValuePair<Color, double>(Color.FromRgb(0, 255, 0), 4d/7),
			new KeyValuePair<Color, double>(Colors.Yellow, 5d/7),
			new KeyValuePair<Color, double>(Colors.Orange, 6d/7),
			new KeyValuePair<Color, double>(Colors.Red, 1)
		};
		private static List<KeyValuePair<Color, double>> verticalColourStops = new List<KeyValuePair<Color, double>>()
		{
			new KeyValuePair<Color, double>(Colors.Black, 0),
			new KeyValuePair<Color, double>(Colors.Transparent, 1/2),
			new KeyValuePair<Color, double>(Colors.White, 1)
		};

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

			var borderCursor = e.GetPosition(ColourSelectionBorder);
			SelectedColourBorder.Margin = new Thickness(borderCursor.X - 8, borderCursor.Y - 8, 0, 0);

			var cursor = this.PointToScreen(e.GetPosition(this));
			this.Dispatcher.Invoke(DispatcherPriority.Render, delegate () { });
			var c = GetColorAt((int)borderCursor.X, (int)borderCursor.Y);
			Colour = c;
		}

		public Color GetColorAt(int x, int y)
		{
			var horizFrac = x / ColourSelectionBorder.ActualWidth;
			var vertFrac = (float)(((y / ColourSelectionBorder.ActualHeight) - 0.5) * 2);

			var leftColour = horizontalColourStops.Where(x => x.Value <= horizFrac).Last();
			var rightColour = horizontalColourStops.Where(x => x.Value >= horizFrac).First();

			var outputColour = leftColour.Key.Combine(rightColour.Key, (horizFrac - leftColour.Value) / (rightColour.Value - leftColour.Value));

			var output = outputColour.ChangeColourBrightness(-vertFrac);

			return output;
		}

		public Point GetPointAtColour(Color colour)
		{
			var brightness = colour.Brightness();
			var y = -(brightness * ColourSelectionBorder.ActualHeight - ColourSelectionBorder.ActualHeight);
			for (int x = 0; x < ColourSelectionBorder.ActualWidth; x ++)
			{
				if (GetColorAt(x, (int)y).ColoursAreClose(colour, 20))
				{
					return new Point(x, y);
				}
			}
			return new Point(0, y);
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
			//if (isMouseDown) return;

			//var point = GetPointAtColour(Colour);
			//SelectedColourBorder.Margin = new Thickness(point.X - 8, point.Y - 8, 0, 0);
		}

		private void ColourSelectionBorder_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var borderCursor = e.GetPosition(ColourSelectionBorder);
			SelectedColourBorder.Margin = new Thickness(borderCursor.X - 8, borderCursor.Y - 8, 0, 0);

			var cursor = this.PointToScreen(e.GetPosition(this));
			this.Dispatcher.Invoke(DispatcherPriority.Render, delegate () { });
			var c = GetColorAt((int)borderCursor.X, (int)borderCursor.Y);
			Colour = c;
		}
	}
}
