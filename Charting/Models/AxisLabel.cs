namespace ModernThemables.Charting.Models
{
    /// <summary>
    /// A label with a height for axis displays
    /// </summary>
    internal struct AxisLabel
    {
        /// <summary>
        /// The axis value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The height of the label.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Initialises a new <see cref="AxisLabel"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="height">The height.</param>
        public AxisLabel(string value, double height)
        {
            Value = value;
            Height = height;
        }
    }
}
