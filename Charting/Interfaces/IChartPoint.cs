using System;

namespace ModernThemables.Charting.Interfaces
{
    /// <summary>
    /// A generic chart point.
    /// </summary>
    public interface IChartEntity
    {
        /// <summary>
        /// Raised when the point is focused or unfocused.
        /// </summary>
        event EventHandler<bool> FocusedChanged;

        /// <summary>
        /// The point name. 
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The main value.
        /// </summary>
        double XValue { get; }

        /// <summary>
        /// The raw Y value.
        /// </summary>
        double YValue { get; }

        /// <summary>
        /// The point stroke.
        /// </summary>
        public IChartBrush Stroke { get; }

        /// <summary>
        /// The point fill.
        /// </summary>
        public IChartBrush Fill { get; }

        /// <summary>
        /// The point UID
        /// </summary>
        public Guid Identifier { get; }

        /// <summary>
        /// Whether the point is focused.
        /// </summary>
        public bool IsFocused { get; set; }

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
