using System;

namespace Aksl.Timing
{
    /// <summary>
    /// Exponential Timing
    /// </summary>
    public class ExponentialTiming : CalculateTiming
    {
        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="minimumPeriodInSeconds">Minimum Period In Seconds</param>
        /// <param name="maximumPeriodInSeconds">Maximum Period In Seconds</param>
        public ExponentialTiming(int minimumPeriod = BaseTimes.MinimumTiming, int maximumPeriod = BaseTimes.MaximumTiming)
            : base(minimumPeriod, maximumPeriod)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Exponential strategy, within bounds
        /// </summary>
        /// <param name="attempts">attempts</param>
        /// <returns>timing</returns>
        public override double Get(ulong attempt)
        {
            if (0 == attempt)
            {
                return FrequencyInSeconds.Minimum;
            }

            //按指数增加
            var current = ((Math.Pow(2, attempt) * .1d) * FrequencyInSeconds.Minimum) + FrequencyInSeconds.Minimum;

            return current < FrequencyInSeconds.Maximum ? current : FrequencyInSeconds.Maximum;//一直到最大值
        }
        #endregion
    }
}