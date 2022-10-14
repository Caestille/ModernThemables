using Microsoft.Toolkit.Mvvm.ComponentModel;
using ModernThemables.Controls;
using ModernThemables.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

			PathStrokeData = ConvertDataToPath(data);

            var dataMin = Data.Min(x => x.BackingPoint.Value).Value;
			var dataMax = Data.Max(x => x.BackingPoint.Value).Value;
			var range = dataMax - dataMin;
			var zero = Math.Min(Math.Max(0d, dataMin), dataMax);
			var ratio = (double)(1 - (zero - dataMin) / range);
			var zeroPoint = ratio * (Data.Max(x => x.Y) - Data.Min(x => x.Y)) * 1.1;
			PathFillData = $"M{Data.First().X} {zeroPoint} {PathStrokeData.Replace("M", "L")} L{Data.Last().X} {zeroPoint}";
		}

		private string ConvertDataToPath(IEnumerable<ChartPoint> data)
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

		public void SetZoomData(ZoomStep zoomDetails)
		{
			var zoomLevel = zoomDetails.Step;
			var zoomCentre = zoomDetails.Centre;
			var zoomOffset = zoomDetails.Offset;

			var currentMin = ZoomData.Min(x => x.X);
			var currentMax = ZoomData.Max(x => x.X);

			var currentWidth = currentMax - currentMin;
			var widthDiff = currentWidth / zoomLevel - currentWidth;
			var leftDiff = widthDiff * zoomDetails.Centre;
			var rightDiff = widthDiff * (1 - zoomDetails.Centre);

			var newMin = currentMin - leftDiff;
			var newMax = currentMax + rightDiff;

			var data = new List<ChartPoint>();
			foreach (var point in ZoomData)
			{
				var percentThroughData = (point.X - currentMin) / (currentMax - currentMin);
				data.Add(new ChartPoint(newMin + percentThroughData * (newMax - newMin) - zoomOffset, point.Y, point.BackingPoint));
			}
			ZoomData = data;
        }

		public void ResetZoomData()
		{
			ZoomData = Data;
		}
	}
}
