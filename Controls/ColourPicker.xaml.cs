using CoreUtilities.HelperClasses.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace ModernThemables.Controls
{
	/// <summary>
	/// Interaction logic for ColourPicker.xaml
	/// </summary>
	public partial class ColourPicker : UserControl
	{
		public Action<Color> colourChangedCallback;

		public Color Colour
		{
			get => (Color)GetValue(ColourProperty);
			set => SetValue(ColourProperty, value);
		}
		public static readonly DependencyProperty ColourProperty = DependencyProperty.Register(
			"Colour",
			typeof(Color),
			typeof(ColourPicker),
			new UIPropertyMetadata(Colors.Black));

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

		public ColourPicker()
		{
			InitializeComponent();
			this.Loaded += ColourPicker_Loaded;
		}

		private void ColourPicker_Loaded(object sender, RoutedEventArgs e)
		{
			var point = GetPointAtColour(Colour);
			AdjustSelectedColourCursor((int)point.X, (int)point.Y);
			this.Loaded -= ColourPicker_Loaded;
		}

		private void Border_MouseMove(object sender, MouseEventArgs e)
		{
			if (!ColourSelectionBorder.IsMouseDirectlyOver) return;
			if (!isMouseDown) return;

			var borderCursor = e.GetPosition(ColourSelectionBorder);
			AdjustSelectedColourCursor((int)borderCursor.X, (int)borderCursor.Y);

			var cursor = this.PointToScreen(e.GetPosition(this));
			var c = GetColorAt((int)borderCursor.X, (int)borderCursor.Y);
			Colour = c ?? Colour;
			if (colourChangedCallback != null) colourChangedCallback(Colour);
		}

		public Color? GetColorAt(int x, int y)
		{
			if (x < 0 || x > ColourSelectionBorder.ActualWidth - 0 || y < 0 || y > ColourSelectionBorder.ActualHeight - 0) return null;

			var horizFrac = x / ColourSelectionBorder.ActualWidth;
			var vertFrac = (float)(((y / ColourSelectionBorder.ActualHeight) - 0.5) * 2);

			var leftColour = horizontalColourStops.Where(x => x.Value <= horizFrac)
				.DefaultIfEmpty(new KeyValuePair<Color, double>(Colors.Red, 0)).Last();
			var rightColour = horizontalColourStops.Where(x => x.Value >= horizFrac)
				.DefaultIfEmpty(new KeyValuePair<Color, double>(Colors.Red, 1)).First();

			var outputColour = leftColour.Key.Combine(rightColour.Key, (horizFrac - leftColour.Value) / (rightColour.Value - leftColour.Value));

			var output = outputColour.ChangeColourBrightness(-vertFrac);

			return output;
		}

		public Point GetPointAtColour(Color colour)
		{
			int width = (int)ColourSelectionBorder.ActualWidth;
			int height = (int)ColourSelectionBorder.ActualHeight;
			var xStep = width / 50;
			var yStep = height / 50;
			int xPos = 0;
			int yPos = 0;
			for (int i = 0; i < 50; i++)
			{
				var doBreak = false;
				for (int j = 0; j < 50; j++)
				{
					xPos = xStep * i;
					yPos = yStep * j;
					var sampled = GetColorAt(xPos, yPos);
					if (sampled.HasValue && sampled.Value.ColoursAreClose(colour, 50))
					{
						doBreak = true;
						break;
					}
				}
				if (doBreak)
				{
					break;
				}
			}

			var initialPoint = new Point(xPos, yPos);

			width = xStep * 2;
			height = yStep * 2;
			xPos -= xStep;
			yPos -= yStep;

			for (int i = xPos; i <= xPos + width; i++)
			{
				for (int j = yPos; j <= yPos + height; j++)
				{
					var sampled = GetColorAt(i, j);
					if (sampled.HasValue && sampled.Value.ColoursAreClose(colour, 50))
					{
						return new Point(i, j);
					}
				}
			}

			return initialPoint;
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
			if (isMouseDown) return;

			var point = GetPointAtColour(Colour);
			AdjustSelectedColourCursor((int)point.X, (int)point.Y);
			if (colourChangedCallback != null) colourChangedCallback(Colour);
		}

		private void ColourSelectionBorder_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var borderCursor = e.GetPosition(ColourSelectionBorder);
			AdjustSelectedColourCursor((int)borderCursor.X, (int)borderCursor.Y);
			var c = GetColorAt((int)borderCursor.X, (int)borderCursor.Y);
			Colour = c ?? Colour;
			if (colourChangedCallback != null) colourChangedCallback(Colour);
		}

		private void ColourSelectionBorder_MouseLeave(object sender, MouseEventArgs e)
		{
			if (isMouseDown) isMouseDown = false;
		}

		private void AdjustSelectedColourCursor(int x, int y)
		{
			SelectedColourBorder.Margin = new Thickness(x - 5, y - 5, 0, 0);
		}
	}
}
