using Microsoft.Toolkit.Mvvm.ComponentModel;
using ModernThemables.Charting.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;

namespace ModernThemables.Charting.ViewModels.CartesianChart
{
	/// <summary>
	/// A view model for an internal representation of a series used by the <see cref="CartesianChart"/>.
	/// </summary>
	internal class InternalPathSeriesViewModel : ObservableObject
	{
		private string pathStrokeData;
		private string pathFillData;
		private Point topLeft;
		private Point bottomRight;

		private double topMargin;
		private double bottomMargin;
		private double leftMargin;
		private double rightMargin;

		/// <summary>
		/// The data making up the rendered points in pixels scale.
		/// </summary>
		public IEnumerable<InternalChartEntity> Data;

		/// <summary>
		/// The string used the render the series line using a <see cref="Path"/>.
		/// </summary>
		public string PathStrokeData => $"M{topLeft.X - leftMargin},{topLeft.Y - topMargin} {pathStrokeData} M{bottomRight.X + rightMargin},{bottomRight.Y + bottomMargin}";

		/// <summary>
		/// The string used the render the series fill using a <see cref="Path"/>. Due to how paths render a fill, this
		/// may not be identical to the <see cref="PathStrokeData"/>.
		/// </summary>
		public string PathFillData => $"M{topLeft.X - leftMargin},{topLeft.Y - topMargin} {pathFillData} M{bottomRight.X + rightMargin},{bottomRight.Y + bottomMargin}";

		/// <summary>
		/// The <see cref="IChartBrush"/> the path stroke uses to colour itself.
		/// </summary>
		public IChartBrush? Stroke { get; }

		/// <summary>
		/// The <see cref="IChartBrush"/> the path fill uses to colour itself.
		/// </summary>
		public IChartBrush? Fill { get; }

		/// <summary>
		/// A unique identifier, used to relate to a <see cref="ISeries.Identifier"/>.
		/// </summary>
		public Guid Identifier { get; }

		/// <summary>
		/// The series name for the legend.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Initialises a new <see cref="InternalPathSeriesViewModel"/>.
		/// </summary>
		/// <param name="name">The series name.</param>
		/// <param name="guid">The unique identifier.</param>
		/// <param name="data">The data this series represents.</param>
		/// <param name="stroke">The <see cref="IChartBrush"/> path stroke.</param>
		/// <param name="fill">The <see cref="IChartBrush"/> path fill.</param>
		/// <param name="yBuffer">The distance by which the extremes in the yDirection will be reduced by to maintain
		/// an empty border to the chart.</param>
		/// <param name="tooltipFormatter">The Func used to format the tooltip string.</param>
		public InternalPathSeriesViewModel(
			string name,
			Guid guid,
			IEnumerable<InternalChartEntity> data,
			IChartBrush? stroke,
			IChartBrush? fill)
		{
			Name = name;
			Identifier = guid;
			Data = data;
			Stroke = stroke;
			Fill = fill;

			if (!data.Any()) return;

			pathStrokeData = ConvertDataToPath(data);
			pathFillData = ConvertPathForFill(pathStrokeData);
		}

		public void SetMargins(double topMargin, double bottomMargin, double leftMargin, double rightMargin)
		{
			this.topMargin = topMargin * (bottomRight.Y - topLeft.Y);
			this.bottomMargin = bottomMargin * (bottomRight.Y - topLeft.Y);
			this.leftMargin = leftMargin * (bottomRight.X - topLeft.X);
			this.rightMargin = rightMargin * (bottomRight.X - topLeft.X);
			OnPropertyChanged(nameof(PathStrokeData));
			OnPropertyChanged(nameof(PathFillData));
		}

		/// <summary>
		/// Given a cursor coordinate scaled to match the scaled series (if required), returns a chart point reverse
		/// scaled to the cursor scaling under the cursor.
		/// </summary>
		/// <param name="dataWidth">The width of the data in the data representation (i.e.: not pixels).</param>
		/// <param name="dataHeight">The height of the data in the data representation</param>
		/// <param name="mouseX">The scaled cursor X coordinate.</param>
		/// <param name="mouseY">The scaled cursor Y coordinate.</param>
		/// <param name="zoomWidth">The current height of the container for these points. As the points here are not
		/// scaled by point, but instead the container stretched, this is used to determine the zoom level in X.
		/// </param>
		/// <param name="zoomHeight">The current width of the container for these points. As the points here are not
		/// scaled by point, but instead the container stretched, this is used to determine the zoom level in Y.
		/// </param>
		/// <param name="xLeftOffset">The offset in pixels in the X direction for panning/zooming.</param>
		/// <param name="yTopOffset">The offset in pixels in the Y direction for panning/zooming.</param>
		/// <param name="yBuffer">The fractional distance by which the y direction is scaled to create a margin.
		/// </param>
		/// <returns></returns>
		public InternalChartEntity? GetChartPointUnderTranslatedMouse(
			Point cursor,
			double xZoom,
			double yZoom,
			double xLeftOffset,
			double yTopOffset)
		{
			var translatedX = cursor.X / xZoom;
			var translatedY = cursor.Y / yZoom;

			var nearestPoint = Data.FirstOrDefault(
				x => Math.Abs(x.X - translatedX) == Data.Min(x => Math.Abs(x.X - translatedX)));
			if (nearestPoint == null) return null;

			var hoveredChartPoints = Data.Where(x => x.X == nearestPoint.X);
			var hoveredChartPoint = hoveredChartPoints.Count() > 1
				? hoveredChartPoints.First(
					x => Math.Abs(x.Y - translatedY) == hoveredChartPoints.Min(x => Math.Abs(x.Y - translatedY)))
				: hoveredChartPoints.First();

			var x = hoveredChartPoint.X * xZoom - xLeftOffset;
			var y = hoveredChartPoint.Y * yZoom - yTopOffset;
			return new InternalChartEntity(x, y, hoveredChartPoint.BackingPoint);
		}

		/// <summary>
		/// Updates the internal points for tooltip finding purposes without updating anything else, since the path
		/// itself scales to fit its container, it is never re-rendered to match these points.
		/// </summary>
		/// <param name="data">The new data.</param>
		public void UpdatePoints(IEnumerable<InternalChartEntity> data)
		{
			Data = data;
		}

		/// <summary>
		/// Indicates whether mouse coordinates scaled to match the scaled series, are inside the bounds of the series
		/// (only in the X direction).
		/// </summary>
		/// <param name="dataWidth">The width of the data in the data coordinate (i.e.: not pixels).</param>
		/// <param name="mouseX">The cursor X coordinate in pixels.</param>
		/// <param name="zoomWidth">The width of the series container in pixels (due to zoom).</param>
		/// <returns>A <see cref="bool"/> indicating whether the cursor X coordinate is within the bounds of the 
		/// (potentially scaled) series.</returns>
		public bool IsTranslatedMouseInBounds(double dataWidth, double mouseX, double zoomWidth)
		{
			var xZoom = zoomWidth / dataWidth;
			var translatedX = mouseX / xZoom;

			return translatedX <= Data.Max(x => x.X)
				&& translatedX >= Data.Min(x => x.X);
		}

		private string ConvertDataToPath(IEnumerable<InternalChartEntity> data)
		{
			topLeft = new Point(data.Min(x => x.X), data.Min(x => x.Y));
			bottomRight = new Point(data.Max(x => x.X), data.Max(x => x.Y));

			var sb = new StringBuilder();
			var pointType = "M";
			foreach (var point in data)
			{
				sb.Append($" {pointType}{point.X} {point.Y}");
				pointType = "L";
			}
			return sb.ToString().Trim();
		}

		private string ConvertPathForFill(string strokePath)
		{
			var dataMin = Data.Min(x => x.BackingPoint.YValue);
			var dataMax = Data.Max(x => x.BackingPoint.YValue);
			var dataRange = dataMax - dataMin;
			var zero = Math.Min(Math.Max(0d, dataMin), dataMax);
			var ratio = (double)(1 - (zero - dataMin) / dataRange);
			var min = Data.Min(x => x.Y);
			var max = Data.Max(x => x.Y);
			var zeroPoint = min + ratio * (max - min);
			return $"M{Data.First().X} {zeroPoint} {strokePath.Replace("M", "L")} L{Data.Last().X} {zeroPoint}";
		}
	}
}
