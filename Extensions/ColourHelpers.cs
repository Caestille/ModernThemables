using System;
using System.Windows.Media;

namespace Win10Themables.Extensions
{
	public static class ColourHelpers
	{
		public static void ChangeThisColourBrightness(this ref Color colour, float factor)
		{
			float red = (float)colour.R;
			float green = (float)colour.G;
			float blue = (float)colour.B;

			if (factor < 0)
			{
				factor = 1 + factor;
				red *= factor;
				green *= factor;
				blue *= factor;
			}
			else
			{
				red = (255 - red) * factor + red;
				green = (255 - green) * factor + green;
				blue = (255 - blue) * factor + blue;
			}

			colour = Color.FromArgb(colour.A, (byte)red, (byte)green, (byte)blue);
		}

		public static Color ChangeColourBrightness(this Color colour, float factor)
		{
			float red = (float)colour.R;
			float green = (float)colour.G;
			float blue = (float)colour.B;

			if (factor < 0)
			{
				factor = 1 + factor;
				red *= factor;
				green *= factor;
				blue *= factor;
			}
			else
			{
				red = (255 - red) * factor + red;
				green = (255 - green) * factor + green;
				blue = (255 - blue) * factor + blue;
			}

			return Color.FromArgb(colour.A, (byte)red, (byte)green, (byte)blue);
		}

		public static Color MonoColour(byte value)
		{
			return Color.FromArgb(255, value, value, value);
		}

		public static double PerceivedBrightness(this Color colour)
		{
			var rFactor = 0.299 * Math.Pow(colour.R, 2);
			var gFactor = 0.587 * Math.Pow(colour.G, 2);
			var bFactor = 0.114 * Math.Pow(colour.B, 2);
			return Math.Sqrt(rFactor + gFactor + bFactor) / 255d;
		}

		public static bool ColoursAreClose(this Color colour1, Color colour2, double threshold)
		{
			var rDist = Math.Abs(colour1.R - colour2.R);
			var gDist = Math.Abs(colour1.G - colour2.G);
			var bDist = Math.Abs(colour1.B - colour2.B);

			return rDist + gDist + bDist < threshold;
		}

		public static void SetNearBackgroundColour(this ref Color colour, Color comparison1, Color comparison2, Color set, double factor, double threshold, float changeFactor)
		{
			var themeBrightness = comparison1.PerceivedBrightness();
			var backgroundBrightness = comparison2.PerceivedBrightness();
			var diff = themeBrightness - backgroundBrightness; // >0 means theme is brighter than background
			colour = factor * diff > -1 * threshold ? set.ChangeColourBrightness(changeFactor) : set;
		}
	}
}
