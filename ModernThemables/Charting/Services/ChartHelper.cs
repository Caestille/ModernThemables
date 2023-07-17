using ModernThemables.Charting.Controls;
using ModernThemables.Charting.Controls.ChartComponents;
using ModernThemables.Charting.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernThemables.Charting.Services
{
    public static class ChartHelper
	{
		public static (int row, int column, Visibility visibility, Thickness margin, Orientation orientation) GetLegendProperties(LegendLocation legendLocation)
		{
			int row = 0;
			int column = 0;
			Visibility visibility = Visibility.Visible;
			Thickness margin = new Thickness(0);
			Orientation orientation = Orientation.Horizontal;

			switch (legendLocation)
			{
				case LegendLocation.Left:
					row = 1;
					margin = new Thickness(0, 10, 15, 0);
					orientation = Orientation.Vertical;
					break;
				case LegendLocation.Top:
					column = 1;
					margin = new Thickness(0, 0, 0, 15);
					break;
				case LegendLocation.Right:
					row = 1;
					column = 2;
					margin = new Thickness(15, 10, 0, 0);
					orientation = Orientation.Vertical;
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

			return (row, column, visibility, margin, orientation);
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

		public static bool FindMouseCoordinatorFromVisualTree(
			DependencyObject root,
			out MouseCoordinator coordinator,
			bool foundParentChart = false,
			int currentLevel = 0)
		{
			var parentSearchDepth = 10;
			var childSearchDepth = 10;

			if (!foundParentChart)
			{
				for (int i = 0; i < parentSearchDepth; i++)
				{
					var parent = VisualTreeHelper.GetParent(root);
					if (parent is CartesianChart || parent is PieChart || parent is BarChart)
					{
						return FindMouseCoordinatorFromVisualTree(parent, out coordinator, true, 0);
					}
					root = parent;
				}
			}

			if (foundParentChart && currentLevel < childSearchDepth)
			{
				currentLevel++;
				var childCount = VisualTreeHelper.GetChildrenCount(root);
				for (int i = 0; i < childCount; i++)
				{
					var child = VisualTreeHelper.GetChild(root, i);
					if (child is MouseCoordinator coord)
					{
						coordinator = coord;
						return true;
					}

					if (FindMouseCoordinatorFromVisualTree(child, out coordinator, true, currentLevel))
					{
						return true;
					}
				}
			}

			coordinator = null;
			return false;
		}
	}
}
