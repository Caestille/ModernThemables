namespace ModernThemables.Charting.Models
{
    /// <summary>
    /// A label for axis displays
    /// </summary>
    public struct AxisLabel
    {
        /// <summary>
        /// The axis value to display.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The absolute position of the label along the axis.
        /// </summary>
        public double Location { get; set; }

        /// <summary>
        /// Initialises a new <see cref="AxisLabel"/>.
        /// </summary>
        /// <param name="value">The value to display.</param>
        /// <param name="location">The absolute position of the label along the axis.</param>
        public AxisLabel(string value, double location)
        {
            Value = value;
            Location = location;
        }
    }
}
