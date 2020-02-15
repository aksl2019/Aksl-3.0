using System;

namespace Aksl.Timing
{
    /// <summary>
    /// Calculate Timing
    /// </summary>
    public abstract class CalculateTiming : ICalculateTiming
    {
        #region Members
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="minimumPeriodInSeconds">Minimum Period In Seconds</param>
        /// <param name="maximumPeriodInSeconds">Maximum Period In Seconds</param>
        public CalculateTiming(int minimumPeriod, int maximumPeriod)
        {
            if (minimumPeriod >= maximumPeriod) throw new ArgumentException(nameof(CalculateTiming), "maximum must greet than minimum");

            FrequencyInSeconds = new Range<int>
            {
                Minimum = 0 >= minimumPeriod ? 1 : minimumPeriod,
                Maximum = minimumPeriod >= maximumPeriod ? minimumPeriod + 1 : maximumPeriod,
            };
        }
        #endregion

        #region Properties
        /// Timeframe (seconds) (starting timeframe)
        /// </summary>
        public virtual Range<int> FrequencyInSeconds { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Get timing
        /// </summary>
        /// <param name="attempts">attempts</param>
        /// <returns>timing</returns>
        public abstract double Get(ulong attempt);
        #endregion
    }
}