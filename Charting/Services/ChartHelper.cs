using ModernThemables.Charting.Models;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ModernThemables.Charting.Services
{
	public static class ChartHelper
	{
		public static (int row, int column, Visibility visibility, Thickness margin) GetLegendProperties(LegendLocation legendLocation)
		{
			int row = 0;
			int column = 0;
			Visibility visibility = Visibility.Visible;
			Thickness margin = new Thickness(0);

			switch (legendLocation)
			{
				case LegendLocation.Left:
					row = 1;
					margin = new Thickness(0, 10, 15, 0);
					break;
				case LegendLocation.Top:
					column = 1;
					margin = new Thickness(0, 0, 0, 15);
					break;
				case LegendLocation.Right:
					row = 1;
					column = 2;
					margin = new Thickness(15, 10, 0, 0);
					break;
				case LegendLocation.Bottom:
					row = 2;
					column = 1;
					margin = new Thickness(0, 15, 0, 0);
					break;
				case LegendLocation.None:
					visibility = Visibility.Collapsed;
					break;
			}

			return (row, column, visibility, margin);
		}

		public static List<double> IdealAxisSteps(int itemCount, double min, double max)
		{
			var yVals = new List<double>();

			var yRange = max - min;
			var idealStep = yRange / (double)itemCount;
			double valMin = double.MaxValue;
			int stepAtMin = 1;
			var roundedSteps = new List<int>()
				{ 1, 10, 20, 100, 500, 1000, 1500, 2000, 3000, 4000, 5000, 10000, 20000, 50000, 1000000, 10000000 };
			roundedSteps.Reverse();
			foreach (var step in roundedSteps)
			{
				var val = Math.Abs(idealStep - step);
				if (val < valMin)
				{
					valMin = val;
					stepAtMin = step;
				}
			}

			bool startAt0 = min <= 0 && max >= 0;
			if (startAt0)
			{
				double currVal = 0;
				while (currVal > min)
				{
					yVals.Insert(0, currVal);
					currVal -= stepAtMin;
				}

				currVal = stepAtMin;

				while (currVal < max)
				{
					yVals.Add(currVal);
					currVal += stepAtMin;
				}
			}
			else
			{
				int dir = max < 0 ? -1 : 1;
				double currVal = 0;
				bool adding = true;
				bool hasStartedAdding = false;
				while (adding)
				{
					if (currVal < max && currVal > min)
					{
						hasStartedAdding = true;
						yVals.Add(currVal);
					}
					else if (hasStartedAdding)
					{
						adding = false;
					}

					currVal += dir * stepAtMin;
				}
			}

			return yVals;
		}
	}
}
