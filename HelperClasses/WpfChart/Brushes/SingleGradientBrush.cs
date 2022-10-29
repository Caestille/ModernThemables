using CoreUtilities.HelperClasses.Extensions;
using ModernThemables.Interfaces;
using System;
using System.Windows.Media;

namespace ModernThemables.HelperClasses.WpfChart.Brushes
{
	/// <summary>
	/// A brush with a gradient between two <see cref="Color"/>s from top to bottom.
	/// <see cref="Reevaluate(double, double, double, double, double, double)"/> adjusts the midpoint of this gradient.
	/// </summary>
	public sealed class SingleGradientBrush : IChartBrush
	{
		/// <inheritdoc />
		public Brush CoreBrush { get; private set; }

		private Color topColour;
		private Color bottomColour;

		private double yMax;
		private double yMin;

		/// <summary>
		/// Initialises a new <see cref="SingleGradientBrush"/>.
		/// </summary>
		/// <param name="topColour">The <see cref="Color"/> at the top of the brush.</param>
		/// <param name="bottomColour">The <see cref="Color"/> at the bottom of the brush.</param>
		public SingleGradientBrush(Color topColour, Color bottomColour)
		{
			this.topColour = topColour;
			this.bottomColour = bottomColour;

			GradientStopCollection collection = new()
			{
				new GradientStop(topColour, 0),
				new GradientStop(bottomColour, 1.0)
			};

			CoreBrush = new LinearGradientBrush(collection, angle: 90);
		}

		/// <inheritdoc />
		public void Reevaluate(double yMax, double yMin, double yCentre, double xMax, double xMin, double xCentre)
		{
			this.yMax = yMax;
			this.yMin = yMin;

			GradientStopCollection collection = new()
			{
				new GradientStop(topColour, 0),
				new GradientStop(bottomColour, 1.0)
			};

			CoreBrush = new LinearGradientBrush(collection, angle: 90);
		}

		/// <inheritdoc />
		public Color ColourAtPoint(double x, double y)
		{
			if (y >= yMax)
			{
				return topColour;
			}
			else if (y < yMax && y > yMin)
			{
				var ratio = (double)(1 - (y - yMin) / (yMax - yMin));
				return topColour.Combine(bottomColour, ratio);
			}
			else if (y <= yMin)
			{
				return bottomColour;
			}
			else
			{
				return topColour;
			}
		}
	}
}
