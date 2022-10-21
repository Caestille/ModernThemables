using ModernThemables.Interfaces;
using System.Windows.Media;

namespace ModernThemables.HelperClasses.WpfChart.Brushes
{
	public sealed class SolidBrush : IChartBrush
	{
		public Brush CoreBrush { get; private set; }

		private Color colour;

		public SolidBrush(Color colour)
		{
			this.colour = colour;
			CoreBrush = new SolidColorBrush(colour);
		}

		public void Reevaluate(double yMax, double yMin, double yCentre, double xMax, double xMin, double xCentre)
		{

		}

		public Color ColourAtPoint(double x, double y)
		{
			return colour;
		}
	}
}
