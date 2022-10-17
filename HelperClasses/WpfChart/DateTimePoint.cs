using ModernThemables.Interfaces;
using System;

namespace ModernThemables.HelperClasses.WpfChart
{
	public class DateTimePoint : IChartPoint
	{
		public DateTime DateTime { get; }

		public double Value { get; }

		public double XValue => new TimeSpan(DateTime.Ticks).TotalDays;

		public double YValue => Value;

		public DateTimePoint(DateTime dateTime, double value)
		{
			DateTime = dateTime;
			Value = value;
		}

		public object XValueToImplementation()
		{
			return new DateTime(TimeSpan.FromDays(XValue).Ticks);
		}

		public object YValueToImplementation()
		{
			return YValue;
		}

		public object XValueToImplementation(double convert)
		{
			return new DateTime(TimeSpan.FromDays(convert).Ticks);
		}

		public object YValueToImplementation(double convert)
		{
			return convert;
		}
	}
}
