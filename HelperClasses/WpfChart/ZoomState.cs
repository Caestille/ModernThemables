using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;

namespace ModernThemables.HelperClasses.WpfChart
{
	internal class ZoomState : ObservableObject
	{
		public double Min { get; }
		public double Max { get; }
		public double Offset { get; }

		public ZoomState(double min, double max, double offset)
		{
			Min = min;
			Max = max;
			Offset = offset;
		}
	}
}
