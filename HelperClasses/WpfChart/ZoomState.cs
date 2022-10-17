using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;

namespace ModernThemables.HelperClasses.WpfChart
{
	internal class ZoomState : ObservableObject
	{
		public double XMin { get; }
		public double XMax { get; }
		public double YMin { get; }
		public double YMax { get; }
		public double XOffset { get; }

		public ZoomState(double xMin, double xMax, double yMin, double yMax, double xOffset, bool expandY = true)
		{
			XMin = xMin;
			XMax = xMax;
			if (expandY)
			{
				var yRange = yMax - yMin;
				YMin = yMin - yRange * 0.1;
				YMax = yMax + yRange * 0.1;
			}
			else
			{
				YMin = yMin;
				YMax = yMax;
			}
			XOffset = xOffset;
		}
	}
}
