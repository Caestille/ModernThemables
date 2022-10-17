namespace ModernThemables.Interfaces
{
	public interface IChartPoint
	{
		double XValue { get; }
		double YValue { get; }
		object XValueToImplementation();
		object YValueToImplementation();
		object XValueToImplementation(double convert);
		object YValueToImplementation(double convert);
	}
}
