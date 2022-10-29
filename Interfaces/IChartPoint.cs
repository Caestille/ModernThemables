namespace ModernThemables.Interfaces
{
	/// <summary>
	/// A generic chart point.
	/// </summary>
	public interface IChartPoint
	{
		/// <summary>
		/// The raw X value.
		/// </summary>
		double XValue { get; }

		/// <summary>
		/// The raw Y value.
		/// </summary>
		double YValue { get; }

		/// <summary>
		/// Converts the raw X value to the underlying representation of an inheriting class.
		/// </summary>
		/// <returns>A <see cref="object"/>. It is up to the user to correctly unbox this.</returns>
		object XValueToImplementation();

		/// <summary>
		/// Converts the raw Y value to the underlying representation of an inheriting class.
		/// </summary>
		/// <returns>A <see cref="object"/>. It is up to the user to correctly unbox this.</returns>
		object YValueToImplementation();

		/// <summary>
		/// Converts a given X value to the underlying representation of an inheriting class.
		/// </summary>
		/// <returns>A <see cref="object"/>. It is up to the user to correctly unbox this and ensure the given input is
		/// of the correct scale.</returns>
		object XValueToImplementation(double convert);

		/// <summary>
		/// Converts a given Y value to the underlying representation of an inheriting class.
		/// </summary>
		/// <returns>A <see cref="object"/>. It is up to the user to correctly unbox this and ensure the given input is
		/// of the correct scale.</returns>
		object YValueToImplementation(double convert);
	}
}
