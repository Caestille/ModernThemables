namespace ModernThemables.HelperClasses.Charting.PieChart
{
    /// <summary>
    /// A value with a height value for axis displays.
    /// </summary>
    internal struct ValueWithHeight
    {
        /// <summary>
        /// The value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The height.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Initialises a new <see cref="ValueWithHeight"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="height">The height.</param>
        public ValueWithHeight(string value, double height)
        {
            Value = value;
            Height = height;
        }
    }
}
