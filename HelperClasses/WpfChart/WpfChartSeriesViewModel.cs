using Microsoft.Toolkit.Mvvm.ComponentModel;
using ModernThemables.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModernThemables.HelperClasses.WpfChart
{
	internal class WpfChartSeriesViewModel : ObservableObject
	{
		public IEnumerable<InternalChartPointRepresentation> Data;
		public string PathStrokeData { get; }
		public string PathFillData { get; }
		public IChartBrush Stroke { get; }
		public IChartBrush Fill { get; }
		public double Height => Data.Max(x => x.Y) - Data.Min(x => x.Y);

		public WpfChartSeriesViewModel(IEnumerable<InternalChartPointRepresentation> data, IChartBrush stroke, IChartBrush fill)
		{
			Data = data;
			Stroke = stroke;
			Fill = fill;

			PathStrokeData = ConvertDataToPath(data);

			var dataMin = Data.Min(x => x.BackingPoint.YValue);
			var dataMax = Data.Max(x => x.BackingPoint.YValue);
			var range = dataMax - dataMin;
			var zero = Math.Min(Math.Max(0d, dataMin), dataMax);
			var ratio = (double)(1 - (zero - dataMin) / range);
			var zeroPoint = ratio * (Data.Max(x => x.Y) - Data.Min(x => x.Y)) * 1.1;
			PathFillData = $"M{Data.First().X} {zeroPoint} {PathStrokeData.Replace("M", "L")} L{Data.Last().X} {zeroPoint}";
		}

		public InternalChartPointRepresentation GetChartPointUnderTranslatedMouse(double mouseX, double mouseY, double zoomWidth, double zoomToStandardOffset)
		{
			var dataWidth = Data.Max(x => x.X) - Data.Min(x => x.X);
			var standardToZoomMultiplier = dataWidth / zoomWidth;

			var translatedX = mouseX * standardToZoomMultiplier;
			var translatedY = mouseY;

			var nearestPoint = Data.First(x => Math.Abs(x.X - translatedX) == Data.Min(x => Math.Abs(x.X - translatedX)));
			var hoveredChartPoints = Data.Where(x => x.X == nearestPoint.X);
			var hoveredChartPoint = hoveredChartPoints.Count() > 1
				? hoveredChartPoints.First(x => Math.Abs(x.Y - translatedY) == hoveredChartPoints.Min(x => Math.Abs(x.Y - translatedY)))
				: hoveredChartPoints.First();

			return new InternalChartPointRepresentation(hoveredChartPoint.X / standardToZoomMultiplier - zoomToStandardOffset, hoveredChartPoint.Y, hoveredChartPoint.BackingPoint);
		}

		private string ConvertDataToPath(IEnumerable<InternalChartPointRepresentation> data)
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
