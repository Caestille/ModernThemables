using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace ModernThemables.HelperClasses.WpfChart
{
	internal class ZoomState : ObservableObject
	{
		public double XMin { get; }
		public double XMax { get; }
		public double YMin { get; }
		public double YMax { get; }
		public double XOffset { get; }

		public ZoomState(double xMin, double xMax, double yMin, double yMax, double xOffset, double yBuffer, bool expandY = true)
		{
			XMin = xMin;
			XMax = xMax;
			if (expandY)
			{
				var yRange = yMax - yMin;
				YMin = yMin - yRange * yBuffer;
				YMax = yMax + yRange * yBuffer;
			}
			else
			{
				YMin = yMin;
				YMax = yMax;
			}
			XOffset = xOffset;
		}

		public bool IsPointInBounds(double x, double y)
		{
			return x <= XMax && x >= XMin && y <= YMax && y >= YMin;
		}
	}
}
