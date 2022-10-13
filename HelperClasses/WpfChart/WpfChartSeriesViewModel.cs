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
		public IEnumerable<ChartPoint> Data;
		public string PathStrokeData { get; }
		public string PathFillData { get; }
		public IChartBrush Stroke { get; }
		public IChartBrush Fill { get; }
		public double Height => Data.Max(x => x.Y) - Data.Min(x => x.Y);

		public IEnumerable<ChartPoint> ZoomData { get; private set; }

		public WpfChartSeriesViewModel(IEnumerable<ChartPoint> data, IChartBrush stroke, IChartBrush fill)
		{
			Data = data;
			ZoomData = data;
			Stroke = stroke;
			Fill = fill;

			var sb = new StringBuilder();
			bool setM = true;
			foreach (var point in Data)
			{
				var pointType = setM ? "M" : "L";
				setM = false;
				sb.Append($" {pointType}{point.X} {point.Y}");
			}
			PathStrokeData = sb.ToString();
			PathStrokeData += $" L{Data.Last().X} {Data.First().Y}";

			var dataMin = Data.Min(x => x.BackingPoint.Value).Value;
			var dataMax = Data.Max(x => x.BackingPoint.Value).Value;
			var range = dataMax - dataMin;
			var zero = Math.Min(Math.Max(0d, dataMin), dataMax);
			var ratio = (double)(1 - (zero - dataMin) / range);
			var zeroPoint = ratio * (Data.Max(x => x.Y) - Data.Min(x => x.Y)) * 1.1;
			PathFillData = $"M{Data.First().X} {zeroPoint} {PathStrokeData.Replace("M", "L")} L{Data.Last().X} {zeroPoint}";
		}

		public void SetZoomData(double zoomLevel, double zoomCentre, double zoomOffset)
		{
			var zoomCentreX = Data.Min(x => x.X) + (Data.Max(x => x.X) - Data.Min(x => x.X)) * zoomCentre;
			var data = new List<ChartPoint>();
			foreach (var point in Data)
			{
				data.Add(new ChartPoint(point.X + ((point.X - zoomCentreX) * (1 / zoomLevel - 1)) - zoomOffset, point.Y, point.BackingPoint));
			}
			ZoomData = data;
		}
	}
}
