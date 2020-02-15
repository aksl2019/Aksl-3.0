
namespace Aksl.Timing
{
    /// <summary>
    /// Backoff Timing
    /// </summary>
    public class BackoffTiming : DynamicTiming
    {
        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="minimumPeriodInSeconds">Minimum Period in Seconds</param>
        /// <param name="maximumPeriodInSeconds">Maximum Period in Seconds</param>
        public BackoffTiming(int minimumPeriod = BaseTimes.MinimumTiming, int maximumPeriod = BaseTimes.MaximumTiming)
            : base(new ExponentialTiming(minimumPeriod, maximumPeriod))
        {
        }

        /// <summary>
        /// Mockable Constructor
        /// </summary>
        /// <param name="timing">Calculate Timing</param>
        public BackoffTiming(ICalculateTiming timing)
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
                _noWorkCount = 0;//清零

                return Timing.FrequencyInSeconds.Minimum;//取最小值，重新开始
            }
            else
            {
                _noWorkCount++;

                return Timing.Get(_noWorkCount);//最多Maximum次
            }
        }
        #endregion
    }
}