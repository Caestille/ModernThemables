namespace ModernThemables.HelperClasses.Charting
{
    /// <summary>
    /// Enum describing how a chart will find a tooltip across all its series'.
    /// </summary>
    public enum TooltipFindingStrategy
    {
        /// <summary>
        /// Takes the nearest X point across all series.
        /// </summary>
        NearestXAllY,

        /// <summary>
        /// Takes the nearest X point from the nearest series.
        /// </summary>
        NearestXNearestY,

        /// <summary>
        /// Takes the nearest X point within a threshold number of pixels from all series. If the nearest point for a
        /// series falls outside the threshold, it will not be included.
        /// </summary>
        NearestXWithinThreshold,

        /// <summary>
        /// No tooltip points will be found.
        /// </summary>
        None,
    }
}
