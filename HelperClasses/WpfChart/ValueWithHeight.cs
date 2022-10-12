namespace ModernThemables.HelperClasses.WpfChart
{
    internal struct ValueWithHeight
    {
        public string Value { get; set; }
        public double Height { get; set; }

        public ValueWithHeight(string value, double height)
        {
            Value = value;
            Height = height;
        }
    }
}
