namespace Aksl.Timing
{
    #region ICalculateTiming
    /// <summary>
    /// Timing interface for Timing Maths
    /// </summary>
    public interface ICalculateTiming
    {
        #region Properties
        /// <summary>
        /// Timeframe (seconds) (starting timeframe)
        /// </summary>
        Range<int> FrequencyInSeconds
        {
            get;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get Timing
        /// </summary>
        /// <param name="attempts">Attempts</param>
        /// <returns>Timing</returns>
        double Get(ulong attempt);
        #endregion
    }
    #endregion
}