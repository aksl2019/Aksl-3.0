
namespace Aksl.Timing
{
    /// <summary>
    /// Adaptive Timing
    /// </summary>
    public class AdaptiveTiming : DynamicTiming
    {
        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="minimumPeriodInSeconds">Minimum Period in Seconds</param>
        /// <param name="maximumPeriodInSeconds">Maximum Period in Seconds</param>
        public AdaptiveTiming(int minimumPeriod = BaseTimes.MinimumTiming, int maximumPeriod = BaseTimes.MaximumTiming)
            : base(new ExponentialTiming(minimumPeriod, maximumPeriod))
        {
        }

        /// <summary>
        /// Mockable Constructor
        /// </summary>
        /// <param name="timing">Calculate Timing</param>
        public AdaptiveTiming(ICalculateTiming timing)
            : base(timing)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get Timing
        /// </summary>
        /// <param name="workWasDone">Work was done</param>
        /// <returns>New Timing</returns>
        public override double Get(bool workWasDone)
        {
            //如果正确处理完数据
            if (workWasDone)
            {
                if (0 < _noWorkCount)
                {
                    _noWorkCount--;
                }
            }
            else if (Timing.FrequencyInSeconds.Maximum > Timing.Get(_noWorkCount))
            {
                _noWorkCount++;
            }

            return Timing.Get(_noWorkCount);
        }
        #endregion
    }
}