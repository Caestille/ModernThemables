using Microsoft.Toolkit.Mvvm.ComponentModel;
using ModernThemables.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ModernThemables.HelperClasses.WpfChart
{
	internal class ConvertedSeriesViewModel : ObservableObject
	{
		public IEnumerable<InternalChartPoint> Data;
		public string PathStrokeData { get; }
		public string PathFillData { get; }
		public IChartBrush Stroke { get; }
		public IChartBrush Fill { get; }
		public Func<IEnumerable<IChartPoint>, IChartPoint, string> TooltipLabelFormatter;

		public ConvertedSeriesViewModel(
			IEnumerable<InternalChartPoint> data,
			IChartBrush stroke,
			IChartBrush fill,
			double yBuffer,
			Func<IEnumerable<IChartPoint>, IChartPoint, string> tooltipFormatter)
		{
			Data = data;
			Stroke = stroke;
			Fill = fill;
			TooltipLabelFormatter = tooltipFormatter;

			PathStrokeData = ConvertDataToPath(data);

			var dataMin = Data.Min(x => x.BackingPoint.YValue);
			var dataMax = Data.Max(x => x.BackingPoint.YValue);
			var range = dataMax - dataMin;
			var zero = Math.Min(Math.Max(0d, dataMin), dataMax);
			var ratio = (double)(1 - (zero - dataMin) / range);
			var min = Data.Min(x => x.Y);
			var max = Data.Max(x => x.Y);
			var zeroPoint = min - (max - min) * yBuffer + ratio * (max - min) * (1 + yBuffer);
			PathFillData = 
				$"M{Data.First().X} {zeroPoint} {PathStrokeData.Replace("M", "L")} L{Data.Last().X} {zeroPoint}";
		}

		public InternalChartPoint? GetChartPointUnderTranslatedMouse(
			double dataWidth,
			double dataHeight,
			double mouseX,
			double mouseY,
			double zoomWidth,
			double zoomHeight,
			double xLeftOffset,
			double yTopOffset,
			double yBuffer)
		{
			var xZoom = zoomWidth / dataWidth;
			var yZoom = zoomHeight / dataHeight;

			var translatedX = mouseX / xZoom;
			var translatedY = (mouseY + yBuffer * dataHeight * yZoom) / yZoom;

			var nearestPoint = Data.FirstOrDefault(
				x => Math.Abs(x.X - translatedX) == Data.Min(x => Math.Abs(x.X - translatedX)));
			if (nearestPoint == null) return null;

			var hoveredChartPoints = Data.Where(x => x.X == nearestPoint.X);
			var hoveredChartPoint = hoveredChartPoints.Count() > 1
				? hoveredChartPoints.First(
					x => Math.Abs(x.Y - translatedY) == hoveredChartPoints.Min(x => Math.Abs(x.Y - translatedY)))
				: hoveredChartPoints.First();

			var x = hoveredChartPoint.X * xZoom - xLeftOffset;
			var y = hoveredChartPoint.Y * yZoom - yTopOffset - yBuffer * dataHeight * yZoom;
			return new InternalChartPoint(x, y, hoveredChartPoint.BackingPoint);
		}

		private string ConvertDataToPath(IEnumerable<InternalChartPoint> data)
		{
			var sb = new StringBuilder();
			bool setM = true;
			foreach (var point in data)
			{
				var pointType = setM ? "M" : "L";
				setM = false;
				sb.Append($" {pointType}{point.X} {point.Y}");
			}
			var ret = sb.ToString();
			ret += $" L{data.Last().X} {data.First().Y}";
			return ret;
		}
	}
}
